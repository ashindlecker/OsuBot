using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.osu
{
    class Slider:HitObject
    {
        List<WinAPI.POINT> m_points;
        List<WinAPI.POINT> m_curvePoints;

        int m_useCount;
        int m_distance;
        float m_totalDistance;

        public int UseCount
        {
            get { return m_useCount; }
        }
        public int DataDistance
        {
            get { return m_distance; }
            
        }
        public List<WinAPI.POINT> Points
        {
            get { return m_curvePoints; }
            set { m_curvePoints = value; }
        }
        public float Distance
        {
            get { return m_totalDistance; }
        }


        public Slider()
        {
            HitData.Type = 2;
            m_useCount = 0;
            m_distance = 0;
            m_points = new List<WinAPI.POINT>();
            m_curvePoints = new List<WinAPI.POINT>();
        }

        protected override void Parse(System.IO.StreamReader reader, string line)
        {
            if (line != null)
            {
                string curveData = "";
                if (line.Contains("B|"))
                {
                    curveData = line.Substring(line.IndexOf(',', line.IndexOf("B|") - 1));
                }
                else if (line.Contains("C|"))
                {
                    curveData = line.Substring(line.IndexOf(',', line.IndexOf("C|") - 1));
                }
                else if (line.Contains("|"))
                {
                    curveData = line.Substring(line.IndexOf(',', line.IndexOf("|") - 2));
                }
                int points = curveData.Count(f => f == ':');
                double[] data = Parsers.NumbersFromString.Parse(curveData);

                WinAPI.POINT startPoint = new WinAPI.POINT();
                startPoint.x = HitData.X;
                startPoint.y = HitData.Y;
                m_points.Add(startPoint);

                for (int i = 0; i < points; i++)
                {
                    WinAPI.POINT point = new WinAPI.POINT();
                    point.x = (int)data[i * 2];
                    point.y = (int)data[i * 2 + 1];
                    m_points.Add(point);
                }

                m_useCount = (int)data[(points * 2)];
                m_distance = (int)data[(points * 2) + 1];


                double[] dPoints = new double[m_points.Count * 2];
                for (int i = 0; i < m_points.Count; i++)
                {
                    dPoints[i * 2] = m_points[i].x;
                    dPoints[i * 2 + 1] = m_points[i].y;
                }

                double[] dCurvePoints = new double[dPoints.Length];
                Curves.BezierCurve bCurve = new Curves.BezierCurve();
                bCurve.Bezier2D(dPoints, m_points.Count, dCurvePoints);

                for (int i = 0; i < dCurvePoints.Length / 2; i++)
                {
                    WinAPI.POINT point = new WinAPI.POINT();
                    point.x = (int)dCurvePoints[i * 2];
                    point.y = (int)dCurvePoints[i * 2 + 1];
                    m_curvePoints.Add(point);
                }

                for (int i = 0; i < m_curvePoints.Count; i++)
                {
                    if (i > 0)
                    {
                        int disX = Math.Abs(m_curvePoints[i].x) - Math.Abs(m_curvePoints[i - 1].x);
                        int disY = Math.Abs(m_curvePoints[i].y) - Math.Abs(m_curvePoints[i - 1].y);
                        float add = (float)Math.Sqrt((disX * disX) + (disY * disY));
                        m_totalDistance += add;
                    }
                }
            }
        }
    }
}
