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
        private Model flatoutCar { get; set; }
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
            CantAutos = 80;
            // Load the Car Model
            CarModel = content.Load<Model>(ContentFolder3D+"autos/RacingCarA/RacingCar");
            //CartoonCar = content.Load<Model>(ContentFolder3D+"autos/cartoonCar/carton_car");
            //flatoutCar = content.Load<Model>(ContentFolder3D+"autos/flatOutCar/Car");
            //deLorean = content.Load<Model>(ContentFolder3D+"autos/DeLorean");
            //kombi = content.Load<Model>(ContentFolder3D+"autos/kombi2");
            combatVehicle = content.Load<Model>(ContentFolder3D+"autos/CombatVehicle/Vehicle");

            // Load an effect that will be used to draw the scene
            EffectCar = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            
            traslaciones = GenerarPuntosEnCirculo(CantAutos, 1200f);

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
           

            var world = Matrix.Identity;


            var random = new Random(Seed:0);

            for (int i = 0; i < CantAutos; i++){
                
                var scala = 0.1f + (0.1f - 0.05f) * random.NextSingle();
                // var colorcito = new Vector3((CameraPosition.X) + random.NextSingle(), CameraPosition.Y + random.NextSingle(), CameraPosition.Z + random.NextSingle());
                var color = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());         /*Usamos verto3 porque es BasicEffect. Se usa vector4 si tenemos activado el AlphaShader*/

                var angulo = angulosHaciaCentro[i];
                /*var traslacion = new Vector3(
                -1000f + (100f - (-1000f)) * random.NextSingle(),
                0,
                -2000f + (1500f - (-2000f)) * random.NextSingle()
                );*/

                //SCALA DE 0.5 A 1
                //-770 A 650
                //-1200 A 1000


                foreach (var mesh in listaModelos[i].Meshes)
                {   
                    if (listaModelos[i] == combatVehicle){
                        scala = 0.004f + (0.004f - 0.001f) * random.NextSingle();
                        angulo =   angulosHaciaCentro[i] + (float)Math.PI/2;
                    }
                    EffectCar.Parameters["DiffuseColor"].SetValue(color);
                    EffectCar.Parameters["World"].SetValue(mesh.ParentBone.Transform * Matrix.CreateRotationY(angulo) * Matrix.CreateScale(scala) * Matrix.CreateTranslation(traslaciones[i]) );

                    mesh.Draw();
                }
            }
        }

        public List<Vector3> GenerarPuntosEnCirculo(int numPuntos, float radio)
        {
            List<Vector3> puntos = new List<Vector3>();
            float anguloIncremento = MathHelper.TwoPi / numPuntos; // Divide el círculo en partes iguales

            for (int i = 0; i < numPuntos; i++)
            {
                float angulo = i * anguloIncremento;
                float x = radio * (float)Math.Cos(angulo);
                float z = radio * (float)Math.Sin(angulo);
                float y = 0; // Fijamos la altura Y a 0 (puedes cambiarla si lo necesitas)

                puntos.Add(new Vector3(x, y, z));
            }

            return puntos;
        }

        public List<float> CalcularAngulosHaciaCentro(List<Vector3> posiciones)
        {
            List<float> angulos = new List<float>();

            foreach (var posicion in posiciones)
            {
                // Vector desde la posición del auto hacia el origen en el eje X y Z
                float dx =  -posicion.X;
                float dz = posicion.Z;

                // Calculamos el ángulo con atan2(dz, dx)
                float angulo = (float)Math.Atan2(dz, dx);

                // Añadimos el ángulo a la lista
                angulos.Add(angulo + (float)Math.PI/2);
            }

            return angulos;
        }


    }
}
