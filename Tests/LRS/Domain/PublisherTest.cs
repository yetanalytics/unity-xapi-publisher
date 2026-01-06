using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LRS.Domain;
using System.Text.Json.Nodes;

public class Hook
{

    public List<JsonObject> results = new List<JsonObject>();

    // function that fires every time a statement is sent off.
    private void OnStatementSent(JsonObject statement)
    {
        results.Add(statement);
    }

    // Unity boilerplate for adding hooks to handlers
    public Hook()
    {
        Publisher.OnStatementSent += OnStatementSent;
    }

    // cleanup
    ~Hook()
    {
        Publisher.OnStatementSent -= OnStatementSent;
    }

}

public class PublisherTest
{

    string LRSAccountId = "123456";
    string LRSHomepage = "https://www.yetanalytics.com";
    string LRSUsernameDisplay = "John Doe"; 
    string LRSGameId = "http://video.games/button-clicker";
    string LRSGameDisplay = "Button Clicker";
    string LRSActivityId = "http://video.games/button-clicker/level/1";
    string LRSActivityDefinition = "Level 1 of button-clicker";
    string LRSSessionIdentifier = Guid.NewGuid().ToString();


    [SetUp]
    public void SetUp()
    {
        // set Account and homepage for a more anonymous and contained identity:
        PlayerPrefs.SetString("LRSAccountId",LRSAccountId);
        PlayerPrefs.SetString("LRSHomepage",LRSHomepage);

        // Or if you prefer, Email can be set the following way:
        //PlayerPrefs.SetString("LRSEmail","user@example.com");

        PlayerPrefs.SetString("LRSUsernameDisplay",LRSUsernameDisplay);

        // Game Identity Data
        // This sets the Platform of the statement under context.platform
        // by default, the object.id (aka the Activity) will also use this as it's identifier:
        PlayerPrefs.SetString("LRSGameId", LRSGameId);
        PlayerPrefs.SetString("LRSGameDisplay", LRSGameDisplay);

        // In addition to LRSGameId, Set the following if you'd like to override the ActivityId via playerPrefs.
        // Note that both LRSActivityId and LRSActivityDefinition need to be set in order for this to work:
        PlayerPrefs.SetString("LRSActivityId", LRSActivityId);
        PlayerPrefs.SetString("LRSActivityDefinition", LRSActivityDefinition);


        // Session Identity Data
        PlayerPrefs.SetString("LRSSessionIdentifier",LRSSessionIdentifier);

        // Location Data
        // Add this to the PlayerPrefs if you wish to get user location data
        // PlayerPrefs.SetString("LRSEnableUserLocation", "true");
    }

    [Test]
    public void SendStartedStatementBehaves()
    {
        Hook hook = new Hook();
        Publisher publisher = new Publisher("http://example.com/LRSUrl","LRSKey","LRSSecret");
        publisher.SendStartedStatement();
        JsonObject statement = hook.results.First();

        Assert.AreEqual(LRSHomepage, statement["actor"]["account"]["homePage"].ToString());
        Assert.AreEqual(LRSAccountId, statement["actor"]["account"]["name"].ToString());
        Assert.AreEqual("http://adlnet.gov/expapi/verbs/initialized", statement["verb"]["id"].ToString());
        Assert.AreEqual(LRSActivityId, statement["object"]["id"].ToString());
        Assert.AreEqual("", statement["object"]["definition"]["extensions"]["https://docs.unity3d.com/ScriptReference/XR.XRSettings.html"]["loadedDeviceName"].ToString());
        Assert.AreEqual("false", statement["object"]["definition"]["extensions"]["https://docs.unity3d.com/ScriptReference/XR.XRDisplaySubsystem.html"]["running"].ToString());
        Assert.AreEqual(LRSSessionIdentifier, statement["context"]["registration"].ToString());
        Assert.AreEqual(LRSGameId, statement["context"]["platform"].ToString());
        Assert.AreEqual("LinuxEditor", statement["context"]["extensions"]["https://docs.unity3d.com/ScriptReference/Application-platform.html"]["platform"].ToString());
        Assert.IsInstanceOf<string>(statement["timestamp"].ToString());
    }

