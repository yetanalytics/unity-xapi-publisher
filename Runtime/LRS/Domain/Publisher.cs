using System;
using XAPI;
using XAPI.Metadata;
using System.Threading.Tasks;
using RestSharp;
using System.Collections.Concurrent;
using UnityEngine;
using Util;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Globalization;

namespace LRS
{
    namespace Domain
    {
        // callback for Statement sendoff hook
        public delegate void StatementSendoffEventHandler(JsonObject statement);

        public class Publisher
        {
            public Publisher(String LRSUrl, String LRSKey, String LRSSecret)
            {
                // set location on initialize and cache it.
                this.sender = new Sender(LRSUrl, LRSKey, LRSSecret);
                this.locationTask = GetLocation();
            }

            // statement sendoff hook definition
            public static event StatementSendoffEventHandler OnStatementSent;

            public static void InvokeStatementSent(JsonObject statement)
            {
                OnStatementSent?.Invoke(statement);
            }

            private JsonObject formAgent() {
                bool hasEmail = PlayerPrefs.HasKey("LRSEmail");
                bool hasAccount = PlayerPrefs.HasKey("LRSAccountId") && PlayerPrefs.HasKey("LRSHomepage");
                if (hasEmail)
                {
                    return new JsonObject
                    {
                        ["mbox"] = "mailto:" + PlayerPrefs.GetString("LRSEmail"),
                        ["name"] = PlayerPrefs.GetString("LRSUsernameDisplay")
                    };
                }
                else if (hasAccount)
                {
                    return new JsonObject
                    {
                        ["account"] = new JsonObject
                        {
                            ["homePage"] = PlayerPrefs.GetString("LRSHomepage"),
                            ["name"] = PlayerPrefs.GetString("LRSAccountId")
                        },
                        ["name"] = PlayerPrefs.GetString("LRSUsernameDisplay")
                    };

                }
                else
                {
                    throw new ArgumentException("Invalid user identifiers. Must supply the following in PlayerPrefs: (LRSAccountId + LRSHomepage) OR LRSEmail");
                }
            }

            // setters getters
            private Task<JsonObject> locationTask { set; get; }
            private static readonly ConcurrentDictionary<string, JsonObject> downloadCache = new();
            private static readonly String VERB_URI = "http://adlnet.gov/expapi/verbs/";
            private String verbUri { get { return VERB_URI; } }
            private Sender sender { set; get; }

            // Player Data
            private bool enableUserLocation { get { return PlayerPrefs.HasKey("LRSEnableUserLocation"); } }
            private String email { get { return String.Format("mailto:{0}", PlayerPrefs.GetString("LRSEmail")); } }

            private JsonObject user { get { return formAgent(); } }
            private String nameDisplay { get { return PlayerPrefs.GetString("LRSUsernameDisplay"); } }
            private String gameId { get { return PlayerPrefs.GetString("LRSGameId"); } }
            private String gameDisplay { get { return PlayerPrefs.GetString("LRSGameDisplay"); } }

            private bool hasCustomActivityEnv { get { return PlayerPrefs.HasKey("LRSActivityId") &&
                                                             PlayerPrefs.HasKey("LRSActivityDefinition"); } }

            private String customActivityId { get { return PlayerPrefs.GetString("LRSActivityId"); } }
            private String customActivityDefinition { get { return PlayerPrefs.GetString("LRSActivityDefinition"); } }

            // TODO: make this optional
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

            private async Task<JsonObject> GetLocation()
            {
                var response = await GetIp();
                var ip = response.Content;
                var client = new RestClient("http://ip-api.com");

                // return whatever we get out of the cache if something exists there.
                if (downloadCache.TryGetValue("location", out JsonObject location))
                {
                    return await Task.FromResult(location);
                }

                // otherwise, we go ahead and populate the cache by calling the API
                return await Task.Run(async () =>
                {
                    location = await client.GetJsonAsync<JsonObject>(string.Format("json/{0}", ip));
                    downloadCache.TryAdd("location", location);

                    return location;
                });
            }

            private async Task<JsonObject> FormBasicStatement(String verbId,
                                                              String verbDisplay,
                                                              JsonObject user,
                                                              String gameId,
                                                              String gameDisplay,
                                                              String registrationIdentifier,
                                                              Func<JsonObject, JsonObject> statementFn)
            {
                JsonObject contextExtension = new JsonObject
                {
                    ["https://docs.unity3d.com/ScriptReference/Application-platform.html"] = new JsonObject
                    {
                        ["platform"] = Application.platform.ToString()
                    }
                };

                JsonObject objectDefinitionExtension = new JsonObject
                {
                    ["https://docs.unity3d.com/ScriptReference/XR.XRSettings.html"] = new JsonObject
                    {
                        ["loadedDeviceName"] = XR.deviceName()
                    },
                    ["https://docs.unity3d.com/ScriptReference/XR.XRDisplaySubsystem.html"] = new JsonObject
                    {
                        ["running"] = XR.isPresent()
                    }
                };

                // location
                if (enableUserLocation) {
                    JsonObject loc = await this.locationTask;
                    contextExtension["http://ip-api.com/location"] = loc;
                }

                String activityId = gameId;
                String activityDefinition = gameDisplay;

                // if there's a custom ActivityID in the env, set activityId to that one.
                if (hasCustomActivityEnv) {
                    activityId = customActivityId;
                    activityDefinition = customActivityDefinition;
                }

                // statement construction
                var statement = new JsonObject
                {
                    ["actor"] = user,
                    ["verb"] = new JsonObject
                    {
                        ["id"] = verbId,
                        ["display"] = new JsonObject
                        {
                            ["en-US"] = verbDisplay
                        }
                    },
                    ["object"] = FormActivity(activityId, activityDefinition, objectDefinitionExtension),
                    ["context"] = new JsonObject
                    {
                        ["registration"] = registrationIdentifier,
                        ["platform"] = gameId,
                        ["extensions"] = contextExtension
                    },
                    ["timestamp"] = DateTime.UtcNow.ToString("o",CultureInfo.InvariantCulture)
                };

                return statementFn(statement);
            }



