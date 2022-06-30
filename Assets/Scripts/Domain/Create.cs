using System;
using XAPI;
using System.Threading.Tasks;
using RestSharp;
using System.Dynamic;
using System.Collections.Generic;
using UnityEngine;

namespace Domain {
    class Create
    {
        public Create() {
            // set location on initialize
            this.location = GetLocation();
        }
        
        // setters getters
        private Task<ExpandoObject> location { set; get; }
        private static readonly String VERB_URI = "http://adlnet.gov/expapi/verbs/";
        private String verbUri { get {return VERB_URI; }}

        private String formVerbId(String verb) {
            return String.Format("{0}{1}",verbUri,verb);
        }

        // getLocation stuff
        private async Task<RestResponse> GetIp() {
            var client = new RestClient("http://canhazip.com");
            var request = new RestRequest("");
            return await client.GetAsync(request);

        }

        private async Task<ExpandoObject> GetLocation() {
            var response = await GetIp();
            var ip = response.Content;
            var client = new RestClient("http://ip-api.com");
            return await client.GetJsonAsync<ExpandoObject>(string.Format("json/{0}", ip)); //.ConfigureAwait(false);
        }

        public async Task<Statement<Agent,Activity>> StartedStatement(String userMbox, 
                                                                      String username,
                                                                      String gameId,
                                                                      String gameDisplay) 
        {
            dynamic loc = await this.location;
            dynamic locationObject = new Dictionary<String,ExpandoObject>();
            locationObject.Add("http://ip-api.com/location",loc);
             
            return new Statement<Agent,Activity> {
                actor = new Agent{
                    mbox = userMbox,
                    name = username
                },
                verb = new Verb {
                    id = formVerbId("initialized")
                }, 
                objekt = new Activity {
                    id = gameId,
                    definition = new ActivityDefinition {
                        name = new LanguageMap {
                            enUS = gameDisplay
                        }
                    }
                },
                context = new Context{
                    extensions = locationObject
                }
            };
        }

        public Statement<Agent,Activity> StartedStatement(String mbox, 
                                                          String gameId,
                                                          String gameDisplay, 
                                                          String gameSectionId,
                                                          String gameSectionDisplay) 
        {
            return new Statement<Agent,Activity>();
        }

        public Statement<Agent,Activity> CompletedStatement(String mbox,
                                                            String gameIdentifier)
        {
            return new Statement<Agent,Activity>();
        }

        public Statement<Agent,Activity> CompletedStatement(String mbox,
                                                            String gameIdentifier,
                                                            String gameSectionIdentifier)
        {
            return new Statement<Agent,Activity>();
        }
    }
}