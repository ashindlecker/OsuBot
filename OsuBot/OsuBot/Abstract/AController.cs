using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OsuBot
{
    abstract class AController
    {
        public AWindow WindowHandle
        {
            get;
            set;
        }
        public AController(AWindow window)
        {
            WindowHandle = window;
        }

        public abstract void Move(int x, int y);
        public abstract void Press(int x, int y);
        public abstract void Release(int x, int y);

        public void Click(int x, int y)
        {
            Press(x, y);
            Thread.Sleep(25);
            Release(x, y);
        }
    }
}