            private async Task<JsonObject> FormBasicStatement(String verbId,
                                                              String verbDisplay,
                                                              JsonObject user,
                                                              String gameId,
                                                              String gameDisplay,
                                                              String registrationIdentifier,
                                                              String activityID,
                                                              String activityDescription,
                                                              Func<JsonObject, JsonObject> statementFn)
            {
                Func <JsonObject, JsonObject> setObjektFn = (s) =>
                {
                    s["object"]["id"] = activityID;
                    s["object"]["definition"]["name"]["en-US"] = activityDescription;
                    return statementFn(s);

                };
                JsonObject statement =  await FormBasicStatement(verbId,
                                                                 verbDisplay,
                                                                 user,
                                                                 gameId,
                                                                 gameDisplay,
                                                                 registrationIdentifier,
                                                                 setObjektFn);
                return statement;

            }


            private JsonObject FormActivity(String activityID,
                                            String activityDescription,
                                            JsonObject extension)
            {
                return new JsonObject
                {
                    ["id"] = activityID,
                    ["definition"] = new JsonObject
                    {
                        ["name"] = new JsonObject
                        {
                            ["en-US"] = activityDescription
                        },
                        ["extensions"] = extension
                    }
                };
            }

            private void DebugStatements(string statement, RestResponse response) {
                Debug.Log(statement);
                Debug.Log(response.Content);
                Debug.Log(response.ResponseStatus);

            }


            public void SendStartedStatement()
            {
                SendStatement(formVerbId("initialized"), "Initialized");

            }

            public void SendStartedStatement(String activityID,
                                             String activityDisplay)
            {
                SendStatement(formVerbId("initialized"),
                              "Initialized",
                              activityID,
                              activityDisplay);
            }

            public void SendCompletedStatement()
            {
                SendStatement(formVerbId("completed"), "Completed");
            }

            public void SendCompletedStatement(String activityID,
                                               String activityDisplay)
            {
                SendStatement(formVerbId("completed"),
                              "Completed",
                              activityID,
                              activityDisplay);
            }

            public async void SendStatement(String verbId,
                                            String verbDisplay)
            {
                Func <JsonObject, JsonObject> identity = (s) =>
                {
                    return s;
                };
                var statement = await FormBasicStatement(verbId,
                                                         verbDisplay,
                                                         user,
                                                         gameId,
                                                         gameDisplay,
                                                         registrationIdentifier,
                                                         identity);
                InvokeStatementSent(statement);
                var statementStr = statement.ToJsonString();
                var response = await sender.SendStatement(statementStr);
                // DebugStatements(statementStr, response);

            }

            public async void SendStatement(String verbId,
                                            String verbDisplay,
                                            Func <JsonObject, JsonObject> statementFn)
            {
                var statement = await FormBasicStatement(verbId,
                                                         verbDisplay,
                                                         user,
                                                         gameId,
                                                         gameDisplay,
                                                         registrationIdentifier,
                                                         statementFn);
                InvokeStatementSent(statement);
                var statementStr = statement.ToJsonString();
                var response = await sender.SendStatement(statementStr);
                // DebugStatements(statementStr, response);

            }

            public async void SendStatement(String verbId,
                                            String verbDisplay,
                                            String activityID,
                                            String activityDisplay)
            {
                Func <JsonObject, JsonObject> identity = (s) =>
                {
                    return s;
                };
                var statement = await FormBasicStatement(verbId,
                                                         verbDisplay,
                                                         user,
                                                         gameId,
                                                         gameDisplay,
                                                         registrationIdentifier,
                                                         activityID,
                                                         activityDisplay,
                                                         identity);
                InvokeStatementSent(statement);
                var statementStr = statement.ToJsonString();
                var response = await sender.SendStatement(statementStr);
                // DebugStatements(statementStr, response);

            }

            public async void SendStatement(String verbId,
                                            String verbDisplay,
                                            String activityID,
                                            String activityDisplay,
                                            Func <JsonObject, JsonObject> statementFn)
            {
                var statement = await FormBasicStatement(verbId,
                                                         verbDisplay,
                                                         user,
                                                         gameId,
                                                         gameDisplay,
                                                         registrationIdentifier,
                                                         activityID,
                                                         activityDisplay,
                                                         statementFn);
                InvokeStatementSent(statement);
                var statementStr = statement.ToJsonString();
                var response = await sender.SendStatement(statementStr);
                // DebugStatements(statementStr, response);

            }

            async void SendCustomStatement(String verbId,
                                           String verbDisplay,
                                           JsonObject user,
                                           String gameId,
                                           String gameDisplay,
                                           String registrationIdentifier)
            {
                Func <JsonObject, JsonObject> identity = (s) =>
                {
                    return s;
                };
                var statement = await FormBasicStatement(verbId,
                                                         verbDisplay,
                                                         user,
                                                         gameId,
                                                         gameDisplay,
                                                         registrationIdentifier,
                                                         identity);
                var statementStr = statement.ToJsonString();
                var response = await sender.SendStatement(statementStr);
                // DebugStatements(statementStr, response);

            }

            /*async void SendScratchStatement<TActor, TObjekt>(Statement<TActor, TObjekt> statement)
                where TActor: IActor
                where TObjekt: IObjekt
            {
                var statementStr = statement.ToJsonString();
                var response = await sender.SendStatement(statementStr);
            }*/
        }
    }
}
