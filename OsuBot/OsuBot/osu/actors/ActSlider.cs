using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.osu.actors
{
    class ActSlider:HitActor
    {
        private Slider m_sliderData;
        SongData m_song;

        float m_millisecondsToComplete;
        float m_curX, m_curY;
        bool m_init;
        TimeSpan m_totalElapsed;

        int m_curId;
        int m_curRep;

        bool m_checkX;
        bool m_checkY;
        bool m_checkChange; 
        public ActSlider(Slider slider)
        {
            m_checkX = true;
            m_checkY = true;
            m_checkChange = true;
            m_sliderData = slider;
        }

        public override void Init(SongData songData, AController cont, TimeSpan totalTime, TimeSpan elapsed, bool doubletime)
        {
            base.Init(songData, cont, totalTime, elapsed, doubletime);
            m_song = songData;

            float msPBeat = -1;
            float multiplier = -1;


            if (m_song.Times.Count > 0)
            {
                if (msPBeat == -1 || multiplier == -1)
                {
                    for (int i = m_song.Times.Count - 1; i >= 0; i--)
                    {
                        if (totalTime.TotalMilliseconds >= m_song.Times[i].Time)
                        {
                            if (m_song.Times[i].Multiplier != -1 && multiplier == -1)
                            {
                                multiplier = m_song.Times[i].Multiplier;
                            }
                            if (m_song.Times[i].MSPerBeat != -1 && msPBeat == -1)
                            {
                                msPBeat = m_song.Times[i].MSPerBeat;
                            }
                        }
                    }
                }
            }

            m_millisecondsToComplete = ((float)(((float)m_sliderData.DataDistance / 100) * ((float)msPBeat / 1000)) / m_song.SliderMultiplier);
            m_millisecondsToComplete *= 1000;
            if (multiplier > 0)
            {
                m_millisecondsToComplete /= multiplier;
            }
            /*
            if (m_song.Times.Count > 0)
            {
                for (int i = m_song.Times.Count - 1; i >= 0; i--)
                {
                    if (totalTime.TotalMilliseconds >= m_song.Times[i].Time)
                    {
                        m_millisecondsToComplete /= m_song.Times[i].Multiplier;
                        break;
                    }
                }
            }*/

            if (doubletime)
            {
                m_millisecondsToComplete /= Program.DOUBLE_TIME_MULTIPLIER;
            }

            m_curX = 0;
            m_curY = 0;
            m_init = true;
            m_curId = 0;
            m_curRep = 1;
            m_totalElapsed = new TimeSpan();
        }

        const float MIN_SLIDER_DISTANCE = 50;

        public override bool Act(AController controller, TimeSpan totalTime, TimeSpan elapsed, bool dTime)
        {
            if (m_init)
            {
                m_init = false;
                m_curX = m_sliderData.HitData.X;
                m_curY = m_sliderData.HitData.Y;
                controller.Press((int)m_curX, (int)m_curY);
            }
            m_totalElapsed += elapsed;
            //Console.WriteLine(m_curId + " " + m_sliderData.Points.Count + m_millisecondsToComplete);
            if (m_totalElapsed.TotalMilliseconds >= m_millisecondsToComplete + 10)
            {
                if (CheckIfRepeat() == false)
                {
                    controller.Release((int)m_curX, (int)m_curY);
                    return true;
                }
                else
                {
                    if(m_sliderData.DataDistance >= MIN_SLIDER_DISTANCE)
                        controller.Move((int)m_curX, (int)m_curY);
                }
            }
            else if(m_curId < m_sliderData.Points.Count)
            {
                WinAPI.POINT destination = new WinAPI.POINT();
                float msPerPoint = m_millisecondsToComplete / ((float)m_sliderData.Points.Count - 1);

                destination = m_sliderData.Points[m_curId];
                if (m_curId == 0)
                {
                    m_curX = destination.x;
                    m_curY = destination.y;
                    controller.Move((int)m_curX, (int)m_curY);
                    IncreaseId(controller);
                }
                else
                {

                    float angleX = destination.x - m_curX;
                    float angleY = destination.y - m_curY;

                    float angle = (float)Math.Atan2(angleY, angleX);
                    float addX = (float)Math.Cos(angle);
                    float addY = (float)Math.Sin(angle);

                    if (m_checkChange)
                    {
                        m_checkChange = false;
                        m_checkX = true;
                        m_checkY = true;
                        if (m_curX < destination.x)
                        {
                            m_checkX = false;
                        }
                        if (m_curY < destination.y)
                        {
                            m_checkY = false;
                        }
                    }

                    const float BRUTE_FORCE_SLOWDOWN = .94f; //Have no idea why I need to do this, but my shit goes too fast
                    if (m_sliderData.Distance >= MIN_SLIDER_DISTANCE)
                    {
                        m_curX += addX * (((float)m_sliderData.Distance / m_millisecondsToComplete) * (float)elapsed.TotalMilliseconds) * BRUTE_FORCE_SLOWDOWN;
                        m_curY += addY * (((float)m_sliderData.Distance / m_millisecondsToComplete) * (float)elapsed.TotalMilliseconds) * BRUTE_FORCE_SLOWDOWN;

                        bool[] complete = new bool[2] { false, false };
                        if (!m_checkX)
                        {
                            if (m_curX >= destination.x)
                            {
                                complete[0] = true;
                            }
                        }
                        else
                        {
                            if (m_curX <= destination.x)
                            {
                                complete[0] = true;
                            }
                        }
                        if (!m_checkY)
                        {
                            if (m_curY >= destination.y)
                            {
                                complete[1] = true;
                            }
                        }
                        else
                        {
                            if (m_curY <= destination.y)
                            {
                                complete[1] = true;
                            }
                        }
                        if (complete[0] && complete[1])
                        {
                            IncreaseId(controller);
                        }
                    }
                    controller.Move((int)m_curX, (int)m_curY);
                }
            }

            return false;
        }

        bool IncreaseId(AController controller)
        {
            m_curId++;
            m_checkChange = true;
            if (m_curId >= m_sliderData.Points.Count)
            {
                /*if (CheckIfRepeat())
                {
                    controller.Move((int)m_curX, (int)m_curY);
                }
                else
                    return true;*/
            }
            return false;
        }

        bool CheckIfRepeat()
        {
            if (m_curRep < m_sliderData.UseCount)
            {
                m_curRep++;
                List<WinAPI.POINT> m_curvePoints = new List<WinAPI.POINT>();
                for (int i = m_sliderData.Points.Count - 1; i >= 0; i--)
                {
                    m_curvePoints.Add(m_sliderData.Points[i]);
                }
                m_sliderData.Points = m_curvePoints;
                m_totalElapsed = new TimeSpan(0,0,0,0);
                m_curId = 0;
                m_curX = m_sliderData.Points[0].x;
                m_curY = m_sliderData.Points[0].y;
                m_checkChange = true;
                return true;
            }
            return false;
        }
    }
}
