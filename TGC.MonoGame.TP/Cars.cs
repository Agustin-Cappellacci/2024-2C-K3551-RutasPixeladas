using BepuPhysics.Constraints;
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
    class Cars
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";

        private Model CarModel { get; set; }
        private Model CartoonCar { get; set; }
       // private Model flatoutCar { get; set; }
        private Model deLorean { get; set; }
        private Model kombi { get; set; }
        private Model combatVehicle { get; set; }
        
        private Effect EffectCar { get; set; }
        private List<Vector3> traslaciones { get; set; }

        private List<float> angulosHaciaCentro { get; set; }
        private int CantAutos { get; set; }
        public List<Model> listaModelos {get; set;}

        // <summary>
        /// Creates a Car Model with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>
        public Cars (ContentManager content)
        {
            CantAutos = 70;
            // Load the Car Model
            CarModel = content.Load<Model>(ContentFolder3D+"autos/RacingCarA/RacingCar");
            //CartoonCar = content.Load<Model>(ContentFolder3D+"autos/cartoonCar/carton_car");
          //  flatoutCar = content.Load<Model>(ContentFolder3D+"autos/flatOutCar/Car");
            //deLorean = content.Load<Model>(ContentFolder3D+"autos/DeLorean");
            //kombi = content.Load<Model>(ContentFolder3D+"autos/kombi2");
            combatVehicle = content.Load<Model>(ContentFolder3D+"autos/CombatVehicle/Vehicle");

            // Load an effect that will be used to draw the scene
            EffectCar = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            
            traslaciones = GenerarPuntosEnCirculo(CantAutos, 700f);

            angulosHaciaCentro = CalcularAngulosHaciaCentro(traslaciones);

            // Assign the mesh effect
            // A model contains a collection of meshes
           foreach (var mesh in CarModel.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = EffectCar;
                }
            }

       //     foreach (var mesh in flatoutCar.Meshes)
          //  {
            // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
        //        foreach (var meshPart in mesh.MeshParts)
          //      {
            //        meshPart.Effect = EffectCar;
              //  }
            //}

            foreach (var mesh in combatVehicle.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = EffectCar;
                }
            }


            listaModelos = new List<Model>();
            for (int i = 0; i < CantAutos/2; i++)
            {
                listaModelos.Add(CarModel);
                listaModelos.Add(combatVehicle);
      //          listaModelos.Add(flatoutCar);
            }

            // Mezclar la lista de modelos
            var random = new Random(0);
            for (int i = listaModelos.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = listaModelos[i];
                listaModelos[i] = listaModelos[j];
                listaModelos[j] = temp;
            }

        }

        // <summary>
        /// Draws the City Scene
        /// </summary>
        /// <param name="gameTime">The Game Time for this frame</param>
        /// <param name="view">A view matrix, generally from a camera</param>
        /// <param name="projection">A projection matrix</param>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            EffectCar.Parameters["View"].SetValue(view);
            EffectCar.Parameters["Projection"].SetValue(projection);
            
            var meshBaseCombatVehicle = new Matrix[combatVehicle.Bones.Count];
            combatVehicle.CopyAbsoluteBoneTransformsTo(meshBaseCombatVehicle);
            var meshBaseModelCar = new Matrix[CarModel.Bones.Count];
            CarModel.CopyAbsoluteBoneTransformsTo(meshBaseModelCar);
       //     var meshBaseFlatoutCar = new Matrix[flatoutCar.Bones.Count];
        //    flatoutCar.CopyAbsoluteBoneTransformsTo(meshBaseFlatoutCar);

            var random = new Random(Seed:0);

            for (int i = 0; i < CantAutos; i++){
                
                //scala calculada para carmodel
                var scala = 0.1f + (0.1f - 0.05f) * random.NextSingle();
                //var color = new Vector3((CameraPosition.X) + random.NextSingle(), CameraPosition.Y + random.NextSingle(), CameraPosition.Z + random.NextSingle());
                var color = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());         /*Usamos verto3 porque es BasicEffect. Se usa vector4 si tenemos activado el AlphaShader*/

                //angulo calculado para carmodel
                var angulo = angulosHaciaCentro[i];

                var traslacion = traslaciones[i];

                //SCALA DE 0.5 A 1
                //-770 A 650
                //-1200 A 1000

         /*       if (listaModelos[i] == flatoutCar){
                    scala = 1.4f + (1.4f - 0.9f) * random.NextSingle();
                    angulo =   angulosHaciaCentro[i] +  (float)Math.PI;
                    traslacion = traslaciones[i] * (1.03f);
                }  
*/
                if (listaModelos[i] == combatVehicle){
                    scala = 0.004f + (0.004f - 0.001f) * random.NextSingle();
                    angulo =   angulosHaciaCentro[i] + (float)Math.PI/2;
                }   

                var worldFinal = Matrix.Identity;

                foreach (var mesh in listaModelos[i].Meshes)
                {   
                    if (listaModelos[i] == CarModel){
                        worldFinal = meshBaseModelCar[mesh.ParentBone.Index] * Matrix.CreateRotationY(angulo) * Matrix.CreateScale(scala) * Matrix.CreateTranslation(traslacion);
                    }

      /*              if (listaModelos[i] == flatoutCar){
                        worldFinal = meshBaseFlatoutCar[mesh.ParentBone.Index] * Matrix.CreateRotationY(angulo) * Matrix.CreateScale(scala) * Matrix.CreateTranslation(traslacion);
                    }*/

                    if (listaModelos[i] == combatVehicle){
                        worldFinal = meshBaseCombatVehicle[mesh.ParentBone.Index] * Matrix.CreateRotationY(angulo) * Matrix.CreateScale(scala) * Matrix.CreateTranslation(traslacion);
                    }   

                    EffectCar.Parameters["DiffuseColor"].SetValue(color);
                    //EffectCar.Parameters["World"].SetValue(mesh.ParentBone.Transform * Matrix.CreateRotationY(angulo) * Matrix.CreateScale(scala) * Matrix.CreateTranslation(traslaciones[i]) );
                    EffectCar.Parameters["World"].SetValue(worldFinal);

                    mesh.Draw();
                }
            }
        }

        public List<Vector3> GenerarPuntosEnCirculo(int numPuntos, float radio)
        {
            List<Vector3> puntos = new List<Vector3>();
            float anguloIncremento = MathHelper.TwoPi / numPuntos; // Divide el círculo en partes iguales
            float centroX = -900f; // Desplazamiento en X
            float centroZ = -1100f; // Desplazamiento en Z

            for (int i = 0; i < numPuntos; i++)
            {
                float angulo = i * anguloIncremento;
                float x = centroX + radio * (float)Math.Cos(angulo); // Coordenada X con desplazamiento
                float z = centroZ + radio * (float)Math.Sin(angulo); // Coordenada Z con desplazamiento
                float y = 5; // Coordenada Y fija

                puntos.Add(new Vector3(x, y, z));
            }

            return puntos;
        }

        public List<float> CalcularAngulosHaciaCentro(List<Vector3> posiciones)
        {
            List<float> angulos = new List<float>();

            // Coordenadas del centro del círculo en el plano XZ
            float centroX = -900f;
            float centroZ = -1100f;

            foreach (var posicion in posiciones)
            {
                // Calculamos la diferencia en X y Z con respecto al centro desplazado
                float dx = centroX - posicion.X; 
                float dz = centroZ - posicion.Z; 

                // Calculamos el ángulo con Atan2. Intercambiamos dx y dz para invertir la dirección
                float angulo =  MathHelper.TwoPi - (float)Math.Atan2(dz, dx) + MathHelper.Pi/2; 

                angulos.Add(angulo);
            }

            return angulos;
        }

    }
}
