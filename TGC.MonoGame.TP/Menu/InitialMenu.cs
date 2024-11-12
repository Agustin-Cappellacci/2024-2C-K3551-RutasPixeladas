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
        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;
        private Model Model { get; set; }
        private Effect effectAuto { get; set; }
        Texture2D texturaAuto;
        Texture2D texturaRueda;
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
            //var skyBoxTexture = Game.Content.Load<TextureCube>(ContentFolderTextures + "/skyboxes/sunset/sunset");
            //var skyBoxTexture = Game.Content.Load<TextureCube>(ContentFolderTextures + "/skyboxes/islands/islands");
            var skyBoxTexture = game.Content.Load<TextureCube>(ContentFolderTextures + "skybox/skybox");
            var skyBoxEffect = game.Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            _skyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect);

            Model = content.Load<Model>("Models/autos/RacingCarA/RacingCar");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "Player1");

            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();



            texturaAuto = content.Load<Texture2D>("texturas/colorRojo");
            texturaRueda = content.Load<Texture2D>("texturas/rueda");

            foreach (var mesh in Model.Meshes)
            {
                // Aquí verificas si el nombre del mesh corresponde a una rueda
                foreach (var meshPart in mesh.MeshParts)
                {

                    meshPart.Effect = effectAuto;
                }
                if (mesh.Name.Contains("Wheel"))
                {
                    ruedas.Add(mesh);
                }
                else restoAuto.Add(mesh);

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


            foreach (ModelMesh mesh in restoAuto)
            {   /*
         effectAuto.Parameters["ModelTexture"].SetValue(texturaAuto);
         //effectAuto.Parameters["DiffuseColor"].SetValue(color);
         effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * CarWorld);
         */

                Vector3 lightPosition = new Vector3(-1000, 3000, 1000); // Luz en una posiciÃ³n elevada en el espacio

                effectAuto.Parameters["ambientColor"].SetValue(new Vector3(0.75f, 0.75f, 0.75f));
                effectAuto.Parameters["diffuseColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                effectAuto.Parameters["specularColor"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));

                effectAuto.Parameters["KAmbient"].SetValue(0.7f);
                effectAuto.Parameters["KDiffuse"].SetValue(0.5f);
                effectAuto.Parameters["KSpecular"].SetValue(0.3f);
                effectAuto.Parameters["shininess"].SetValue(50.0f);

                effectAuto.Parameters["lightPosition"].SetValue(lightPosition);

                effectAuto.Parameters["eyePosition"].SetValue(lightPosition);
                effectAuto.Parameters["ModelTexture"].SetValue(texturaAuto);

                // We set the main matrices for each mesh to draw
                effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * CarWorld);
                // InverseTransposeWorld is used to rotate normals
                effectAuto.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(CarWorld)));
                // WorldViewProjection is used to transform from model space to clip space
                effectAuto.Parameters["WorldViewProjection"].SetValue(mesh.ParentBone.Transform * CarWorld * View * Projection);

                mesh.Draw();
            }

            foreach (ModelMesh rueda in ruedas)
            {
                effectAuto.Parameters["ModelTexture"].SetValue(texturaRueda);
                //effectAuto.Parameters["DiffuseColor"].SetValue(colorRueda);
                if (rueda.Name.Contains("WheelA") || rueda.Name.Contains("WheelB"))
                {
                    effectAuto.Parameters["World"].SetValue(rueda.ParentBone.Transform * CarWorld);
                    effectAuto.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(rueda.ParentBone.Transform * CarWorld)));
                    effectAuto.Parameters["WorldViewProjection"].SetValue(rueda.ParentBone.Transform * CarWorld * View * Projection);

                }
                else
                {
                    effectAuto.Parameters["World"].SetValue(rueda.ParentBone.Transform * CarWorld);
                    effectAuto.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(rueda.ParentBone.Transform * CarWorld)));
                    effectAuto.Parameters["WorldViewProjection"].SetValue(rueda.ParentBone.Transform * CarWorld * View * Projection);
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