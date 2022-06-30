using System;
using XAPI;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;


namespace Domain {
    class Sender {
        public Sender(String LRSUrl,
                      String LRSKey,
                      String LRSSecret) 
        {
            this.LRSUrl = LRSUrl;
            this.LRSKey = LRSKey;
            this.LRSSecret = LRSSecret;
        }

        private String LRSUrl {set;get;}
        private String LRSKey {set;get;}
        private String LRSSecret {set;get;}

        public async Task<RestResponse> SendStatement(String statement) {
            var client = new RestClient(LRSUrl) {
                Authenticator = new HttpBasicAuthenticator(LRSKey,LRSSecret)
            };
            var request = new RestRequest("/xapi/statements", Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddStringBody(statement,DataFormat.Json);
            request.AddHeader("X-Experience-API-Version", "1.0.1");
            return await client.GetAsync(request);
        }
    }
}