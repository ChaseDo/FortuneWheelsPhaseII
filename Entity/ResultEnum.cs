using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public enum ResultEnum
    {
        Ipad = 1,
        OneYuan = 2,
        TwoYuan = 3,
        FiveYuan = 4,
        TenYuan = 5
    }

    public class LotteryResult
    {
        public ResultEnum ResultName { get; set; }
        public int Angle { get; set; }
        public int MinNumber { get; set; }
        public int MaxNumber { get; set; }
    }
}
