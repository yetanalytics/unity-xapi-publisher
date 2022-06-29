using System;
using System.Dynamic;
using System.Collections.Generic;


namespace XAPI {
    class Result : IExtension {
        public Score score { get; set; }
        public Boolean success { get; set; }
        public Boolean completion { get; set; }
        public String response { get; set; }
        public String duration { get; set; }
        public List<ExpandoObject> extensions {set;get;}

    }
}