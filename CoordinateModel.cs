using System;
using System.Collections.Generic;
using System.Text;

namespace CoordManagerTool
{

    public class CoordinateModel
    {
        public List<RECORD> RECORDS { get; set; }
    }

    public class RECORD
    {
        public string Id { get; set; }
        public string adcode { get; set; }
        public string center_lat { get; set; }
        public string center_lon { get; set; }
        public string citycode { get; set; }
        public string level { get; set; }
        public string name { get; set; }
        public string polyline { get; set; }
    }

}
