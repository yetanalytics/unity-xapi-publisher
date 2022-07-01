using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System;
using UnityEngine;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;
using XAPI;
using Domain;

public class xApiIntegration : MonoBehaviour
{
    // LRS Credentials
    public string lrsUrl;
    public string lrsKey;
    public string lrsSecret;

    // Metadata Variables
    public string email;
    public string uname;
    public string gameId;
    public string gameName;

    private Creator creator{get {return new Creator();}}
    private Sender sender{get {return new Sender(lrsUrl,lrsKey,lrsSecret);}}


    // Start is called before the first frame update
    void Start() {
        SendStartedStatement();
    }

    public void OnButtonPress() {
        SendCompletedStatement();
    }

    async void SendStartedStatement() {
        var statement = await creator.StartedStatement(String.Format("mailto:{0}",email),
                                                       uname,
                                                       gameId,
                                                       gameName);
        var statementStr = statement.Serialize();
        var response = await sender.SendStatement(statementStr);
        print(statementStr);
        print(response.Content);
        print(response.ResponseStatus);
    }

    async void SendCompletedStatement() {
        var statement = await creator.CompletedStatement(String.Format("mailto:{0}",email),
                                                         uname,
                                                         gameId,
                                                         gameName);
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