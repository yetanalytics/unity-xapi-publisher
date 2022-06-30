using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;
using XAPI;
using Domain;

public class xApiIntegration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CreateStatement();
    }

    async void CreateStatement() {
        var creator = new Create();
        var statement = await creator.StartedStatement("mailto:user@example.com",
                                                       "John Doe",
                                                       "http://video.gms/pac-man",
                                                       "Pac-Man");
        var sender = new Sender("http://localhost:8080",
                                "username",
                                "password");
        var statementStr = statement.Serialize();
        var response = await sender.SendStatement(statementStr);
        print(statementStr);
        print(response.Content);
        print(response.ResponseStatus);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
