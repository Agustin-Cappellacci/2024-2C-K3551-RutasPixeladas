﻿using System;
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

//using System.Numerics;


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
        InitialMenu initialMenu;

        private bool _isMenuOpen = false;
        private bool _liberarCamara = false;
        private bool _debugColisiones = false;

        // Cámara
        IsometricCamera IsometricCamera { get; set; }
        FreeCamera FreeCamera { get; set; }
        BoundingFrustum _boundingFrustum { get; set; }


        private KeyboardState oldState { get; set; }

        // Modelos y efectos
        private Model Model { get; set; }
        private Model DeLoreanModel { get; set; }
        private Effect Effect { get; set; }

        //#regin Clases
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
        public static List<AutoEnemigo> listaAutos { get; set; }
        private List<System.Numerics.Vector3> traslacionesIniciales { get; set; }
        private List<float> angulosIniciales { get; set; }

        public int aiCount = 1;

        // ------

        private Simulation simulation;
        private BufferPool bufferPool;
        private SimpleThreadDispatcher threadDispatcher;

        private SimpleCarController playerController;
        private SimpleCarController enemyController;

        private BodyHandle playerBodyHandle;
        private BodyHandle enemyBodyHandle;

        private System.Numerics.Vector3 posicionJugador;

        private CarControllerContainer carControllerContainer;
        private AutoJugadorWrapper autoJugadorWrapper {get; set;}

        private CarCallbacks carCallbacks;


        Buffer<SimpleCarController> aiControllers;

        IPowerUp nitro;
        IPowerUp hamster;
        IPowerUp arma;

        IPowerUp hamster2;
        IPowerUp arma2;

        IPowerUp hamster3;
        IPowerUp arma3;

        private bool soundIsPaused = false;
        private Song _backgroundMusic;
        private bool isInitialMenuOpen = true;
        private bool lastDraw = false;

        private Microsoft.Xna.Framework.Vector3 lightPosition;
        private Microsoft.Xna.Framework.Vector3 lightDirection; // La dirección de la luz (hacia adelante)

        private RenderTargetCube EnvironmentMapRenderTarget { get; set; }
        private StaticCamera CubeMapCamera { get; set; }

        private const int EnvironmentmapSize = 2048;


        #region Variables Post Porcesado
        private const int PassCount = 2;

        private Effect _effect;
        private Effect _blurEffect;

        private RenderTarget2D _firstPassBloomRenderTarget;

        private FullScreenQuad _fullScreenQuad;

        private RenderTarget2D _mainSceneRenderTarget;

        private RenderTarget2D _secondPassBloomRenderTarget;
        private RenderTarget2D _horizontalRenderTarget;

        #endregion

        public static List<BodyHandle> listaBodyHandle;
        private List<BodyHandle> enemyBodyHandles;
        private List<SimpleCarController> enemyControllers;


        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            // Consejo: Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
            Graphics.ToggleFullScreen();

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
            #region Logica Autos
            CantidadDeAutos = 6;    //tiene que ser par

            traslacionesIniciales = GenerarPuntosEnCirculo(CantidadDeAutos, 700f);
            angulosIniciales = CalcularAngulosHaciaCentro(traslacionesIniciales);

            listaModelos = new List<TipoAuto>();
            listaAutos = new List<AutoEnemigo>();
            enemyBodyHandles = new List<BodyHandle>();
            enemyControllers = new List<SimpleCarController>();

            #endregion
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
            #region Autos Enemigos
            int tessellation = 2;
            if (CantidadDeAutos % tessellation != 0) // Aquí tienes que tener cuidado y asegurarte que sea divisible por el número de tesselation.
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            for (int i = 0; i < CantidadDeAutos; i++)
            {
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
            #endregion
            // para optimizar
            _boundingFrustum = new BoundingFrustum(IsometricCamera.View * IsometricCamera.Projection);

            CubeMapCamera = new StaticCamera(1f, Vector3.UnitX * -500f, Vector3.UnitX, Vector3.Up);
            CubeMapCamera.BuildProjection(1f, 1f, 3000f, Microsoft.Xna.Framework.MathHelper.PiOver2);

            listaBodyHandle = new List<BodyHandle>();
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

            #region Music
            _backgroundMusic = Content.Load<Song>(ContentFolder3D + "autos/RacingCarA/backgroundmusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_backgroundMusic);
            #endregion

            #region Cargo Clases
            Hub = new Hub(Content);
            //Logo = new Logo(Content);
            autoJugador = new Jugador(Content, simulation, GraphicsDevice, carControllerContainer.Controller, traslacionesIniciales[0], angulosIniciales[0], playerBodyHandle);
            autoJugadorWrapper.AutoJugador = autoJugador;

            autoJugadorWrapper.autoEnemigos = new List<AutoEnemigo>();

            ToyCity = new ToyCity(Content);
            SimpleTerrain = new SimpleTerrain(Content, GraphicsDevice);
            Toys = new Toys(Content, simulation, GraphicsDevice);
            Cuarto = new Cuarto(Content, simulation, GraphicsDevice);
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            hamster = new Hamster(GraphicsDevice, Content, autoJugador, new System.Numerics.Vector3(50, 10, 50));
            arma = new Gun(GraphicsDevice, Content, autoJugador, new System.Numerics.Vector3(300, 32, -603));
            hamster2 = new Hamster(GraphicsDevice, Content, autoJugador, new System.Numerics.Vector3(-900, 10, 200));
            arma2 = new Gun(GraphicsDevice, Content, autoJugador, new System.Numerics.Vector3(800, 32, 0));
            hamster3 = new Hamster(GraphicsDevice, Content, autoJugador, new System.Numerics.Vector3(-900, 70, -1050));
            arma3 = new Gun(GraphicsDevice, Content, autoJugador, new System.Numerics.Vector3(-1100, 32, 503));
            for (int i = 0; i < CantidadDeAutos; i++) //empieza de 1, porque actualmente el autoDeJugador no es de tipoAuto, entonces no lo podemos tratar como tal. Es lo que quiero hablar con kevin
            {
                /*if (listaModelos[i] == TipoAuto.tipoCarrera)
                {
                    listaAutos.Add(new AutoEnemigoCarrera(Content, simulation, GraphicsDevice,traslacionesIniciales[i], angulosIniciales[i], enemyBodyHandle));
                }*/
                if (listaModelos[i] == TipoAuto.tipoCombate)
                {   
                    var a = new AutoEnemigoCombate(Content, simulation, GraphicsDevice, new System.Numerics.Vector3(100, 100, 100), angulosIniciales[0], enemyBodyHandles[i]);
                    listaAutos.Add(a);
                    autoJugadorWrapper.autoEnemigos.Add(a);
                }
                //aca se pueden agregar todos los tipos de auto que querramos, es una forma de identificar en que lugar queda cada uno, para luego instanciar clases.
            }
            #endregion

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.

            menu = new MainMenu(/*autoJugador,*/ SpriteBatch, Content.Load<SpriteFont>(ContentFolder3D + "menu/File"), Graphics, GraphicsDevice, this);
            initialMenu = new InitialMenu(/*autoJugador,*/ Content, SpriteBatch, Content.Load<SpriteFont>(ContentFolder3D + "menu/File"), Graphics, GraphicsDevice, this);
            initialMenu.Initialize();
            initialMenu.LoadContent();
            loadBloom();

            // Create a render target for the menu scene
            EnvironmentMapRenderTarget = new RenderTargetCube(GraphicsDevice, EnvironmentmapSize, false,
                SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

            base.LoadContent();
        }
        protected void loadBloom()
        {
            // Load the base bloom pass effect
            _effect = Content.Load<Effect>(ContentFolderEffects + "Bloom");

            // Load the blur effect to blur the bloom texture
            _blurEffect = Content.Load<Effect>(ContentFolderEffects + "GaussianBlur");
            _blurEffect.Parameters["screenSize"]
                .SetValue(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));

            // Create a full screen quad to post-process
            _fullScreenQuad = new FullScreenQuad(GraphicsDevice);

            // Create render targets. 
            // MainRenderTarget is used to store the scene color
            // BloomRenderTarget is used to store the bloom color and switches with MultipassBloomRenderTarget
            // depending on the pass count, to blur the bloom color
            _mainSceneRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0,
                RenderTargetUsage.DiscardContents);
            _firstPassBloomRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0,
                RenderTargetUsage.DiscardContents);
            _secondPassBloomRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None, 0,
                RenderTargetUsage.DiscardContents);
            _horizontalRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None, 0,
                RenderTargetUsage.DiscardContents);
        }
        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {

            Console.WriteLine("Number of bodies: " + simulation.Bodies.ActiveSet.Count);

            autoJugador.isGrounded = false;
            simulation.Timestep(1f / 60f, threadDispatcher);

            var keyboardState = Keyboard.GetState();
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            if (isInitialMenuOpen) {
                initialMenu.Update(gameTime);
                isInitialMenuOpen = initialMenu.HandleMenuInput(keyboardState, oldState);
                if (!isInitialMenuOpen) {
                    lastDraw = true;
                }
                oldState = keyboardState;
                base.Update(gameTime);
                return;
            }
            // DEBUG
            if (keyboardState.IsKeyDown(Keys.C) & oldState.IsKeyUp(Keys.C))
            {
                _debugColisiones = !_debugColisiones;
            }
            // CAMBIO DE CAMARA
            if (!_liberarCamara)
            {
                autoJugador.Update(gameTime, simulation);
                posicionJugador = autoJugador.carPosition;

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
                _isMenuOpen = menu.HandleMenuInput(keyboardState, oldState);
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



            for (int i = 0; i < CantidadDeAutos; i++)
            {
                listaAutos[i].Update(gameTime, simulation, enemyControllers[i], posicionJugador);
            }

            arma.Update(gameTime, listaAutos);
            hamster.Update(gameTime, listaAutos);
            arma2.Update(gameTime, listaAutos);
            hamster2.Update(gameTime, listaAutos);
            arma3.Update(gameTime, listaAutos);
            hamster3.Update(gameTime, listaAutos);

            Cuarto.Update(_boundingFrustum);
            Toys.Update(_boundingFrustum);

            lightPosition = Microsoft.Xna.Framework.Vector3.Transform(new Microsoft.Xna.Framework.Vector3(0, 0, 0), autoJugador.carWorld);
            lightDirection = autoJugador.forwardVector;

            CubeMapCamera.Position = autoJugador.carPosition;
            
            oldState = keyboardState;

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


            if (isInitialMenuOpen || lastDraw) {
                if (lastDraw) {
                    lastDraw = false;
                }
                initialMenu.Draw(gameTime, Projection);
                base.Draw(gameTime);
                return;
            }
            

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            #region Pass 1-6

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Draw to our cubemap from the robot position
            for (var face = CubeMapFace.PositiveX; face <= CubeMapFace.NegativeZ; face++)
            {
                // Set the render target as our cubemap face, we are drawing the scene in this texture
                GraphicsDevice.SetRenderTarget(EnvironmentMapRenderTarget, face);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

                SetCubemapCameraForOrientation(face);
                CubeMapCamera.BuildView();

                // Draw our scene. Do not draw our tank as it would be occluded by itself 
                // (if it has backface culling on)
                //Scene.Draw(Matrix.Identity, CubeMapCamera.View, CubeMapCamera.Projection);
                Toys.DrawCube(gameTime, CubeMapCamera.View, CubeMapCamera.Projection, autoJugador.carPosition, lightPosition, lightDirection);

                ToyCity.DrawCube(gameTime, CubeMapCamera.View, CubeMapCamera.Projection);

                Cuarto.DrawCube(gameTime, CubeMapCamera.View, CubeMapCamera.Projection, IsometricCamera.CameraPosition, lightPosition, lightDirection);

            }

            #endregion

            #region Pass 7

            // Set the render target as null, we are drawing on the screen!
            GraphicsDevice.SetRenderTarget(_mainSceneRenderTarget);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

            Toys.Draw(gameTime, View, Projection, autoJugador.carPosition, lightPosition, lightDirection);
            
            ToyCity.Draw(gameTime, View, Projection, IsometricCamera.CameraPosition, lightPosition, lightDirection);
            //SimpleTerrain.Draw(gameTime, View, Projection);
            Cuarto.Draw(gameTime, View, Projection, IsometricCamera.CameraPosition, lightPosition, lightDirection);
            
            autoJugador.Draw(View, Projection, IsometricCamera.CameraPosition, EnvironmentMapRenderTarget);
            foreach (var auto in listaAutos){
                auto.Draw(gameTime, View, Projection);
            }
            //Logo.Draw(gameTime, View, Projection);
            
            arma.Draw(gameTime, View, Projection);
            hamster.Draw(gameTime, View, Projection);
            arma2.Draw(gameTime, View, Projection);
            hamster2.Draw(gameTime, View, Projection);
            arma3.Draw(gameTime, View, Projection);
            hamster3.Draw(gameTime, View, Projection);

            #endregion
            
            #region Pass 8 Bloom

            // Set the render target as our bloomRenderTarget, we are drawing the bloom color into this texture
            GraphicsDevice.SetRenderTarget(_firstPassBloomRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            _effect.CurrentTechnique = _effect.Techniques["BloomPass"];
            _effect.Parameters["baseTexture"].SetValue(hamster.textura);
            _effect.Parameters["principalColor"].SetValue(new Vector3(0.73f, 0.46f, 0.29f));
            // We get the base transform for each mesh
            var modelMeshesBaseTransforms = new Microsoft.Xna.Framework.Matrix[hamster.modelo.Bones.Count];
            hamster.modelo.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);
            foreach (var modelMesh in hamster.modelo.Meshes)
            {
                foreach (var part in modelMesh.MeshParts)
                    part.Effect = _effect;

                // We set the main matrices for each mesh to draw
                var worldMatrix = modelMeshesBaseTransforms[modelMesh.ParentBone.Index];
                
                // WorldViewProjection is used to transform from model space to clip space
                _effect.Parameters["WorldViewProjection"].SetValue(worldMatrix * hamster.world * View * Projection);

                // Once we set these matrices we draw
                modelMesh.Draw();
                _effect.Parameters["WorldViewProjection"].SetValue(worldMatrix * hamster2.world * View * Projection);
                modelMesh.Draw();
                _effect.Parameters["WorldViewProjection"].SetValue(worldMatrix * hamster3.world * View * Projection);
                modelMesh.Draw();
            }
            _effect.Parameters["baseTexture"].SetValue(arma.textura);
            _effect.Parameters["principalColor"].SetValue(new Vector3(0.56f, 0.57f, 0.59f));
            modelMeshesBaseTransforms = new Microsoft.Xna.Framework.Matrix[arma.modelo.Bones.Count];
            arma.modelo.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);
            foreach (var modelMesh in arma.modelo.Meshes)
            {
                foreach (var part in modelMesh.MeshParts)
                    part.Effect = _effect;

                // We set the main matrices for each mesh to draw
                var worldMatrix = modelMeshesBaseTransforms[modelMesh.ParentBone.Index];

                // WorldViewProjection is used to transform from model space to clip space
                _effect.Parameters["WorldViewProjection"].SetValue(worldMatrix * arma.world * View * Projection);

                // Once we set these matrices we draw
                modelMesh.Draw();
                _effect.Parameters["WorldViewProjection"].SetValue(worldMatrix * arma2.world * View * Projection);
                modelMesh.Draw();
                _effect.Parameters["WorldViewProjection"].SetValue(worldMatrix * arma3.world * View * Projection);
                modelMesh.Draw();
            }
            #endregion
            #region Multipass Bloom

            // Now we apply a blur effect to the bloom texture
            // Note that we apply this a number of times and we switch
            // the render target with the source texture
            // Basically, this applies the blur effect N times


            var bloomTexture = _firstPassBloomRenderTarget;
            var horizontalBlur = _horizontalRenderTarget;
            var finalBloomRenderTarget = _secondPassBloomRenderTarget;

            for (var index = 0; index < PassCount; index++)
            {
                //Exchange(ref SecondaPassBloomRenderTarget, ref FirstPassBloomRenderTarget);

                // Set the render target as null, we are drawing into the screen now!
                GraphicsDevice.SetRenderTarget(horizontalBlur);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
                _blurEffect.CurrentTechnique = _blurEffect.Techniques["BlurHorizontalTechnique"];
                _blurEffect.Parameters["baseTexture"].SetValue(bloomTexture);
                _fullScreenQuad.Draw(_blurEffect);

                GraphicsDevice.SetRenderTarget(finalBloomRenderTarget);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
                _blurEffect.CurrentTechnique = _blurEffect.Techniques["BlurVerticalTechnique"];
                _blurEffect.Parameters["baseTexture"].SetValue(horizontalBlur);
                _fullScreenQuad.Draw(_blurEffect);

                if (index != PassCount - 1)
                {
                    var auxiliar = bloomTexture;
                    bloomTexture = finalBloomRenderTarget;
                    finalBloomRenderTarget = auxiliar;
                }
            }

            #endregion
            #region Final Pass

            // Set the depth configuration as none, as we don't use depth in this pass
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            // Set the render target as null, we are drawing into the screen now!
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            // Set the technique to our blur technique
            // Then draw a texture into a full-screen quad
            // using our rendertarget as texture
            _effect.CurrentTechnique = _effect.Techniques["Integrate"];
            _effect.Parameters["baseTexture"].SetValue(_mainSceneRenderTarget);
            _effect.Parameters["bloomTexture"].SetValue(finalBloomRenderTarget);
            _fullScreenQuad.Draw(_effect);

            #endregion
            
             
            #region Hub and Colitions
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            if (_debugColisiones)
            {
                // dibujar las cajas de colisiones de todos los objetos
                // si se quiere dibujar un convexHull hay que usar el mtodo DrawConvexHull (esta medio cursed ese)
                Toys.DrawCollisionBoxes(View, Projection);
                Cuarto.DrawCollisionBoxes(View, Projection);
            } // caja de colisiones

            //Podríamos hacer un método para SpriteBatch de ser necesario.
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Hub.Draw(SpriteBatch, gameTime, autoJugador);

            SpriteBatch.End();

            #endregion
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

            EnvironmentMapRenderTarget.Dispose();

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
            carControllerContainer = new CarControllerContainer(); // Contenedor vacío al principio.
            autoJugadorWrapper = new AutoJugadorWrapper();
            bufferPool = new BufferPool();
            carCallbacks = new CarCallbacks() { Properties = properties, ControllerContainer = carControllerContainer, AutoJugadorWrapper = autoJugadorWrapper};
            simulation = Simulation.Create(bufferPool, carCallbacks, new DemoPoseIntegratorCallbacks(new System.Numerics.Vector3(0, -100, 0)), new SolveDescription(8, 1));

            var builder = new CompoundBuilder(bufferPool, simulation.Shapes, 2);
            builder.Add(new Box(10f, 30f, 100f), RigidPose.Identity, 350);
            builder.Add(new Box(40f, 30f, 50f), new System.Numerics.Vector3(0, 20f, -5f), 1f);
            builder.BuildDynamicCompound(out var children, out var bodyInertia, out _);
            builder.Dispose();
            var bodyShape = new Compound(children);
            var bodyShapeIndex = simulation.Shapes.Add(bodyShape);
            var wheelShape = new Cylinder(5f, 5f);
            var wheelInertia = wheelShape.ComputeInertia(10f);
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
            playerBodyHandle = auto.Body;

            listaBodyHandle.Add(playerBodyHandle);

            // Actualiza el contenedor con el `CarController` después de su creación.
            carControllerContainer.Controller = playerController;
            carCallbacks.ControllerContainer = carControllerContainer;

            // ACA SE INICIALIZAN LOS AUTOS DE IA
            bufferPool.Take(CantidadDeAutos - 1, out aiControllers);

            
            var random = new Random(1);
            float maxRandom = int.MaxValue;
            for (int i = 0; i < CantidadDeAutos; ++i)
            {
                float randomNumber = random.Next();
                float randomIndex = randomNumber / maxRandom;
                var poseEnemigo = new RigidPose(new System.Numerics.Vector3(randomIndex * 3500f - 1500f, 100f, randomIndex * 3000f - 1500f), System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, angulosIniciales[0]));
                var autoEnemigo = SimpleCar.Create(simulation, properties, poseEnemigo, bodyShapeIndex, bodyInertia, 0.5f, wheelShapeIndex, wheelInertia, 3.6f,
                new System.Numerics.Vector3(-x, y, frontZ), new System.Numerics.Vector3(x, y, frontZ), new System.Numerics.Vector3(-x, y, backZ), new System.Numerics.Vector3(x, y, backZ), new System.Numerics.Vector3(0, -1, 0), 0.25f,
                new SpringSettings(50f, 0.9f), QuaternionEx.CreateFromAxisAngle(System.Numerics.Vector3.UnitZ, MathF.PI * 0.5f));
                enemyController = new SimpleCarController(autoEnemigo, forwardSpeed: 15000 + 20000 * randomIndex, forwardForce: 15000 + 20000 * randomIndex, zoomMultiplier: 3, backwardSpeed: 20000, backwardForce: 20000, idleForce: 10000f, brakeForce: 15000f, steeringSpeed: 150f, maximumSteeringAngle: MathF.PI * 0.23f,
                wheelBaseLength: wheelBaseLength, wheelBaseWidth: wheelBaseWidth, ackermanSteering: 1);
                enemyBodyHandles.Add(autoEnemigo.Body);
                enemyControllers.Add(enemyController);
                listaBodyHandle.Add(enemyBodyHandle);
            }




        }
        private void SetCubemapCameraForOrientation(CubeMapFace face)
        {
            switch (face)
            {
                default:
                case CubeMapFace.PositiveX:
                    CubeMapCamera.FrontDirection = -Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Up;
                    break;

                case CubeMapFace.NegativeX:
                    CubeMapCamera.FrontDirection = Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Up;
                    break;

                case CubeMapFace.PositiveY:
                    CubeMapCamera.FrontDirection = Vector3.Up;
                    CubeMapCamera.UpDirection = Vector3.UnitZ;
                    break;

                case CubeMapFace.NegativeY:
                    CubeMapCamera.FrontDirection = Vector3.Down;
                    CubeMapCamera.UpDirection = -Vector3.UnitZ;
                    break;

                case CubeMapFace.PositiveZ:
                    CubeMapCamera.FrontDirection = -Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Up;
                    break;

                case CubeMapFace.NegativeZ:
                    CubeMapCamera.FrontDirection = Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Up;
                    break;
            }
        }
    }
}