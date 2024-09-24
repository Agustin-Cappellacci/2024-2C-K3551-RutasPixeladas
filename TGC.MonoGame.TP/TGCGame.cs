using System;
using System.Net.Mime;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;
using System.Collections.Generic;

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
        
        FreeCamera FreeCamera { get; set; }

        private SpriteBatch SpriteBatch { get; set; }
        private Model Model { get; set; }
        private Model DeLoreanModel { get; set; }
        private Effect Effect { get; set; }
        
        private Jugador autoJugador {get; set;}
        private CityScene City { get; set; }
       // private Cars Cars { get; set; }
        private Grass Grass { get; set; }

        private Matrix CarWorld { get; set; }
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


        private bool liberarCamara = false;

        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            
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
            CantidadDeAutos = 70;
            traslacionesIniciales = GenerarPuntosEnCirculo(CantidadDeAutos, 700f);
            angulosIniciales = CalcularAngulosHaciaCentro(traslacionesIniciales); 

            listaModelos = new List<TipoAuto>();
            listaAutos = new List<AutoEnemigo>();

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
            
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio);

            CarWorld = Matrix.Identity;
          
            // Seria hasta aca.
           
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


            // Cargo el modelo del logo.
            autoJugador = new Jugador(Content);
            //Cars = new Cars(Content);
            City = new CityScene(Content);
            Grass = new Grass(Content);

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
            
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                liberarCamara = !liberarCamara;
            }
            if (!liberarCamara)
            {
                CarWorld = autoJugador.Update(gameTime, CarWorld);
                Camera.Update(gameTime, CarWorld);
                View = Camera.View;
                Projection = Camera.Projection;
            }
            else
            {
                FreeCamera.Update(gameTime, CarWorld);
                View = FreeCamera.View;
                Projection = FreeCamera.Projection;
            }

            /*  
            foreach ( var Auto in listaAutos){
                Auto.Update();
            }
            */
            

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
            

            City.Draw(gameTime, View, Projection);
            autoJugador.Draw(CarWorld,View, Projection);

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Grass.Draw(gameTime, View, Projection, CarWorld);
            
            
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