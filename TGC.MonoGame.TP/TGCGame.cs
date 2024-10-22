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
        private MainMenu menu;
        private bool _isMenuOpen = false;

        // Cámara
        FollowCamera Camera { get; set; }
        IsometricCamera IsometricCamera { get; set; }
        FreeCamera FreeCamera { get; set; }

        private bool liberarCamara = false;
        private KeyboardState oldState { get; set; }

        // Modelos y efectos
        private Model Model { get; set; }
        private Model DeLoreanModel { get; set; }
        private Effect Effect { get; set; }
        
        // Clases
        private Jugador autoJugador {get; set;}
        private Toys Toys { get; set; }
        private Cuarto Cuarto { get; set; }

        private Logo Logo {  get; set; }
        // Matrices
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }

        //------ Variables para los SpawnPoint
        public enum TipoAuto
        {
            tipoJugador,
            tipoCarrera,
            tipoCombate
        }        
        private float CantidadDeAutos {get; set;}
        public List<TipoAuto> listaModelos { get; set; }
        public List<AutoEnemigo> listaAutos { get; set; }
        private List<Vector3> traslacionesIniciales { get; set; }
        private List<float> angulosIniciales { get; set; }
        // ------

        private Simulation simulation;
        private BufferPool bufferPool;
        private SimpleThreadDispatcher threadDispatcher;
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

            
            
            Camera = new FollowCamera(GraphicsDevice.Viewport.AspectRatio);
            
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio);
            
            IsometricCamera = new IsometricCamera(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);

            // Configuramos nuestras matrices de la escena.
           
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

            // setea el threadCount para el update de la simulacion de bepu
            var targetThreadCount = Math.Max(1,
                Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
            threadDispatcher = new SimpleThreadDispatcher(targetThreadCount);
            
             // Inicializar la simulación de física de Bepu
            bufferPool = new BufferPool();
            var narrowPhaseCallbacks = new NarrowPhaseCallbacks(new SpringSettings(60, 1)); // Callback para manejar colisiones, rebotes, etc
            var poseIntegratorCallbacks = new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -500, 0)); // Callback para manejar gravedad
            var solveDescription = new SolveDescription(8,1);
            simulation = Simulation.Create(bufferPool, narrowPhaseCallbacks, poseIntegratorCallbacks, solveDescription);


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


            // CARGAR LISTA DE AUTOS CON SUS INSTANCIAS
            for (int i = 1; i < CantidadDeAutos; i++) //empieza de 1, porque actualmente el autoDeJugador no es de tipoAuto, entonces no lo podemos tratar como tal. Es lo que quiero hablar con kevin
            {   
                if (listaModelos[i] == TipoAuto.tipoCarrera){
                    listaAutos.Add(new AutoEnemigoCarrera(Content, traslacionesIniciales[i], angulosIniciales[i]));
                }
                if (listaModelos[i] == TipoAuto.tipoCombate){
                    listaAutos.Add(new AutoEnemigoCombate(Content, traslacionesIniciales[i], angulosIniciales[i]));
                }
                //aca se pueden agregar todos los tipos de auto que querramos, es una forma de identificar en que lugar queda cada uno, para luego instanciar clases.
            }


            // Cargo Clases
            autoJugador = new Jugador(Content, simulation,GraphicsDevice);
            Toys = new Toys(Content, simulation, GraphicsDevice);
            Cuarto = new Cuarto(Content);
            Logo = new Logo(Content, simulation, GraphicsDevice);


            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.

            menu = new MainMenu(autoJugador, SpriteBatch, Content.Load<SpriteFont>(ContentFolder3D + "menu/File"), Graphics, GraphicsDevice, this);
            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {      
            simulation.Timestep(1f/60f, threadDispatcher);
            var keyboardState = Keyboard.GetState();
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            
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
                // Aca deberiamos poner toda la logica de actualizacion del juego.

                // Capturar Input teclado
                
                if (keyboardState.IsKeyDown(Keys.Tab) & oldState.IsKeyUp(Keys.Tab))
                {
                    liberarCamara = !liberarCamara;
                }
                if (!liberarCamara)
                {
                    autoJugador.Update(gameTime);
                    IsometricCamera.Update(gameTime, autoJugador.carWorld);
                    View = IsometricCamera.View;
                    Projection = IsometricCamera.Projection;
                }
                else
                {
                    FreeCamera.Update(gameTime, autoJugador.carWorld);
                    View = FreeCamera.View;
                    Projection = FreeCamera.Projection;
                }

                /*  
                foreach ( var Auto in listaAutos){
                    Auto.Update();
                }
                */

            oldState = keyboardState;
            float fps = 1f / elapsedTime;
            Console.WriteLine("FPS: " + fps);
            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
           

            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque ;

            foreach ( var Auto in listaAutos){
                Auto.Draw(gameTime, View, Projection);
            }
            

            
            autoJugador.Draw(View, Projection);
            Toys.Draw(gameTime, View, Projection);
            Cuarto.Draw(gameTime, View, Projection);
            Logo.Draw(gameTime, View, Projection);
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            

            
            
            // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando. En el método Draw.
            
            /*
            Effect.Parameters["View"].SetValue(View);
            Effect.Parameters["Projection"].SetValue(Projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());

            foreach (var mesh in Model.Meshes)
            {
                Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * World);
                mesh.Draw();
            }
            */
        
            if (_isMenuOpen)
            {
                menu.DrawMenuOverlay();
            }
            
            base.Draw(gameTime);
        }

        public void ToggleMusic() {
            MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }

        public List<Vector3> GenerarPuntosEnCirculo(float numPuntos, float radio)
        {
            List<Vector3> puntos = new List<Vector3>();
            float anguloIncremento = MathHelper.TwoPi / numPuntos; // Divide el círculo en partes iguales
            float centroX = -900f; // Desplazamiento en X
            float centroZ = -1100f; // Desplazamiento en Z

            for (int i = 0; i < numPuntos; i++)
            {
                float angulo = i * anguloIncremento;
                float x = centroX + radio * (float)Math.Cos(angulo); // Coordenada X con desplazamiento
                float z = centroZ + radio * (float)Math.Sin(angulo); // Coordenada Z con desplazamiento
                float y = 5; // Coordenada Y fija

                puntos.Add(new Vector3(x, y, z));
            }

            return puntos;
        }

        public List<float> CalcularAngulosHaciaCentro(List<Vector3> posiciones)     // Gira los autos para que miren al centro
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
                float angulo = MathHelper.TwoPi - (float)Math.Atan2(dz, dx) + MathHelper.Pi / 2;

                angulos.Add(angulo);
            }

            return angulos;
        }

    }
}