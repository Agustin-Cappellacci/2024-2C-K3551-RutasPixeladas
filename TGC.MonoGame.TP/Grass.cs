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
    class Grass
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";

        private Model Model { get; set; }
        private Effect Effect { get; set; }


        // <summary>
        /// Creates a Car Model with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>
        public Grass (ContentManager content)
        {
            // Load the Car Model
            Model = content.Load<Model>(ContentFolder3D+"Village/nature/rpgpp_lt_terrain_grass_02");

            // Load an effect that will be used to draw the scene
            Effect = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            
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

        }

        // <summary>
        /// Draws the City Scene
        /// </summary>
        /// <param name="gameTime">The Game Time for this frame</param>
        /// <param name="view">A view matrix, generally from a camera</param>
        /// <param name="projection">A projection matrix</param>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection, Matrix world)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            
            var random = new Random(Seed:0);

            var scala = 2000.0f; // Escala entre 1.0 y 11.0
            // var colorcito = new Vector3((CameraPosition.X) + random.NextSingle(), CameraPosition.Y + random.NextSingle(), CameraPosition.Z + random.NextSingle());
            var color = new Vector3(0.0f, 1.0f, 0.0f); //color verde puro
            Effect.Parameters["DiffuseColor"].SetValue(color);        /*Usamos verto3 porque es BasicEffect. Se usa vector4 si tenemos activado el AlphaShader*/
                
            var traslacion = new Vector3(2f, -1f, 1f);

            foreach (var mesh in Model.Meshes)
            {
                Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * Matrix.CreateTranslation(traslacion) * Matrix.CreateScale(scala));

                mesh.Draw();
            }
        
        }
    }
}