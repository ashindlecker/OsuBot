using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.Win32
{
    class Window:AWindow
    {
        public IntPtr WindowHandle
        {
            get;
            set;
        }
        WinAPI.RECT m_winRect = new WinAPI.RECT();

        public Window(IntPtr hwnd)
        {
            WindowHandle = hwnd;
            WinAPI.GetWindowRect(WindowHandle, ref m_winRect);
        }

        public override Vector2 ConvertToScreen(Vector2 point)
        {
            WinAPI.POINT ret = new WinAPI.POINT();
            ret.x = point.x;
            ret.y = point.y;

            WinAPI.ClientToScreen(WindowHandle, ref ret);

            return new Vector2(ret.x, ret.y);
        }

        public override Vector2 Size()
        {
            int width = m_winRect.right - m_winRect.left;
            int height = m_winRect.bottom - m_winRect.top;
            WinAPI.POINT ret = new WinAPI.POINT();
            ret.x = width;
            ret.y = height;
            return new Vector2(ret.x, ret.y);
        }

        public override string Title()
        {
            try
            {
                int length = WinAPI.GetWindowTextLength(WindowHandle) * 2;
                Console.WriteLine(length);
                StringBuilder sb = new StringBuilder();
                WinAPI.GetWindowText(WindowHandle, sb, 3);

                return sb.ToString();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return "NA";
        }
    }
}
