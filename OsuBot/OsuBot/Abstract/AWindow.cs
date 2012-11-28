using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot
{
    abstract class AWindow
    {
        public abstract Vector2 Size();
        public abstract Vector2 ConvertToScreen(Vector2 point);
        public abstract string Title();
    }
}
