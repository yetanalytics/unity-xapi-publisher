using System;

namespace XAPI.Metadata {
    public class Location {
        public String query { set; get; }
        public String status { set; get; }
        public String country { set; get; }
        public String countryCode { set; get; }
        public String region { set; get; }
        public String regionName { set; get; }
        public String city { set; get; }
        public String zip { set; get; }
        public float lat { set; get; }
        public float lon { set; get; }
        public String timezone { set; get; }
        public String isp { set; get; }
        public String org { set; get; }
    }
}