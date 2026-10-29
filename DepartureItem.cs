using System;
using System.Collections.Generic;
using System.Text;

namespace RailScope
{
    public class DepartureItem
    {
        public string Destination{ get; set; }
        public string Platform { get; set; }
        public string ScheduledTime { get; set; }
        public string TrainHeadcode { get; set; }
        public string Status { get; set; }
        public string EstimatedTime { get; set; }
        public string TrainOperator { get; set; }
    }
}
