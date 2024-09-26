using BepuPhysics;
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
    /// A City Scene to be drawn
    /// </summary>
    class Toys
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
        private Model rampaPanza {get; set;}

        private Model rampaDoble {get; set;}

        private Model rampa {get; set;}

        private Model LegoPile {get; set;}
        private Model carpet {get; set;}
        private Model Puente {get; set;}

        private Model caballo1 {get; set;}
        private Model caballo2 {get; set;}

        private Model Television {get; set;}



        private List<Matrix> WorldMatrices { get; set; }
        private Effect EfectoComun { get; set; }
        private Effect EfectoTexture { get; set; }
        public List<Tuple<Model, Effect>> listaCombinada;
        public List<Model> listaModelos;
        public List<Effect> listaEfectos;

        //Load textures
        private Texture2D textureMadera2 { get; set; }
        private Texture2D textureMadera { get; set; }
        private Texture2D textureRampa { get; set; }
        private Texture2D textureCarpet { get; set; }
        private Texture2D textureLegoPile { get; set; }
        private Texture2D textureCaballo1 { get; set; }
        private Texture2D textureCaballo2 { get; set; }
        private Texture2D texture { get; set; }
        private Texture2D textureLegoPJ { get; set; }
        private Texture2D textureMetal { get; set; }


        // <summary>
        /// Creates a City Scene with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>
        public Toys(ContentManager content)
        {   


            listaCombinada = new List<Tuple<Model, Effect>>(){

            };
            // Load the City Model
            Ajedrez = content.Load<Model>(ContentFolder3D + "escenario/chess");
            Lego = content.Load<Model>(ContentFolder3D + "escenario/legoBrick");
            Torre = content.Load<Model>(ContentFolder3D + "escenario/torre");
            LegoPJ = content.Load<Model>(ContentFolder3D + "escenario/legoPJ/FireNinjaBlueOcatpus2/Fireninja_blueninja");
            Puente = content.Load<Model>(ContentFolder3D + "escenario/puente");
            Television = content.Load<Model>(ContentFolder3D + "tele/televisionModern");
            rampaDoble = content.Load<Model>(ContentFolder3D + "escenario/nuevos/rampadoble");
            rampaPanza = content.Load<Model>(ContentFolder3D + "escenario/nuevos/rampaPanza");
            rampa = content.Load<Model>(ContentFolder3D + "escenario/nuevos/rampa");
            carpet = content.Load<Model>(ContentFolder3D + "escenario/nuevos/carpet");
            LegoPile = content.Load<Model>(ContentFolder3D + "escenario/nuevos/legoPile");
            caballo1 = content.Load<Model>(ContentFolder3D + "escenario/nuevos/caballo1");
            caballo2 = content.Load<Model>(ContentFolder3D + "escenario/nuevos/caballo2");
           listaModelos = new List<Model>(){
                Ajedrez, Lego, Torre, LegoPJ, Puente, Television, rampaDoble, rampaPanza, rampa, carpet, LegoPile, caballo1, caballo2
            };

            //Load textures
            textureMetal = content.Load<Texture2D>(ContentFolder3D + "escenario/Textures/metal");
            textureLegoPJ = content.Load<Texture2D>(ContentFolder3D + "escenario/legoPJ/Skin1");
            textureMadera2 = content.Load<Texture2D>(ContentFolder3D + "escenario/Rubics_cube/wood/cherry_1");
            textureRampa = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/texture");
            textureMadera = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/wood");
            textureCarpet = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/alfombra2");
            textureLegoPile = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/pila_legos");
            textureCaballo1 = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/bullseye");
            textureCaballo2 = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/ColAlphY_horse_brown");
            texture = content.Load<Texture2D>(ContentFolder3D + "escenario/nuevos/default-grey");


            // Load an effect that will be used to draw the scene
            EfectoTexture = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            EfectoComun = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            listaEfectos = new List<Effect>(){
                EfectoComun, EfectoComun, EfectoTexture, EfectoTexture, EfectoTexture, EfectoComun, EfectoTexture, EfectoTexture, EfectoTexture, EfectoTexture, EfectoTexture, EfectoTexture, EfectoTexture
            };

            for (int i = 0; i < listaModelos.Count; i++){
                listaCombinada.Add(new Tuple<Model, Effect>(listaModelos[i], listaEfectos[i]));    
            }


            // Get the first texture we find
            // The city model only contains a single texture
            //var effect = Model.Meshes.FirstOrDefault().Effects.FirstOrDefault() as BasicEffect;
            //var texture = effect.Texture;
            //var texture = content.Load<Texture2D>(ContentFolder3D + "scene/tex/Palette"); // Asegúrate de usar la ruta correcta

            var colorMesa = new Vector3(0.8f, 0.8f, 0.8f); //gris
            var colorAjedrez = new Vector3(0.8f, 0.8f, 0.8f); //blanco
            var colorRampa = new Vector3(1f, 0f, 1f); //rojo
            //EffectLego.Parameters["DiffuseColor"].SetValue(colorLego);
            // Set the Texture to the Effect
             
            //Effect.Parameters["ModelTexture"].SetValue(texture);
            
            // Assign the mesh effect
            // A model contains a collection of meshes
            for (int i = 0; i < listaCombinada.Count; i++){
                foreach (var mesh in listaCombinada[i].Item1.Meshes){
                    foreach (var meshPart in mesh.MeshParts) meshPart.Effect = listaCombinada[i].Item2;
                }   
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
            EfectoComun.Parameters["View"].SetValue(view);
            EfectoComun.Parameters["Projection"].SetValue(projection);

            // Get the base transform for each mesh
            // These are center-relative matrices that put every mesh of a model in their corresponding location

            var modelChessMeshesBaseTransforms = new Matrix[Ajedrez.Bones.Count];
            Ajedrez.CopyAbsoluteBoneTransformsTo(modelChessMeshesBaseTransforms);

            var modelCarpetMeshesBaseTransforms = new Matrix[carpet.Bones.Count];
            carpet.CopyAbsoluteBoneTransformsTo(modelCarpetMeshesBaseTransforms);

            var modellegoPileMeshesBaseTransforms = new Matrix[LegoPile.Bones.Count];
            LegoPile.CopyAbsoluteBoneTransformsTo(modellegoPileMeshesBaseTransforms);

             var modelCaballo1MeshesBaseTransforms = new Matrix[caballo1.Bones.Count];
            caballo1.CopyAbsoluteBoneTransformsTo(modelCaballo1MeshesBaseTransforms);

             var modelCaballo2MeshesBaseTransforms = new Matrix[caballo2.Bones.Count];
            caballo2.CopyAbsoluteBoneTransformsTo(modelCaballo2MeshesBaseTransforms);

            var modelrampaDobleMeshesBaseTransforms = new Matrix[rampaDoble.Bones.Count];
            rampaDoble.CopyAbsoluteBoneTransformsTo(modelrampaDobleMeshesBaseTransforms);

            var modelrampaMeshesBaseTransforms = new Matrix[rampa.Bones.Count];
            rampa.CopyAbsoluteBoneTransformsTo(modelrampaMeshesBaseTransforms);

            var modelrampaPanzaMeshesBaseTransforms = new Matrix[rampaPanza.Bones.Count];
            rampaPanza.CopyAbsoluteBoneTransformsTo(modelrampaPanzaMeshesBaseTransforms);

            var modelLegoMeshesBaseTransforms = new Matrix[Lego.Bones.Count];
            Lego.CopyAbsoluteBoneTransformsTo(modelLegoMeshesBaseTransforms);

            var modelPuenteMeshesBaseTransforms = new Matrix[Puente.Bones.Count];
            Puente.CopyAbsoluteBoneTransformsTo(modelPuenteMeshesBaseTransforms);

            var modelTorreMeshesBaseTransforms = new Matrix[Torre.Bones.Count];
            Torre.CopyAbsoluteBoneTransformsTo(modelTorreMeshesBaseTransforms);

            var modelLegoPJMeshesBaseTransforms = new Matrix[LegoPJ.Bones.Count];
            LegoPJ.CopyAbsoluteBoneTransformsTo(modelLegoPJMeshesBaseTransforms);

            var modelTelevisionMeshesBaseTransforms = new Matrix[Television.Bones.Count];
            Television.CopyAbsoluteBoneTransformsTo(modelTelevisionMeshesBaseTransforms);


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
            var traslacion = new Vector3(0f,0f,0f);
            var rotacion = 0f;
            var escala = 1f;
            var worldFinal = Matrix.Identity; 
            var color = new Vector3(1f,1f,1f);
            var random = new Random(Seed:0);

            for (int i = 0; i < listaCombinada.Count; i++){

                traslacion = new Vector3(0f,0f,0f);
                rotacion = 0f;
                escala = 1f;
                worldFinal = Matrix.Identity; 
                color = new Vector3(1f,1f,1f);
                
                if (listaCombinada[i].Item1 == carpet){
                    traslacion = new Vector3(-900f, -1f, -1100f);
                    escala = 10f;
                    texture = textureCarpet;
                }

                if (listaCombinada[i].Item1 == rampaDoble){
                    traslacion = new Vector3(-1400f, 0f, -1000f);
                    escala = 4f;
                    color = new Vector3(0.8f, 0.8f, 0.8f);
                    texture = textureMadera;
                }     

                if (listaCombinada[i].Item1 == rampa){
                    traslacion = new Vector3(0f, 0f, 1000f);
                    escala = 10f;
                    rotacion = (float)Math.PI/2;
                    color = new Vector3(1f, 0.3f, 0f);
                    texture = textureMadera;
                }

                if (listaCombinada[i].Item1 == rampaPanza){
                    traslacion = new Vector3(-1200f, 10f, 0f);
                    escala = 300f;
                    rotacion = (float)Math.PI/3;
                    color = new Vector3(0.8f, 0.8f, 0.8f);
                    texture = textureRampa;
                } 

                if (listaCombinada[i].Item1 == caballo1){
                    traslacion = new Vector3(1000f, 730f, 300f);
                    escala = 130f;
                    rotacion = -(float)Math.PI * (5/4);
                    color = new Vector3(0f, 1f, 0.5f);
                    texture = textureCaballo2;
                }   
                
                if (listaCombinada[i].Item1 == caballo2){
                    traslacion = new Vector3(1200f, 730f, -100f);
                    escala = 130f;
                    rotacion = -(float)Math.PI/4;
                    color = new Vector3(0f, 0.5f, 1f);
                    texture = textureCaballo1;
                }   

                if (listaCombinada[i].Item1 == Ajedrez){
                     traslacion = new Vector3(-1200f, 2f, 1700f);
                    escala = 0.4f;
                    rotacion = -(float)Math.PI/4;
                    color = new Vector3(0.5f, 0.5f, 1f);
                }

                if (listaCombinada[i].Item1 == LegoPJ){
                     traslacion = new Vector3(1200f, 2f, 1700f);
                    escala = 0.1f;
                    rotacion = -(float)Math.PI/4;
                    color = new Vector3(0f, 0.5f, 1f);
                    texture = textureLegoPJ;
                }

                if (listaCombinada[i].Item1 == Puente){
                    traslacion = new Vector3(360F, 2f, -1700f);
                    escala = 100f;
                    rotacion = (float)Math.PI/4;
                    color = new Vector3(1f, 1f, 0f);
                    texture = textureMadera2;
                }

                if (listaCombinada[i].Item1 == Torre){
                    traslacion = new Vector3(-1300f, 1f, 700f);
                    escala = 1.5f;
                    rotacion = (float)Math.PI/10;
                    color = new Vector3(0.2f, 0.2f, 1f);
                    texture = textureMetal;
                }

               


                //  dibujarLego();

                /*
                                    if (listaCombinada[i].Item1 == LegoPile){
                        color = new Vector3(random.NextSingle(), random.NextSingle(), 0.6f);
                        worldFinal = modellegoPileMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);

                    }
                */ 
                //dibujarLegoPile();

              if(listaCombinada[i].Item1 != LegoPile || listaCombinada[i].Item1 != Lego){
                    foreach (var mesh in listaCombinada[i].Item1.Meshes){                    
                    if (listaCombinada[i].Item1 == carpet){
                        worldFinal = modelCarpetMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                    if (listaCombinada[i].Item1 == rampaDoble){
                        worldFinal = modelrampaDobleMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                    if (listaCombinada[i].Item1 == rampa){
                        worldFinal = modelrampaMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationX(-(float)Math.PI/2)  * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                    if (listaCombinada[i].Item1 == rampaPanza){
                        worldFinal = modelrampaPanzaMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                    if (listaCombinada[i].Item1 == caballo1){
                        worldFinal = modelCaballo1MeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                    if (listaCombinada[i].Item1 == caballo2){
                        worldFinal = modelCaballo2MeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                    if (listaCombinada[i].Item1 == Ajedrez){
                        worldFinal = modelChessMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoComun.Parameters["DiffuseColor"].SetValue(color);
                        }

                    if (listaCombinada[i].Item1 == LegoPJ){
                        worldFinal = modelLegoPJMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }
                    
                    if (listaCombinada[i].Item1 == Puente){
                        worldFinal = modelPuenteMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationX((float)Math.PI/-2) * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                    if (listaCombinada[i].Item1 == Torre){
                        worldFinal = modelTorreMeshesBaseTransforms[mesh.ParentBone.Index] *  Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }
        
                        //EffectCar.Parameters["World"].SetValue(mesh.ParentBone.Transform * Matrix.CreateRotationY(angulo) * Matrix.CreateScale(scala) * Matrix.CreateTranslation(traslaciones[i]) );
                        EfectoComun.Parameters["World"].SetValue(worldFinal);
                        EfectoTexture.Parameters["World"].SetValue(worldFinal);
                        mesh.Draw();
                    }
                }

                if (listaCombinada[i].Item1 == Lego){
                    for (int j = 0; j < 10; j++){
                        traslacion = new Vector3(
                        -1600f + (-100f - (-1600f)) * random.NextSingle(),
                        0,
                        -2000f + (-2200f - (-2000f)) * random.NextSingle()
                        );
                        escala = 0.7f + (0.7f - 1f) * random.NextSingle();
                        color = new Vector3(0.6f, random.NextSingle(), random.NextSingle());
                        
                       foreach (var mesh in listaCombinada[i].Item1.Meshes){
                            worldFinal = modelLegoMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoComun.Parameters["DiffuseColor"].SetValue(color);
                            EfectoComun.Parameters["World"].SetValue(worldFinal);
                            mesh.Draw();
                       }
                    }
                    traslacion = new Vector3(-600f, 2f, 1400f);
                    escala = 1f;

            // -1600 -2000 a -100 a -2200
            // 400 2200 a -600 1400
            //1800 -500 a 100 300

                if (listaCombinada[i].Item1 == LegoPile){
                        traslacion = new Vector3(1000f, 2f, -1200f);
                        escala = 1f;
                    }
                }

            }
/*
             
            
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
                    EfectoComun.Parameters["DiffuseColor"].SetValue(color);
                    EfectoComun.Parameters["World"].SetValue(meshWorldLego *  Matrix.CreateTranslation(traslacion)  * Matrix.CreateScale(scala));
                    mesh.Draw();
                }
            
            }*/
        }
    }
}
