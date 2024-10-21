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
    class ToyCity
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";

        public const float DistanceBetweenCities = 2100f;

        private Model Model { get; set; }
        private List<Matrix> WorldMatrices { get; set; }
        private Effect Effect { get; set; }

        private Texture2D texture { get; set; }


        /// <summary>
        /// Creates a City Scene with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>
        public ToyCity(ContentManager content)
        {
            // Load the City Model
            Model = content.Load<Model>(ContentFolder3D + "scene/city");

            // Load an effect that will be used to draw the scene
            Effect = content.Load<Effect>(ContentFolderEffects + "CityShader");

            // Get the first texture we find
            // The city model only contains a single texture
            texture = content.Load<Texture2D>(ContentFolder3D + "scene/tex/Palette");

            // Set the Texture to the Effect
            // 
            Effect.Parameters["ModelTexture"].SetValue(texture);

            // Assign the mesh effect
            // A model contains a collection of meshes
            foreach (var mesh in Model.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                    // Assign the loaded effect to each part
                    meshPart.Effect = Effect;
            }

            // Create a list of places where the city model will be drawn
            /*
            WorldMatrices = new List<Matrix>()
            {
                Matrix.Identity,
                Matrix.CreateTranslation(Vector3.Right * DistanceBetweenCities),
                Matrix.CreateTranslation(Vector3.Left * DistanceBetweenCities),
                Matrix.CreateTranslation(Vector3.Forward * DistanceBetweenCities),
                Matrix.CreateTranslation(Vector3.Backward * DistanceBetweenCities),
                Matrix.CreateTranslation((Vector3.Forward + Vector3.Right) * DistanceBetweenCities),
                Matrix.CreateTranslation((Vector3.Forward + Vector3.Left) * DistanceBetweenCities),
                Matrix.CreateTranslation((Vector3.Backward + Vector3.Right) * DistanceBetweenCities),
                Matrix.CreateTranslation((Vector3.Backward + Vector3.Left) * DistanceBetweenCities),
            };
            */
            /*
            float scaleZ = 1.37f; // Escalar en Z por 2, ajusta según sea necesario

            WorldMatrices = new List<Matrix>()
            {
                Matrix.CreateScale(1, 1, scaleZ) * Matrix.Identity,
                Matrix.CreateScale(1, 1, scaleZ) * Matrix.CreateTranslation(Vector3.Right * DistanceBetweenCities),
                Matrix.CreateScale(1, 1, scaleZ) * Matrix.CreateTranslation(Vector3.Left * DistanceBetweenCities),
                Matrix.CreateScale(1, 1, scaleZ) * Matrix.CreateTranslation(Vector3.Forward * DistanceBetweenCities),
                Matrix.CreateScale(1, 1, scaleZ) * Matrix.CreateTranslation(Vector3.Backward * DistanceBetweenCities),
                Matrix.CreateScale(1, 1, scaleZ) * Matrix.CreateTranslation((Vector3.Forward + Vector3.Right) * DistanceBetweenCities),
                Matrix.CreateScale(1, 1, scaleZ) * Matrix.CreateTranslation((Vector3.Forward + Vector3.Left) * DistanceBetweenCities),
                Matrix.CreateScale(1, 1, scaleZ) * Matrix.CreateTranslation((Vector3.Backward + Vector3.Right) * DistanceBetweenCities),
                Matrix.CreateScale(1, 1, scaleZ) * Matrix.CreateTranslation((Vector3.Backward + Vector3.Left) * DistanceBetweenCities),
            };
            */

        }

        /// <summary>
        /// Draws the City Scene
        /// </summary>
        /// <param name="gameTime">The Game Time for this frame</param>
        /// <param name="view">A view matrix, generally from a camera</param>
        /// <param name="projection">A projection matrix</param>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            // Set the View and Projection matrices, needed to draw every 3D model
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);

            // Get the base transform for each mesh
            // These are center-relative matrices that put every mesh of a model in their corresponding location
            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            var traslacion = new Vector3(-1200f, 0f, 1700f);


            // For each mesh in the model,
            foreach (var mesh in Model.Meshes)
            {
                // Obtain the world matrix for that mesh (relative to the parent)
                var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];

                // We set the main matrices for each mesh to draw
                Effect.Parameters["World"].SetValue(meshWorld * Matrix.Identity * Matrix.CreateRotationY(MathHelper.Pi / 4) * Matrix.CreateScale(0.23f) * Matrix.CreateTranslation(traslacion));

                // Draw the mesh
                mesh.Draw();
                /*

                // Then for each world matrix
                foreach (var worldMatrix in WorldMatrices)
                {
                    // We set the main matrices for each mesh to draw
                    Effect.Parameters["World"].SetValue(meshWorld * worldMatrix * Matrix.CreateRotationY(MathHelper.Pi/4) * Matrix.CreateScale(0.08f) * Matrix.CreateTranslation(traslacion));

                    // Draw the mesh
                    mesh.Draw();
                }*/
            }

        }
    }
}
