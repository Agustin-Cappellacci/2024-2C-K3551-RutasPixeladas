using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
//using System.Numerics;
using System.Text;

namespace TGC.MonoGame.TP.Content.Models
{
    /// <summary>
    /// A Car Model to be drawn
    /// </summary>
    class Cuarto
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";

        private Model Model { get; set; }
        private Model ChairModel { get; set; }
        private Model BedModel { get; set; }
        private Effect Effect { get; set; }
        private Effect EffectChair { get; set; }
        private Effect EffectBed { get; set; }
        private Texture2D textureFloor { get; set; }
        private Texture2D textureChair { get; set; }
        private List<Matrix> WorldMatrices { get; set; }
        private Matrix ChairWorld { get; set; }
        private Matrix BedWorld { get; set; }

        private Vector3 lightPosition = new Vector3(1000, 2000, 1000);



        // colisiones
        private Simulation simulation;
        private GraphicsDevice graphicsDevice;
        private StaticHandle pata1Cama;
        private StaticHandle pata2Cama;
        private StaticHandle pata3Cama;
        private StaticHandle pata4Cama;
        private StaticHandle cama;
        private StaticHandle respaldoCama;
        private StaticHandle almohadaCama;

        private StaticHandle pata1Silla;
        private StaticHandle pata2Silla;
        private StaticHandle pata3Silla;
        private StaticHandle pata4Silla;
        private StaticHandle respaldoSilla;
        private StaticHandle respaldoPared1Silla;
        private StaticHandle respaldoPared2Silla;
        private StaticHandle silla;



        // <summary>
        /// Creates a Car Model with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>
        public Cuarto(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice)
        {
            // Load the Car Model
            Model = content.Load<Model>(ContentFolder3D + "escenario/Floor");
            ChairModel = content.Load<Model>(ContentFolder3D + "chair/chair");
            BedModel = content.Load<Model>(ContentFolder3D + "Cama/bedSingle");


            // Load an effect that will be used to draw the scene
            Effect = content.Load<Effect>(ContentFolderEffects + "BlinnPhong");
            EffectBed = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            EffectChair = content.Load<Effect>(ContentFolderEffects + "BlinnPhong");

            // load texture

            textureChair = content.Load<Texture2D>(ContentFolder3D + "chair/chair_tex"); // Aseg�rate de usar la ruta correcta
            textureFloor = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/piso"); // Aseg�rate de usar la ruta correcta



            // Assign the mesh effect
            // A model contains a collection of meshes
            foreach (var mesh in Model.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }


            foreach (var mesh in ChairModel.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = EffectChair; // Usa un efecto que soporte texturas
                }
            }

