using System;
using XAPI;
using System.Threading.Tasks;
using RestSharp;
using System.Dynamic;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Domain {
    class Creator
    {
        public Creator() {
            // set location on initialize and cache it.
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

        private async Task<Statement<Agent,Activity>> FormBasicStatement(String verbName,
                                                                         String userMbox, 
                                                                         String username,
                                                                         String gameId,
                                                                         String gameDisplay,
                                                                         String registrationIdentifier) {
            dynamic loc = await this.location;
            dynamic locationObject = new Dictionary<String,ExpandoObject>();
            dynamic vrObject = new Dictionary<String,ExpandoObject>();
            dynamic vrSubsystemMetadata = new ExpandoObject();
            dynamic vrSettingsMetadata = new ExpandoObject();
            // determines what type of VR device the user is using
            vrSettingsMetadata.loadedDeviceName = XR.deviceName();

            //determines whether or not VR is being used at all.
            vrSubsystemMetadata.running = XR.isPresent();
            vrObject.Add("https://docs.unity3d.com/ScriptReference/XR.XRDisplaySubsystem.html",
                         vrSubsystemMetadata);
            vrObject.Add("https://docs.unity3d.com/ScriptReference/XR.XRSettings.html",
                         vrSettingsMetadata);
            locationObject.Add("http://ip-api.com/location",loc);

            // statement construction
            return new Statement<Agent,Activity> {
                actor = new Agent{
                    mbox = userMbox,
                    name = username
                },
                verb = new Verb {
                    id = formVerbId(verbName),
                    display = new LanguageMap {
                        enUS = verbName
                    }
                }, 
                objekt = new Activity {
                    id = gameId,
                    definition = new ActivityDefinition {
                        name = new LanguageMap {
                            enUS = gameDisplay
                        },
                        extensions = vrObject
                    }
                },
                context = new Context{
                    registration = registrationIdentifier,
                    platform = Application.platform.ToString(),
                    extensions = locationObject
                }
            };
        }

        public async Task<Statement<Agent,Activity>> StartedStatement(String userMbox, 
                                                                      String username,
                                                                      String gameId,
                                                                      String gameDisplay,
                                                                      String registrationIdentifier) 
        {
            return await FormBasicStatement("initialized",
                                            userMbox,
                                            username,
                                            gameId,
                                            gameDisplay,
                                            registrationIdentifier);
        }

        public async Task<Statement<Agent,Activity>> CompletedStatement(String userMbox, 
                                                                        String username,
                                                                        String gameId,
                                                                        String gameDisplay,
                                                                        String registrationIdentifier) 
        {
            return await FormBasicStatement("completed",
                                            userMbox,
                                            username,
                                            gameId,
                                            gameDisplay,
                                            registrationIdentifier);
        }
    }
}