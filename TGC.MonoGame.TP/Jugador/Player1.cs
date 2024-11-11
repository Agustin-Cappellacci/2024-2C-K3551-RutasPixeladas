using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TGC.MonoGame.Samples.Collisions;


namespace TGC.MonoGame.TP.Content.Models
{
    // Nunca olvides mesh.ParentBone.Transform

    class Jugador
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        private Model Model { get; set; }
        private Effect effectAuto { get; set; }

        // Jugabilidad
        private Microsoft.Xna.Framework.Vector3 direccionFrontal { get; set; }
        private Matrix carRotation = Matrix.CreateRotationY(0f);
        public Matrix rotationMatrix;

        public Matrix carWorld { get; set; }

        private const float carAcceleration = 500f;
        //    private const float carAcceleratioSpeedMax = 0;
        //    private const float carAcceleratioSpeedMin = 0;
        private const float carSpeedMax = 1000f;
        private const float carSpeedMin = -700f;
        public float carJumpSpeed { get; set; }
        private const float gravity = 98f;
        private const float carSpinSpeed = 0.3f;


        public System.Numerics.Vector3 carPosition { get; set; }
        public float CarSpeed { get; set; }
        private const float CarMaxSpeed = 500f;
        public float CarAcceleration { get; set; }
        private const float CarBrakeForce = 5000f;
        private float CarDeceleration = 500f;

        private float wheelRotationAngle;
        private float wheelSteeringAngle;
        private float maxWheelSteer = 0.5f;
        private float wheelSteerDelta = 0.1f;
        private float CarRotationY;
        private const float CarjumpStrength = 300f;
        private float elapsedTime;
        private Simulation simulation;
        private BodyHandle carBodyHandle;
        private BodyHandle wheelBodyHandle;
        SimpleCarController playerController;

        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;
        private GraphicsDevice graphicsDevice;

        public IPowerUp powerUp = null;
        private SoundEffect _engineSound;
        private SoundEffectInstance _engineSoundInstance;
        private bool isMuted = false;

        public int power = -1;
        public float cooldownTime = 1.5f;  // Tiempo de cooldown (1.5 segundos)
        public float cooldownTimer = 0f;   // Contador para el cooldown
        public bool isOnCooldown = false;

        private float vida = 200;

        private Vector3 lightPosition = new Vector3(1000, 2000, 1000);

        Texture2D texturaAuto;
        Texture2D texturaRueda;

        public BoundingBox ColisionCaja { get; set; }

        private float jumpCooldown = 3f; // Tiempo de espera en segundos
        private float timeSinceLastJump = 3f; // Inicializa para permitir el primer salto de inmediato
        private bool canJump = true;

        public Vector3 forwardVector;

        private RenderTargetCube EnvironmentMapRenderTarget { get; set; }
        private StaticCamera CubeMapCamera { get; set; }

        private const int EnvironmentmapSize = 2048;

