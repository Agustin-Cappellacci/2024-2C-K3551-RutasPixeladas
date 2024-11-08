using System;
using System.Net.Mime;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;
using System.Collections.Generic;
using BepuPhysics;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
using BepuPhysics.Collidables;
using BepuUtilities;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal del juego.
    ///     
    ///     SUPER IMPORTANTE: No olvidar una parte fundamental de teoría (porque yo me la olvide xd) . Cada modelo tiene su propia Matrix de Mundo. Por lo que Cars.cs no nos serviría para nada. 
    ///     De momento sirve como decoración nomás.
    ///     Para que sirva y cree otros enemigos cada uno debería guardar su propia Matrix de Mundo. Se podría usar la función pero tendríamos que hacer que cada Modelo tuviese un nombre propio o podría hacer
    ///     una lista con todas las matrices mundo o algo así. Ya iremos viendo. 
    /// 
    ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";
        public const string ContenidoAutoCombate = "Models/CombatVehicle";
        public const string ContenidoAutoCarrera = "Models/RacingCarA";

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }

        // Menu
        MainMenu menu;

        private bool _isMenuOpen = false;
        private bool _liberarCamara = false;
        private bool _debugColisiones = true;

        // Cámara
        IsometricCamera IsometricCamera { get; set; }
        FreeCamera FreeCamera { get; set; }
        BoundingFrustum _boundingFrustum { get; set; }
        

        private KeyboardState oldState { get; set; }

        // Modelos y efectos
        private Model Model { get; set; }
        private Model DeLoreanModel { get; set; }
        private Effect Effect { get; set; }

        // Clases
        private Jugador autoJugador { get; set; }
        private Toys Toys { get; set; }
        private Cuarto Cuarto { get; set; }
        private ToyCity ToyCity { get; set; }
        private SimpleTerrain SimpleTerrain { get; set; }
        private Logo Logo { get; set; }
        private Hub Hub { get; set; }
        // Matrices
        private Microsoft.Xna.Framework.Matrix View { get; set; }
        private Microsoft.Xna.Framework.Matrix Projection { get; set; }

        //------ Variables para los SpawnPoint
        public enum TipoAuto
        {
            tipoJugador,
            tipoCarrera,
            tipoCombate
        }
        private int CantidadDeAutos { get; set; }
        public List<TipoAuto> listaModelos { get; set; }
        public List<AutoEnemigo> listaAutos { get; set; }
        private List<System.Numerics.Vector3> traslacionesIniciales { get; set; }
        private List<float> angulosIniciales { get; set; }

        public int aiCount = 1;

        // ------

        private Simulation simulation;
        private BufferPool bufferPool;
        private SimpleThreadDispatcher threadDispatcher;

        private SimpleCarController playerController;


        Buffer<SimpleCarController> aiControllers;

        IPowerUp nitro;
        IPowerUp hamster;
        IPowerUp arma;

        private bool soundIsPaused = false;
        private Song _backgroundMusic;
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);

            // Consejo: Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.


            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";

            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }


        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.
            CantidadDeAutos = 70;

            traslacionesIniciales = GenerarPuntosEnCirculo(CantidadDeAutos, 700f);
            angulosIniciales = CalcularAngulosHaciaCentro(traslacionesIniciales);

            listaModelos = new List<TipoAuto>();
            listaAutos = new List<AutoEnemigo>();

            // Apago el backface culling.
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio);

            IsometricCamera = new IsometricCamera(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);

            // Configuramos nuestras matrices de la escena.
            int tessellation = 2;
            if (CantidadDeAutos % tessellation != 0) // Cuidado que aquí tienes que tener cuidado y asegurarte que sea divisible por el número.
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            listaModelos.Add(TipoAuto.tipoJugador);


            for (int i = 0; i < CantidadDeAutos / tessellation; i++)
            {
                listaModelos.Add(TipoAuto.tipoCarrera);
                listaModelos.Add(TipoAuto.tipoCombate);
                //aca se pueden agregar todos los tipos de auto que querramos, es una forma de identificar en que lugar queda cada uno, para luego instanciar clases.
            }


            // mezclar posiciones
            var random = new Random(0);
            for (int i = listaModelos.Count - 1; i > 1; i--) // Empezar desde el último índice y detenerse en el índice 1
            {
                int j = random.Next(1, i + 1); // Limitar la mezcla a los elementos a partir del índice 1
                var temp = listaModelos[i];
                listaModelos[i] = listaModelos[j];
                listaModelos[j] = temp;
            }

            _boundingFrustum = new BoundingFrustum(IsometricCamera.View * IsometricCamera.Projection);


            // INICIALIZO LOGICA DE BEPU
            iniciarSimulacion();


            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            

            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundMusic = Content.Load<Song>(ContentFolder3D + "autos/RacingCarA/backgroundmusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_backgroundMusic);
            // CARGAR LISTA DE AUTOS CON SUS INSTANCIAS
            for (int i = 1; i < CantidadDeAutos; i++) //empieza de 1, porque actualmente el autoDeJugador no es de tipoAuto, entonces no lo podemos tratar como tal. Es lo que quiero hablar con kevin
            {
                if (listaModelos[i] == TipoAuto.tipoCarrera)
                {
                    listaAutos.Add(new AutoEnemigoCarrera(Content, simulation, GraphicsDevice, aiControllers[i], traslacionesIniciales[i], angulosIniciales[i]));
                }
                if (listaModelos[i] == TipoAuto.tipoCombate)
                {
                    listaAutos.Add(new AutoEnemigoCombate(Content, simulation, GraphicsDevice, aiControllers[i], traslacionesIniciales[i], angulosIniciales[i]));
                }
                //aca se pueden agregar todos los tipos de auto que querramos, es una forma de identificar en que lugar queda cada uno, para luego instanciar clases.
            }


            // Cargo Clases
            Hub = new Hub(Content);
            //Logo = new Logo(Content);
            autoJugador = new Jugador(Content, simulation, GraphicsDevice, playerController, traslacionesIniciales[0], angulosIniciales[0]);
            ToyCity = new ToyCity(Content);
            SimpleTerrain = new SimpleTerrain(Content, GraphicsDevice);
            Toys = new Toys(Content, simulation, GraphicsDevice);
            Cuarto = new Cuarto(Content, simulation, GraphicsDevice);
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            nitro = new SuperSpeed(Content, autoJugador, new Vector3(0, 0, 0));
            hamster = new Hamster(GraphicsDevice, Content, autoJugador, new Vector3(50, 10, 50));
            arma = new Gun(Content, autoJugador, new Vector3(-50, 24, 50));


            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.

            menu = new MainMenu(/*autoJugador,*/ SpriteBatch, Content.Load<SpriteFont>(ContentFolder3D + "menu/File"), Graphics, GraphicsDevice, this);
            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {

            Console.WriteLine("Number of bodies: " + simulation.Bodies.ActiveSet.Count);
            simulation.Timestep(1f / 60f, threadDispatcher);


            var keyboardState = Keyboard.GetState();
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            // DEBUG
            if (keyboardState.IsKeyDown(Keys.C) & oldState.IsKeyUp(Keys.C))
            {
                _debugColisiones = !_debugColisiones;
            }
            // CAMBIO DE CAMARA
            if (keyboardState.IsKeyDown(Keys.Enter) & oldState.IsKeyUp(Keys.Enter))
            {
                _liberarCamara = !_liberarCamara;
            }
            if (!_liberarCamara)
            {
                autoJugador.Update(gameTime, simulation);
                IsometricCamera.Update(gameTime, autoJugador.carWorld);
                View = IsometricCamera.View;
                Projection = IsometricCamera.Projection;
                _boundingFrustum.Matrix = View * Projection;
                Hub.Update(autoJugador);
            }
            else
            {
                FreeCamera.Update(gameTime, autoJugador.carWorld);
                View = FreeCamera.View;
                Projection = FreeCamera.Projection;
            }
            
            if (keyboardState.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape))
            {
                _isMenuOpen = !_isMenuOpen;
            }

            if (_isMenuOpen)
            {
                _isMenuOpen = menu.HandleMenuInput(keyboardState);
            }

            if (keyboardState.IsKeyDown(Keys.P) && !oldState.IsKeyDown(Keys.P))
            {
                soundIsPaused = !soundIsPaused;
                if (soundIsPaused)
                    MediaPlayer.Pause();
                else
                    MediaPlayer.Resume();
            }

            // Capturar Input teclado
                
            if (keyboardState.IsKeyDown(Keys.Tab) & oldState.IsKeyUp(Keys.Tab))
            {
                _liberarCamara = !_liberarCamara;
            }

            
            Toys.Update(_boundingFrustum);
            /*      
            foreach ( var Auto in listaAutos){
                Auto.Update();
            }
            */
            oldState = keyboardState;

            arma.Update(gameTime);
            hamster.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;


            Toys.Draw(gameTime, View, Projection, IsometricCamera.CameraPosition);
            autoJugador.Draw(View, Projection, IsometricCamera.CameraPosition);
            ToyCity.Draw(gameTime, View, Projection, IsometricCamera.CameraPosition);
            SimpleTerrain.Draw(gameTime, View, Projection);
            Cuarto.Draw(gameTime, View, Projection, IsometricCamera.CameraPosition);
            foreach (var Auto in listaAutos)
            {
                Auto.Draw(gameTime, View, Projection);
            }
            //Logo.Draw(gameTime, View, Projection);

            arma.Draw(gameTime, View, Projection);
            hamster.Draw(gameTime, View, Projection);

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            if (_debugColisiones)
            {
                // dibujar las cajas de colisiones de todos los objetos
                // si se quiere dibujar un convexHull hay que usar el mtodo DrawConvexHull (esta medio cursed ese)
                Toys.DrawCollisionBoxes(View, Projection);
                Cuarto.DrawCollisionBoxes(View, Projection);
            }

            //Podríamos hacer un método para SpriteBatch de ser necesario.
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Hub.Draw(SpriteBatch, gameTime);

            SpriteBatch.End();

        
            if (_isMenuOpen)
            {
                menu.DrawMenuOverlay();
            }
        
            base.Draw(gameTime);

        }

        public void ToggleMusic()
        {
            MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();
            simulation.Dispose();
            threadDispatcher.Dispose();
            bufferPool.Clear();
            base.UnloadContent();
        }

        public List<System.Numerics.Vector3> GenerarPuntosEnCirculo(float numPuntos, float radio)
        {
            List<System.Numerics.Vector3> puntos = new List<System.Numerics.Vector3>();
            float anguloIncremento = Microsoft.Xna.Framework.MathHelper.TwoPi / numPuntos; // Divide el círculo en partes iguales
            float centroX = -900f; // Desplazamiento en X
            float centroZ = -1100f; // Desplazamiento en Z

            for (int i = 0; i < numPuntos; i++)
            {
                float angulo = i * anguloIncremento;
                float x = centroX + radio * (float)Math.Cos(angulo); // Coordenada X con desplazamiento
                float z = centroZ + radio * (float)Math.Sin(angulo); // Coordenada Z con desplazamiento
                float y = 20; // Coordenada Y fija

                puntos.Add(new System.Numerics.Vector3(x, y, z));
            }

            return puntos;
        }

        public List<float> CalcularAngulosHaciaCentro(List<System.Numerics.Vector3> posiciones)     // Gira los autos para que miren al centro
        {
            List<float> angulos = new List<float>();

            // Coordenadas del centro del círculo en el plano XZ
            float centroX = -900f;
            float centroZ = -1100f;

            foreach (var posicion in posiciones)
            {
                // Calculamos la diferencia en X y Z con respecto al centro desplazado
                float dx = centroX - posicion.X;
                float dz = centroZ - posicion.Z;

                // Calculamos el ángulo con Atan2. Intercambiamos dx y dz para invertir la dirección
                float angulo = Microsoft.Xna.Framework.MathHelper.TwoPi - (float)Math.Atan2(dz, dx) + Microsoft.Xna.Framework.MathHelper.Pi / 2;

                angulos.Add(angulo);
            }

            return angulos;
        }
        private void iniciarSimulacion()
        {

            // inicializo logica de bepu
            // setea el threadCount para el update de la simulacion de bepu
            var targetThreadCount = Math.Max(1,
                Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
            threadDispatcher = new SimpleThreadDispatcher(targetThreadCount);

            var properties = new CollidableProperty<CarBodyProperties>();
            bufferPool = new BufferPool();
            simulation = Simulation.Create(bufferPool, new CarCallbacks() { Properties = properties }, new DemoPoseIntegratorCallbacks(new System.Numerics.Vector3(0, -100, 0)), new SolveDescription(8, 1));

            var builder = new CompoundBuilder(bufferPool, simulation.Shapes, 2);
            builder.Add(new Box(50f, 30f, 100f), RigidPose.Identity, 300);
            builder.Add(new Box(40f, 30f, 50f), new System.Numerics.Vector3(0, 20f, -5f), 1f);
            builder.BuildDynamicCompound(out var children, out var bodyInertia, out _);
            builder.Dispose();
            var bodyShape = new Compound(children);
            var bodyShapeIndex = simulation.Shapes.Add(bodyShape);
            var wheelShape = new Cylinder(5f, 5f);
            var wheelInertia = wheelShape.ComputeInertia(5f);
            var wheelShapeIndex = simulation.Shapes.Add(wheelShape);

            const float x = 30f;
            const float y = -10f;
            const float frontZ = 35f;
            const float backZ = -35f;
            const float wheelBaseWidth = x * 3f;
            const float wheelBaseLength = frontZ - backZ;

            var pose = new RigidPose(traslacionesIniciales[0], System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, angulosIniciales[0]));

            var auto = SimpleCar.Create(simulation, properties, pose, bodyShapeIndex, bodyInertia, 0.5f, wheelShapeIndex, wheelInertia, 3.6f,
            new System.Numerics.Vector3(-x, y, frontZ), new System.Numerics.Vector3(x, y, frontZ), new System.Numerics.Vector3(-x, y, backZ), new System.Numerics.Vector3(x, y, backZ), new System.Numerics.Vector3(0, -1, 0), 0.25f,
            new SpringSettings(50f, 0.9f), QuaternionEx.CreateFromAxisAngle(System.Numerics.Vector3.UnitZ, MathF.PI * 0.5f));

            Console.WriteLine("Inertia: " + bodyInertia);
            playerController = new SimpleCarController(auto, forwardSpeed: 50000, forwardForce: 50000, zoomMultiplier: 3, backwardSpeed: 30000, backwardForce: 30000, idleForce: 10000f, brakeForce: 15000f, steeringSpeed: 150f, maximumSteeringAngle: MathF.PI * 0.23f,
            wheelBaseLength: wheelBaseLength, wheelBaseWidth: wheelBaseWidth, ackermanSteering: 1);

            // ACA SE INICIALIZAN LOS AUTOS DE IA
            bufferPool.Take(CantidadDeAutos - 1, out aiControllers);
            /* var random = new Random(5);
            for (int i = 1; i < CantidadDeAutos; ++i)
            {
                var position = traslacionesIniciales[i];
                var orientation = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, angulosIniciales[i]);
                aiControllers[i] = new SimpleCarController(SimpleCar.Create(simulation, properties, new RigidPose(position, orientation), bodyShapeIndex, bodyInertia, 0.5f, wheelShapeIndex, wheelInertia, 2f,
                    new System.Numerics.Vector3(-x, y, frontZ), new System.Numerics.Vector3(x, y, frontZ), new System.Numerics.Vector3(-x, y, backZ), new System.Numerics.Vector3(x, y, backZ), new System.Numerics.Vector3(0, -1, 0), 0.25f,
                    new SpringSettings(5, 0.7f), QuaternionEx.CreateFromAxisAngle(System.Numerics.Vector3.UnitZ, MathF.PI * 0.5f)),
                    forwardSpeed: 50, forwardForce: 5, zoomMultiplier: 2, backwardSpeed: 10, backwardForce: 4, idleForce: 0.25f, brakeForce: 7, steeringSpeed: 1.5f, maximumSteeringAngle: MathF.PI * 0.23f,
                    wheelBaseLength: wheelBaseLength, wheelBaseWidth: wheelBaseWidth, ackermanSteering: 1);

                //aiControllers[i].LaneOffset = random.NextSingle() * 20 - 10;
            }  */

        }


        /*
            // CARGAR LISTA DE AUTOS CON SUS INSTANCIAS
            for (int i = 1; i < CantidadDeAutos; i++) //empieza de 1, porque actualmente el autoDeJugador no es de tipoAuto, entonces no lo podemos tratar como tal. Es lo que quiero hablar con kevin
            {
                if (listaModelos[i] == TipoAuto.tipoCarrera)
                {
                    listaAutos.Add(new AutoEnemigoCarrera(Content, simulation, GraphicsDevice, traslacionesIniciales[i], angulosIniciales[i]));
                }
                if (listaModelos[i] == TipoAuto.tipoCombate)
                {
                    listaAutos.Add(new AutoEnemigoCombate(Content, simulation, GraphicsDevice, traslacionesIniciales[i], angulosIniciales[i]));
                }
                //aca se pueden agregar todos los tipos de auto que querramos, es una forma de identificar en que lugar queda cada uno, para luego instanciar clases.
            }
        */

    }
}