    [Test]
    public void SendStatement_ModifiedVerbs()
    {
        string verbId = "http://video.games/verbs/quit";
        string verbName = "Quit";
        
        Hook hook = new Hook();
        Publisher publisher = new Publisher("http://example.com/LRSUrl","LRSKey","LRSSecret");
        publisher.SendStatement(verbId, verbName);
        JsonObject statement = hook.results.First();

        Assert.AreEqual(verbId, statement["verb"]["id"].ToString());
        Assert.AreEqual(verbName, statement["verb"]["display"]["en-US"].ToString());   
    }

    [Test]
    public void SendStatement_ModifiedVerbsWithAdditionalChanges()
    {
        string verbId = "http://video.games/verbs/quit";
        string verbName = "Quit";
        string publisherName = "ACME Games Corp.";

        Func <JsonObject, JsonObject> modifyFn = (statement) =>
        {
            // modify the statement
            statement["object"]["definition"]["extensions"]["https://video.games/publisher"] = new JsonObject{ ["name"] = publisherName };

            // make sure to return for the callback
            return statement;
        };
        
        Hook hook = new Hook();
        Publisher publisher = new Publisher("http://example.com/LRSUrl","LRSKey","LRSSecret");
        publisher.SendStatement(verbId, verbName, modifyFn);
        JsonObject statement = hook.results.First();

        Assert.AreEqual(verbId, statement["verb"]["id"].ToString());
        Assert.AreEqual(verbName, statement["verb"]["display"]["en-US"].ToString());
        Assert.AreEqual(publisherName, statement["object"]["definition"]["extensions"]["https://video.games/publisher"]["name"].ToString());
    }

    [Test]
    public void SendStatement_ModifiedVerbsActivities()
    {
        string verbId = "http://video.games/verbs/quit";
        string verbName = "Quit";
        string activityId = "http://video.games/pong";
        string activityName = "Pong";
        
        Hook hook = new Hook();
        Publisher publisher = new Publisher("http://example.com/LRSUrl","LRSKey","LRSSecret");
        publisher.SendStatement(verbId, verbName, activityId, activityName);
        JsonObject statement = hook.results.First();

        Assert.AreEqual(verbId, statement["verb"]["id"].ToString());
        Assert.AreEqual(verbName, statement["verb"]["display"]["en-US"].ToString());
        Assert.AreEqual(activityId, statement["object"]["id"].ToString());
        Assert.AreEqual(activityName, statement["object"]["definition"]["name"]["en-US"].ToString());   
    }

    [Test]
    public void SendStatement_ModifiedVerbsActivitiesWithAdditionalChanges()
    {
        string verbId = "http://video.games/verbs/quit";
        string verbName = "Quit";
        string activityId = "http://video.games/pong";
        string activityName = "Pong";
        string publisherName = "ACME Games Corp.";

        Func <JsonObject, JsonObject> modifyFn = (statement) =>
        {
            // modify the statement
            statement["object"]["definition"]["extensions"]["https://video.games/publisher"] = new JsonObject{ ["name"] = publisherName };

            // make sure to return for the callback
            return statement;
        };
        
        Hook hook = new Hook();
        Publisher publisher = new Publisher("http://example.com/LRSUrl","LRSKey","LRSSecret");
        publisher.SendStatement(verbId, verbName, activityId, activityName, modifyFn);
        JsonObject statement = hook.results.First();

        Assert.AreEqual(verbId, statement["verb"]["id"].ToString());
        Assert.AreEqual(verbName, statement["verb"]["display"]["en-US"].ToString());
        Assert.AreEqual(activityId, statement["object"]["id"].ToString());
        Assert.AreEqual(activityName, statement["object"]["definition"]["name"]["en-US"].ToString()); 
        Assert.AreEqual(publisherName, statement["object"]["definition"]["extensions"]["https://video.games/publisher"]["name"].ToString());
    }
}