        public Jugador(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice, SimpleCarController playerController, Vector3 posicion, float angulo)
        {
            //carPosition = PositionToNumerics(new Vector3(0f, 500f, 0f));
            //direccionFrontal = Vector3.Forward;
            Model = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");
            _engineSound = content.Load<SoundEffect>(ContentFolder3D + "autos/RacingCarA/high ACC");
            _engineSoundInstance = _engineSound.CreateInstance();
            _engineSoundInstance.IsLooped = true;
            _engineSoundInstance.Play();
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "Player1");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "BlinnPhong");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "ModelsTexture");

            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();



            texturaAuto = content.Load<Texture2D>("texturas/colorRojo");
            texturaRueda = content.Load<Texture2D>("texturas/rueda");

            CarAcceleration = 500f;
            carJumpSpeed = 50000f;

            // A model contains a collection of meshes
            foreach (var mesh in Model.Meshes)
            {
                // Aquí verificas si el nombre del mesh corresponde a una rueda
                foreach (var meshPart in mesh.MeshParts)
                {

                    meshPart.Effect = effectAuto;
                }
                if (mesh.Name.Contains("Wheel"))
                {
                    ruedas.Add(mesh);
                }
                else restoAuto.Add(mesh);

            }

            // LOGICA DE SIMULACION, TODO LO QUE ESTA ACA SON LOS OBJETOS DE COLISION LOGICOS
            // utilizo el graphicsDevice para dibujar las cajas
            this.graphicsDevice = graphicsDevice;
            this.simulation = simulation;
            this.playerController = playerController;

            // Create a render target for the scene
            EnvironmentMapRenderTarget = new RenderTargetCube(graphicsDevice, EnvironmentmapSize, false,
                SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
            graphicsDevice.BlendState = BlendState.Opaque;

            CubeMapCamera = new StaticCamera(1f, carPosition, Vector3.UnitX, Vector3.Up);
            CubeMapCamera.BuildProjection(1f, 1f, 3000f, MathHelper.PiOver2);

            //carBodyHandle = CrearCuerpoDelAutoEnSimulacion(simulation, PositionToNumerics(posicion), angulo);

            ColisionCaja = BoundingVolumesExtensions.CreateAABBFrom(Model);
            ColisionCaja = new BoundingBox(ColisionCaja.Min + posicion, ColisionCaja.Max + posicion);

        }

        private BodyHandle CrearCuerpoDelAutoEnSimulacion(Simulation simulation, System.Numerics.Vector3 posicionInicial, float anguloInicial)
        {
            // Crear el cuerpo del coche en la simulación con una posición y orientación iniciales
            var bodyDescription = BodyDescription.CreateDynamic(
                new RigidPose(posicionInicial, System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, anguloInicial)),
                new BodyInertia(),
                new CollidableDescription(),
                new BodyActivityDescription(0.01f)
            );

            return simulation.Bodies.Add(bodyDescription);
        }


        public void Update(GameTime gameTime, Simulation simulation)
        {

            // Caputo el estado del teclado.
            var keyboardState = Keyboard.GetState();
            // La logica debe ir aca.
            elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            // Obtener la referencia del cuerpo del auto en la simulación
            var carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carBodyReference.Awake = true;

            float steeringSum = 0;

            // Actualizar rotación del coche
            if (keyboardState.IsKeyDown(Keys.A))
            {
                steeringSum += 1;
                wheelSteeringAngle = Math.Min(wheelSteeringAngle + wheelSteerDelta, maxWheelSteer);

            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    steeringSum -= 1;
                    wheelSteeringAngle = Math.Max(wheelSteeringAngle - wheelSteerDelta, -maxWheelSteer);
                }
                else
                {
                    wheelSteeringAngle = 0;
                }
            }

            timeSinceLastJump += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (canJump && keyboardState.IsKeyDown(Keys.Space) && carPosition.Y < 100f)
            {
                carBodyReference.ApplyLinearImpulse(new System.Numerics.Vector3(0, carJumpSpeed, 0));
                canJump = false; // Desactiva el salto
                timeSinceLastJump = 0f; // Reinicia el temporizador
            }

            // Si el tiempo de espera ha pasado, permite saltar de nuevo
            if (timeSinceLastJump >= jumpCooldown)
            {
                canJump = true;
            }


            bool isFlipped = rotationMatrix.Up.Y < 0; // Si Y es negativo, el auto está al revés

            // Si está volcado y se presiona "R"
            if (isFlipped && keyboardState.IsKeyDown(Keys.R))
            {
                // Rota el auto para enderezarlo
                carBodyReference.Pose.Orientation = System.Numerics.Quaternion.Identity;
                carBodyReference.Pose.Position = new System.Numerics.Vector3(
                    carBodyReference.Pose.Position.X,
                    carBodyReference.Pose.Position.Y + 1, // Ajusta la altura si es necesario
                    carBodyReference.Pose.Position.Z
                );
            }


            var targetSpeedFraction = keyboardState.IsKeyDown(Keys.W) ? 10f : keyboardState.IsKeyDown(Keys.S) ? -10f : 0;
            Console.WriteLine("TargetSpeed " + targetSpeedFraction);
            playerController.Update(simulation, 1 / 60f, steeringSum, targetSpeedFraction, keyboardState.IsKeyDown(Keys.LeftAlt));

            _engineSoundInstance.Pitch = steeringSum* 0.00000000002f;
            
            // Actualizar la posición y la matriz de mundo del auto
            carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carPosition = carBodyReference.Pose.Position;

            //ColisionCaja = new BoundingBox(ColisionCaja.Min + carPosition, ColisionCaja.Max + carPosition);
            
            rotationMatrix = Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation); //PUEDE VENIR DE ACA
            carWorld = rotationMatrix * Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(carPosition);

            forwardVector = rotationMatrix.Forward;

            CubeMapCamera.Position = carPosition;

            Console.WriteLine("posicion del auto: " + carPosition);
        }

        public void Draw(Matrix View, Matrix Projection, Vector3 cameraPosition)
        {
            var random = new Random(Seed: 0);
            var color = new Microsoft.Xna.Framework.Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
            var colorRueda = new Microsoft.Xna.Framework.Vector3(0, 0, 0);
            // effectAuto.Parameters["View"].SetValue(View);
            //effectAuto.Parameters["Projection"].SetValue(Projection);
            /*
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Draw to our cubemap from the robot position
            for (var face = CubeMapFace.PositiveX; face <= CubeMapFace.NegativeZ; face++)
            {
                // Set the render target as our cubemap face, we are drawing the scene in this texture
                graphicsDevice.SetRenderTarget(EnvironmentMapRenderTarget, face);
                graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

                SetCubemapCameraForOrientation(face);
                CubeMapCamera.BuildView();

                // Draw our scene. Do not draw our tank as it would be occluded by itself 
                // (if it has backface culling on)
                cama.Draw(Matrix.Identity, CubeMapCamera.View, CubeMapCamera.Projection);
            }

            // Set the render target as null, we are drawing on the screen!
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);


            // Draw our scene with the default effect and default camera
            cama.Draw(Matrix.Identity, View, Projection);
            */
            foreach (ModelMesh mesh in restoAuto)
            {
                Vector3 lightPosition = new Vector3(-1000, 3000, 1000); // Luz en una posición elevada en el espacio

                //effectAuto.Parameters["View"].SetValue(View);
                //effectAuto.Parameters["Projection"].SetValue(Projection);
                
                //effectAuto.Parameters["lightPosition"].SetValue(lightPosition);
                //effectAuto.Parameters["LightViewProjection"].SetValue(lightViewProjection);
                //effectAuto.Parameters["ShadowMap"].SetValue(shadowRenderTarget);

                //effectAuto.Parameters["ModelTexture"].SetValue(texturaAuto);
                //effectAuto.Parameters["DiffuseColor"].SetValue(color);
                //effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * carWorld);
                /*
                effectAuto.Parameters["environmentMap"].SetValue(EnvironmentMapRenderTarget);
                */
                effectAuto.Parameters["ambientColor"].SetValue(new Vector3(0.25f, 0.0f, 0.0f));
                effectAuto.Parameters["diffuseColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                effectAuto.Parameters["specularColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));

                effectAuto.Parameters["KAmbient"].SetValue(0.7f);
                effectAuto.Parameters["KDiffuse"].SetValue(0.5f);
                effectAuto.Parameters["KSpecular"].SetValue(0.3f);
                effectAuto.Parameters["shininess"].SetValue(50.0f);

                effectAuto.Parameters["lightPosition"].SetValue(lightPosition);
                
                effectAuto.Parameters["eyePosition"].SetValue(cameraPosition);
                effectAuto.Parameters["ModelTexture"].SetValue(texturaAuto);

                //effectAuto.Parameters["lightDirection"].SetValue(forwardVector); // Dirección hacia adelante
                //effectAuto.Parameters["cutoffAngle"].SetValue(MathHelper.ToRadians(30f));
                //effectAuto.Parameters["baseTexture"].SetValue(texturaAuto);
                // We set the main matrices for each mesh to draw
                effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * carWorld);
                // InverseTransposeWorld is used to rotate normals
                effectAuto.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(carWorld)));
                // WorldViewProjection is used to transform from model space to clip space
                effectAuto.Parameters["WorldViewProjection"].SetValue(mesh.ParentBone.Transform * carWorld * View * Projection);

                mesh.Draw();
            }

            foreach (ModelMesh rueda in ruedas)
            {   
                effectAuto.Parameters["ModelTexture"].SetValue(texturaRueda);
                //effectAuto.Parameters["DiffuseColor"].SetValue(colorRueda);
                if (rueda.Name.Contains("WheelA") || rueda.Name.Contains("WheelB"))
                {
                    effectAuto.Parameters["World"].SetValue(Matrix.CreateRotationX(wheelRotationAngle) * Matrix.CreateRotationY(wheelSteeringAngle) * rueda.ParentBone.Transform * carWorld);
                    // InverseTransposeWorld is used to rotate normals
                    effectAuto.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.CreateRotationX(wheelRotationAngle) * Matrix.CreateRotationY(wheelSteeringAngle) * carWorld)));
                    // WorldViewProjection is used to transform from model space to clip space
                    effectAuto.Parameters["WorldViewProjection"].SetValue(Matrix.CreateRotationX(wheelRotationAngle) * Matrix.CreateRotationY(wheelSteeringAngle) * rueda.ParentBone.Transform * carWorld * View * Projection);
                }
                else
                {
                    effectAuto.Parameters["World"].SetValue(Matrix.CreateRotationX(wheelRotationAngle) * rueda.ParentBone.Transform * carWorld);
                    // InverseTransposeWorld is used to rotate normals
                    effectAuto.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.CreateRotationX(wheelRotationAngle) * carWorld)));
                    // WorldViewProjection is used to transform from model space to clip space
                    effectAuto.Parameters["WorldViewProjection"].SetValue(Matrix.CreateRotationX(wheelRotationAngle) * rueda.ParentBone.Transform * carWorld * View * Projection);
                }
                rueda.Draw();
            }

            // Dibujar las cajas de colisión del auto y las ruedas
            DrawCollisionBoxes(View, Projection);
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


        public void ToggleSound()
        {
            if (isMuted) {
                _engineSoundInstance.Play();
                isMuted = false;
                return;
            }
            _engineSoundInstance.Stop();
            isMuted = true;
            return;
        }

        public static System.Numerics.Vector3 PositionToNumerics(Microsoft.Xna.Framework.Vector3 xnaVector3)
        {
            return new System.Numerics.Vector3(xnaVector3.X, xnaVector3.Y, xnaVector3.Z);
        }

        public void DrawCollisionBoxes(Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Obtener la matriz de rotación del auto (supongo que tienes una variable CarRotationY para la rotación en Y)
            //var carRotationMatrix = Matrix.CreateRotationY(rotationMatrix); // Rotación del auto en Y (ajusta esto si tienes más rotaciones en otros ejes)

            // Crear la matriz de mundo del coche, que incluye rotación y traslación
            var carWorldMatrix = rotationMatrix * Matrix.CreateTranslation(carPosition + new System.Numerics.Vector3(0, 80f, 0));

            
            // Dibujar la caja de colisión del auto usando la matriz de mundo del auto
            //DrawBox(carWorldMatrix, new Vector3(40f, -30f, 100f), viewMatrix, projectionMatrix);
            /*
            // Dibujar las cajas de colisión de las ruedas
            foreach (var rueda in ruedas)
            {
                // Obtener la matriz de transformación local de la rueda dentro del coche
                var wheelMatrix = rueda.ParentBone.Transform;

                // Obtener la matriz de mundo del coche (posición + rotación)
                var carRotationMatrixWheels = Matrix.CreateRotationY(CarRotationY); // Supongo que tienes una variable CarRotationY para la rotación del coche en Y
                var carWorldMatrixWheels = carRotationMatrixWheels * Matrix.CreateTranslation(carPosition);

                // Transformar la posición local de la rueda a la posición en el mundo
                var wheelPositionInWorld = Vector3.Transform(Vector3.Zero, wheelMatrix * carWorldMatrixWheels);

                // Ajusta las dimensiones de las cajas de colisión según tus necesidades
                var wheelBox = new Box(100f, 100f, 100f); // Cambia las dimensiones según tu modelo

                // Dibujar la caja de colisión para cada rueda
                DrawBox(Matrix.CreateTranslation(wheelPositionInWorld), new Vector3(100f, 100f, 100f), viewMatrix, projectionMatrix);
            } */
        }

        private void ElevateWheel(BodyReference wheelBodyReference)
        {
            // Aquí ajustas la posición de la rueda en función de la colisión
            var wheelPosition = wheelBodyReference.Pose.Position;

            // Simula que la rueda se eleva al subir la rampa
            wheelPosition.Y += 50f;  // Puedes ajustar este valor en función de la altura de la rampa

            // Actualiza la nueva posición de la rueda en la simulación
            wheelBodyReference.Pose.Position = wheelPosition;
        }

        public void DrawBox(Matrix worldMatrix, Vector3 size, Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Crear un efecto básico para dibujar la caja
            BasicEffect effect = new BasicEffect(graphicsDevice);
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            effect.VertexColorEnabled = true;

            // Definir los vértices de una caja (un cubo unitario que escalaremos)
            VertexPositionColor[] vertices = new VertexPositionColor[8];
            vertices[0] = new VertexPositionColor(new Vector3(-1, 1, 1), Color.Red);   // Front top left
            vertices[1] = new VertexPositionColor(new Vector3(1, 1, 1), Color.Red);    // Front top right
            vertices[2] = new VertexPositionColor(new Vector3(-1, -1, 1), Color.Red);  // Front bottom left
            vertices[3] = new VertexPositionColor(new Vector3(1, -1, 1), Color.Red);   // Front bottom right
            vertices[4] = new VertexPositionColor(new Vector3(-1, 1, -1), Color.Red);  // Back top left
            vertices[5] = new VertexPositionColor(new Vector3(1, 1, -1), Color.Red);   // Back top right
            vertices[6] = new VertexPositionColor(new Vector3(-1, -1, -1), Color.Red); // Back bottom left
            vertices[7] = new VertexPositionColor(new Vector3(1, -1, -1), Color.Red);  // Back bottom right

            // Escalar la caja en función del tamaño dado
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position *= size / 2f;
            }

            // Definir los índices que forman las líneas de la caja
            int[] indices = new int[]
            {
        0, 1, 1, 3, 3, 2, 2, 0,  // Front face
        4, 5, 5, 7, 7, 6, 6, 4,  // Back face
        0, 4, 1, 5, 2, 6, 3, 7   // Connecting edges
            };

            // Dibujar la caja usando el efecto básico
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertices,
                    0,
                    vertices.Length,
                    indices,
                    0,
                    indices.Length / 2
                );
            }
        }

        private void SetCubemapCameraForOrientation(CubeMapFace face)
        {
            switch (face)
            {
                default:
                case CubeMapFace.PositiveX:
                    CubeMapCamera.FrontDirection = -Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeX:
                    CubeMapCamera.FrontDirection = Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.PositiveY:
                    CubeMapCamera.FrontDirection = Vector3.Down;
                    CubeMapCamera.UpDirection = Vector3.UnitZ;
                    break;

                case CubeMapFace.NegativeY:
                    CubeMapCamera.FrontDirection = Vector3.Up;
                    CubeMapCamera.UpDirection = -Vector3.UnitZ;
                    break;

                case CubeMapFace.PositiveZ:
                    CubeMapCamera.FrontDirection = -Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeZ:
                    CubeMapCamera.FrontDirection = Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;
            }
        }
        /*
        /// <inheritdoc />
        protected override void UnloadContent()
        {
            base.UnloadContent();
            EnvironmentMapRenderTarget.Dispose();
        }
        */
    }
}