            foreach (var mesh in BedModel.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = EffectBed;
                }
            }

            // Set uniforms
            

            // Set uniforms
            
            List<Matrix> positionMatrix = new List<Matrix>()
            {
                Matrix.Identity,
                Matrix.CreateTranslation(Vector3.Right *75f),
                Matrix.CreateTranslation(Vector3.Left * 75f),
                Matrix.CreateTranslation(Vector3.Forward * 150f),
                Matrix.CreateTranslation(Vector3.Backward * 150f),
                Matrix.CreateTranslation(Vector3.Right * 2f* 75f),
                Matrix.CreateTranslation(Vector3.Left * 2f* 75f),
                Matrix.CreateTranslation(Vector3.Forward * 150f + Vector3.Right * 75f),
                Matrix.CreateTranslation(Vector3.Forward * 150f + Vector3.Left * 75f),
                Matrix.CreateTranslation(Vector3.Forward * 150f + Vector3.Right * 2f* 75f),
                Matrix.CreateTranslation(Vector3.Forward * 150f + Vector3.Left * 2f*75f),
                Matrix.CreateTranslation(Vector3.Backward * 150f + Vector3.Right * 75f),
                Matrix.CreateTranslation(Vector3.Backward * 150f + Vector3.Left * 75f),
                Matrix.CreateTranslation(Vector3.Backward * 150f + Vector3.Right *2f* 75f),
                Matrix.CreateTranslation(Vector3.Backward * 150f + Vector3.Left *2f* 75f),
            };



            Vector3 traslacion = new Vector3(0f, -1f, 0f);
            Vector3 traslacionChair = new Vector3(1500f, 400f, -1900f);
            Vector3 traslacionBed = new Vector3(3300f, -20f, -1000f);

            WorldMatrices = new List<Matrix>();

            foreach (var position in positionMatrix)
            {
                WorldMatrices.Add(position * Matrix.CreateTranslation(traslacion) * Matrix.CreateScale(10f));

                traslacion += new Vector3(0f, 0.001f, 0f);
            }
            ChairWorld = Matrix.CreateRotationY(-MathHelper.Pi / 2) * Matrix.CreateScale(10f) * Matrix.CreateTranslation(traslacionChair);
            BedWorld = Matrix.CreateScale(300f) * Matrix.CreateTranslation(traslacionBed);

            inicializadorColisionables(simulation, graphicsDevice);

        }

        // <summary>
        /// Draws the City Scene
        /// </summary>
        /// <param name="gameTime">The Game Time for this frame</param>
        /// <param name="view">A view matrix, generally from a camera</param>
        /// <param name="projection">A projection matrix</param>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            /*
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);

            EffectChair.Parameters["View"].SetValue(view);
            EffectChair.Parameters["Projection"].SetValue(projection);
            */

            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            //EffectBed.Parameters["View"].SetValue(view);
            //EffectBed.Parameters["Projection"].SetValue(projection);

            var random = new Random(Seed: 0);

            // Piso
            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach (var mesh in Model.Meshes)
            {
                var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];

                foreach (var worldMatrix in WorldMatrices)
                {
                    Effect.Parameters["ambientColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                    Effect.Parameters["diffuseColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                    Effect.Parameters["specularColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));

                    Effect.Parameters["KAmbient"].SetValue(0.3f);
                    Effect.Parameters["KDiffuse"].SetValue(0.8f);
                    Effect.Parameters["KSpecular"].SetValue(0.5f);
                    Effect.Parameters["shininess"].SetValue(50.0f);

                    Effect.Parameters["lightPosition"].SetValue(lightPosition);
                    Effect.Parameters["eyePosition"].SetValue(cameraPosition);
                    Effect.Parameters["ModelTexture"].SetValue(textureFloor);
                    // We set the main matrices for each mesh to draw
                    Effect.Parameters["World"].SetValue(meshWorld * worldMatrix);
                    // InverseTransposeWorld is used to rotate normals
                    Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorld)));
                    // WorldViewProjection is used to transform from model space to clip space
                    Effect.Parameters["WorldViewProjection"].SetValue(meshWorld * worldMatrix * view * projection);

                    // Draw the mesh
                    mesh.Draw();

               
                }
            }

            // Silla
            var modelMeshesBaseTransformsChair = new Matrix[ChairModel.Bones.Count];
            ChairModel.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransformsChair);

            

            foreach (var mesh in ChairModel.Meshes)
            {
                EffectChair.Parameters["ambientColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                EffectChair.Parameters["diffuseColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                EffectChair.Parameters["specularColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));

                EffectChair.Parameters["KAmbient"].SetValue(0.7f);
                EffectChair.Parameters["KDiffuse"].SetValue(0.3f);
                EffectChair.Parameters["KSpecular"].SetValue(2.5f);
                EffectChair.Parameters["shininess"].SetValue(20.0f);
                
                lightPosition = new Vector3(0, 2000, 0);

                EffectChair.Parameters["lightPosition"].SetValue(lightPosition);
                EffectChair.Parameters["eyePosition"].SetValue(cameraPosition);

                EffectChair.Parameters["ModelTexture"].SetValue(textureChair);
                var meshWorldChair = modelMeshesBaseTransformsChair[mesh.ParentBone.Index];
                // We set the main matrices for each mesh to draw
                EffectChair.Parameters["World"].SetValue(meshWorldChair * ChairWorld);
                // InverseTransposeWorld is used to rotate normals
                EffectChair.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(ChairWorld)));
                // WorldViewProjection is used to transform from model space to clip space
                EffectChair.Parameters["WorldViewProjection"].SetValue(meshWorldChair * ChairWorld * view * projection);


                // Draw the mesh
                mesh.Draw();
            }

            // Cama
            var modelMeshesBaseTransformsBed = new Matrix[BedModel.Bones.Count];
            BedModel.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransformsBed);

            foreach (var mesh in BedModel.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    var colorAzul = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
                    
                    EffectBed.Parameters["DiffuseColor"].SetValue(colorAzul);
                    
                    var meshWorldBed = modelMeshesBaseTransformsBed[mesh.ParentBone.Index];
                    /*// We set the main matrices for each mesh to draw
                    EffectBed.Parameters["World"].SetValue(meshWorldBed * BedWorld);
                    */

                    EffectBed.Parameters["ambientColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                    EffectBed.Parameters["diffColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                    EffectBed.Parameters["specularColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));

                    EffectBed.Parameters["KAmbient"].SetValue(0.3f);
                    EffectBed.Parameters["KDiffuse"].SetValue(0.8f);
                    EffectBed.Parameters["KSpecular"].SetValue(0.1f);
                    EffectBed.Parameters["shininess"].SetValue(1.0f);

                    EffectBed.Parameters["lightPosition"].SetValue(lightPosition);
                    EffectBed.Parameters["eyePosition"].SetValue(cameraPosition);
                    // We set the main matrices for each mesh to draw
                    EffectBed.Parameters["World"].SetValue(meshWorldBed * BedWorld);
                    // InverseTransposeWorld is used to rotate normals
                    EffectBed.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(meshWorldBed)));
                    // WorldViewProjection is used to transform from model space to clip space
                    EffectBed.Parameters["WorldViewProjection"].SetValue(meshWorldBed * BedWorld * view * projection);
                }
                // Draw the mesh
                mesh.Draw();
            }
            
        }

        public void DrawCollisionBoxes(Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Dibujar la caja de colisión de la cama
            DrawBox(Matrix.CreateTranslation(1350f, 440f, 680f), new Vector3(1680f, 570f, 3380f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de pata cama
            DrawBox(Matrix.CreateTranslation(658f, 75f, -888f), new Vector3(315f, 170f, 224f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de pata cama
            DrawBox(Matrix.CreateTranslation(1919f, 75f, -888f), new Vector3(315f, 170f, 224f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de pata cama
            DrawBox(Matrix.CreateTranslation(658f, 75f, 2220f), new Vector3(315f, 170f, 294f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de pata cama
            DrawBox(Matrix.CreateTranslation(1919f, 75f, 2220f), new Vector3(315f, 170f, 294f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de la almohada
            DrawBox(Matrix.CreateTranslation(1265, 720, 1800), new Vector3(1000f, 150f, 600f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión del respaldo
            DrawBox(Matrix.CreateTranslation(1298f, 630f, 2220f), new Vector3(1680f, 1000f, 300f), viewMatrix, projectionMatrix);

            // Dibujar la caja de colisión de pata silla
            DrawBox(Matrix.CreateFromYawPitchRoll((float)Math.PI / 3, (float)Math.PI / 12, 0)*Matrix.CreateTranslation(1320f, 250f, -2100f), new Vector3(50f, 500f, 50f), viewMatrix, projectionMatrix);
             // Dibujar la caja de colisión de pata silla
            DrawBox(Matrix.CreateFromYawPitchRoll((float)Math.PI / 3, -(float)Math.PI / 12, 0)*Matrix.CreateTranslation(1630f, 250f, -1700f), new Vector3(50f, 500f, 50f), viewMatrix, projectionMatrix);
             // Dibujar la caja de colisión de pata silla
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 3, (float)Math.PI / 12, 0)*Matrix.CreateTranslation(1630f, 250f, -2100f), new Vector3(50f, 500f, 50f), viewMatrix, projectionMatrix);
             // Dibujar la caja de colisión de pata silla
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI / 3, -(float)Math.PI / 12, 0)*Matrix.CreateTranslation(1320f, 250f, -1700f), new Vector3(50f, 500f, 50f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de silla
            DrawBox(Matrix.CreateTranslation(1498f, 500f, -1920f), new Vector3(400f, 100f, 400f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de respaldo silla
            DrawBox(Matrix.CreateTranslation(1698f, 600f, -1920f), new Vector3(100f, 400f, 400f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de respaldo silla
            DrawBox(Matrix.CreateTranslation(1498f, 600f, -2120f), new Vector3(400f, 200f, 100f), viewMatrix, projectionMatrix);
            // Dibujar la caja de colisión de respaldo silla
            DrawBox(Matrix.CreateTranslation(1498f, 600f, -1680f), new Vector3(400f, 200f, 100f), viewMatrix, projectionMatrix);

        }

        private VertexBuffer? _vertexBuffer;
        private IndexBuffer? _indexBuffer;
        private BasicEffect? _effect;
        public void DrawBox(Matrix worldMatrix, Vector3 size, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (_effect == null){
                _effect = new BasicEffect(graphicsDevice);
                _effect.VertexColorEnabled = true;
            }
            _effect.World = Matrix.CreateScale(size/2f) * worldMatrix;
            _effect.View = viewMatrix;
            _effect.Projection = projectionMatrix;
            // Crear un efecto básico para dibujar la caja
            

            // Definir los vértices de una caja (un cubo unitario que escalaremos)
            if (_vertexBuffer == null){
                VertexPositionColor[] vertices = new VertexPositionColor[8];
                vertices[0] = new VertexPositionColor(new Vector3(-1, 1, 1), Microsoft.Xna.Framework.Color.Red);   // Front top left
                vertices[1] = new VertexPositionColor(new Vector3(1, 1, 1),  Microsoft.Xna.Framework.Color.Red);    // Front top right
                vertices[2] = new VertexPositionColor(new Vector3(-1, -1, 1),  Microsoft.Xna.Framework.Color.Red);  // Front bottom left
                vertices[3] = new VertexPositionColor(new Vector3(1, -1, 1),  Microsoft.Xna.Framework.Color.Red);   // Front bottom right
                vertices[4] = new VertexPositionColor(new Vector3(-1, 1, -1),  Microsoft.Xna.Framework.Color.Red);  // Back top left
                vertices[5] = new VertexPositionColor(new Vector3(1, 1, -1),  Microsoft.Xna.Framework.Color.Red);   // Back top right
                vertices[6] = new VertexPositionColor(new Vector3(-1, -1, -1),  Microsoft.Xna.Framework.Color.Red); // Back bottom left
                vertices[7] = new VertexPositionColor(new Vector3(1, -1, -1),  Microsoft.Xna.Framework.Color.Red);  // Back bottom right 
                _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 8, BufferUsage.None);
                _vertexBuffer.SetData(vertices);
            }
        

            // Escalar la caja en función del tamaño dado
            /*for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position *= size / 2f;
            }*/

            // Definir los índices que forman las líneas de la caja
            
            if (_indexBuffer == null){
                ushort[] indices = new ushort[]
                {
            0, 1, 1, 3, 3, 2, 2, 0,  // Front face
            4, 5, 5, 7, 7, 6, 6, 4,  // Back face
            0, 4, 1, 5, 2, 6, 3, 7   // Connecting edges
                };
                _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
                _indexBuffer.SetData(indices);
            } //mover a initialice
            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;
            // Dibujar la caja usando el efecto básico
            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.LineList,
                    0,
                    0,
                    _vertexBuffer.VertexCount,
                    0,
                    _indexBuffer.IndexCount / 2
                );
            }
        }

        private void inicializadorColisionables(Simulation simulation, GraphicsDevice graphicsDevice)
        {
            this.simulation = simulation;
            this.graphicsDevice = graphicsDevice;

            // Crear colisiones
            
            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 camaSize = new System.Numerics.Vector3(1680f, 570f, 3380f);
            // Crear el Collidable Box
            var camaShape = new Box(camaSize.X, camaSize.Y, camaSize.Z); // Crea la forma del box
            var camaShapeIndex = simulation.Shapes.Add(camaShape); // Registra la forma en el sistema de colisiones
            // Crear el objeto estático para la cama
            cama = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1350f, 440f, 680f), // Posición inicial del box
                camaShapeIndex // Fricción
            ));


            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 pataCamaSize = new System.Numerics.Vector3(315f, 170f, 224f);
            // Crear el Collidable Box
            var pataCamaShape = new Box(pataCamaSize.X, pataCamaSize.Y, pataCamaSize.Z); // Crea la forma del box
            var pataCamaShapeIndex = simulation.Shapes.Add(pataCamaShape); // Registra la forma en el sistema de colisiones
            // Crear el objeto estático para las patas de la cama
            pata1Cama = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(658f, 75f, -888f), // Posición inicial de la pata
                pataCamaShapeIndex // Fricción
            ));
            pata2Cama = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1919f, 75f, -888f), // Posición inicial de la pata
                pataCamaShapeIndex // Fricción
            ));
            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 pata2CamaSize = new System.Numerics.Vector3(315f, 170f, 294f);
            // Crear el Collidable Box
            var pata2CamaShape = new Box(pata2CamaSize.X, pata2CamaSize.Y, pata2CamaSize.Z); // Crea la forma del box
            var pata2CamaShapeIndex = simulation.Shapes.Add(pata2CamaShape); // Registra la forma en el sistema de colisiones
            pata3Cama = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(658f, 75f, 2220f), // Posición inicial de la pata
                pata2CamaShapeIndex // Fricción
            ));
            pata4Cama = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1919f, 75f, 2220f), // Posición inicial de la pata
                pata2CamaShapeIndex // Fricción
            ));

            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 respaldoCamaSize = new System.Numerics.Vector3(1680f, 1000f, 300f);
            // Crear el Collidable Box
            var respaldoCamaShape = new Box(respaldoCamaSize.X, respaldoCamaSize.Y, respaldoCamaSize.Z); // Crea la forma del box
            var respaldoCamaShapeIndex = simulation.Shapes.Add(respaldoCamaShape); // Registra la forma en el sistema de colisiones
            // Crear el objeto estático para la cama
            respaldoCama = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1298f, 630f, 2220f), // Posición inicial del box
                respaldoCamaShapeIndex // Fricción
            ));
            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 almohadaSize = new System.Numerics.Vector3(1000f, 150f, 600f);
            // Crear el Collidable Box
            var almohadaShape = new Box(almohadaSize.X, almohadaSize.Y, almohadaSize.Z); // Crea la forma del box
            var almohadaShapeIndex = simulation.Shapes.Add(almohadaShape); // Registra la forma en el sistema de colisiones
            // Crear el objeto estático para la cama
            almohadaCama = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1265, 720, 1800), // Posición inicial del box
                almohadaShapeIndex // Fricción
            ));

            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 sillaSize = new System.Numerics.Vector3(400f, 100f, 400f);
            // Crear el Collidable Box
            var sillaShape = new Box(sillaSize.X, sillaSize.Y, sillaSize.Z); // Crea la forma del box
            var sillaShapeIndex = simulation.Shapes.Add(sillaShape); // Registra la forma en el sistema de colisiones
            // Crear el objeto estático para la cama
            silla = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1498f, 500f, -1920f), // Posición inicial del box
                sillaShapeIndex // Fricción
            ));

            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 pataSillaSize = new System.Numerics.Vector3(50f, 500f, 50f);
            // Crear el Collidable Box
            var pataSillaShape = new Box(pataCamaSize.X, pataCamaSize.Y, pataCamaSize.Z); // Crea la forma del box
            var pataSillaShapeIndex = simulation.Shapes.Add(pataSillaShape); // Registra la forma en el sistema de colisiones
            // Crear el objeto estático para las patas de la cama
            pata1Silla = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1320f, 250f, -2100f), // Posición inicial de la pata
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll((float)Math.PI / 3, (float)Math.PI / 12, 0),
                pataSillaShapeIndex // Fricción
            ));
            pata2Silla = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1630f, 250f, -1700f), // Posición inicial de la pata
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll((float)Math.PI / 3, -(float)Math.PI / 12, 0),
                pataSillaShapeIndex // Fricción
            ));
            pata3Silla = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1630f, 250f, -2100f), // Posición inicial de la pata
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 3, (float)Math.PI / 12, 0),
                pataSillaShapeIndex // Fricción
            ));
            pata4Silla = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1320f, 250f, -1700f), // Posición inicial de la pata
                BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI / 3, -(float)Math.PI / 12, 0),
                pataSillaShapeIndex // Fricción
            ));


            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 respaldo1SillaSize = new System.Numerics.Vector3(100f, 400f, 400f);
            // Crear el Collidable Box
            var respaldo1SillaShape = new Box(respaldo1SillaSize.X, respaldo1SillaSize.Y, respaldo1SillaSize.Z); // Crea la forma del box
            var respaldo1SillaShapeIndex = simulation.Shapes.Add(respaldo1SillaShape); // Registra la forma en el sistema de colisiones
            // Crear el objeto estático para la cama
            respaldoSilla = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1698f, 600f, -1920f), // Posición inicial del box
                respaldo1SillaShapeIndex // Fricción
            ));
            // Define el tamaño del box (ancho, alto, profundo)
            System.Numerics.Vector3 respaldoParedSillaSize = new System.Numerics.Vector3(400f, 200f, 100f);
            // Crear el Collidable Box
            var respaldoParedSillaShape = new Box(respaldoParedSillaSize.X, respaldoParedSillaSize.Y, respaldoParedSillaSize.Z); // Crea la forma del box
            var respaldoParedSillaShapeIndex = simulation.Shapes.Add(respaldoParedSillaShape); // Registra la forma en el sistema de colisiones
            // Crear el objeto estático para la cama
            respaldoPared1Silla = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1498f, 600f, -2120f), // Posición inicial del box
                respaldoParedSillaShapeIndex // Fricción
            ));
            // Crear el objeto estático para la cama
            respaldoPared2Silla = simulation.Statics.Add(new StaticDescription(
                new System.Numerics.Vector3(1498f, 600f, -1680f), // Posición inicial del box
                respaldoParedSillaShapeIndex // Fricción
            ));



        }

    }
}