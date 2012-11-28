using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.osu
{
    class OsuOptions
    {
        public static int NoteOffsetX = 140;
        public static int NoteOffsetY = 121;

        public static int DefaultResX = 1401;
        public static int DefaultResY = 1049;

        public static int DataMaxX = 512;
        public static int DataMaxY = 384;

        public static void Convert(ref WinAPI.POINT point, int resX, int resY)
        {
            //point.x = (int)(((float)point.x + (float)NoteOffsetX) * ((float)resX / (float)DefaultResX));
            //point.y = (int)(((float)point.y + (float)NoteOffsetY) * ((float)resY / (float)DefaultResY));
            point.x = (int)(1.6f * point.x) + 106;
            point.y = (int)(1.4f * point.y) + 106;

            point.x = (int)((float)point.x * (float)resX / 1024f);
            point.y = (int)((float)point.y * (float)resY / 768f);
        }
    }
}
