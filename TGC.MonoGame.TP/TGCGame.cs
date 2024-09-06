using System;
using System.Net.Mime;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal del juego.
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

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            
            // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private Model Model { get; set; }
        private Effect Effect { get; set; }
        private float Rotation { get; set; }

        private Model CarModel { get; set; }

        private CityScene City { get; set; }
        private Model DeLoreanModel { get; set; }

        private Matrix Scale { get; set; }
        private Matrix World { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private Vector3 CameraPosition = Vector3.UnitZ * 150;
        private Vector3 CameraForward = Vector3.Forward;
        private Vector3 CameraTarget = Vector3.Zero;
        private Vector3 CameraUp = Vector3.Up;

        private float Yaw {get; set;}

        private float Pitch {get; set;}
        

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
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            // Seria hasta aca.

           
            // Configuramos nuestras matrices de la escena.
            World = Matrix.Identity;
            Scale = Matrix.CreateScale(0.2f);
            View = Matrix.CreateLookAt(Vector3.UnitZ * 150, Vector3.Zero, Vector3.Up);
            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 2500);

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
            Model = Content.Load<Model>(ContentFolder3D + "tgc-logo/tgc-logo");

            City = new CityScene(Content);
            
            CarModel = Content.Load<Model>(ContentFolder3D+"RacingCarA/RacingCar");

           

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");
            

            // Asigno el efecto que cargue a cada parte del mesh.
            // Un modelo puede tener mas de 1 mesh internamente.
            foreach (var mesh in CarModel.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }
            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logica de actualizacion del juego.

            // Capturar Input teclado
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

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                //Salgo del juego.
                Exit(); 
            }
             // Input de teclado para mover la cámara

            // --- Captura del movimiento con WASD ---
            if (keyboardState.IsKeyDown(Keys.W))
                CameraPosition += CameraForward * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.S))
                CameraPosition -= CameraForward * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.A))
                CameraPosition -= CameraRight * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboardState.IsKeyDown(Keys.D))
                CameraPosition += CameraRight * cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            CameraTarget = CameraPosition + CameraForward;

            View = Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUp);

            World =Scale *  Matrix.CreateRotationY(Rotation);

            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
           

            GraphicsDevice.Clear(Color.White);

           

            Effect.Parameters["View"].SetValue(View);
            Effect.Parameters["Projection"].SetValue(Projection);
            
            var random = new Random(Seed:0);
            
            
            for (int i = 0; i < 200; i++){
                
                var scala = random.NextSingle() * random.NextSingle();
                // var colorcito = new Vector3((CameraPosition.X) + random.NextSingle(), CameraPosition.Y + random.NextSingle(), CameraPosition.Z + random.NextSingle());
                var color = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());         /*Usamos verto3 porque es BasicEffect. Se usa vector4 si tenemos activado el AlphaShader*/
                
                var traslacion = new Vector3(
                random.NextSingle()*1500f -5f,
                random.NextSingle(),
                random.NextSingle()*1500f -5f);



                foreach (var mesh in CarModel.Meshes)
                {
                    Effect.Parameters["DiffuseColor"].SetValue(color);
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * World * Matrix.CreateTranslation(traslacion) * Matrix.CreateScale(scala));

                    mesh.Draw();
                }
            }

            City.Draw(gameTime, View, Projection);
            /*
            foreach (var mesh in DeLorianModel.Meshes)
                {
                    Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());
                    Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * World);

                    mesh.Draw();
                }
            */
            // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
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