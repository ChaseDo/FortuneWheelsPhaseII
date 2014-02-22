using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Model
{
    public class LotteryUser
    {
        public string CellPhoneNo { get; set; }
        public int LotteryCount { get; set; }
        public DateTime LastLoginTime { get; set; }
    }
}
