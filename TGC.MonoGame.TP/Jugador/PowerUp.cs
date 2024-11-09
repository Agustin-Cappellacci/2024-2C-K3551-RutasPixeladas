
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

namespace TGC.MonoGame.TP.Content.Models
{
    public abstract class IPowerUp
    {
    public Model modelo;
    public Effect efectoPwUP;
    public Jugador jugador;
    private float rotationSpeed = 1f; // Velocidad de rotación
    private float verticalSpeed = 0.005f; // Velocidad de movimiento vertical
    private float maxHeight = 20f; // Altura máxima del movimiento vertical
    private float currentHeight = 0f;
    private bool goingUp = true;
    private SoundEffect _powerUpSound;
    public SoundEffectInstance _powerUpSoundInstance;
    private bool isMuted = false;
    public Matrix world;
    public Vector3 PosicionInicial;
    public Texture2D textura;
    public const string ContentFolderEffects = "Effects/";
    protected abstract void CargarModelo(ContentManager content);

    
    public bool Seleccionado = false;
    public bool AreAABBsTouching = false;

    public abstract void Update(GameTime gameTime);
        


        public abstract void Draw(GameTime gametime, Matrix View, Matrix Projection);


        public IPowerUp(ContentManager content, Jugador jugador, Vector3 posInicial) {
            this.jugador = jugador;
            this.PosicionInicial = posInicial;
            CargarModelo(content);
            efectoPwUP = content.Load<Effect>(ContentFolderEffects + "ModelsTexture");
            _powerUpSound = content.Load<SoundEffect>("Models/autos/RacingCarA/powerup");
            _powerUpSoundInstance = _powerUpSound.CreateInstance();
        }
        public abstract void Apply();
    }
    class SuperSpeed : IPowerUp
    {
        private BoundingBox ColisionCaja { get; set; }
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

        public override void Draw(GameTime gametime, Matrix View, Matrix Projection) {}
        
        public override void Update(GameTime gameTime){}

        protected override void CargarModelo(ContentManager content) {
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
    private float rotationSpeed = 1f; // Velocidad de rotación
    private float verticalSpeed = 0.5f; // Velocidad de movimiento vertical
    private float maxHeight = 7f; // Altura máxima del movimiento vertical
    private float currentHeight = 0f;
    private bool goingUp = true;
    private BoundingBox ColisionCaja { get; set; }

        public Gun(ContentManager content, Jugador jugador, Vector3 posInicial) : base(content, jugador, posInicial)
        {
            this.jugador = jugador;
            this.PosicionInicial = posInicial;
            CargarModelo(content);

            ColisionCaja = BoundingVolumesExtensions.CreateAABBFrom(modelo);
            ColisionCaja = new BoundingBox(ColisionCaja.Min + posInicial, ColisionCaja.Max + posInicial);
        }

        public override void Apply()
        {
        //    jugador.CarSpeed = 10000;
        }

        protected override void CargarModelo(ContentManager content) {
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

        public override void Update(GameTime gameTime)
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
        // Aplicar los movimientos al modelo
        world = Matrix.CreateScale(2f) * rotation * Matrix.CreateTranslation(PosicionInicial.X, PosicionInicial.Y + currentHeight, PosicionInicial.Z);
        // Aquí deberías aplicar esta matriz `world` a la transformación del modelo en el juego
        //AreAABBsTouching = ColisionCaja.Intersects(jugador.ColisionCaja);
        }

        public override void Draw(GameTime gametime, Matrix View, Matrix Projection) {
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
        }

    }

class Hamster : IPowerUp
    {
    private float rotationSpeed = 1f; // Velocidad de rotación
    private float verticalSpeed = 0.5f; // Velocidad de movimiento vertical
    private float maxHeight = 7f; // Altura máxima del movimiento vertical
    private float currentHeight = 0f;
    private bool goingUp = true;

    private Vector3 posicion;

    private GraphicsDevice graphicsDevice;

    private BoundingBox ColisionCaja { get; set; }
        public Hamster(GraphicsDevice graphicsDevice, ContentManager content, Jugador jugador, Vector3 posInicial) : base(content, jugador, posInicial)
        {   
            this.graphicsDevice = graphicsDevice;
            this.jugador = jugador;
            this.PosicionInicial = posInicial;
            CargarModelo(content);
            ColisionCaja = BoundingVolumesExtensions.CreateAABBFrom(this.modelo);
            ColisionCaja = new BoundingBox(ColisionCaja.Min + PosicionInicial, ColisionCaja.Max + PosicionInicial);
        }

        protected override void CargarModelo(ContentManager content) {
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

        public override void Update(GameTime gameTime)
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

        //ColisionCaja = new BoundingBox(ColisionCaja.Min + PosicionInicial, ColisionCaja.Max + PosicionInicial);

        Console.WriteLine("AreAABBsTouching: " + AreAABBsTouching);
        AreAABBsTouching = ColisionCaja.Intersects(jugador.ColisionCaja);

        if (AreAABBsTouching & jugador.powerUp != null){
            jugador.powerUp = this;
            Seleccionado = true;
        }

        Seleccionado = true;

        if (Seleccionado) {
            // Ajusta la posición en relación con la rotación y posición del coche
            var offset = new System.Numerics.Vector3(0, 25f, 0); // Altura sobre el techo
            posicion = jugador.carPosition + Vector3.Transform(offset, jugador.rotationMatrix); // Usa la rotación del coche
            world = Matrix.CreateScale(0.05f) * jugador.rotationMatrix * Matrix.CreateTranslation(posicion);
        } else {
            world = Matrix.CreateScale(0.1f) * rotation * Matrix.CreateTranslation(PosicionInicial.X, PosicionInicial.Y + currentHeight, PosicionInicial.Z);
        }


        // Aplicar los movimientos al modelo
        // Aquí deberías aplicar esta matriz `world` a la transformación del modelo en el juego



        }

        public override void Draw(GameTime gametime, Matrix View, Matrix Projection) {
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

            DrawBoundingBox(ColisionCaja, graphicsDevice, View, Projection);
    
        }

        public void DrawBoundingBox(BoundingBox boundingBox, GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
{
    var corners = boundingBox.GetCorners();
    var vertices = new VertexPositionColor[24];

    // Define color para las líneas del bounding box
    var color = Color.Red;

    // Asigna los vértices de las líneas de cada borde del bounding box
    int[] indices = { 0, 1, 1, 2, 2, 3, 3, 0, // Bottom face
                      4, 5, 5, 6, 6, 7, 7, 4, // Top face
                      0, 4, 1, 5, 2, 6, 3, 7  // Vertical edges
                    };

    for (int i = 0; i < indices.Length; i++)
    {
        vertices[i] = new VertexPositionColor(corners[indices[i]], color);
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
        graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 12);
    }
}


        public override void Apply()
        {
        //    jugador.CarSpeed = 10000;
        }
    }


}