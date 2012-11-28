using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.osu.actors
{
    class ActBeatCircle:HitActor
    {
        osu.HitObject.Data m_data;

        public ActBeatCircle(osu.HitObject.Data data)
        {
            m_data = data;
        }

        public override void Init(SongData songData, AController cont, TimeSpan totalTime, TimeSpan elapsed, bool doubletime)
        {
            base.Init(songData, cont, totalTime, elapsed, doubletime);
            
        }
        public override bool Act(AController controller, TimeSpan totalTime, TimeSpan elapsed, bool dTime)
        {
            controller.Release(m_data.X, m_data.Y);
            controller.Click(m_data.X, m_data.Y);
            return true;
        }
    }
}
