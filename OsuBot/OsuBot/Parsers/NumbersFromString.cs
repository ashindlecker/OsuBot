using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuBot.Parsers
{
    class NumbersFromString
    {
        public static double[] Parse(string line)
        {
            List<double> ret = new List<double>();

            string number = "";
            for (int i = 0; i < line.Length; i++)
            {
                if (((int)line[i] >= 48 && (int)line[i] <= 57) || line[i] == '.' || line[i] == '-')
                {
                    number += line[i];
                }
                else
                {
                    if (number.Length > 0)
                    {
                        ret.Add(Convert.ToDouble(number));
                        number = "";
                    }
                }
            }
            if (number.Length > 0)
            {
                ret.Add(Convert.ToDouble(number));
                number = "";
            }
            return ret.ToArray();
        }
    }
}
