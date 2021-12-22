using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLib
{
    public static class DateTimeUtil
    {
        public static DateTime Datetime { get { return CalcDatetime; } }
        public static DateTime CalcDatetime { get; private set; } = DateTime.UtcNow;

        public static void SetDatetime(int _addMinute)
        {
            CalcDatetime = CalcDatetime.AddMinutes(_addMinute);
        }

        //NOTE : src값이 dest보다 같거나 크다면 true, 작다면 false
        public static bool CompareDateTime(DateTime _src, DateTime _dest)
        {
            if (DateTime.Compare(_src, _dest) >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
