using System.Runtime.CompilerServices;
using System;
using System.Net.Mime;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;
using System.Collections.Generic;

namespace TGC.MonoGame.TP.Content.Models
{

    public class InitialMenu
    {
        public const string ContentFolder3D = "skybox/";
        public const string ContentFolderTextures = "texturas/";
        public const string ContentFolderEffects = "Effects/";
        private SpriteFont _menuFont;
        private GraphicsDeviceManager Graphics { get; }
        private GraphicsDevice GraphicsDevice;
        private bool _isSoundMenuOpen = false;
        private string[] _menuItems = { "Start Game", "Exit" };
        private int _selectedIndex = 0;
        // private Jugador autoJugador {get; set;}
        private SpriteBatch SpriteBatch { get; set; }
        private TGCGame game;
        private float _angle;
        private Vector3 _cameraPosition;
        private Vector3 _cameraTarget;
        private float _distance;
        private Matrix _view;
        private Matrix _view_sb;
        private Vector3 _viewVector;
        private Matrix _projection;
        private Matrix _projection_sb;
        private SkyBox _skyBox;
        
        private List<ModelMesh> ruedasMI;
        private List<ModelMesh> restoAutoMI;
        private Model ModelMI { get; set; }
        private Effect effectAutoMI { get; set; }
        Texture2D texturaAutoMI;
        Texture2D texturaRuedaMI;
        
        public ContentManager content;
        public Matrix CarWorld { get; set; }


        public InitialMenu(/*Jugador jugador,*/ ContentManager content, SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, TGCGame game)
        {
            //this.autoJugador = jugador;
            this.SpriteBatch = spriteBatch;
            this._menuFont = font;
            this.Graphics = graphics;
            this.GraphicsDevice = graphicsDevice;
            this.game = game;
            this.content = content;
        }

        public void Initialize()
        {
            _cameraTarget = Vector3.Zero;
            _view_sb = Matrix.CreateLookAt(Vector3.UnitX * 20, _cameraTarget, Vector3.UnitY);
            _projection_sb =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f,
                    100f);
            _distance = 20;

            //_cameraTarget = new Vector3(2f,2f,2f);
            _view = Matrix.CreateLookAt(Vector3.UnitX * 20, _cameraTarget, Vector3.UnitY);
            _projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f,
                    100f);

