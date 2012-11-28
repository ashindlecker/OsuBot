using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.osu
{
    class Spinner:HitObject
    {
        public int EndTime
        {
            get;
            private set;
        }
        public Spinner()
        {
        }

        protected override void Parse(System.IO.StreamReader reader, string line)
        {
            double[] numbers = Parsers.NumbersFromString.Parse(line);
            EndTime = (int)numbers[1];
        }
    }
}
