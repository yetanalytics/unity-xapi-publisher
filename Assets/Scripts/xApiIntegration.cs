using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System;
using System.Diagnostics;
using UnityEngine;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;
using XAPI;
using Domain;

public class xApiIntegration : MonoBehaviour
{
    public xApiIntegration() {

    }
    // LRS Credentials
    public string lrsUrl;
    public string lrsKey;
    public string lrsSecret;

    // Metadata Variables
    public string emailPref = "LRSEmail";
    public string nameDisplayPref = "LRSUsernameDisplay";
    public string gameIdPref = "LRSGameId";
    public string gameDisplayPref = "LRSGameDisplay";

    private Creator creator{get {return new Creator();}}
    private Sender sender{get {return new Sender(lrsUrl,lrsKey,lrsSecret);}}
    public static readonly String REGISTRATION_IDENTIFIER = Guid.NewGuid().ToString();
    private String registrationIdentifier {get {return REGISTRATION_IDENTIFIER;}}


    // Start is called before the first frame update
    void Start() {
        SendStartedStatement();
    }

    public void OnButtonPress() {
        SendCompletedStatement();
    }

    void onQuit() {
    }

    async void SendStartedStatement() {
        var statement = await creator.StartedStatement(String.Format("mailto:{0}",PlayerPrefs.GetString(emailPref)),
                                                                                  PlayerPrefs.GetString(nameDisplayPref),
                                                                                  PlayerPrefs.GetString(gameIdPref),
                                                                                  PlayerPrefs.GetString(gameDisplayPref),
                                                                                  registrationIdentifier);
        var statementStr = statement.Serialize();
        var response = await sender.SendStatement(statementStr);
        print(statementStr);
        print(response.Content);
        print(response.ResponseStatus);
    }

    async void SendCompletedStatement() {
        var statement = await creator.CompletedStatement(String.Format("mailto:{0}",PlayerPrefs.GetString(emailPref)),
                                                                                    PlayerPrefs.GetString(nameDisplayPref),
                                                                                    PlayerPrefs.GetString(gameIdPref),
                                                                                    PlayerPrefs.GetString(gameDisplayPref),
                                                                                    registrationIdentifier);
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
