using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.Controller
{
    class Win_Controller:AController
    {
        public Win_Controller(AWindow window):base(window)
        {
        }
        public override void Move(int x, int y)
        {
            Vector2 size = WindowHandle.Size();

            WinAPI.POINT p = new WinAPI.POINT();
            p.x = x;
            p.y = y;

            osu.OsuOptions.Convert(ref p, size.x, size.y);

            Vector2 converted = WindowHandle.ConvertToScreen(new Vector2(p.x, p.y));
            p.x = converted.x;
            p.y = converted.y;
            WinAPI.SetCursorPos(p.x, p.y);
        }

        public override void Release(int x, int y)
        {
            Move(x, y);
            WinAPI.ReleaseKey('X');
        }

        public override void Press(int x, int y)
        {
            Move(x, y);
            WinAPI.PressKey('X');
        }
    }
}
