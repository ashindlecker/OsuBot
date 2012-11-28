using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OsuBot.osu
{
    class BeatCircle:HitObject
    {
        public BeatCircle()
        {
            HitData.Type = 1;
        }

        protected override void Parse(System.IO.StreamReader reader, string line)
        {
            //The values after object type are unknown, and (to my knowledge) aren't important to this bot, nor game mechanic changing.
        }
    }
}
