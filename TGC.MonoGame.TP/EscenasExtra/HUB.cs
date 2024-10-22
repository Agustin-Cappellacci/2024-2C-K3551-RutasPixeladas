using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.MonoGame.TP.Content.Models
{
    class Hub
    {
        
        public Texture2D texturaBarraVida;
        public Texture2D texturaCuadroItem;
        public Texture2D texturaItem;
        public Texture2D texturaNitro;
        public Texture2D texturaCohete;
        public Texture2D texturaArma;

        public Texture2D Circulo;

        public SpriteFont myFont;
        
        // Puedes dibujar el círculo dependiendo del progreso
        // Aquí se asume que el círculo tiene un tamaño de 100x100 píxeles
        public Rectangle circleRect = new Rectangle(13, 43, 65, 65);
        
        public Hub(ContentManager content) {
            // Imagino que no afecta mucho al espacio así que mejor cargamos todas las texturas.
            texturaBarraVida = content.Load<Texture2D>("HUD/textura-vida");
            texturaCuadroItem = content.Load<Texture2D>("HUD/marco");
            texturaNitro = content.Load<Texture2D>("HUD/textura-nitro");
            texturaCohete = content.Load<Texture2D>("HUD/textura-cohete");
            texturaArma = content.Load<Texture2D>("HUD/textura-arma");
            Circulo = content.Load<Texture2D>("HUD/circulo");
            myFont = content.Load<SpriteFont>("myFont");  // Carga la fuente
            
            
            texturaItem = texturaNitro;
        }

        public void Update(Jugador autoJugador)
        {
            if (autoJugador.power == 0)
            {
                texturaItem = texturaCohete;
            }
            if (autoJugador.power == 1)
            {
                texturaItem = texturaNitro;
            }
            if (autoJugador.power == 2)
            {
                texturaItem = texturaArma;
            }
            /*
            if (autoJugador.power != -1){
                SpriteBatch.Draw(texturaItem, new Rectangle(13, 43, 65, 65), Color.White);
            }
            if (autoJugador.isOnCooldown){
                // Para optimizar, algunas veces si se calcularía y unas veces no. Si es que se quiere usar. Aunque no consume tanto...
                float cooldownProgress = autoJugador.cooldownTimer / autoJugador.cooldownTime;  // Porcentaje de carga (0.0 a 1.0)
            }

            */
        }

        public void Draw(SpriteBatch spriteBatch, string tiempoDesdeInicio) {
            Vector2 position = new Vector2(560, 620);  // Posición en la pantalla
            Color textColor = Color.White;      

            spriteBatch.Draw(texturaBarraVida, new Rectangle(10, 10, 210, 25), Color.Black);
            spriteBatch.Draw(texturaBarraVida, new Rectangle(12, 12, 200, 20), Color.White);
            spriteBatch.Draw(texturaCuadroItem, new Rectangle(10, 40, 70, 70), Color.White);

            // Puedes dibujar el círculo dependiendo del progreso
            // Aquí se asume que el círculo tiene un tamaño de 100x100 píxeles
            // Puedes usar una técnica para "recortar" o escalar el círculo según el progreso

            // Rectangle circleRect = new Rectangle(13, 43, 65, 65);
            // SpriteBatch.Draw(Circulo, circleRect, Color.White * (1 - cooldownProgress));

            spriteBatch.Draw(texturaItem, new Rectangle(13, 43, 65, 65), Color.White);


            // Puedes dibujar el círculo dependiendo del progreso
            // Aquí se asume que el círculo tiene un tamaño de 100x100 píxeles
            Rectangle circleRect = new Rectangle(13, 43, 65, 65);

            
            
            // Puedes usar una técnica para "recortar" o escalar el círculo según el progreso
            spriteBatch.Draw(texturaBarraVida, new Rectangle(540, 615, 150, 40), Color.Black * 0.5f);
            spriteBatch.DrawString(myFont, tiempoDesdeInicio, position, textColor);

        }

    }
}
