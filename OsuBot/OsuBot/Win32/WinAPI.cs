using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace OsuBot
{
    public class WinAPI
    {
        [DllImport("user32.dll", EntryPoint = "SendInput", SetLastError = true)]
        public extern static uint SendInput(
           uint nInputs,
           INPUT[] pInputs,
           int cbSize);

        //C# signature for "GetMessageExtraInfo()"
        [DllImport("user32.dll", EntryPoint = "GetMessageExtraInfo", SetLastError = true)]
        public extern static IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        public extern static Boolean SetCursorPos(int x, int y);

        [DllImport("user32.dll", EntryPoint = "ClientToScreen")]
        public extern static Boolean ClientToScreen(IntPtr hwnd, ref POINT point);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public extern static IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll", EntryPoint = "GetAsyncKeyState")]
        public extern static short GetAsyncKeyState(int key);

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public extern static short GetCursorPos(ref POINT point);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        public extern static Boolean GetWindowRect(IntPtr hwnd, ref RECT rect);

        public delegate bool CallBack(int hwnd, int lParam); 

        [DllImport("user32.Dll")]
        public static extern int EnumWindows(CallBack x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private enum InputType
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2,
        }

        [Flags()]
        private enum MOUSEEVENTF
        {
            MOVE = 0x0001,  // mouse move 
            LEFTDOWN = 0x0002,  // left button down
            LEFTUP = 0x0004,  // left button up
            RIGHTDOWN = 0x0008,  // right button down
            RIGHTUP = 0x0010,  // right button up
            MIDDLEDOWN = 0x0020,  // middle button down
            MIDDLEUP = 0x0040,  // middle button up
            XDOWN = 0x0080,  // x button down 
            XUP = 0x0100,  // x button down
            WHEEL = 0x0800,  // wheel button rolled
            VIRTUALDESK = 0x4000,  // map to entire virtual desktop
            ABSOLUTE = 0x8000,  // absolute move
        }

        [Flags()]
        private enum KEYEVENTF
        {
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            UNICODE = 0x0004,
            SCANCODE = 0x0008,
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }


        // This function simulates a simple mouseclick at the current cursor position.
        public static uint Click()
        {
            INPUT input_down = new INPUT();
            input_down.mi.dx = 0;
            input_down.mi.dy = 0;
            input_down.mi.mouseData = 0;
            input_down.mi.dwFlags = (int)MOUSEEVENTF.LEFTDOWN;
            input_down.type = 0;
            INPUT input_up = input_down;
            input_up.mi.dwFlags = (int)MOUSEEVENTF.LEFTUP;

            INPUT[] input = { input_down, input_up };

            return SendInput(2, input, Marshal.SizeOf(input_down));
        }

        public static uint LeftPress()
        {
            INPUT input_down = new INPUT();
            input_down.mi.dx = 0;
            input_down.mi.dy = 0;
            input_down.mi.mouseData = 0;
            input_down.mi.dwFlags = (int)MOUSEEVENTF.LEFTDOWN;
            input_down.type = 0;

            INPUT[] input = { input_down };

            return SendInput(1, input, Marshal.SizeOf(input_down));
        }
        public static uint LeftRelease()
        {
            INPUT input_down = new INPUT();
            input_down.mi.dx = 0;
            input_down.mi.dy = 0;
            input_down.mi.mouseData = 0;
            input_down.mi.dwFlags = (int)MOUSEEVENTF.LEFTUP;
            input_down.type = 0;

            INPUT[] input = { input_down };

            return SendInput(1, input, Marshal.SizeOf(input_down));
        }

        public static uint PressKey(int key)
        {
            INPUT input_down = new INPUT();
            input_down.ki.wVk = (short)key;
            input_down.ki.dwFlags = 1;
            input_down.type = 1;

            INPUT[] input = { input_down };

            return SendInput(1, input, Marshal.SizeOf(input_down));
        }
        public static uint ReleaseKey(int key)
        {
            INPUT input_down = new INPUT();
            input_down.ki.wVk = (short)key;
            input_down.ki.dwFlags = 2;
            input_down.type = 1;

            INPUT[] input = { input_down };

            return SendInput(1, input, Marshal.SizeOf(input_down));
        }
    }
}
