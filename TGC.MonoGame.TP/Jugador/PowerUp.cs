using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace TGC.MonoGame.TP.Content.Models
{

    abstract class IPowerUp
    {
        private SoundEffect _powerUpSound;
        public SoundEffectInstance _powerUpSoundInstance;
        private bool isMuted = false;
        public Jugador jugador;

        public IPowerUp(ContentManager content, Jugador jugador) {
            this.jugador = jugador;
            _powerUpSound = content.Load<SoundEffect>("Models/autos/RacingCarA/powerup");
            _powerUpSoundInstance = _powerUpSound.CreateInstance();
        }
        public abstract void Apply();
    }

    class SuperJump : IPowerUp
    {
        public SuperJump(ContentManager content, Jugador jugador) : base(content, jugador)
        {
        }

        public override void Apply()
        {
            _powerUpSoundInstance.Play();
            jugador.carJumpSpeed *= 2;
        }
    }

    class SuperSpeed : IPowerUp
    {
        public SuperSpeed(ContentManager content, Jugador jugador) : base(content, jugador)
        {
        }

        public override void Apply()
        {
            _powerUpSoundInstance.Play();
            jugador.CarSpeed = 10000;
        }
    }


}