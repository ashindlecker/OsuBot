using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OsuBot.osu
{
    abstract class HitObject
    {
        public class Data
        {
            public Data()
            {
                Type = 0;
                Time = 0;
                X = 0;
                Y = 0;
            }

            public int Type
            {
                get;
                set;
            }

            public float Time
            {
                get;
                set;
            }

            public int X
            {
                get;
                set;
            }
            public int Y
            {
                get;
                set;
            }
            public int OrigX
            {
                get;
                set;
            }
            public int OrigY
            {
                get;
                set;
            }

            public string Parse(StreamReader reader)
            {
                string str = reader.ReadLine();
                if (str != null && str.Contains(','))
                {
                    int comma1 = str.IndexOf(',');
                    int comma2 = str.IndexOf(',', comma1 + 1);
                    int comma3 = str.IndexOf(',', comma2 + 1);
                    int comma4 = str.IndexOf(',', comma3 + 1);

                    string sX = str.Substring(0, comma1);
                    X = Convert.ToInt32(sX);
                    string sY = str.Substring(comma1 + 1, comma2 - (comma1 + 1));
                    Y = Convert.ToInt32(sY);
                    string sTime = str.Substring(comma2 + 1, comma3 - (comma2 + 1));
                    Time = Convert.ToSingle(sTime);
                    string sType = str.Substring(comma3 + 1, comma4 - (comma3 + 1));
                    Type = Convert.ToInt32(sType);
                    OrigX = X;
                    OrigY = Y;
                    str = str.Substring(comma4 + 1, str.Length - (comma4 + 1));
                }
                return str;
            }
        }

        public Data HitData
        {
            get;
            set;
        }
        

        public HitObject()
        {
            HitData = new Data();
        }

        public static HitObject CreateParse(StreamReader stream, SongData song)
        {
            HitObject.Data data = new Data();
            string line = data.Parse(stream);
            HitObject ret = null;
            if (line != null)
            {
                switch (data.Type)
                {
                    default:
                    case 1:
                        ret = new BeatCircle();
                        break;
                    case 2:
                    case 6:
                        ret = new Slider();
                        break;
                    case 12:
                        ret = new Spinner();
                        break;
                }
                if (ret != null)
                {
                    ret.HitData = data;
                    ret.Parse(stream, line);
                }
            }
            return ret;
        }

        protected abstract void Parse(StreamReader reader,string line);
    }
}
