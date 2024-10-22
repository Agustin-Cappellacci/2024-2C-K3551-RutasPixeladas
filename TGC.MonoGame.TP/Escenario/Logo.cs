using BepuPhysics;
using BepuUtilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.MonoGame.TP.Content.Models
{
    class Logo
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        
        private Model Model;
        private Effect Effect;

        private Microsoft.Xna.Framework.Matrix World;

        public Logo(ContentManager content)
        {
            Model = content.Load<Model>(ContentFolder3D + "tgc-logo/tgc-logo");
            Effect = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            foreach (var mesh in Model.Meshes)
            {
                // Aquí verificas si el nombre del mesh corresponde a una rueda
                foreach (var meshPart in mesh.MeshParts)
                { 
                    meshPart.Effect = Effect;
                }
            }
            World = Microsoft.Xna.Framework.Matrix.CreateFromYawPitchRoll(MathF.PI, MathF.PI, 0) * Microsoft.Xna.Framework.Matrix.CreateScale(500f) * Microsoft.Xna.Framework.Matrix.CreateTranslation(1f,-25f,-1000f);
        }

        public void Update()
        {
            if (Model != null)
            {
            
            
            }

        }

        public void Draw(GameTime gameTime, Microsoft.Xna.Framework.Matrix view, Microsoft.Xna.Framework.Matrix projection)
        {
            Effect.Parameters["View"].SetValue( view);
            Effect.Parameters["Projection"].SetValue(projection);

            var modelChessMeshesBaseTransforms = new Microsoft.Xna.Framework.Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelChessMeshesBaseTransforms);
            
            foreach (var mesh in Model.Meshes)
            {
                // Aquí verificas si el nombre del mesh corresponde a una rueda
              
                Effect.Parameters["World"].SetValue(World);
                Effect.Parameters["DiffuseColor"].SetValue(new Vector3(0,0,1));
                mesh.Draw();
                    
                
            }
        }

    }
}
