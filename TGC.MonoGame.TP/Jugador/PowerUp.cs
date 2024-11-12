
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TGC.MonoGame.TP;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TGC.MonoGame.Samples.Collisions;
using System.Runtime.Serialization;
using System.Diagnostics.Contracts;
using TGC.MonoGame.TP.Content.Models;
using System.Xml.Linq;

namespace TGC.MonoGame.TP.Content.Models
{
    public abstract class IPowerUp
    {

        
        public Model modelo;
        public Effect efectoPwUP;
        public Jugador jugador;
        protected BoundingBox ColisionCaja;
        protected float rotationSpeed = 1f; // Velocidad de rotación
        protected float verticalSpeed = 0.5f; // Velocidad de movimiento vertical
        protected float maxHeight = 20f; // Altura máxima del movimiento vertical
        protected float currentHeight = 0f;
        protected bool goingUp = true;
        public float balasRestantes = 5;
        protected Vector3 PosicionInicial;
        protected GraphicsDevice graphicsDevice;
        private SoundEffect _powerUpSound;
        public SoundEffectInstance _powerUpSoundInstance;
        private bool isMuted = false;
        public Matrix world;
        public Texture2D textura;
        public const string ContentFolderEffects = "Effects/";
        protected abstract void CargarModelo(ContentManager content);

        public bool Seleccionado = false;
        public bool AreAABBsTouching = false;

        public abstract void Update(GameTime gameTime, List<AutoEnemigo> listaAutos);



        public abstract void Draw(GameTime gametime, Matrix View, Matrix Projection);


        public IPowerUp(ContentManager content, Jugador jugador, Vector3 posInicial)
        {
            this.jugador = jugador;
            this.PosicionInicial = posInicial;
            CargarModelo(content);
            efectoPwUP = content.Load<Effect>(ContentFolderEffects + "ModelsTexture");
            _powerUpSound = content.Load<SoundEffect>("Models/autos/RacingCarA/powerup");
            _powerUpSoundInstance = _powerUpSound.CreateInstance();
        }

