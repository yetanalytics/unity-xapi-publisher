using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;
using XAPI;

public class xApiIntegration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //SendStatement();
        //SendForLocation();
        //var statement = new Statement<Location>();
        dynamic location = new ExpandoObject();
        location.City = "New York";
        var statement = new Statement<Agent,Agent> {
            actor = new Agent{
                name = "Obiwan Kenobi"
            },
            verb = new Verb{
                id = "http://starwars.vocab/kicked",
                display = new LanguageMap {
                    enUS = "kicked butt"
                }
            },
            objekt = new Agent{
                name = "Obiwan Kenobi"
            }
        };
        //print(statement.Serialize());
        var other_statement = new Statement<Agent,Activity> {
            actor = new Agent{
                mbox = "obiwan@starwars.co",
                name = "Obiwan Kenobi"
            },
            verb = new Verb{
                id = "http://starwars.vocab/kicked",
                display = new LanguageMap {
                    enUS = "kicked butt"
                }
            },
            objekt = new Activity {
                id = "http://starwars.vocab/butt",
                definition = new ActivityDefinition {
                    name = new LanguageMap {
                        enUS = "Bill Buttlicker"
                    },
                    description = new LanguageMap {
                        enUS = "Some Guy who likes to lick butt"
                    }
                }
            }
        };
        print(other_statement.Serialize());
        print(GetTimestamp(DateTime.Now));
    }

    async void SendStatement() {
        var response = await PrepareStatement();
        print(response.Content);
    }

    //async void SendForLocation() {
    //    dynamic response = await PrepareLocationRequest();
    //    print(response.country);
    //}

    async Task<RestResponse> PrepareStatement() {
        var client = new RestClient("http://localhost:8080") {
            Authenticator = new HttpBasicAuthenticator("username","password")
        };
        var request = new RestRequest("/xapi/about");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("X-Experience-API-Version", "1.0.1");
        return await client.GetAsync(request);

    }

    async Task<RestResponse> GetIp() {
        var client = new RestClient("http://canhazip.com");
        var request = new RestRequest("");
        return await client.GetAsync(request);

    }

    async Task<ExpandoObject> GetLocation() {
        var response = await GetIp();
        var ip = response.Content;
        var client = new RestClient("http://ip-api.com");
        return await client.GetJsonAsync<ExpandoObject>(string.Format("json/{0}", ip));
    }

    // Update is called once per frame
    void Update()
    {
    }
}
