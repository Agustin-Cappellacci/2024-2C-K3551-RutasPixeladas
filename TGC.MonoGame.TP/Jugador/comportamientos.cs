using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TGC.MonoGame.TP.Content.Models
{

    public abstract class Comportamiento {

        protected Comportamiento() {
        }
        protected abstract void ComportamientoUpdate(ContentManager content);

    }

    class AutoControlado : Comportamiento {
        public AutoControlado()
            : base()
        {}

        protected override void ComportamientoUpdate(ContentManager content){

        }
    }

    class AutoCPU : Comportamiento {
        public AutoCPU()
            : base()
        {}

        protected override void ComportamientoUpdate(ContentManager content){

        }
    }

    class AutoPerseguidor : Comportamiento {
        public AutoPerseguidor()
            : base()
        {}

        protected override void ComportamientoUpdate(ContentManager content){

        }
    }
}