        public void DrawBoundingBox(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            var corners = ColisionCaja.GetCorners();
            var vertices = new VertexPositionColor[24];

            // Define color para las líneas del bounding box
            var color = Color.Red;

            // Asigna los vértices de las líneas de cada borde del bounding box
            int[] indices = { 0, 1, 1, 2, 2, 3, 3, 0, // Bottom face
                            4, 5, 5, 6, 6, 7, 7, 4, // Top face
                            0, 4, 1, 5, 2, 6, 3, 7  // Vertical edges
                            };

            for (int i = 0; i < corners.Length; i++)
            {
                vertices[i] = new VertexPositionColor(corners[i], color);
            }

            BasicEffect basicEffect = new BasicEffect(graphicsDevice)
            {
                World = Matrix.Identity,
                View = view,
                Projection = projection,
                VertexColorEnabled = true
            };

            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, vertices.Length,indices,0, 12);
            }
        }
        public abstract void Apply();
    }
    class SuperSpeed : IPowerUp
    {
        //private BoundingBox ColisionCaja { get; set; }
        public SuperSpeed(ContentManager content, Jugador jugador, Vector3 posInicial) : base(content, jugador, posInicial)
        {
            this.jugador = jugador;
            this.PosicionInicial = posInicial;
            CargarModelo(content);
        }

        public override void Apply()
        {
            _powerUpSoundInstance.Play();
            jugador.CarSpeed = 10000;
        }

        public override void Draw(GameTime gametime, Matrix View, Matrix Projection) { }

        public override void Update(GameTime gameTime, List<AutoEnemigo> listaAutos) { }

        protected override void CargarModelo(ContentManager content)
        {
            /* this.modelo = content.Load<Model>("powerUp/3078-eagle-handgun-3d-model/Eagle");

             foreach (var mesh in modelo.Meshes)
             {
                 // Aquí verificas si el nombre del mesh corresponde a una rueda
                 foreach (var meshPart in mesh.MeshParts)
                 {
                     meshPart.Effect = efectoPwUP;
                 }
             }*/
        }

    }

    class Gun : IPowerUp
    {

        private Vector3 posicion;
        /*
        private BoundingBox ColisionCaja { get; set; }
        private float rotationSpeed = 1f; // Velocidad de rotación
        private float verticalSpeed = 0.5f; // Velocidad de movimiento vertical
        private float maxHeight = 7f; // Altura máxima del movimiento vertical
        private float currentHeight = 0f;
        private bool goingUp = true;
        private GraphicsDevice graphicsDevice;
        public float balasRestantes = 5;
        */

        public float radioDeteccion = 5f;
        private Matrix AimRotation = Matrix.Identity;


        public Gun(GraphicsDevice graphicsDevice, ContentManager content, Jugador jugador, Vector3 posInicial) : base(content, jugador, posInicial)
        {

            this.graphicsDevice = graphicsDevice;
            this.jugador = jugador;
            this.PosicionInicial = posInicial;
            CargarModelo(content);

            ColisionCaja = new BoundingBox((posInicial + new Vector3(-15f, -30f, -20f)), (posInicial) + new Vector3(20f, 40f, 10f));

        }

        public override void Apply()
        {
            if (balasRestantes > 0)
            {
                balasRestantes--;
            }
            else
            {
                Seleccionado = false;
                jugador.powerUp = null;
                balasRestantes = 5;
            }
            //    jugador.CarSpeed = 10000;
        }

        public static BoundingBox ModificarDimensiones(BoundingBox cajaOriginal, Vector3 nuevaDimension)
        {
            // Calcular el centro de la caja original
            Vector3 centro = (cajaOriginal.Min + cajaOriginal.Max) / 2;

            // Calcular los nuevos límites Min y Max en función del centro y las dimensiones deseadas
            Vector3 nuevoMin = centro - nuevaDimension / 2;
            nuevoMin = new Vector3(nuevoMin.X, 0, nuevoMin.Z);
            Vector3 nuevoMax = centro + nuevaDimension / 2;
            nuevoMax = new Vector3(nuevoMax.X, 100, nuevoMax.Z);

            // Crear una nueva BoundingBox con los nuevos límites
            return new BoundingBox(nuevoMin, nuevoMax);
        }


        protected override void CargarModelo(ContentManager content)
        {
            this.modelo = content.Load<Model>("poweUp/3078-eagle-handgun-3d-model/Eagle");
            this.textura = content.Load<Texture2D>("poweUp/3078-eagle-handgun-3d-model/Texture/Eagle_02 - Default_AlbedoTransparency");

            foreach (var mesh in modelo.Meshes)
            {
                // Aquí verificas si el nombre del mesh corresponde a una rueda
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = efectoPwUP;
                }
            }
        }

        override public void Update(GameTime gameTime, List<AutoEnemigo> listaAutos)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Acumular la rotación en el eje Y
            rotationSpeed += deltaTime; // rotationSpeed actúa ahora como un acumulador de rotación

            // Crear la rotación en base al acumulador
            Matrix rotation = Matrix.CreateRotationY(rotationSpeed);

            // Movimiento vertical oscilante
            if (goingUp)
            {
                currentHeight += verticalSpeed;
                if (currentHeight >= maxHeight)
                {
                    goingUp = false;
                }
            }
            else
            {
                currentHeight -= verticalSpeed;
                if (currentHeight <= -maxHeight)
                {
                    goingUp = true;
                }
            }

            this.AreAABBsTouching = ColisionCaja.Intersects(jugador.ColisionCaja);

            if (this.AreAABBsTouching && jugador.powerUp == null && jugador.tiempoRestante <= 0)
            {
                jugador.powerUp = this;
                Seleccionado = true;
            }

            if (Seleccionado)
            {
                bool autoCerca = false;
                // Ajusta la posición en relación con la rotación y posición del coche
                /*foreach (var auto in listaAutos)
                    {
                        Vector3 autoPosicion = new Vector3(0,0,0); //aca iria auto.carposition

                        float distancia = Vector3.Distance(posicion, autoPosicion);

                        if (distancia <= radioDeteccion)
                        {
                            autoCerca = true;
                            // Calcular la dirección hacia el auto
                            Vector3 direccionHaciaAuto = Vector3.Normalize(autoPosicion - posicion);

                            // Mantener la rotación del auto y ajustar para que apunte al auto detectado
                            Microsoft.Xna.Framework.Quaternion rotacionApuntado = Microsoft.Xna.Framework.Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(posicion, direccionHaciaAuto, Vector3.Up));

                            // Combina la rotación del coche con la rotación hacia el objetivo
                            AimRotation = jugador.rotationMatrix * Matrix4x4.CreateFromQuaternion(ToNumericsQuaternion(rotacionApuntado));
                        }
                    }
                if (!autoCerca){
                    AimRotation = 
                }*/

                var offset = new System.Numerics.Vector3(0, 40f, 0); // Altura sobre el techo
                posicion = jugador.carPosition + Vector3.Transform(offset, jugador.rotationMatrix); // Usa la rotación del coche
                world = Matrix.CreateScale(1.5f) * jugador.rotationMatrix * Matrix.CreateTranslation(posicion);

            }
            else
            {
                world = Matrix.CreateScale(2f) * rotation * Matrix.CreateTranslation(PosicionInicial.X, PosicionInicial.Y + currentHeight, PosicionInicial.Z);
            }

            // Aplicar los movimientos al modelo
            // Aquí deberías aplicar esta matriz `world` a la transformación del modelo en el juego
            //AreAABBsTouching = ColisionCaja.Intersects(jugador.ColisionCaja);
        }

        public static System.Numerics.Quaternion ToNumericsQuaternion(Microsoft.Xna.Framework.Quaternion xnaQuaternion)
        {
            return new System.Numerics.Quaternion(xnaQuaternion.X, xnaQuaternion.Y, xnaQuaternion.Z, xnaQuaternion.W);
        }
        public override void Draw(GameTime gametime, Matrix View, Matrix Projection)
        {
            efectoPwUP.Parameters["View"].SetValue(View);
            efectoPwUP.Parameters["Projection"].SetValue(Projection);
            efectoPwUP.Parameters["ModelTexture"].SetValue(textura);

            var random = new Random(Seed: 0);

            var modelMeshesBaseTransforms = new Matrix[modelo.Bones.Count];
            modelo.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach (var mesh in modelo.Meshes)
            {
                var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                // We set the main matrices for each mesh to draw
                efectoPwUP.Parameters["World"].SetValue(meshWorld * world);

                // Draw the mesh
                mesh.Draw();

            }
            DrawBoundingBox(graphicsDevice, View, Projection);
        }

       
    }

    class Hamster : IPowerUp
    {
        /*
        private float rotationSpeed = 1f; // Velocidad de rotación
        private float verticalSpeed = 0.5f; // Velocidad de movimiento vertical
        private float maxHeight = 7f; // Altura máxima del movimiento vertical
        private float currentHeight = 0f;
        private bool goingUp = true;
        private GraphicsDevice graphicsDevice;
        private BoundingBox ColisionCaja { get; set; }
        */
        private Vector3 posicion;
        public Hamster(GraphicsDevice graphicsDevice, ContentManager content, Jugador jugador, Vector3 posInicial) : base(content, jugador, posInicial)
        {
            this.graphicsDevice = graphicsDevice;
            this.jugador = jugador;
            this.PosicionInicial = posInicial;
            CargarModelo(content);
            ColisionCaja = new BoundingBox((posInicial + new Vector3(-28f, -13f, -28f)), (posInicial + new Vector3(33f, 60f, 32f)));    
            //Seleccionado = true;
            listaBalas = new List<BalaHamster>();
        }

        public static BoundingBox ModificarDimensiones(BoundingBox cajaOriginal, Vector3 nuevaDimension)
        {
            // Calcular el centro de la caja original
            Vector3 centro = (cajaOriginal.Min + cajaOriginal.Max) / 2;

            // Calcular los nuevos límites Min y Max en función del centro y las dimensiones deseadas
            Vector3 nuevoMin = centro - nuevaDimension / 2;
            nuevoMin = new Vector3(nuevoMin.X, 0, nuevoMin.Z);
            Vector3 nuevoMax = centro + nuevaDimension / 2;

            // Crear una nueva BoundingBox con los nuevos límites
            return new BoundingBox(nuevoMin, nuevoMax);
        }

        protected override void CargarModelo(ContentManager content)
        {
            this.modelo = content.Load<Model>("poweUp/hamster-3d-model/Humster");
            this.textura = content.Load<Texture2D>("poweUp/hamster-3d-model/Hamster_UV");
            foreach (var mesh in modelo.Meshes)
            {
                // Aquí verificas si el nombre del mesh corresponde a una rueda
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = efectoPwUP;
                }
            }
        }

        override public void Update(GameTime gameTime, List<AutoEnemigo> listaAutos)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Acumular la rotación en el eje Y
            rotationSpeed += deltaTime; // rotationSpeed actúa ahora como un acumulador de rotación

            // Crear la rotación en base al acumulador
            Matrix rotation = Matrix.CreateRotationY(rotationSpeed);

            // Movimiento vertical oscilante
            if (goingUp)
            {
                currentHeight += verticalSpeed;
                if (currentHeight >= maxHeight)
                {
                    goingUp = false;
                }
            }
            else
            {
                currentHeight -= verticalSpeed;
                if (currentHeight <= -maxHeight)
                {
                    goingUp = true;
                }
            }

            Console.WriteLine("AreAABBsTouching: " + AreAABBsTouching);
            AreAABBsTouching = ColisionCaja.Intersects(jugador.ColisionCaja);

            if (AreAABBsTouching && jugador.powerUp == null && jugador.tiempoRestante <= 0f)
            {
                jugador.powerUp = this;
                Seleccionado = true;
            }

            if (Seleccionado)
            {
                // Ajusta la posición en relación con la rotación y posición del coche
                var offset = new System.Numerics.Vector3(0, 25f, 0); // Altura sobre el techo
                posicion = jugador.carPosition + Vector3.Transform(offset, jugador.rotationMatrix); // Usa la rotación del coche
                world = Matrix.CreateScale(0.05f) * jugador.rotationMatrix * Matrix.CreateTranslation(posicion);
            }
            else
            {
                world = Matrix.CreateScale(0.1f) * rotation * Matrix.CreateTranslation(PosicionInicial.X, PosicionInicial.Y + currentHeight, PosicionInicial.Z);
            }

            if (listaBalas.Count != 0)
            {
                Console.WriteLine("count: " + listaBalas.Count);
                foreach (BalaHamster bala in listaBalas)
                {
                    bala.Update(gameTime);
                    if (bala.duracion > 3)
                    {
                        //listaBalas.Remove(bala);
                    }
                }
            }


            // Aplicar los movimientos al modelo
            // Aquí deberías aplicar esta matriz `world` a la transformación del modelo en el juego



        }

        public override void Draw(GameTime gametime, Matrix View, Matrix Projection)
        {
            efectoPwUP.Parameters["View"].SetValue(View);
            efectoPwUP.Parameters["Projection"].SetValue(Projection);
            efectoPwUP.Parameters["ModelTexture"].SetValue(textura);

            var random = new Random(Seed: 0);

            var modelMeshesBaseTransforms = new Matrix[modelo.Bones.Count];
            modelo.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach (var mesh in modelo.Meshes)
            {
                var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                // We set the main matrices for each mesh to draw
                efectoPwUP.Parameters["World"].SetValue(meshWorld * world);

                // Draw the mesh
                mesh.Draw();
            }

            if (listaBalas.Count != 0)
            {
                foreach (BalaHamster bala in listaBalas)
                {
                    bala.Draw(View, Projection);
                    if (bala.duracion > 3) { }
                }
            }

            DrawBoundingBox(graphicsDevice, View, Projection);

        }



        List<BalaHamster> listaBalas;

        public override void Apply()
        {
            var bala = new BalaHamster(graphicsDevice, jugador.contenido, jugador.rotationMatrix.Backward, jugador.carPosition + jugador.rotationMatrix.Backward * 60f);
            Seleccionado = false;
            jugador.powerUp = null;
            listaBalas.Add(bala);
            //    jugador.CarSpeed = 10000;
        }
    }


}

