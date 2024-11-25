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

        private float transparencia;
        
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
            transparencia = 0;
        }

        public void Update(Jugador autoJugador)
        {
            if (autoJugador.powerUp is Hamster)
            {
                texturaItem = texturaCohete;
                transparencia = 1;
            }
            if (autoJugador.powerUp is Gun)
            {
                texturaItem = texturaNitro;
                transparencia = 1;
            }
            if (autoJugador.powerUp is Hamster)
            {
                texturaItem = texturaArma;
                transparencia = 1;
            }
            if (autoJugador.powerUp == null){
                transparencia = 0;
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

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Jugador autoJugador) {
            Vector2 position = new Vector2(560, 620);  // Posición en la pantalla
            Vector2 positionContador = new Vector2(15, 60);  // Posición en la pantalla
            Color textColor = Color.White;      

            int proporcion = (int) autoJugador.vida/200;
            int longitud = 200 * proporcion;

            spriteBatch.Draw(texturaBarraVida, new Rectangle(10, 10, 210, 25), Color.Black);
            spriteBatch.Draw(texturaBarraVida, new Rectangle(12, 12, longitud, 20), Color.White);
            spriteBatch.Draw(texturaCuadroItem, new Rectangle(10, 40, 70, 70), Color.White);

            // Puedes dibujar el círculo dependiendo del progreso
            // Aquí se asume que el círculo tiene un tamaño de 100x100 píxeles
            // Puedes usar una técnica para "recortar" o escalar el círculo según el progreso

            // Puedes dibujar el círculo dependiendo del progreso
            // Aquí se asume que el círculo tiene un tamaño de 100x100 píxeles

            //Rectangle circleRect = new Rectangle(13, 43, 65, 65);
            //spriteBatch.Draw(Circulo, circleRect, Color.White * (1 - autoJugador.tiempoRestante <0?0:autoJugador.tiempoRestante));

            spriteBatch.Draw(texturaItem, new Rectangle(13, 43, 65, 65), new Color(0,0,0, transparencia));

            // Puedes usar una técnica para "recortar" o escalar el círculo según el progreso
            spriteBatch.Draw(texturaBarraVida, new Rectangle(540, 615, 400, 100), Color.Black * 0.5f);

            // Dibuja texto en la pantalla
            var tiempoTotal = Convert.ToSingle(gameTime.TotalGameTime.TotalSeconds);
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            float fps = 1f / elapsedTime;

            // Mostrar el tiempo transcurrido desde el inicio en pantalla
            string tiempoDesdeInicio = $"Seg:{tiempoTotal:F2}\nFPS:{fps}";
            string contador = (autoJugador.powerUp is Gun)? $"x{autoJugador.powerUp.balasRestantes}" :"";

            spriteBatch.DrawString(myFont, tiempoDesdeInicio, position, textColor);
            spriteBatch.DrawString(myFont, contador, positionContador, textColor);

        }

    }
}
