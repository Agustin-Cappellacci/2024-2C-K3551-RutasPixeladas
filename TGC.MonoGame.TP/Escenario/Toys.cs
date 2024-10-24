using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace TGC.MonoGame.TP.Content.Models
{
    /// <summary>
    /// A City Scene to be drawn
    /// </summary>
    class Toys
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const float DistanceBetweenCities = 2100f;
        private Model Lego { get; set; }
        private Model Ajedrez { get; set; }
        private Matrix worldAjedrez { get; set; }
        private Vector3 colorAjedrez = new Vector3(0.5f, 0.5f, 1f);
        private Effect EfectoComun { get; set; }
        private Effect EfectoTexture { get; set; }
        public List<Tuple<Model, Texture2D,Matrix>> listaCombinada;

        private StaticHandle floorHandle;
        private StaticHandle torreHandle;
        private StaticHandle rampa1BodyHandle;
        private StaticHandle rampa2BodyHandle;
        private StaticHandle rampa3BodyHandle;
        private StaticHandle rampa4BodyHandle;
        private StaticHandle rampa5BodyHandle;
        private StaticHandle rampa6BodyHandle;
        private StaticHandle rampa7BodyHandle;
        private StaticHandle rampa8BodyHandle;
        private StaticHandle rampaParedBodyHandle;
        private StaticHandle rampaDoble1BodyHandle;
        private StaticHandle rampaDoble2BodyHandle;
        private StaticHandle caballo1BodyHandle;
        private StaticHandle caballo2BodyHandle;
        private StaticHandle rampaPanza1BodyHandle;
        private StaticHandle rampaPanza2BodyHandle;
        private StaticHandle ajedrezBodyHandle;
        private StaticHandle legoBodyHandle;
        private StaticHandle puenteBase1Handle;
        private StaticHandle puenteBase2Handle;
        private StaticHandle puentePared1Handle;
        private StaticHandle puentePared2Handle;
        
        private Simulation simulation;
        private GraphicsDevice graphicsDevice;
        //private ConvexHull rampHull;



        // <summary>
        /// Creates a City Scene with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>

        public Toys(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice)

        {
            // Load an effect that will be used to draw the scene
            EfectoTexture = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            EfectoComun = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            // ESTA INTERESANTE PERO NO HACE NADA
            listaCombinada = new List<Tuple<Model, Texture2D, Matrix>>()
            {

            };
            // Load the City Model
            Lego = content.Load<Model>(ContentFolder3D + "escenario/legoBrick");


            Model Torre = content.Load<Model>(ContentFolder3D + "escenario/torre");
            Model LegoPJ = content.Load<Model>(ContentFolder3D + "escenario/legoPJ/FireNinjaBlueOcatpus2/Fireninja_blueninja");
            Model Puente = content.Load<Model>(ContentFolder3D + "escenario/puente");
            Model rampaDoble = content.Load<Model>(ContentFolder3D + "escenario/nuevos/rampadoble");
            Model rampaPanza = content.Load<Model>(ContentFolder3D + "escenario/nuevos/rampaPanza");
            Model rampa = content.Load<Model>(ContentFolder3D + "escenario/nuevos/rampa");
            Model carpet = content.Load<Model>(ContentFolder3D + "escenario/nuevos/carpet");
            Model caballo1 = content.Load<Model>(ContentFolder3D + "escenario/nuevos/caballo1");
            Model caballo2 = content.Load<Model>(ContentFolder3D + "escenario/nuevos/caballo2");
            List<Model> listaModelos = new List<Model>(){
                Torre, LegoPJ, Puente, rampaDoble, rampaPanza, rampa, carpet, caballo1, caballo2
            };

            //Load textures
            
            Texture2D textureMetal = content.Load<Texture2D>(ContentFolder3D + "escenario/Textures/metal");
            Texture2D textureLegoPJ = content.Load<Texture2D>(ContentFolder3D + "escenario/legoPJ/Skin1");
            Texture2D textureMadera2 = content.Load<Texture2D>(ContentFolder3D + "escenario/Rubics_cube/wood/cherry_1");
            Texture2D textureRampa = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/texture");
            Texture2D textureMadera = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/wood");
            Texture2D textureCarpet = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/alfombra2");
            Texture2D textureCaballo1 = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/bullseye");
            Texture2D textureCaballo2 = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/ColAlphY_horse_brown");
            Texture2D texture = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/default-grey");

            List<Texture2D> listaTexturas = new List<Texture2D>()
            {
                textureMetal,textureLegoPJ,textureMadera2,textureMadera,textureRampa,textureMadera,textureCarpet,textureCaballo2,textureCaballo1
            };

            List<Matrix> WorldMatrix = new List<Matrix>()
            {
                Matrix.CreateRotationY((float)Math.PI / 10) * Matrix.CreateScale(1.5f) * Matrix.CreateTranslation(new Vector3(-1300f, 1f, 700f)),   //Torre
                Matrix.CreateRotationY(-(float)Math.PI / 4) * Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(new Vector3(1200f, 2f, 1700f)),   //LegoPJ
                Matrix.CreateRotationX((float)Math.PI / -2) * Matrix.CreateRotationY((float)Math.PI / 4) * Matrix.CreateScale(100f) * Matrix.CreateTranslation(new Vector3(360F, 2f, -1700f)),    //Puente
                Matrix.CreateScale(4f) * Matrix.CreateTranslation(new Vector3(-1400f, 0f, -1000f)),              //RampaDoble
                Matrix.CreateRotationY((float)Math.PI / 3) * Matrix.CreateScale(300f) * Matrix.CreateTranslation(new Vector3(-1200f, 10f, 0f)),   //RampaPanza
                Matrix.CreateRotationX(-(float)Math.PI / 2) * Matrix.CreateRotationY( (float)Math.PI / 2) * Matrix.CreateScale(10f) * Matrix.CreateTranslation(new Vector3(0f, 0f, 1000f)),   //Rampa
                Matrix.CreateScale(10f) * Matrix.CreateTranslation(new Vector3(-900f, -1f, -1100f)), //Carpet
                Matrix.CreateRotationY(-(float)Math.PI * (5 / 4)) * Matrix.CreateScale(130f) * Matrix.CreateTranslation(new Vector3(1000f, 730f, 300f)),//Caballo1
                Matrix.CreateRotationY(-(float)Math.PI / 4) * Matrix.CreateScale(130f) * Matrix.CreateTranslation(new Vector3(1200f, 730f, -100f))//Caballo2
            };


            for (int i = 0; i < listaModelos.Count; i++)
            {
                listaCombinada.Add(new Tuple<Model, Texture2D,Matrix>(listaModelos[i], listaTexturas[i], WorldMatrix[i]));
            }
            // Ponemos efectos a todas las partes
            for (int i = 0; i < listaCombinada.Count; i++)
            {
                foreach (var mesh in listaCombinada[i].Item1.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts) meshPart.Effect = EfectoTexture;
                }
            }

            foreach (var mesh in Lego.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts) meshPart.Effect = EfectoComun;
            }

            inicializadorColisionables(simulation, graphicsDevice);
        }

        // <summary>
        /// Draws the City Scene
        /// </summary>
        /// <param name="gameTime">The Game Time for this frame</param>
        /// <param name="view">A view matrix, generally from a camera</param>
        /// <param name="projection">A projection matrix</param>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            // Set the View and Projection matrices, needed to draw every 3D model
            EfectoComun.Parameters["View"].SetValue(view);
            EfectoComun.Parameters["Projection"].SetValue(projection);

            // Texturas
            foreach (var tupla in listaCombinada)
            {
                var modelMeshesBaseTransforms = new Matrix[tupla.Item1.Bones.Count];
                tupla.Item1.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);
                foreach (var mesh in tupla.Item1.Meshes)
                {
                    EfectoTexture.Parameters["ModelTexture"].SetValue(tupla.Item2);
                    var worldFinal = modelMeshesBaseTransforms[mesh.ParentBone.Index] * tupla.Item3;
                    EfectoTexture.Parameters["World"].SetValue(worldFinal);
                    mesh.Draw();
                }

            }

            
            // Legos
            var random = new Random(Seed: 0);

            var modelLegoMeshesBaseTransforms = new Matrix[Lego.Bones.Count];
            Lego.CopyAbsoluteBoneTransformsTo(modelLegoMeshesBaseTransforms);

            for (int j = 0; j < 10; j++)
            {
                var traslacion = new Vector3(
                -1600f + (1500f) * random.NextSingle(),
                0,
                -2000f + 200f * random.NextSingle()
                );
                var escala = 0.7f + (0.7f - 1f) * random.NextSingle();
                var color = new Vector3(0.6f, random.NextSingle(), random.NextSingle());

                foreach (var mesh in Lego.Meshes)
                {
                    var worldFinal = modelLegoMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                    EfectoComun.Parameters["DiffuseColor"].SetValue(color);
                    EfectoComun.Parameters["World"].SetValue(worldFinal);
                    mesh.Draw();
                }
            }

        }
        // Función para extraer vértices de un modelo 3D (similar a la que se usó en el Jugador)
        private List<System.Numerics.Vector3> ExtractVertices(Model model)
        {
            var vertices = new List<System.Numerics.Vector3>();

            foreach (var mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    var vertexData = new Microsoft.Xna.Framework.Vector3[part.VertexBuffer.VertexCount];
                    part.VertexBuffer.GetData(vertexData);

                    foreach (var vertex in vertexData)
                    {
                        vertices.Add(PositionToNumerics(vertex));
                    }
                }
            }

            return vertices;
        }
        // Función para convertir Vector3 de XNA a System.Numerics
        public static System.Numerics.Vector3 PositionToNumerics(Microsoft.Xna.Framework.Vector3 xnaVector3)
        {
            return new System.Numerics.Vector3(xnaVector3.X, xnaVector3.Y, xnaVector3.Z);
        }
        public List<System.Numerics.Vector3> ObtenerVerticesTransformados(Model model, System.Numerics.Vector3 traslacion, float escala, float rotacionX, float rotacionY)
        {
            var verticesOriginales = ExtractVertices(model);
            List<System.Numerics.Vector3> verticesTransformados = new List<System.Numerics.Vector3>();

            // Combinar las matrices en el orden correcto
            System.Numerics.Matrix4x4 matrizTransformacion = ObtenerMatrizTransformada(rotacionX, rotacionY, escala, traslacion);

            // Aplicar la transformación a cada vértice
            foreach (var vertice in verticesOriginales)
            {
                // Convertir el vértice a Vector3
                System.Numerics.Vector3 verticeTransformado = System.Numerics.Vector3.Transform(vertice, matrizTransformacion);
                verticesTransformados.Add(verticeTransformado);
            }

            return verticesTransformados;
        }
        public System.Numerics.Matrix4x4 ObtenerMatrizTransformada(float rotX, float rotY, float esc, System.Numerics.Vector3 tras)
        {
            // Crear matrices de transformación
            System.Numerics.Matrix4x4 matrizEscala = System.Numerics.Matrix4x4.CreateScale(esc);
            System.Numerics.Matrix4x4 matrizTraslacion = System.Numerics.Matrix4x4.CreateTranslation(tras);
            return System.Numerics.Matrix4x4.CreateFromYawPitchRoll(rotX, rotY, 0) * matrizEscala * matrizTraslacion;
        }
        public void DrawCollisionBoxes(Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Dibujar la caja de colisión del plano
            DrawBox(Matrix.CreateTranslation(0, -50, 0), new Vector3(5000f, 100f, 5000f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de la torre
            DrawBox(Matrix.CreateFromYawPitchRoll((float)Math.PI / 10,0,0) * Matrix.CreateTranslation(-1300f, 1f, 700f), new Vector3(180f, 500f, 180f), viewMatrix, projectionMatrix);
            // Dibujar cajas de colision de la rampa
            this.DrawCollisionBoxesRampaGrande(viewMatrix,projectionMatrix);
            

            // Dibujar Rampa doble
            DrawBox(Matrix.CreateFromYawPitchRoll(0, 0, (float)Math.PI / 20)*Matrix.CreateTranslation(-1220f, -10f, -1120f), new Vector3(540f, 100f, 230f), viewMatrix, projectionMatrix);
             // Dibujar Rampa doble
            DrawBox(Matrix.CreateFromYawPitchRoll(0, 0, -(float)Math.PI / 20)*Matrix.CreateTranslation(-550f, -10f, -1120f), new Vector3(540f, 100f, 230f), viewMatrix, projectionMatrix);
            
            // Dibujar Rampa panza
            DrawBox(Matrix.CreateFromYawPitchRoll((float)Math.PI / 3, (float)Math.PI / 7, 0)*Matrix.CreateTranslation(-1125f, -20f, 50f), new Vector3(330f, 150f, 260f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll((float)Math.PI / 3, -(float)Math.PI / 7, 0)*Matrix.CreateTranslation(-1275f, -20f, -50f), new Vector3(330f, 150f, 260f), viewMatrix, projectionMatrix);
            // Dibujar caballo1
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI * (5/4), 0, 0)*Matrix.CreateTranslation(1000f, 730f, 300f), new Vector3(100f, 500f, 260f), viewMatrix, projectionMatrix);
            // Dibujar caballo2
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, 0)*Matrix.CreateTranslation(1200f, 730f, -100f), new Vector3(100f, 550f, 260f), viewMatrix, projectionMatrix);
            // Dibujar ajedrez
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, 0)*Matrix.CreateTranslation(-1200f, 2f, 1700f), new Vector3(500f, 100f, 500f), viewMatrix, projectionMatrix);
            // Dibujar lego
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, 0)*Matrix.CreateTranslation(1200f, 2f, 1700f), new Vector3(150f, 100f, 150f), viewMatrix, projectionMatrix);
            // Dibujar puente
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4,(float)Math.PI /8, (float)Math.PI / -2)*Matrix.CreateTranslation(305f, -200f, -1625f), new Vector3(500f,150f, 400f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4,-(float)Math.PI /8, (float)Math.PI / -2)*Matrix.CreateTranslation(435f, -200f, -1755f), new Vector3(500f, 150f, 400f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4,0, (float)Math.PI / -2)*Matrix.CreateTranslation(320f, 0f, -1775f), new Vector3(300f, 30f, 550f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4,0, (float)Math.PI / -2)*Matrix.CreateTranslation(430f, 0f, -1635f), new Vector3(300f, 30f, 550f), viewMatrix, projectionMatrix);
            
        }// 450 -1665

        private void DrawCollisionBoxesRampaGrande(Matrix viewMatrix, Matrix projectionMatrix)
        {
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 5f, 0)*Matrix.CreateTranslation(455f, 620f, 900f), new Vector3(900f, 100f, 230f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 4, 0)*Matrix.CreateTranslation(290f, 480f, 900f), new Vector3(900f, 100f, 280f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 5, 0)*Matrix.CreateTranslation(100f, 320f, 900f), new Vector3(900f, 100f, 280f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 6, 0)*Matrix.CreateTranslation(-25f, 235f, 900f), new Vector3(900f, 100f, 280f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 9, 0)*Matrix.CreateTranslation(-245f, 135f, 900f), new Vector3(900f, 100f, 280f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 13, 0)*Matrix.CreateTranslation(-480f, 65f, 900f), new Vector3(900f, 100f, 280f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 16, 0)*Matrix.CreateTranslation(-720f, 15f, 900f), new Vector3(900f, 95f, 280f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 17, 0)*Matrix.CreateTranslation(-950f, -32f, 900f), new Vector3(900f, 100f, 260f), viewMatrix, projectionMatrix);
            DrawBox(Matrix.CreateTranslation(-30f, 50f, 900f), new Vector3(1000f, 100f, 900f), viewMatrix, projectionMatrix);
            
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
            vertices[0] = new VertexPositionColor(new Vector3(-1, 1, 1), Microsoft.Xna.Framework.Color.Red);   // Front top left
            vertices[1] = new VertexPositionColor(new Vector3(1, 1, 1), Microsoft.Xna.Framework.Color.Red);    // Front top right
            vertices[2] = new VertexPositionColor(new Vector3(-1, -1, 1), Microsoft.Xna.Framework.Color.Red);  // Front bottom left
            vertices[3] = new VertexPositionColor(new Vector3(1, -1, 1), Microsoft.Xna.Framework.Color.Red);   // Front bottom right
            vertices[4] = new VertexPositionColor(new Vector3(-1, 1, -1), Microsoft.Xna.Framework.Color.Red);  // Back top left
            vertices[5] = new VertexPositionColor(new Vector3(1, 1, -1), Microsoft.Xna.Framework.Color.Red);   // Back top right
            vertices[6] = new VertexPositionColor(new Vector3(-1, -1, -1), Microsoft.Xna.Framework.Color.Red); // Back bottom left
            vertices[7] = new VertexPositionColor(new Vector3(1, -1, -1), Microsoft.Xna.Framework.Color.Red);  // Back bottom right

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
        
        public void DrawConvexHull(ConvexHull hull, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            BasicEffect effect = new BasicEffect(graphicsDevice);
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            effect.VertexColorEnabled = true;

            // Obtener los vértices del hull
            var vertices = hull.Points;
            VertexPositionColor[] vertexColors = new VertexPositionColor[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                // Extraer cada componente del Vector3Wide y convertirlo a Vector3
                System.Numerics.Vector3 vertex;
                BepuUtilities.Vector3Wide.ReadFirst(vertices[i], out vertex);

                // Convertir el vector de Bepu a Microsoft.Xna.Framework.Vector3 y asignar el color
                vertexColors[i] = new VertexPositionColor(
                    new Microsoft.Xna.Framework.Vector3(
                        vertex.X, // X
                        vertex.Y, // Y
                        vertex.Z  // Z
                    ),
                    Microsoft.Xna.Framework.Color.Green
                );
            }

            // Crear los índices para las líneas que forman el convex hull
            List<int> indices = new List<int>();
            for (int i = 0; i < vertices.Length - 1; i++)
            {
                indices.Add(i);
                indices.Add(i + 1);
            }
            // Cerrar el convex hull conectando el último vértice con el primero
            indices.Add(vertices.Length - 1);
            indices.Add(0);

            // Dibujar el convex hull
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertexColors,
                    0,
                    vertexColors.Length,
                    indices.ToArray(),
                    0,
                    indices.Count / 2
                );
            }
        }
        private void inicializadorColisionables(Simulation simulation, GraphicsDevice graphicsDevice)
        {

            // TODO LO QUE ESTA ACA ES PARA LOS OBJETOS DE COLISION, NO TIENE NADA QUE VER CON LO QUE SE VE EN PANTALLA
            // SI SE MODIFICA ALGO DE ACA, HAY QUE MODIFICAR MANUALMENTE EL DRAWBOX YA QUE NO SE VA A ACTUALIZAR CON LOS CAMBIOS EN LOS OBJETOS DE COLISION
            this.simulation = simulation;
            // utilizo el graphicsDevice para dibujar las cajas
            this.graphicsDevice = graphicsDevice;

            // Crear colisiones para el suelo como caja
            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 boxSize = new System.Numerics.Vector3(5000f, 100f, 5000f);
            // Crear el Collidable Box
            var boxShape = new Box(boxSize.X, boxSize.Y, boxSize.Z); // Crea la forma del box
            var boxShapeIndex = simulation.Shapes.Add(boxShape); // Registra la forma en el sistema de colisiones
            // Crear el objeto estático para el suelo
            floorHandle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(0, -50f, 0), // Posición inicial del box
                boxShapeIndex // Fricción
            ));


            // Crear colisiones para la torre eiffel
            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 torreSize = new System.Numerics.Vector3(180f, 500f, 180f);
            // Crear el Collidable Box
            var torreShape = new Box(torreSize.X, torreSize.Y, torreSize.Z); // Crea la forma del box
            var torreOrientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll((float)Math.PI / 10, 0, 0);
            var torreShapeIndex = simulation.Shapes.Add(torreShape); // Registra la forma en el sistema de colisiones

            // Crear el objeto estático para el suelo
            torreHandle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(-1300f, 1f, 700f), // Posición inicial del box
                torreOrientation,
                torreShapeIndex // Fricción

            ));


            // Definir las dimensiones de la rampa

            var ramp1Size = new System.Numerics.Vector3(900f, 100f, 230f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var ramp1Position = new System.Numerics.Vector3(455f, 620f, 900f); // Posición de la rampa
            var ramp1Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 5f, 0); // Rotación
            var ramp1Shape = new Box(ramp1Size.X, ramp1Size.Y, ramp1Size.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var ramp1ShapeIndex = simulation.Shapes.Add(ramp1Shape);

            // Crear el cuerpo estático para la rampa
            rampa1BodyHandle = simulation.Statics.Add(new StaticDescription(
                ramp1Position, // Posición inicial de la rampa
                ramp1Orientation,
                ramp1ShapeIndex
            ));

            var ramp2Size = new System.Numerics.Vector3(900f, 100f, 280f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var ramp2Position = new System.Numerics.Vector3(290f, 480f, 900f); // Posición de la rampa
            var ramp2Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 4, 0); // Rotación
            var ramp2Shape = new Box(ramp2Size.X, ramp2Size.Y, ramp2Size.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var ramp2ShapeIndex = simulation.Shapes.Add(ramp2Shape);

            // Crear el cuerpo estático para la rampa
            rampa2BodyHandle = simulation.Statics.Add(new StaticDescription(
                ramp2Position, // Posición inicial de la rampa
                ramp2Orientation,
                ramp2ShapeIndex
            ));

            var ramp3Size = new System.Numerics.Vector3(900f, 100f, 280f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var ramp3Position = new System.Numerics.Vector3(100f, 320f, 900f); // Posición de la rampa
            var ramp3Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 5, 0); // Rotación
            var ramp3Shape = new Box(ramp3Size.X, ramp3Size.Y, ramp3Size.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var ramp3ShapeIndex = simulation.Shapes.Add(ramp3Shape);

            // Crear el cuerpo estático para la rampa
            rampa3BodyHandle = simulation.Statics.Add(new StaticDescription(
                ramp3Position, // Posición inicial de la rampa
                ramp3Orientation,
                ramp3ShapeIndex
            ));

            var ramp4Size = new System.Numerics.Vector3(900f, 100f, 280f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var ramp4Position = new System.Numerics.Vector3(-25f, 235f, 900f); // Posición de la rampa
            var ramp4Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 6, 0); // Rotación
            var ramp4Shape = new Box(ramp4Size.X, ramp4Size.Y, ramp4Size.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var ramp4ShapeIndex = simulation.Shapes.Add(ramp4Shape);

            // Crear el cuerpo estático para la rampa
            rampa4BodyHandle = simulation.Statics.Add(new StaticDescription(
                ramp4Position, // Posición inicial de la rampa
                ramp4Orientation,
                ramp4ShapeIndex
            ));

            var ramp5Size = new System.Numerics.Vector3(900f, 100f, 280f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var ramp5Position = new System.Numerics.Vector3(-245f, 135f, 900f); // Posición de la rampa
            var ramp5Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 9, 0); // Rotación
            var ramp5Shape = new Box(ramp5Size.X, ramp5Size.Y, ramp5Size.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var ramp5ShapeIndex = simulation.Shapes.Add(ramp5Shape);

            // Crear el cuerpo estático para la rampa
            rampa5BodyHandle = simulation.Statics.Add(new StaticDescription(
                ramp5Position, // Posición inicial de la rampa
                ramp5Orientation,
                ramp5ShapeIndex
            ));


            var ramp6Size = new System.Numerics.Vector3(900f, 100f, 280f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var ramp6Position = new System.Numerics.Vector3(-480f, 65f, 900f); // Posición de la rampa
            var ramp6Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 13, 0); // Rotación
            var ramp6Shape = new Box(ramp6Size.X, ramp6Size.Y, ramp6Size.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var ramp6ShapeIndex = simulation.Shapes.Add(ramp6Shape);

            // Crear el cuerpo estático para la rampa
            rampa6BodyHandle = simulation.Statics.Add(new StaticDescription(
                ramp6Position, // Posición inicial de la rampa
                ramp6Orientation,
                ramp6ShapeIndex
            ));


            var ramp7Size = new System.Numerics.Vector3(900f, 95f, 280f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var ramp7Position = new System.Numerics.Vector3(-720f, 15f, 900f); // Posición de la rampa
            var ramp7Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 16, 0); // Rotación
            var ramp7Shape = new Box(ramp7Size.X, ramp7Size.Y, ramp7Size.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var ramp7ShapeIndex = simulation.Shapes.Add(ramp7Shape);

            // Crear el cuerpo estático para la rampa
            rampa7BodyHandle = simulation.Statics.Add(new StaticDescription(
                ramp7Position, // Posición inicial de la rampa
                ramp7Orientation,
                ramp7ShapeIndex
            ));

            var ramp8Size = new System.Numerics.Vector3(900f, 100f, 260f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var ramp8Position = new System.Numerics.Vector3(-950f, -32f, 900f); // Posición de la rampa
            var ramp8Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 2, (float)Math.PI / 17, 0); // Rotación
            var ramp8Shape = new Box(ramp8Size.X, ramp8Size.Y, ramp8Size.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var ramp8ShapeIndex = simulation.Shapes.Add(ramp8Shape);

            // Crear el cuerpo estático para la rampa
            rampa8BodyHandle = simulation.Statics.Add(new StaticDescription(
                ramp8Position, // Posición inicial de la rampa
                ramp8Orientation,
                ramp8ShapeIndex
            ));

            var rampParedSize = new System.Numerics.Vector3(1000f, 100f, 900f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var rampParedPosition = new System.Numerics.Vector3(-30f, 50f, 900f); // Posición de la rampa
            var rampParedShape = new Box(rampParedSize.X, rampParedSize.Y, rampParedSize.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var rampParedShapeIndex = simulation.Shapes.Add(rampParedShape);

            // Crear el cuerpo estático para la rampa
            rampaParedBodyHandle = simulation.Statics.Add(new StaticDescription(
                rampParedPosition, // Posición inicial de la rampa
                rampParedShapeIndex
            ));



            /* ESTA IMPLEMENTACION ES CON UN CONVEX HULL, ES LA IDEAL PERO NO PUDE HACERLA FUNCIONAR MUY BIEN 
            // Crear colisiones para la rampa
            var rampVerticesTrasladados = ObtenerVerticesTransformados(rampa, new System.Numerics.Vector3(0f, -10f, 1000f), 10f, -(float)Math.PI / 2, (float)Math.PI / 2);
            // transformo lista a span para parametro de convexHull
            Span<System.Numerics.Vector3> verticesSpan = CollectionsMarshal.AsSpan(rampVerticesTrasladados);
            // crea el convexHull para la rampa
            rampHull = new ConvexHull(verticesSpan, simulation.BufferPool, out var rampCenter);
            // Registra la forma de la rampa en el sistema de colisiones y obtiene un TypedIndex.
            var rampShapeIndex = simulation.Shapes.Add(rampHull);

            // Crear el cuerpo estático para la rampa
            rampaBodyHandle = simulation.Statics.Add(new StaticDescription(
            rampCenter, // Posición inicial de la rampa
            rampShapeIndex
            )); */




            // Crear colisiones con rampa doble
            // Definir las dimensiones de la rampa
            var rampaDobleSize = new System.Numerics.Vector3(540f, 100f, 300f); //Tamaño
            var rampaDobleShape = new Box(rampaDobleSize.X, rampaDobleSize.Y, rampaDobleSize.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var rampaDobleShapeIndex = simulation.Shapes.Add(rampaDobleShape);

            // Crear el cuerpo estático para la rampa
            rampaDoble1BodyHandle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(-1220f, -10f, -1100f), // Posición inicial de la rampa
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(0, 0, (float)Math.PI / 20),
                rampaDobleShapeIndex
            ));
            // Crear el cuerpo estático para la rampa
            rampaDoble2BodyHandle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(-550f, -10f, -1100f), // Posición inicial de la rampa
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(0, 0, -(float)Math.PI / 20),
                rampaDobleShapeIndex
            ));

            // Definir las dimensiones de la rampa
            var rampaPanzaSize = new System.Numerics.Vector3(330f, 150f, 260f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
             var rampaPanzaShape = new Box(rampaPanzaSize.X, rampaPanzaSize.Y, rampaPanzaSize.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var rampaPanzaShapeIndex = simulation.Shapes.Add(rampaPanzaShape);

            // Crear el cuerpo estático para la rampa
            rampaPanza1BodyHandle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(-1125f, -20f, 50f), // Posición inicial de la rampa
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll((float)Math.PI / 3, (float)Math.PI / 7, 0),
                rampaPanzaShapeIndex
            ));
            // Crear el cuerpo estático para la rampa
            rampaPanza2BodyHandle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(-1275f, -20f, -50f), // Posición inicial de la rampa
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll((float)Math.PI / 3, -(float)Math.PI / 7, 0),
                rampaPanzaShapeIndex
            ));

            // Definir las dimensiones del caballo
            var caballo1PanzaSize = new System.Numerics.Vector3(100f, 500f, 260f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var caballo1Position = new System.Numerics.Vector3(1000f, 730f, 300f); // Posición del caballo
            var caballo1Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI * (5 / 4), 0, 0); // Rotación
            var caballo1Shape = new Box(caballo1PanzaSize.X, caballo1PanzaSize.Y, caballo1PanzaSize.Z);

            // Registrar la forma del caballo en el sistema de colisiones y obtener un TypedIndex
            var caballo1ShapeIndex = simulation.Shapes.Add(caballo1Shape);
            // Crear el cuerpo estático para el caballo
            caballo1BodyHandle = simulation.Statics.Add(new StaticDescription(
                caballo1Position, // Posición inicial del caballo
                caballo1Orientation,
                caballo1ShapeIndex
            ));

            // Definir las dimensiones del caballo
            var caballo2Size = new System.Numerics.Vector3(100f, 550f, 260f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var caballo2Position = new System.Numerics.Vector3(1200f, 730f, -100f); // Posición de la rampa
            var caballo2Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 4, 0, 0); // Rotación
            var caballo2Shape = new Box(caballo2Size.X, caballo2Size.Y, caballo2Size.Z);
            // Registrar la forma del caballo en el sistema de colisiones y obtener un TypedIndex
            var caballo2ShapeIndex = simulation.Shapes.Add(caballo2Shape);
            // Crear el cuerpo estático para el caballo
            caballo2BodyHandle = simulation.Statics.Add(new StaticDescription(
                caballo2Position, // Posición inicial del caballo
                caballo2Orientation,
                caballo2ShapeIndex
            ));

            // Definir las dimensiones del tablero
            var ajedrezSize = new System.Numerics.Vector3(500f, 100f, 500f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var ajedrezPosition = new System.Numerics.Vector3(-1200f, 2f, 1700f); // Posición de la rampa
            var ajedrezOrientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 4, 0, 0); // Rotación

            var ajedrezShape = new Box(ajedrezSize.X, ajedrezSize.Y, ajedrezSize.Z);

            // Registrar la forma del tablero en el sistema de colisiones y obtener un TypedIndex
            var ajedrezShapeIndex = simulation.Shapes.Add(ajedrezShape);

            // Crear el cuerpo estático para el trablero
            ajedrezBodyHandle = simulation.Statics.Add(new StaticDescription(
                ajedrezPosition,
                ajedrezOrientation,
                ajedrezShapeIndex
            ));

            // Definir las dimensiones del legoPJ
            var legoPJSize = new System.Numerics.Vector3(150f, 100f, 150f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var legoPJPosition = new System.Numerics.Vector3(1200f, 2f, 1700f); // Posición del legoPJ
            var legoPJOrientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 4, 0, 0); // Rotación
            var legoPJShape = new Box(legoPJSize.X, legoPJSize.Y, legoPJSize.Z);

            // Registrar la forma del legoPJ en el sistema de colisiones y obtener un TypedIndex
            var legoPJShapeIndex = simulation.Shapes.Add(legoPJShape);

            // Crear el cuerpo estático para el legoPJ
            legoBodyHandle = simulation.Statics.Add(new StaticDescription(
                legoPJPosition, // Posición inicial del legoPJ
                legoPJOrientation,
                legoPJShapeIndex
            ));

            // Definir las dimensiones del puente
            var puenteBaseSize = new System.Numerics.Vector3(500f, 150f, 400f); // Ejemplo de tamaño

            var puenteBaseShape = new Box(puenteBaseSize.X, puenteBaseSize.Y, puenteBaseSize.Z);

            // Registrar la forma del puente en el sistema de colisiones y obtener un TypedIndex
            var puenteBaseShapeIndex = simulation.Shapes.Add(puenteBaseShape);

            // Crear el cuerpo estático para el puente
            puenteBase1Handle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(305f, -200f, -1625f),
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 4, (float)Math.PI / 8, (float)Math.PI / -2), // Rotación,
                puenteBaseShapeIndex
            ));
            // Crear el cuerpo estático para el puente
            puenteBase2Handle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(435f, -200f, -1755f),
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 4, -(float)Math.PI / 8, (float)Math.PI / -2),
                puenteBaseShapeIndex
            ));

            // Definir las dimensiones del puente
            var puenteParedSize = new System.Numerics.Vector3(300f, 30f, 550f); // Ejemplo de tamaño
            var puenteParedShape = new Box(puenteParedSize.X, puenteParedSize.Y, puenteParedSize.Z);

            // Registrar la forma del puente en el sistema de colisiones y obtener un TypedIndex
            var puenteParedShapeIndex = simulation.Shapes.Add(puenteParedShape);
            // Crear el cuerpo estático para el puente
            puentePared1Handle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(320f, 0f, -1775f),
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 4, 0, (float)Math.PI / -2),
                puenteParedShapeIndex
            ));
            // Crear el cuerpo estático para el puente
            puentePared2Handle = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(430f, 0f, -1635f),
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 4, 0, (float)Math.PI / -2),
                puenteParedShapeIndex
            ));


        }
    }
}