class BalaHamster
{

    public Model modelo;
    public Effect efectoPwUP;
    public Vector3 posicion;

    public Vector3 direccion;
    public Texture2D textura;
    public float duracion = 0;
    public Matrix world;
    private SoundEffect _powerUpSound;
    public SoundEffectInstance _powerUpSoundInstance;

    public const string ContentFolderEffects = "Effects/";

    public BalaHamster(GraphicsDevice graphicsDevice, ContentManager content, Vector3 direccion, Vector3 posInicial)
    {

        this.modelo = content.Load<Model>("poweUp/hamster-3d-model/Humster");
        efectoPwUP = content.Load<Effect>(ContentFolderEffects + "ModelsTexture");
        this.textura = content.Load<Texture2D>("poweUp/hamster-3d-model/Hamster_UV");

        this.posicion = posInicial;
        this.direccion = direccion;

        foreach (var mesh in modelo.Meshes)
        {
            // Aquí verificas si el nombre del mesh corresponde a una rueda
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = efectoPwUP;
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        duracion += (float)gameTime.ElapsedGameTime.TotalSeconds;

        Matrix rotacionHamster = Matrix.CreateRotationY((float)-direccion.Z);

        posicion += direccion * 1500f * (float)gameTime.ElapsedGameTime.TotalSeconds;

        world = Matrix.CreateScale(0.1f) * rotacionHamster * Matrix.CreateTranslation(posicion);
    }

    public void Draw(Matrix View, Matrix Projection)
    {
        efectoPwUP.Parameters["View"].SetValue(View);
        efectoPwUP.Parameters["Projection"].SetValue(Projection);
        efectoPwUP.Parameters["ModelTexture"].SetValue(textura);

        var modelMeshesBaseTransforms = new Matrix[modelo.Bones.Count];
        modelo.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

        foreach (var mesh in modelo.Meshes)
        {
            var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];
            // We set the main matrices for each mesh to draw
            efectoPwUP.Parameters["World"].SetValue(meshWorld * world);
            // Draw the mesh
            mesh.Draw();
        }
    }
}