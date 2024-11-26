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

    public class MainMenu
    {
        private SpriteFont _menuFont;
        private GraphicsDeviceManager Graphics { get; }
        private GraphicsDevice GraphicsDevice;
        private bool _isSoundMenuOpen = false;
        private string[] _menuItems = { "Resume", "Toggle Sound", "Exit" };
        private int _selectedIndex = 0;
        // private Jugador autoJugador {get; set;}
        private SpriteBatch SpriteBatch { get; set; }
        private TGCGame game;

        public MainMenu(/*Jugador jugador,*/ SpriteBatch spriteBatch, SpriteFont font, GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, TGCGame game)
        {
            //this.autoJugador = jugador;
            this.SpriteBatch = spriteBatch;
            this._menuFont = font;
            this.Graphics = graphics;
            this.GraphicsDevice = graphicsDevice;
            this.game = game;
        }
        public bool HandleMenuInput(KeyboardState keyboardState, KeyboardState oldState)
        {
            if (keyboardState.IsKeyDown(Keys.Down) &  oldState.IsKeyUp(Keys.Down))
            {
                _selectedIndex = (_selectedIndex + 1) % _menuItems.Length;
            }
            else if (keyboardState.IsKeyDown(Keys.Up) & oldState.IsKeyUp(Keys.Up))
            {
                _selectedIndex = (_selectedIndex - 1 + _menuItems.Length) % _menuItems.Length;
            }

            if (keyboardState.IsKeyDown(Keys.Enter) & oldState.IsKeyUp(Keys.Enter))
            {
                if (_menuItems[_selectedIndex] == "Resume")
                {
                    return false; // Close the menu.
                }
                if (_menuItems[_selectedIndex] == "Toggle Sound")
                {
                    game.ToggleMusic();
                    return false;
                }
                else if (_menuItems[_selectedIndex] == "Exit")
                {
                    game.Exit();
                }
            }


            return true;
        }

        public void DrawMenuOverlay()
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
            overlay.SetData(new[] { Color.Black * 0.5f }); // 50% transparent black.
            SpriteBatch.Draw(overlay, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);

            // Draw menu items.
            for (int i = 0; i < _menuItems.Length; i++)
            {
                Color color = (i == _selectedIndex) ? Color.Yellow : Color.White;
                var position = new Vector2(850, 450 + i * 40);
                SpriteBatch.DrawString(_menuFont, _menuItems[i], position, color);
            }
            SpriteBatch.End();
        }

                private string  _gameOver = "Game Over";

        public void Muerte (){
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
            overlay.SetData(new[] { Color.Black * 0.5f }); // 50% transparent black.
            SpriteBatch.Draw(overlay, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);

            // Draw menu items.

            Color color = Color.White;
            var position = new Vector2(850, 450);
            SpriteBatch.DrawString(_menuFont, _gameOver, position, color);
            SpriteBatch.End();
        }
    }

}