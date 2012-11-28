using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.osu.actors
{
    class ActSpinner:HitActor
    {
        float m_angle;
        bool m_init;
        Spinner m_spinData;
        public ActSpinner(Spinner spin)
        {
            m_init = false;
            m_spinData = spin;
        }

        public override void Init(OsuBot.osu.SongData songData, AController cont, TimeSpan totalTime, TimeSpan elapsed, bool doubletime)
        {
            base.Init(songData, cont, totalTime, elapsed, doubletime);
            m_angle = 0;
            m_init = true;
        }

        public override bool Act(AController controller, TimeSpan totalTime, TimeSpan elapsed, bool dTime)
        {
            const float spinSpeed = .049f;

            m_angle += spinSpeed * (float)elapsed.TotalMilliseconds;

            Vector2 point = controller.WindowHandle.Size();
            point.x /= 3;
            point.y /= 3;
            point.x += (int)((float)Math.Cos(m_angle) * controller.WindowHandle.Size().x / 4);
            point.y += (int)((float)Math.Sin(m_angle) * controller.WindowHandle.Size().x / 4);
            if (m_init)
            {
                m_init = false;
                controller.Press(point.x, point.y);
            }
            controller.Move(point.x, point.y);
            if (totalTime.TotalMilliseconds >= m_spinData.EndTime)
            {
                controller.Release(point.x, point.y);
                return true;
            }
            return false;
        }
    }
}
