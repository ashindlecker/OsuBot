using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace OsuBot
{
    class Player
    {
        AController m_controller;
        osu.HitActor m_currentObject;
        osu.SongData m_songData;

        Stopwatch m_stopWatch;
        TimeSpan m_timeOffset;
        Stopwatch m_delta;

        List<bool> m_hasBeenActed;
        List<osu.HitActor> m_actors;

        bool m_started;
        TimeSpan PassedTime
        {
            get
            {
                if (!DoubleTime)
                    return m_stopWatch.Elapsed + m_timeOffset;
                else
                {
                    return (new TimeSpan(0, 0, 0, 0, (int)(m_stopWatch.Elapsed.TotalMilliseconds * Program.DOUBLE_TIME_MULTIPLIER))) + m_timeOffset;
                }
            }
        }

        public bool DoubleTime
        {
            get;
            set;
        }

        public bool IsPlaying
        {
            get { return m_started; }
        }

        int m_lastActId;

        public Player(AController controller)
        {
            m_started = false;
            m_controller = controller;
            m_currentObject = null;
            m_stopWatch = new Stopwatch();
            m_timeOffset = new TimeSpan(0);
            m_hasBeenActed = new List<bool>();
            m_delta = new Stopwatch();

            m_actors = new List<osu.HitActor>();

            DoubleTime = false;
            m_lastActId = -1;
        }

        public void SetSong(osu.SongData songdata)
        {
            m_songData = songdata;
            m_hasBeenActed.Clear();
            m_actors.Clear();
            for (int i = 0; i < m_songData.HitObjects.Count; i++)
            {
                m_hasBeenActed.Add(false);
                m_actors.Add(osu.HitActor.GetActor(m_songData.HitObjects[i]));
            }
            m_started = false;
        }

        public void Reset()
        {
            m_started = false;
            m_delta.Reset();
            m_stopWatch.Reset();
        }

        public void StartFirstNote()
        {
            osu.HitObject.Data data = m_songData.HitObjects[0].HitData;
            m_timeOffset = new TimeSpan(0, 0, 0, 0, (int)data.Time);
            m_stopWatch.Reset();
            m_stopWatch.Start();
            m_delta.Reset();
            m_delta.Start();
            m_started = true;
        }


        public void Update()
        {
            if (m_started)
            {
                TimeSpan elapsed = m_delta.Elapsed;
                m_delta.Restart();

                m_currentObject = null;
                osu.HitObject.Data m_nextObject = null;
                int actedId = -1;
                for (int i = m_songData.HitObjects.Count - 1; i >= 0; i--)
                {
                    if (PassedTime >= new TimeSpan(0, 0, 0, 0, (int)m_songData.HitObjects[i].HitData.Time))
                    {
                        if (m_hasBeenActed[i] == false)
                        {
                            m_currentObject = m_actors[i];
                            actedId = i;

                            if (actedId > m_lastActId && m_lastActId != -1)
                            {
                                m_hasBeenActed[m_lastActId] = true;
                            }

                            m_lastActId = actedId;
                            break;
                        }
                        else
                        {
                            if (m_nextObject == null)
                            {
                                if (i + 1 < m_songData.HitObjects.Count)
                                {
                                    m_nextObject = m_songData.HitObjects[i + 1].HitData;
                                }
                            }
                        }
                    }
                }


                if (m_currentObject != null)
                {
                    if (m_currentObject.HasBeenInit == false)
                    {
                        m_currentObject.Init(m_songData, m_controller, PassedTime, elapsed, DoubleTime);
                    }
                    m_hasBeenActed[actedId] = m_currentObject.Act(m_controller, PassedTime, elapsed, DoubleTime);
                }
                else
                {
                    //Interpolate
                    if (m_nextObject != null)
                    {
                        WinAPI.POINT cursor = new WinAPI.POINT();
                        WinAPI.GetCursorPos(ref cursor);
                        WinAPI.POINT dest = new WinAPI.POINT();
                        dest.x = m_nextObject.X;
                        dest.y = m_nextObject.Y;
                        osu.OsuOptions.Convert(ref dest, m_controller.WindowHandle.Size().x, m_controller.WindowHandle.Size().y);

                        Vector2 newpos = m_controller.WindowHandle.ConvertToScreen(new Vector2(dest.x, dest.y));
                        dest.x = newpos.x;
                        dest.y = newpos.y;
                        const double INTER_SPEED = 1.3f;
                        if (cursor.x < dest.x)
                        {
                            cursor.x += (int)(INTER_SPEED * elapsed.TotalMilliseconds);
                        }
                        if (cursor.x > dest.x)
                        {
                            cursor.x -= (int)(INTER_SPEED * elapsed.TotalMilliseconds);
                        }
                        if (cursor.y < dest.y)
                        {
                            cursor.y += (int)(INTER_SPEED * elapsed.TotalMilliseconds);
                        }
                        if (cursor.y > dest.y)
                        {
                            cursor.y -= (int)(INTER_SPEED * elapsed.TotalMilliseconds);
                        }
                        WinAPI.SetCursorPos(cursor.x, cursor.y);
                    }
                    if (actedId >= 0)
                    {
                        m_hasBeenActed[actedId] = true;
                    }
                }
            }
        }

        

        
    }
}
