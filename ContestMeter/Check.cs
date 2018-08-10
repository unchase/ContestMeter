using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContestMeter
{
    public static class Check
    {
        public static void NotNull(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void NotNullOrEmpty(string value, string name)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
