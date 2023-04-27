using System;


namespace XAPI {
    public class Result : IExtension {
        public Score score { get; set; }
        public Boolean success { get; set; }
        public Boolean completion { get; set; }
        public String response { get; set; }
        public String duration { get; set; }
        public Extension extensions {set;get;}

    }
}