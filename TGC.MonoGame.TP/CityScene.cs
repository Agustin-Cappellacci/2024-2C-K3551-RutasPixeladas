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
    /// A City Scene to be drawn
    /// </summary>
    class CityScene
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";

        public const float DistanceBetweenCities = 2100f;

        private Model Model { get; set; }
        private Model Ajedrez {get; set;}
        private Model Lego {get; set;}
        private Model Torre {get; set;}
        private Model Cubo {get; set;}
        private Model LegoPJ {get; set;}

        private Model Puente {get; set;}



        private List<Matrix> WorldMatrices { get; set; }
        private Effect EffectMesa { get; set; }
        private Effect EffectChess { get; set; }
        private Effect EffectLego { get; set; }




        // <summary>
        /// Creates a City Scene with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>
        public CityScene(ContentManager content)
        {
            // Load the City Model
            Model = content.Load<Model>(ContentFolder3D + "escenario/Buffettable");

            Ajedrez = content.Load<Model>(ContentFolder3D + "escenario/chess");

            Lego = content.Load<Model>(ContentFolder3D + "escenario/legoBrick");

            Torre = content.Load<Model>(ContentFolder3D + "escenario/torre");

            Cubo = content.Load<Model>(ContentFolder3D + "escenario/Rubics_cube/cube");
            
            LegoPJ = content.Load<Model>(ContentFolder3D + "escenario/legoPJ/FireNinjaBlueOcatpus2/Fireninja_blueninja");

            Puente = content.Load<Model>(ContentFolder3D + "escenario/puente");


            // Load an effect that will be used to draw the scene
            //Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            
            EffectMesa = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            EffectChess = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            // Get the first texture we find
            // The city model only contains a single texture
            //var effect = Model.Meshes.FirstOrDefault().Effects.FirstOrDefault() as BasicEffect;
            //var texture = effect.Texture;
            //var texture = content.Load<Texture2D>(ContentFolder3D + "scene/tex/Palette"); // Asegúrate de usar la ruta correcta

            var colorMesa = new Vector3(0.8f, 0.8f, 0.8f); //gris
            var colorAjedrez = new Vector3(0.8f, 0.8f, 0.8f); //blanco
            var colorLego = new Vector3(1f, 0f, 0f); //rojo
            //EffectLego.Parameters["DiffuseColor"].SetValue(colorLego);
            EffectMesa.Parameters["DiffuseColor"].SetValue(colorMesa);
            EffectChess.Parameters["DiffuseColor"].SetValue(colorAjedrez);
            // Set the Texture to the Effect
             
            //Effect.Parameters["ModelTexture"].SetValue(texture);
            
            // Assign the mesh effect
            // A model contains a collection of meshes
            foreach (var mesh in Model.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                    // Assign the loaded effect to each part
                    meshPart.Effect = EffectMesa;
            }

            foreach (var mesh in Puente.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                    // Assign the loaded effect to each part
                    meshPart.Effect = EffectMesa;
            }

            foreach (var mesh in LegoPJ.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                    // Assign the loaded effect to each part
                    meshPart.Effect = EffectMesa;
            }

            foreach (var mesh in Ajedrez.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                    // Assign the loaded effect to each part
                    meshPart.Effect = EffectChess;
            }

            foreach (var mesh in Lego.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                    // Assign the loaded effect to each part
                    meshPart.Effect = EffectChess;
            }

             foreach (var mesh in Torre.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                    // Assign the loaded effect to each part
                    meshPart.Effect = EffectChess;
            }

            foreach (var mesh in Cubo.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                    // Assign the loaded effect to each part
                    meshPart.Effect = EffectChess;
            }


            // Create a list of places where the city model will be drawn
            WorldMatrices = new List<Matrix>()
            {
                Matrix.Identity,
                /*Matrix.CreateTranslation(Vector3.Right * DistanceBetweenCities),
                Matrix.CreateTranslation(Vector3.Left * DistanceBetweenCities),
                Matrix.CreateTranslation(Vector3.Forward * DistanceBetweenCities),
                Matrix.CreateTranslation(Vector3.Backward * DistanceBetweenCities),
                Matrix.CreateTranslation((Vector3.Forward + Vector3.Right) * DistanceBetweenCities),
                Matrix.CreateTranslation((Vector3.Forward + Vector3.Left) * DistanceBetweenCities),
                Matrix.CreateTranslation((Vector3.Backward + Vector3.Right) * DistanceBetweenCities),
                Matrix.CreateTranslation((Vector3.Backward + Vector3.Left) * DistanceBetweenCities),*/
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
            // Set the View and Projection matrices, needed to draw every 3D model
            EffectMesa.Parameters["View"].SetValue(view);
            EffectMesa.Parameters["Projection"].SetValue(projection);

            EffectChess.Parameters["View"].SetValue(view);
            EffectChess.Parameters["Projection"].SetValue(projection);

            // Get the base transform for each mesh
            // These are center-relative matrices that put every mesh of a model in their corresponding location
            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            var modelChessMeshesBaseTransforms = new Matrix[Ajedrez.Bones.Count];
            Ajedrez.CopyAbsoluteBoneTransformsTo(modelChessMeshesBaseTransforms);

            var modelLegoMeshesBaseTransforms = new Matrix[Lego.Bones.Count];
            Lego.CopyAbsoluteBoneTransformsTo(modelLegoMeshesBaseTransforms);

            var modelPuenteMeshesBaseTransforms = new Matrix[Puente.Bones.Count];
            Puente.CopyAbsoluteBoneTransformsTo(modelPuenteMeshesBaseTransforms);

            var modelTorreMeshesBaseTransforms = new Matrix[Torre.Bones.Count];
            Torre.CopyAbsoluteBoneTransformsTo(modelTorreMeshesBaseTransforms);

            var modelCuboMeshesBaseTransforms = new Matrix[Cubo.Bones.Count];
            Cubo.CopyAbsoluteBoneTransformsTo(modelCuboMeshesBaseTransforms);

            var modelLegoPJMeshesBaseTransforms = new Matrix[LegoPJ.Bones.Count];
            LegoPJ.CopyAbsoluteBoneTransformsTo(modelLegoPJMeshesBaseTransforms);

            // For each mesh in the model,
          /*  foreach (var mesh in Model.Meshes)
            {
                // Obtain the world matrix for that mesh (relative to the parent)
                EffectMesa.Parameters["DiffuseColor"].SetValue(new Vector3(0.8f, 0.8f, 0.8f));
                EffectMesa.Parameters["World"].SetValue(mesh.ParentBone.Transform * Matrix.CreateTranslation(10F, -17.7F, -20F) * Matrix.CreateScale(70f));
                mesh.Draw();

                /*var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];

                // Then for each world matrix
                foreach (var worldMatrix in WorldMatrices)
                {
                    // We set the main matrices for each mesh to draw
                    Effect.Parameters["World"].SetValue(meshWorld * worldMatrix);

                    // Draw the mesh
                    mesh.Draw();
                }*/
            //}

            foreach (var mesh in Ajedrez.Meshes)
            {
                // Obtain the world matrix for that mesh (relative to the parent)
                var meshWorldAjedrez = modelChessMeshesBaseTransforms[mesh.ParentBone.Index];

                EffectChess.Parameters["DiffuseColor"].SetValue(new Vector3(0f, 0f, 0f));
                EffectChess.Parameters["World"].SetValue(meshWorldAjedrez  * Matrix.CreateScale(0.2f) *  Matrix.CreateTranslation(-1000F, 0F, 1000F));
                mesh.Draw();
            }

            foreach (var mesh in LegoPJ.Meshes)
            {
                var meshWorldLegoPJ = modelLegoPJMeshesBaseTransforms[mesh.ParentBone.Index];
                // Obtain the world matrix for that mesh (relative to the parent)
                EffectChess.Parameters["DiffuseColor"].SetValue(new Vector3(0.9f, 0.9f, 0.9f));
                EffectChess.Parameters["World"].SetValue(meshWorldLegoPJ  * Matrix.CreateScale(0.1f) *  Matrix.CreateTranslation(0F, 0F, 800F));
                mesh.Draw();
            }

            foreach (var mesh in Puente.Meshes)
            {
                var meshWorldPuente = modelPuenteMeshesBaseTransforms[mesh.ParentBone.Index];
                // Obtain the world matrix for that mesh (relative to the parent)
                EffectChess.Parameters["DiffuseColor"].SetValue(new Vector3(1f, 0.9f, 0.2f));
                EffectChess.Parameters["World"].SetValue(meshWorldPuente* Matrix.CreateRotationX(MathHelper.Pi/-2) * Matrix.CreateScale(100f) *  Matrix.CreateTranslation(-100F, 0F, 150F));
                mesh.Draw();
            }



            foreach (var mesh in Torre.Meshes)
            {
                var meshWorldTorre = modelTorreMeshesBaseTransforms[mesh.ParentBone.Index];
                // Obtain the world matrix for that mesh (relative to the parent)
                EffectChess.Parameters["DiffuseColor"].SetValue(new Vector3(0f, 0f, 1f));
                EffectChess.Parameters["World"].SetValue(meshWorldTorre  * Matrix.CreateScale(1f) *  Matrix.CreateTranslation(300F, 0F, -300F));
                mesh.Draw();
            }
            
            var random = new Random(Seed:0);
            
            for (int i = 0; i < 15; i++){
                
               var scala = 0.01f + (0.1f - 0.01f) * random.NextSingle();
               var color = new Vector3(random.NextSingle(), random.NextSingle(), 0.6f);

                var traslacion = new Vector3(
                -770f + (1500f - (-770f)) * random.NextSingle(),
                0,
                -1200f + (1000f - (-1200f)) * random.NextSingle()
                );

//SCALA DE 0.5 A 1
//-770 A 650
//-1200 A 1000

                foreach (var mesh in Cubo.Meshes)
                {
                    var meshWorldCubo = modelCuboMeshesBaseTransforms[mesh.ParentBone.Index];
                 
                    EffectChess.Parameters["DiffuseColor"].SetValue(color);
                    EffectChess.Parameters["World"].SetValue(meshWorldCubo * Matrix.CreateScale(scala) *  Matrix.CreateTranslation(traslacion));
                    mesh.Draw();
                    
                    // Obtain the world matrix for that mesh (relative to the parent)
                    
                }
            }

            for (int i = 0; i < 50; i++){
                
               var scala = 0.2f + (0.5f - 0.2f) * random.NextSingle();
               var color = new Vector3(random.NextSingle(), 0, random.NextSingle());

                var traslacion = new Vector3(
                -770f + (150f - (-770f)) * random.NextSingle(),
                0,
                -1200f + (1000f - (-1200f)) * random.NextSingle()
                );

//SCALA DE 0.5 A 1
//-770 A 650
//-1200 A 1000

                foreach (var mesh in Lego.Meshes)
                {
                    var meshWorldLego = modelLegoMeshesBaseTransforms[mesh.ParentBone.Index];
                    // Obtain the world matrix for that mesh (relative to the parent)
                    EffectChess.Parameters["DiffuseColor"].SetValue(color);
                    EffectChess.Parameters["World"].SetValue(meshWorldLego *  Matrix.CreateTranslation(traslacion)  * Matrix.CreateScale(scala));
                    mesh.Draw();
                }
            }
        }
    }
}
