using System;
using System.Net.Mime;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;

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

        FollowCamera Camera { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        private Model Model { get; set; }
        private Model DeLoreanModel { get; set; }
        private Effect Effect { get; set; }
        
        private Jugador autoJugador {get; set;}
        private CityScene City { get; set; }
        private Cars Cars { get; set; }
        private Grass Grass { get; set; }

        private Matrix CarWorld { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private Matrix Scale { get; set; }


        private float Rotation { get; set; }
        private float Yaw {get; set;}
        private float Pitch {get; set;}


    /*    private Vector3 CameraPosition = Vector3.UnitZ * 150;
        private Vector3 CameraForward = Vector3.Forward;
        private Vector3 CameraTarget = Vector3.Zero;
        private Vector3 CameraUp = Vector3.Up;
    */

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            /*
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            */
            
            // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
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

            // Apago el backface culling.
            // Esto se hace por un problema en el diseno del modelo del logo de la materia.
            // Una vez que empiecen su juego, esto no es mas necesario y lo pueden sacar.
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

            
            Camera = new FollowCamera(GraphicsDevice.Viewport.AspectRatio);

            CarWorld = Matrix.Identity;
            /*
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;
            */
            // Seria hasta aca.

           
            // Configuramos nuestras matrices de la escena.
            /*
            World = Matrix.Identity;
            Scale = Matrix.CreateScale(1f);
            View = Matrix.CreateLookAt(Vector3.UnitZ * 150, Vector3.Zero, Vector3.Up);
            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 250000);
            */
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

            // Cargo el modelo del logo.
            autoJugador = new Jugador(Content);
            Cars = new Cars(Content);
            City = new CityScene(Content);
            Grass = new Grass(Content);
            /*
            Model = Content.Load<Model>(ContentFolder3D + "tgc-logo/tgc-logo");
            */
            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            

            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {   
            var keyboardState = Keyboard.GetState();
            
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            // Aca deberiamos poner toda la logica de actualizacion del juego.

            // Capturar Input teclado
            /*    
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var cameraSpeed = 500f;
            var rotationSpeed = 0.02f;

            // --- Captura de la rotación con las teclas ---
            if (keyboardState.IsKeyDown(Keys.J))
                Yaw -= rotationSpeed;  // Rotar hacia la izquierda
            if (keyboardState.IsKeyDown(Keys.L))
                Yaw += rotationSpeed;  // Rotar hacia la derecha
            if (keyboardState.IsKeyDown(Keys.I))
                Pitch -= rotationSpeed; // Mirar hacia arriba
            if (keyboardState.IsKeyDown(Keys.K))
                Pitch += rotationSpeed; // Mirar hacia abajo

            // Limitar la rotación vertical para evitar que la cámara se dé vuelta
            Pitch = MathHelper.Clamp(Pitch, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);


                // Calcular la dirección hacia adelante (forward) a partir del yaw y pitch
            CameraForward = Vector3.Normalize(new Vector3(
                (float)(Math.Cos(Pitch) * Math.Cos(Yaw)),
                (float)Math.Sin(Pitch),
                (float)(Math.Cos(Pitch) * Math.Sin(Yaw))
            ));

            // También podemos calcular la dirección hacia la derecha (para el movimiento lateral)
            Vector3 CameraRight = Vector3.Cross(CameraForward, CameraUp);


                // Input de teclado para mover la cámara

            // --- Captura del movimiento con WASD ---
            if (keyboardState.IsKeyDown(Keys.W))
                if(keyboardState.IsKeyDown(Keys.LeftShift)){
                    CameraPosition += CameraForward * 10 *cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else
                CameraPosition += CameraForward * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.S))
                if(keyboardState.IsKeyDown(Keys.LeftShift)){
                    CameraPosition -= CameraForward * 10 * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else
                CameraPosition -= CameraForward * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.A))
                if(keyboardState.IsKeyDown(Keys.LeftShift)){
                    CameraPosition -= CameraRight * 10 *cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else
                CameraPosition -= CameraRight * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.D))
                if(keyboardState.IsKeyDown(Keys.LeftShift)){
                    CameraPosition += CameraRight * 10 *cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else
                CameraPosition += CameraRight * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            CameraTarget = CameraPosition + CameraForward;

            View = Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUp);

            World = Scale *  Matrix.CreateRotationY(Rotation);
            */

            CarWorld = autoJugador.Update(gameTime, CarWorld );

            Camera.Update(gameTime, CarWorld);


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
            Cars.Draw(gameTime, Camera.View, Camera.Projection);
            City.Draw(gameTime, Camera.View, Camera.Projection);
            autoJugador.Draw(CarWorld,Camera.View, Camera.Projection);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Grass.Draw(gameTime, Camera.View, Camera.Projection, CarWorld);
            
            
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
            
            base.Draw(gameTime);
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
    }
}