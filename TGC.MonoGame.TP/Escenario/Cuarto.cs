using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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


        // <summary>
        /// Creates a Car Model with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>
        public Cuarto (ContentManager content)
        {
            // Load the Car Model
            Model = content.Load<Model>(ContentFolder3D+"escenario/Floor");
            ChairModel = content.Load<Model>(ContentFolder3D+"chair/chair");
            BedModel = content.Load<Model>(ContentFolder3D+"Cama/bedSingle");

            Effect = content.Load<Effect>(ContentFolderEffects + "ModelsTexture");
            //Effect = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            // Load an effect that will be used to draw the scene
            EffectBed = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            EffectChair = content.Load<Effect>(ContentFolderEffects + "ModelsTexture");

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
                    // Set the texture from the mesh part, if available
                    //EffectBed.Parameters["Texture0"].SetValue(meshPart.Effect.Parameters["Texture"].GetValueTexture2D());

                    meshPart.Effect = EffectBed;

                }
            }
            

            WorldMatrices = new List<Matrix>()
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


        }

        // <summary>
        /// Draws the City Scene
        /// </summary>
        /// <param name="gameTime">The Game Time for this frame</param>
        /// <param name="view">A view matrix, generally from a camera</param>
        /// <param name="projection">A projection matrix</param>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);

            EffectChair.Parameters["View"].SetValue(view);
            EffectChair.Parameters["Projection"].SetValue(projection);

            EffectBed.Parameters["View"].SetValue(view);
            EffectBed.Parameters["Projection"].SetValue(projection);

            var random = new Random(Seed:0);

            var scala = 10f; // Escala entre 1.0 y 11.0
            // var colorcito = new Vector3((CameraPosition.X) + random.NextSingle(), CameraPosition.Y + random.NextSingle(), CameraPosition.Z + random.NextSingle());
            //var color = new Vector3(1.0f, 1.0f, 1.0f); //color marr�n
            //.Parameters["DiffuseColor"].SetValue(color);        /*Usamos verto3 porque es BasicEffect. Se usa vector4 si tenemos activado el AlphaShader*/
                
            var traslacion = new Vector3(0f, -1f, 0f);

            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach (var mesh in Model.Meshes)
            {   
                var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];
              //  Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * Matrix.CreateTranslation(traslacion) * Matrix.CreateScale(scala));

            //    mesh.Draw();

                foreach (var worldMatrix in WorldMatrices)
                {
                    Effect.Parameters["ModelTexture"].SetValue(textureFloor);
                    // We set the main matrices for each mesh to draw
                    Effect.Parameters["World"].SetValue(meshWorld * worldMatrix * Matrix.CreateTranslation(traslacion) * Matrix.CreateScale(scala));

                    // Draw the mesh
                    mesh.Draw();
                }
            }

            var modelMeshesBaseTransformsChair = new Matrix[ChairModel.Bones.Count];
            ChairModel.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransformsChair);

            var colorRojo = new Vector3(1.0f, 0.0f, 0.0f); //color rojo puro
            var traslacionChair = new Vector3(1500f, 400f, -1900f);
            //Effect.Parameters["DiffuseColor"].SetValue(colorRojo); 

            foreach (var mesh in ChairModel.Meshes)
            {
                EffectChair.Parameters["ModelTexture"].SetValue(textureChair);
                var meshWorldChair = modelMeshesBaseTransformsChair[mesh.ParentBone.Index];
                // We set the main matrices for each mesh to draw
                EffectChair.Parameters["World"].SetValue(meshWorldChair * Matrix.CreateRotationY(-MathHelper.Pi/2) * Matrix.CreateScale(10f) * Matrix.CreateTranslation(traslacionChair));
                // Draw the mesh
                mesh.Draw();
            }

            var modelMeshesBaseTransformsBed = new Matrix[BedModel.Bones.Count];
            BedModel.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransformsBed);


            //var colorAzul = new Vector3(0.0f, 0.0f, 1.0f); //color azul puro
            var traslacionBed = new Vector3(3300f, -20f, -1000f);
            //EffectBed.Parameters["DiffuseColor"].SetValue(colorAzul); 

            foreach (var mesh in BedModel.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    var colorAzul = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
                    EffectBed.Parameters["DiffuseColor"].SetValue(colorAzul);
                    var meshWorldBed = modelMeshesBaseTransformsBed[mesh.ParentBone.Index];
                    // We set the main matrices for each mesh to draw
                    EffectBed.Parameters["World"].SetValue(meshWorldBed * Matrix.CreateScale(300f) * Matrix.CreateTranslation(traslacionBed));
                }
                // Draw the mesh
                mesh.Draw();
            }
        
        }
    }
}