            //base.Initialize();
        }

        public void LoadContent()
        {
            var skyBox = game.Content.Load<Model>("skybox/cube");
            var skyBoxTexture = game.Content.Load<TextureCube>(ContentFolderTextures + "skybox/skybox");
            var skyBoxEffect = game.Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            _skyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect);
            
            ModelMI = content.Load<Model>("Models/autos/RacingCarA/RacingCar");
            effectAutoMI = content.Load<Effect>(ContentFolderEffects + "Player1");

            ruedasMI = new List<ModelMesh>();
            restoAutoMI = new List<ModelMesh>();



            texturaAutoMI = content.Load<Texture2D>("texturas/colorRojo");
            texturaRuedaMI = content.Load<Texture2D>("texturas/rueda");
            
            foreach (var mesh in ModelMI.Meshes)
            {
                // Aquí verificas si el nombre del mesh corresponde a una rueda
                foreach (var meshPart in mesh.MeshParts)
                {

                    meshPart.Effect = effectAutoMI;
                }
                if (mesh.Name.Contains("Wheel"))
                {
                    ruedasMI.Add(mesh);
                }
                else restoAutoMI.Add(mesh);

            }

            //base.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            _cameraPosition = _distance * new Vector3((float)Math.Sin(_angle), 0, (float)Math.Cos(_angle));
            _viewVector = Vector3.Transform(_cameraTarget - _cameraPosition, Matrix.CreateRotationY(0));
            _viewVector.Normalize();

            _angle += 0.002f;
            _view_sb = Matrix.CreateLookAt(_cameraPosition, _cameraTarget, Vector3.UnitY);

            CarWorld = Matrix.CreateRotationY(_angle) * Matrix.CreateScale(0.02f) * Matrix.CreateTranslation(0, -3.5f, -5.5f);
        }

        /// <inheritdoc />
        /// <inheritdoc />
        public void Draw(GameTime gameTime, Matrix Projection)
        {
            /* 
            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            this.GraphicsDevice.RasterizerState = rasterizerState;

            //TODO why I have to set 1 in the alpha channel in the fx file?
            //_view = View;
            //_projection = Projection;
            _skyBox.Draw(_view, _projection, _cameraPosition);

            GraphicsDevice.RasterizerState = originalRasterizerState; */

            var originalRasterizerState = this.GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            this.GraphicsDevice.RasterizerState = rasterizerState;

            _skyBox.Draw(_view_sb, _projection_sb, _cameraPosition);

            this.GraphicsDevice.RasterizerState = originalRasterizerState;

            var View = _view;
            Projection = _projection;

            //effectAuto.Parameters["View"].SetValue(View);
            //effectAuto.Parameters["Projection"].SetValue(Projection);

            
            foreach (ModelMesh mesh in restoAutoMI)
            {   /*
         effectAuto.Parameters["ModelTexture"].SetValue(texturaAuto);
         //effectAuto.Parameters["DiffuseColor"].SetValue(color);
         effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * CarWorld);
         */
            
                Vector3 lightPosition = new Vector3(-1000, 3000, 1000); // Luz en una posiciÃ³n elevada en el espacio
                
                effectAutoMI.CurrentTechnique = effectAutoMI.Techniques["MenuInicial"];

                effectAutoMI.Parameters["ambientColor"].SetValue(new Vector3(0.75f, 0.75f, 0.75f));
                effectAutoMI.Parameters["diffuseColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                effectAutoMI.Parameters["specularColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));

                effectAutoMI.Parameters["KAmbient"].SetValue(0.7f);
                effectAutoMI.Parameters["KDiffuse"].SetValue(0.5f);
                effectAutoMI.Parameters["KSpecular"].SetValue(0.3f);
                effectAutoMI.Parameters["shininess"].SetValue(50.0f);

                effectAutoMI.Parameters["lightPosition"].SetValue(lightPosition);

                effectAutoMI.Parameters["eyePosition"].SetValue(Vector3.Zero);
                effectAutoMI.Parameters["baseTexture"].SetValue(texturaAutoMI);

                // We set the main matrices for each mesh to draw
                effectAutoMI.Parameters["World"].SetValue(mesh.ParentBone.Transform * CarWorld);
                // InverseTransposeWorld is used to rotate normals
                effectAutoMI.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(CarWorld)));
                // WorldViewProjection is used to transform from model space to clip space
                effectAutoMI.Parameters["WorldViewProjection"].SetValue(mesh.ParentBone.Transform * CarWorld * View * Projection);

                mesh.Draw();
            }

            foreach (ModelMesh rueda in ruedasMI)
            {
                effectAutoMI.Parameters["baseTexture"].SetValue(texturaRuedaMI);
                //effectAuto.Parameters["DiffuseColor"].SetValue(colorRueda);
                if (rueda.Name.Contains("WheelA") || rueda.Name.Contains("WheelB"))
                {
                    effectAutoMI.Parameters["World"].SetValue(rueda.ParentBone.Transform * CarWorld);
                    effectAutoMI.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(rueda.ParentBone.Transform * CarWorld)));
                    effectAutoMI.Parameters["WorldViewProjection"].SetValue(rueda.ParentBone.Transform * CarWorld * View * Projection);

                }
                else
                {
                    effectAutoMI.Parameters["World"].SetValue(rueda.ParentBone.Transform * CarWorld);
                    effectAutoMI.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(rueda.ParentBone.Transform * CarWorld)));
                    effectAutoMI.Parameters["WorldViewProjection"].SetValue(rueda.ParentBone.Transform * CarWorld * View * Projection);
                }
                rueda.Draw();
            }
            DrawMenuText();


            //base.Draw(gameTime);
        }

        public void DrawMenuText()
        {
            SpriteBatch.Begin(
                SpriteSortMode.Immediate,         // Ensure it executes immediately
                BlendState.AlphaBlend,             // Proper blending for transparency
                null,                        // No changes to SamplerState or RasterizerState
                DepthStencilState.None,
                null,
                null           // Disable depth testing for the 2D layer
                );
            // Draw a semi-transparent background for the menu.
            var overlay = new Texture2D(GraphicsDevice, 1, 1);
            overlay.SetData(new[] { Color.Black * 0f });
            SpriteBatch.Draw(overlay, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);

            // Draw menu items.
            for (int i = 0; i < _menuItems.Length; i++)
            {
                Color color = (i == _selectedIndex) ? Color.Yellow : Color.White;
                var position = new Vector2(300, 600
                 + i * 80);
                SpriteBatch.DrawString(_menuFont, _menuItems[i], position, color);
            }
            SpriteBatch.End();
        }

        public bool HandleMenuInput(KeyboardState keyboardState, KeyboardState oldState)
        {
            if (keyboardState.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down))
            {
                _selectedIndex = (_selectedIndex + 1) % _menuItems.Length;
            }
            else if (keyboardState.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
            {
                _selectedIndex = (_selectedIndex - 1 + _menuItems.Length) % _menuItems.Length;
            }

            if (keyboardState.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
            {
                if (_menuItems[_selectedIndex] == "Start Game")
                {
                    return false; // Close the menu.
                }
                else if (_menuItems[_selectedIndex] == "Exit")
                {
                    game.Exit();
                }
            }
            return true;
        }
    }

}