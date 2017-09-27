using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCoinLib.Util
{
    public static class UnixDateTime
    {
        public static Int32 UnixTime
        {
            get
            {
                return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }

        private static DateTime toDateTime(double UnixTimeStamp)
        {

            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(UnixTimeStamp);
            return dateTime;
        }

        internal static uint DateTimeToUnixTime(DateTimeOffset value)
        {
            throw new NotImplementedException();
        }

        internal static DateTimeOffset UnixTimeToDateTime(uint nTime)
        {
            throw new NotImplementedException();
        }
    }
}
