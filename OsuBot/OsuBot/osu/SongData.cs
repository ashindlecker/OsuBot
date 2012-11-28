using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OsuBot.osu
{
    class SongData
    {
        /*
        public float MillisecondsPerBeat
        {
            get;
            set;
        }*/
        public float SliderMultiplier
        {
            get;
            set;
        }
        public string AudioFile
        {
            get;
            set;
        }
        public List<TimeData> Times
        {
            get { return m_times; }
        }
        public struct TimeData
        {
            public float MSPerBeat;
            public int Time;
            public float Multiplier;
        }

        List<HitObject> m_hitObjects;
        List<TimeData> m_times;

        public List<HitObject> HitObjects
        {
            get { return m_hitObjects; }
        }

        public SongData()
        {
            m_hitObjects = new List<HitObject>();
            //MillisecondsPerBeat = -1;
            m_times = new List<TimeData>();
        }

        enum HeaderType
        {
            Default,
            Difficulty,
            HitObjects,
            Timing,
            General,
        };

        public void Parse(System.IO.Stream stream)
        {
            HeaderType currentHeader = HeaderType.Default;

            StreamReader reader = new StreamReader(stream);

            while (reader.EndOfStream == false)
            {
                string line = reader.ReadLine();
                bool hitFlag = false;
            hitException:
                if (!hitFlag && line.Contains("[Difficulty]"))
                {
                    currentHeader = HeaderType.Difficulty;
                }
                else if (!hitFlag && line.Contains("[HitObjects]"))
                {
                    currentHeader = HeaderType.HitObjects;
                    hitFlag = true;
                    goto hitException;
                }
                else if (!hitFlag && line.Contains("[General]"))
                {
                    currentHeader = HeaderType.General;
                }
                else if (!hitFlag && line.Contains("[TimingPoints]"))
                {
                    currentHeader = HeaderType.Timing;
                }
                else if (!hitFlag && line.Contains("["))
                {
                    currentHeader = HeaderType.Default;
                }
                else
                {
                    string attribute = "";
                    string value = "";
                    if (line.Contains(":"))
                    {
                        value = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        attribute = line.Substring(0, line.IndexOf(":"));
                    }
                    switch (currentHeader)
                    {
                        default:
                            break;
                        case HeaderType.General:
                            if (attribute == "AudioFilename")
                            {
                                AudioFile = value;
                            }
                            break;
                        case HeaderType.Difficulty:
                            if (attribute == "SliderMultiplier")
                            {
                                SliderMultiplier = Convert.ToSingle(value);
                            }
                            break;
                        case HeaderType.Timing:
                            {
                                if (line.Contains(","))
                                {
                                    string multiply = line.Substring(line.IndexOf(',') + 1, line.IndexOf(',', line.IndexOf(',') + 1) - (line.IndexOf(',') + 1));
                                    double[] nums = Parsers.NumbersFromString.Parse(line);

                                    double mainData = nums[1];

                                    TimeData addData = new TimeData();
                                    addData.Time = (int)nums[0];
                                    addData.Multiplier = -1;
                                    addData.MSPerBeat = -1;
                                    if (mainData < 0)
                                    {
                                        addData.Multiplier = (float)100f / Math.Abs((float)nums[1]);
                                    }
                                    else
                                    {
                                        addData.MSPerBeat = (float)mainData;
                                    }
                                    m_times.Add(addData);
                                    /*
                                    if (MillisecondsPerBeat <= -1)
                                        MillisecondsPerBeat = Convert.ToSingle(multiply);
                                    else
                                    {
                                        TimeData addData = new TimeData();
                                        addData.Time = (int)nums[0];
                                        addData.Multiplier = (float)100f / Math.Abs((float)nums[1]);
                                        m_times.Add(addData);
                                    }*/
                                }
                            }
                            break;
                        case HeaderType.HitObjects:
                            {
                                HitObject add = HitObject.CreateParse(reader, this);
                                while (add != null)
                                {
                                    int startOffsetId = m_hitObjects.Count - 1;

                                    const int backOffSet = 3;
                                    for (int i = m_hitObjects.Count - 1; i >= 0; i--)
                                    {
                                        if (m_hitObjects[i].HitData.OrigX == add.HitData.OrigX && m_hitObjects[i].HitData.OrigY == add.HitData.OrigY)
                                        {
                                            if (m_hitObjects[startOffsetId].HitData.Time - m_hitObjects[i].HitData.Time <= 500)
                                            {
                                                startOffsetId = i;
                                                m_hitObjects[i].HitData.X -= backOffSet;
                                                m_hitObjects[i].HitData.Y -= backOffSet;

                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }


                                    m_hitObjects.Add(add);
                                    add = HitObject.CreateParse(reader, this);
                                    
                                }
                            }
                            break;
                    }
                }
            }
            stream.Close();
        }
    }
}
