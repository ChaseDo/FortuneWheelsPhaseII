using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Model
{
    public class DownloadHistory
    {
        public string CellPhoneNo { get; set; }
        public bool Game1 { get; set; }
        public bool Game2  { get; set; }
        public DateTime Game1Time { get; set; }
        public DateTime Game2Time { get; set; }
    }
}
