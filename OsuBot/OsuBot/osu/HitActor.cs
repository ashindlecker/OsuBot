using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.osu
{
    abstract class HitActor
    {
        public bool HasBeenInit;
        public HitActor()
        {
            HasBeenInit = false;
        }
        public virtual void Init(SongData songData, AController controller, TimeSpan totalTime, TimeSpan elapsed, bool doubletime)
        {
            HasBeenInit = true;
        }

        public abstract bool Act(AController controller, TimeSpan totalTime, TimeSpan elapsed, bool doubletime);

        public static HitActor GetActor(HitObject obj)
        {
            switch (obj.HitData.Type)
            {
                default:
                case 1:
                    return new actors.ActBeatCircle(obj.HitData);
                case 2:
                case 6:
                    return new actors.ActSlider((Slider)obj);
                case 12:
                    return new actors.ActSpinner((Spinner)obj);
                    
            }
            return null;
        }
    }
}
