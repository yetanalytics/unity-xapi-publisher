using System;
using System.Collections;
using System.Dynamic;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using XAPI;
using XAPI.Metadata;

public class StatementTest
{
    [Test]
    public void StatementTimestampSet()
    {
        var statement = new Statement<Agent, Activity>();
        statement.Serialize();
        Assert.IsInstanceOf<String>(statement.timestamp);
    }

    [Test]
    public void StatementActorSet()
    {
        var statement = new Statement<Agent, Activity>(){
            actor = new Agent () {
                name = "Luke Skywalker",
                mbox = "mailto:luke@rebellion.gov"
            }
        };
        Assert.AreSame(statement.actor.name, "Luke Skywalker");
        Assert.AreSame(statement.actor.mbox, "mailto:luke@rebellion.gov");

    }

    [Test]
    public void StatementVerbSet()
    {
        var statement = new Statement<Agent, Activity>(){
            verb = new Verb () {
                id = "http://the.force/verbs/push",
                display = new LanguageMap {
                    enUS = "Pushed"
                }
            }
        };
        Assert.AreSame(statement.verb.id, "http://the.force/verbs/push");
        Assert.AreSame(statement.verb.display.enUS, "Pushed");
    }

    [Test]
    public void StatementObjectSet()
    {
        var statement = new Statement<Agent, Activity>(){
            objekt = new Activity () {
                id = "http://star.wars/activities/battle-of-hoth",
                definition = new ActivityDefinition {
                    description = new LanguageMap {
                        enUS = "Battle at the beginning of Star Wars Episode V: The Empire Strikes Back"
                    }
                }
            }
        };
        Assert.AreSame(statement.objekt.id, "http://star.wars/activities/battle-of-hoth");
        Assert.AreSame(statement.objekt.definition.description.enUS, "Battle at the beginning of Star Wars Episode V: The Empire Strikes Back");
    }
    
    [Test]
    public void StatementContextSet()
    {

        var extension = new Extension () {
            location = new Location() {
                region = "US"
            }
        };
        
        var statement = new Statement<Agent, Activity>(){
            context = new Context {
                extensions = extension
            }
        };
        Assert.AreSame(statement.context.extensions.location.region,"US");
    }

    [Test]
    public void StatementAuthoritySet()
    {
        var statement = new Statement<Agent, Activity>(){
            authority = new Agent {
                mbox = "authority@example.com"
            }
        };

        Assert.AreSame(statement.authority.mbox, "authority@example.com");
    }

    [Test]
    public void StatementAttachmentsSet()
    {
        var attachmentList = new List<Attachment>();
        attachmentList.Add(
            new Attachment{
                description = new LanguageMap {
                    enUS = "Description goes here"
                }
            }
        );

        var statement = new Statement<Agent, Activity>(){
            attachments = attachmentList
        };

        Assert.AreSame(statement.attachments[0].description.enUS, "Description goes here");
    }

    [Test]
    public void StatementActorAsGroupSet() 
    {
        var group = new Group
        {
            name = "testGroup"
        };

        var statement = new Statement<Group, Activity>{
            actor = group
        };

        Assert.AreSame(group, statement.actor);
    }

    [Test]
    public void StatementActivityAsAgentSet() 
    {
        var agent = new Agent
        {
            mbox = "mailto:user@example.com"
        };

        var statement = new Statement<Agent, Agent>{
            objekt = agent
        };

        Assert.AreSame(agent, statement.objekt);
    }

    [Test]
    public void StatementActivityAsGroupSet() 
    {
        var group = new Group
        {
            name = "testGroup"
        };

        var statement = new Statement<Group, Group>{
            objekt = group
        };

        Assert.AreSame(group, statement.objekt);
    }

    [Test]
    public void StatementActivityAsStatementReferenceSet() 
    {
        var sr = new StatementReference{};

        var statement = new Statement<Group, StatementReference>{
            objekt = sr
        };

        Assert.AreSame(sr, statement.objekt);
    }
}
