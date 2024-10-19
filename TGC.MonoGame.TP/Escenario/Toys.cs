using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private Model Model { get; set; }
        private Model Ajedrez { get; set; }
        private Model Lego { get; set; }
        private Model Torre { get; set; }
        private Model Cubo { get; set; }
        private Model LegoPJ { get; set; }
        private Model rampaPanza { get; set; }

        private Model rampaDoble { get; set; }

        private Model rampa { get; set; }

        private Model LegoPile { get; set; }
        private Model carpet { get; set; }
        private Model Puente { get; set; }
        private Model caballo1 {get; set;}
        private Model caballo2 {get; set;}
        private Model Television { get; set; }



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
        private StaticHandle rampaDobleBodyHandle;
        private StaticHandle caballo1BodyHandle;
        private StaticHandle caballo2BodyHandle;
        private StaticHandle rampaPanzaBodyHandle;
        private StaticHandle ajedrezBodyHandle;
        private StaticHandle legoBodyHandle;
        private StaticHandle puenteBodyHandle;
        
        private Simulation simulation;
        private GraphicsDevice graphicsDevice;
        //private ConvexHull rampHull;



        // <summary>
        /// Creates a City Scene with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>

        public Toys(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice)

        {   



            // ESTA INTERESANTE PERO NO HACE NADA
            listaCombinada = new List<Tuple<Model, Effect>>()
            {

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

            for (int i = 0; i < listaModelos.Count; i++)
            {
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

            // Ponemos efectos a todas las partes
            for (int i = 0; i < listaCombinada.Count; i++)
            {
                foreach (var mesh in listaCombinada[i].Item1.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts) meshPart.Effect = listaCombinada[i].Item2;
                }
            }


            // Create a list of places where the city model will be drawn
            WorldMatrices = new List<Matrix>()
            {
                Matrix.Identity,
            };

            inicializadorColisionables(simulation, graphicsDevice);


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
            var torreOrientation =  BepuUtilities.QuaternionEx.CreateFromYawPitchRoll((float)Math.PI / 10,0, 0);
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
            var rampaDobleSize = new System.Numerics.Vector3(1000f, 100f, 300f); //Tamaño
            // Calcular la posición y la rotación
            var rampaDoblePosition = new System.Numerics.Vector3(-900f, 0f, -1100f); // Posición de la rampa
            var rampaDobleShape = new Box(rampaDobleSize.X, rampaDobleSize.Y, rampaDobleSize.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var rampaDobleShapeIndex = simulation.Shapes.Add(rampaDobleShape);

            // Crear el cuerpo estático para la rampa
            rampaDobleBodyHandle = simulation.Statics.Add(new StaticDescription(
                rampaDoblePosition, // Posición inicial de la rampa
                rampaDobleShapeIndex
            ));

            // Definir las dimensiones de la rampa
            var rampaPanzaSize = new System.Numerics.Vector3(350f, 100f, 500f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var rampaPanzaPosition = new System.Numerics.Vector3(-1200f, 10f, 0f); // Posición de la rampa
            var rampaPanzaOrientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll((float)Math.PI / 3, 0, 0); // Rotación

            var rampaPanzaShape = new Box(rampaPanzaSize.X, rampaPanzaSize.Y, rampaPanzaSize.Z);

            // Registrar la forma de la rampa en el sistema de colisiones y obtener un TypedIndex
            var rampaPanzaShapeIndex = simulation.Shapes.Add(rampaPanzaShape);

            // Crear el cuerpo estático para la rampa
            rampaPanzaBodyHandle = simulation.Statics.Add(new StaticDescription(
                rampaPanzaPosition, // Posición inicial de la rampa
                rampaPanzaOrientation,
                rampaPanzaShapeIndex
            ));
            // Definir las dimensiones del caballo
            var caballo1PanzaSize = new System.Numerics.Vector3(100f, 500f, 260f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var caballo1Position = new System.Numerics.Vector3(1000f, 730f, 300f); // Posición del caballo
            var caballo1Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI * (5/4), 0, 0); // Rotación
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
            var caballo2Orientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, 0); // Rotación
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
            var ajedrezOrientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, 0); // Rotación

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
            var legoPJOrientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, 0); // Rotación
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
            var puenteSize = new System.Numerics.Vector3(500f, 200f, 500f); // Ejemplo de tamaño
            // Calcular la posición y la rotación
            var puentePosition = new System.Numerics.Vector3(360F, 2f, -1700f); // Posición del puente
            var puenteOrientation = BepuUtilities.QuaternionEx.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, (float)Math.PI / -2); // Rotación
            var puenteShape = new Box(puenteSize.X, puenteSize.Y, puenteSize.Z);

            // Registrar la forma del puente en el sistema de colisiones y obtener un TypedIndex
            var puenteShapeIndex = simulation.Shapes.Add(puenteShape);

            // Crear el cuerpo estático para el puente
            puenteBodyHandle = simulation.Statics.Add(new StaticDescription(
                puentePosition,
                puenteOrientation,
                puenteShapeIndex
            ));


            
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
            var traslacion = new Vector3(0f, 0f, 0f);
            var rotacion = 0f;
            var escala = 1f;
            var worldFinal = Matrix.Identity;
            var color = new Vector3(1f, 1f, 1f);
            var random = new Random(Seed: 0);

            for (int i = 0; i < listaCombinada.Count; i++)
            {

                traslacion = new Vector3(0f, 0f, 0f);
                rotacion = 0f;
                escala = 1f;
                worldFinal = Matrix.Identity;
                color = new Vector3(1f, 1f, 1f);

                if (listaCombinada[i].Item1 == carpet)
                {
                    traslacion = new Vector3(-900f, -1f, -1100f);
                    escala = 10f;
                    texture = textureCarpet;
                }

                if (listaCombinada[i].Item1 == rampaDoble)
                {
                    traslacion = new Vector3(-1400f, 0f, -1000f);
                    escala = 4f;
                    color = new Vector3(0.8f, 0.8f, 0.8f);
                    texture = textureMadera;
                }     
                if (listaCombinada[i].Item1 == rampa)
                {
                    traslacion = new Vector3(0f, 0f, 1000f);
                    escala = 10f;
                    rotacion = (float)Math.PI / 2;
                    color = new Vector3(1f, 0.3f, 0f);
                    texture = textureMadera;
                }

                if (listaCombinada[i].Item1 == rampaPanza)
                {
                    traslacion = new Vector3(-1200f, 10f, 0f);
                    escala = 300f;
                    rotacion = (float)Math.PI / 3;
                    color = new Vector3(0.8f, 0.8f, 0.8f);
                    texture = textureRampa;
                } 

                if (listaCombinada[i].Item1 == caballo1)
                {
                    traslacion = new Vector3(1000f, 730f, 300f);
                    escala = 130f;
                    rotacion = -(float)Math.PI * (5 / 4);
                    color = new Vector3(0f, 1f, 0.5f);
                    texture = textureCaballo2;
                }   
                
                if (listaCombinada[i].Item1 == caballo2){
                    traslacion = new Vector3(1200f, 730f, -100f);
                    escala = 130f;
                    rotacion = -(float)Math.PI / 4;
                    color = new Vector3(0f, 0.5f, 1f);
                    texture = textureCaballo1;
                }   

                if (listaCombinada[i].Item1 == Ajedrez)
                {
                    traslacion = new Vector3(-1200f, 2f, 1700f);
                    escala = 0.4f;
                    rotacion = -(float)Math.PI / 4;
                    color = new Vector3(0.5f, 0.5f, 1f);
                }

                if (listaCombinada[i].Item1 == LegoPJ)
                {
                    traslacion = new Vector3(1200f, 2f, 1700f);
                    escala = 0.1f;
                    rotacion = -(float)Math.PI / 4;
                    color = new Vector3(0f, 0.5f, 1f);
                    texture = textureLegoPJ;
                }

                if (listaCombinada[i].Item1 == Puente)
                {
                    traslacion = new Vector3(360F, 2f, -1700f);
                    escala = 100f;
                    rotacion = (float)Math.PI / 4;
                    color = new Vector3(1f, 1f, 0f);
                    texture = textureMadera2;
                }

                if (listaCombinada[i].Item1 == Torre)
                {
                    traslacion = new Vector3(-1300f, 1f, 700f);
                    escala = 1.5f;
                    rotacion = (float)Math.PI / 10;
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


                if (listaCombinada[i].Item1 != LegoPile || listaCombinada[i].Item1 != Lego)
                {
                    foreach (var mesh in listaCombinada[i].Item1.Meshes)
                    {
                        if (listaCombinada[i].Item1 == carpet)
                        {
                            worldFinal = modelCarpetMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                        if (listaCombinada[i].Item1 == rampaDoble)
                        {
                            worldFinal = modelrampaDobleMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                        if (listaCombinada[i].Item1 == rampa)
                        {
                            worldFinal = modelrampaMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationX(-(float)Math.PI / 2) * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }


                        if (listaCombinada[i].Item1 == rampaPanza)
                        {
                            worldFinal = modelrampaPanzaMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                        if (listaCombinada[i].Item1 == caballo1)
                        {
                            worldFinal = modelCaballo1MeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                        if (listaCombinada[i].Item1 == caballo2)
                        {
                            worldFinal = modelCaballo2MeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                        if (listaCombinada[i].Item1 == Ajedrez)
                        {
                            worldFinal = modelChessMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                        if (listaCombinada[i].Item1 == LegoPJ)
                        {
                            worldFinal = modelLegoPJMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                        if (listaCombinada[i].Item1 == Puente)
                        {
                            worldFinal = modelPuenteMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationX((float)Math.PI / -2) * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                        if (listaCombinada[i].Item1 == Torre)
                        {
                            worldFinal = modelTorreMeshesBaseTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotacion) * Matrix.CreateScale(escala) * Matrix.CreateTranslation(traslacion);
                            EfectoTexture.Parameters["ModelTexture"].SetValue(texture);
                        }

                        EfectoComun.Parameters["DiffuseColor"].SetValue(color);
                        //EffectCar.Parameters["World"].SetValue(mesh.ParentBone.Transform * Matrix.CreateRotationY(angulo) * Matrix.CreateScale(scala) * Matrix.CreateTranslation(traslaciones[i]) );
                        EfectoComun.Parameters["World"].SetValue(worldFinal);
                        EfectoTexture.Parameters["World"].SetValue(worldFinal);
                        mesh.Draw();
                    }
                }

                if (listaCombinada[i].Item1 == Lego)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        traslacion = new Vector3(
                        -1600f + (-100f - (-1600f)) * random.NextSingle(),
                        0,
                        -2000f + (-2200f - (-2000f)) * random.NextSingle()
                        );
                        escala = 0.7f + (0.7f - 1f) * random.NextSingle();
                        color = new Vector3(0.6f, random.NextSingle(), random.NextSingle());

                        foreach (var mesh in listaCombinada[i].Item1.Meshes)
                        {
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

                    if (listaCombinada[i].Item1 == LegoPile)
                    {
                        traslacion = new Vector3(1000f, 2f, -1200f);
                        escala = 1f;
                    }
                }

            }

            // dibujar las cajas de colisiones de todos los objetos
            // si se quiere dibujar un convexHull hay que usar el mtodo DrawConvexHull (esta medio cursed ese)
            DrawCollisionBoxes(view, projection);
            
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
            DrawBox(Matrix.CreateTranslation(-900f, 0f, -1100f), new Vector3(1000f, 100f, 300f), viewMatrix, projectionMatrix);
            // Dibujar Rampa panza
            DrawBox(Matrix.CreateFromYawPitchRoll((float)Math.PI / 3, 0, 0)*Matrix.CreateTranslation(-1200f, 10f, 0f), new Vector3(350f, 100f, 500f), viewMatrix, projectionMatrix);
            // Dibujar caballo1
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI * (5/4), 0, 0)*Matrix.CreateTranslation(1000f, 730f, 300f), new Vector3(100f, 500f, 260f), viewMatrix, projectionMatrix);
            // Dibujar caballo2
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, 0)*Matrix.CreateTranslation(1200f, 730f, -100f), new Vector3(100f, 550f, 260f), viewMatrix, projectionMatrix);
            // Dibujar ajedrez
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, 0)*Matrix.CreateTranslation(-1200f, 2f, 1700f), new Vector3(500f, 100f, 500f), viewMatrix, projectionMatrix);
            // Dibujar lego
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, 0)*Matrix.CreateTranslation(1200f, 2f, 1700f), new Vector3(150f, 100f, 150f), viewMatrix, projectionMatrix);
            // Dibujar puente
            DrawBox(Matrix.CreateFromYawPitchRoll(-(float)Math.PI /4, 0, (float)Math.PI / -2)*Matrix.CreateTranslation(360F, 2f, -1700f), new Vector3(500f, 200f, 500f), viewMatrix, projectionMatrix);
            
        }

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
            vertices[0] = new VertexPositionColor(new Vector3(-1, 1, 1), Color.Red);   // Front top left
            vertices[1] = new VertexPositionColor(new Vector3(1, 1, 1), Color.Red);    // Front top right
            vertices[2] = new VertexPositionColor(new Vector3(-1, -1, 1), Color.Red);  // Front bottom left
            vertices[3] = new VertexPositionColor(new Vector3(1, -1, 1), Color.Red);   // Front bottom right
            vertices[4] = new VertexPositionColor(new Vector3(-1, 1, -1), Color.Red);  // Back top left
            vertices[5] = new VertexPositionColor(new Vector3(1, 1, -1), Color.Red);   // Back top right
            vertices[6] = new VertexPositionColor(new Vector3(-1, -1, -1), Color.Red); // Back bottom left
            vertices[7] = new VertexPositionColor(new Vector3(1, -1, -1), Color.Red);  // Back bottom right

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
                    Color.Green
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
        
    }
}
