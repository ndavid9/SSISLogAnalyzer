using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSISLogSimplifier
{
  public  class LogEntity
    {
        public string EventTime { get; set; }
        public string Code { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public LogEnum EventType { get; set; }
          }
}
