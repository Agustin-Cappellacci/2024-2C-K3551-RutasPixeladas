using System.Runtime.CompilerServices;

namespace TGC.MonoGame.TP.Content.Models
{

    abstract class IPowerUp
    {
        public Jugador jugador;

        public IPowerUp(Jugador jugador) {
            this.jugador = jugador;
        }
        public abstract void Apply();
    }

    class SuperJump : IPowerUp
    {
        public SuperJump(Jugador jugador) : base(jugador)
        {
        }

        public override void Apply()
        {
            jugador.carJumpSpeed *= 2;
        }
    }

    class SuperSpeed : IPowerUp
    {
        public SuperSpeed(Jugador jugador) : base(jugador)
        {
        }

        public override void Apply()
        {
            jugador.CarSpeed = 10000;
        }
    }


}