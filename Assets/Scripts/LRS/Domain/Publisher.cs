using System;
using XAPI;
using System.Threading.Tasks;
using RestSharp;
using System.Dynamic;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using Util;
using LRS;

namespace LRS
{
    namespace Domain
    {
        class Publisher
        {
            public Publisher(String LRSUrl, String LRSKey, String LRSSecret)
            {
                // set location on initialize and cache it.
                this.sender = new Sender(LRSUrl, LRSKey, LRSSecret);
                this.locationTask = GetLocation();
            }

            // setters getters
            private Task<ExpandoObject> locationTask { set; get; }
            private static readonly ConcurrentDictionary<string, ExpandoObject> downloadCache = new();
            private static readonly String VERB_URI = "http://adlnet.gov/expapi/verbs/";
            private String verbUri { get { return VERB_URI; } }
            private Sender sender { set; get; }

            // Player Data
            private String email { get { return String.Format("mailto:{0}", PlayerPrefs.GetString("LRSEmail")); } }
            private String nameDisplay { get { return PlayerPrefs.GetString("LRSUsernameDisplay"); } }
            private String gameId { get { return PlayerPrefs.GetString("LRSGameId"); } }
            private String gameDisplay { get { return PlayerPrefs.GetString("LRSGameDisplay"); } }
            private String registrationIdentifier { get { return PlayerPrefs.GetString("LRSSessionIdentifier"); } }

            private String formVerbId(String verb)
            {
                return String.Format("{0}{1}", verbUri, verb);
            }

            // getLocation stuff
            private async Task<RestResponse> GetIp()
            {
                var client = new RestClient("http://canhazip.com");
                var request = new RestRequest("");
                return await client.GetAsync(request);

            }

            private async Task<ExpandoObject> GetLocation()
            {
                var response = await GetIp();
                var ip = response.Content;
                var client = new RestClient("http://ip-api.com");

                // return whatever we get out of the cache if something exists there.
                if (downloadCache.TryGetValue("location", out ExpandoObject location))
                {
                    return await Task.FromResult(location);
                }

                // otherwise, we go ahead and populate the cache by calling the API
                return await Task.Run(async () =>
                {
                    location = await client.GetJsonAsync<ExpandoObject>(string.Format("json/{0}", ip));
                    downloadCache.TryAdd("location", location);

                    return location;
                });
            }

            private async Task<Statement<Agent, Activity>> FormBasicStatement(String verbId,
                                                                             String verbDisplay,
                                                                             String userMbox,
                                                                             String username,
                                                                             String gameId,
                                                                             String gameDisplay,
                                                                             String registrationIdentifier)
            {
                dynamic loc = await this.locationTask;
                dynamic locationObject = new Dictionary<String, ExpandoObject>();
                dynamic vrObject = new Dictionary<String, ExpandoObject>();
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
                locationObject.Add("http://ip-api.com/location", loc);

                // statement construction
                return new Statement<Agent, Activity>
                {
                    actor = new Agent
                    {
                        mbox = userMbox,
                        name = username
                    },
                    verb = new Verb
                    {
                        id = verbId,
                        display = new LanguageMap
                        {
                            enUS = verbDisplay
                        }
                    },
                    objekt = new Activity
                    {
                        id = gameId,
                        definition = new ActivityDefinition
                        {
                            name = new LanguageMap
                            {
                                enUS = gameDisplay  // put this into platform
                            },
                            extensions = vrObject
                        }
                    },
                    context = new Context
                    {
                        registration = registrationIdentifier,
                        platform = Application.platform.ToString(),// Game ID goes here.
                        extensions = locationObject // hardware into here
                    }
                };
            }

            public async Task<Statement<Agent, Activity>> StartedStatement(String userMbox,
                                                                          String username,
                                                                          String gameId,
                                                                          String gameDisplay,
                                                                          String registrationIdentifier)
            {
                return await FormBasicStatement(formVerbId("initialized"),
                                                "Initialized",
                                                userMbox,
                                                username,
                                                gameId,
                                                gameDisplay,
                                                registrationIdentifier);
            }

            public async Task<Statement<Agent, Activity>> CompletedStatement(String userMbox,
                                                                            String username,
                                                                            String gameId,
                                                                            String gameDisplay,
                                                                            String registrationIdentifier)
            {
                return await FormBasicStatement(formVerbId("completed"),
                                                "Completed",
                                                userMbox,
                                                username,
                                                gameId,
                                                gameDisplay,
                                                registrationIdentifier);
            }


            public async void SendStartedStatement()
            {
                var statement = await StartedStatement(email,
                                                       nameDisplay,
                                                       gameId,
                                                       gameDisplay,
                                                       registrationIdentifier);
                var statementStr = statement.Serialize();
                var response = await sender.SendStatement(statementStr);
                Debug.Log(statementStr);
                Debug.Log(response.Content);
                Debug.Log(response.ResponseStatus);
            }

            public async void SendCompletedStatement()
            {
                var statement = await CompletedStatement(email,
                                                        nameDisplay,
                                                        gameId,
                                                        gameDisplay,
                                                        registrationIdentifier);
                var statementStr = statement.Serialize();
                var response = await sender.SendStatement(statementStr);
                Debug.Log(statementStr);
                Debug.Log(response.Content);
                Debug.Log(response.ResponseStatus);

            }

            public async void SendStatement(String verbId,
                                            String verbDisplay)
            {
                var statement = await FormBasicStatement(verbId,
                                                         verbDisplay,
                                                         email,
                                                         nameDisplay,
                                                         gameId,
                                                         gameDisplay,
                                                         registrationIdentifier);
                var statementStr = statement.Serialize();
                var response = await sender.SendStatement(statementStr);
                Debug.Log(statementStr);
                Debug.Log(response.Content);
                Debug.Log(response.ResponseStatus);

            }

            async void SendCustomStatement(String verbId,
                                           String verbDisplay,
                                           String email,
                                           String nameDisplay,
                                           String gameId,
                                           String gameDisplay,
                                           String registrationIdentifier)
            {
                var statement = await FormBasicStatement(verbId,
                                                         verbDisplay,
                                                         email,
                                                         nameDisplay,
                                                         gameId,
                                                         gameDisplay,
                                                         registrationIdentifier);
                var statementStr = statement.Serialize();
                var response = await sender.SendStatement(statementStr);
                Debug.Log(statementStr);
                Debug.Log(response.Content);
                Debug.Log(response.ResponseStatus);

            }
        }
    }
}