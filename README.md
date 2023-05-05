# Unity xAPI Publisher

This is a unity package that aims to simplify the process of taking a unity application and integrating it with an LRS.

## Before You Start...


### Unity Version

This package was developed with Unity version 2021.3.4f1 (has not been tested with earlier versions as of yet). For best results use 2021.3.4f1 or later (for earlier versions of Unity, mileage may vary).

### .NET 4.x

Since these packages have upstream dependencies that depend on some tooling from .NET 4.x and not .NET Standard, some steps need to be taken to configure your unity project to pull from .NET 4.x.
1. Inside your Unity Project, go ahead and select the `Edit > Project Settings`
2. Inside the Project Settings window, navigate to `Player`. Open the `Other Settings` dropdown.
3. Under `Configuration`, there's an option that says API Compatibility Level. Make sure that is set to .NET Framework (or .NET 4.x if you're using an earlier version of Unity.)
4. Once this is all finished, you're good to continue. If you'd like to read up on some of the features .NET 4.x comes with, feel free to browse Microsoft's [documentation](https://docs.microsoft.com/en-us/visualstudio/gamedev/unity/unity-scripting-upgrade) on switching from .NET Standard to .NET 4.x for Unity scripting.


## Installation

### Use the Unity Package Manager to download the URL.

1. In the Unity editor, navigate to the Package Manager via `Window > Package Manager`
2. Select the + icon in the upper right hand side and click `Add package from git URL...`
3. In the text box enter the following:
 - if you are using https: https://github.com/yetanalytics/unity-xapi-publisher.git
 - if you are using ssh: git@github.com:yetanalytics/unity-xapi-publisher.git
4. hit the Add button. This will download the URL and dump the contents in Unity's package cache. It will be accessible in your project inspector via Packages/unity-xapi-publisher.

*Please Note that if you import the file and there's an exception, it is likely that your project is referring to the incorrect .NET version. Please go to the `.NET 4.x` for steps on how to fix this.

## Implementation

### Setting the State.

The assumption here is that user identity is managed via some other integration. Whether that's coming from a local database or a service doesn't matter. The user ID needs to persist in the application (in the form of an email). The actual plugin pulls these values out of PlayerPrefs, So for the sake of an example, I've made a file that populates the proper `PlayerPref` keys with the values we expect. How user identity is retrieved will vary, so that part is left to the developer. The important thing to know is the values need to be saved to the following keys in PlayerPrefs:

```csharp
using UnityEngine;
using System;

public class SetPlayerPrefs : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        // Assume here some user identification resolution happens
        // Data then needs to get stored in a persistent session on the client side, i.e. PlayerPrefs

        // User Identity Data

        // set Account and homepage for a more anonymous and contained identity:
        PlayerPrefs.SetString("LRSAccountId","123456789");
        PlayerPrefs.SetString("LRSHomepage","https://www.yetanalytics.com");

        // Or if you prefer, Email can be set the following way:
        PlayerPrefs.SetString("LRSEmail","user@example.com");

        PlayerPrefs.SetString("LRSUsernameDisplay","John Doe");

        // Game Identity Data
        // This sets the Platform of the statement under context.platform
        // by default, the object.id (aka the Activity) will also use this as it's identifier:
        PlayerPrefs.SetString("LRSGameId", "http://video.games/button-clicker");
        PlayerPrefs.SetString("LRSGameDisplay", "Button Clicker");

        // In addition to LRSGameId, Set the following if you'd like to override the ActivityId via playerPrefs.
        // Note that both LRSActivityId and LRSActivityDefinition need to be set in order for this to work:
        PlayerPrefs.SetString("LRSActivityId", "http://video.games/button-clicker/level/1");
        PlayerPrefs.SetString("LRSActivityDefinition", "Level 1 of button-clicker");


        // Session Identity Data
        PlayerPrefs.SetString("LRSSessionIdentifier",Guid.NewGuid().ToString());

        // Location Data
        // Add this to the PlayerPrefs if you wish to get user location data
        PlayerPrefs.SetString("LRSEnableUserLocation", "true");

        PlayerPrefs.Save();

    }
}
```
### Session Variable Documentation

|Variable Name | Description | Statement Fields Populated |
| -----------  | ----------- | ----------- |
|LRSEnableUserLocation | This enables the external calls required to get user regional information packaged with the statement. | `$.context.extensions.http://ip-api.com/location`|
|LRSEmail      | User ID in the form of an email. Follows the [RFC 3987](https://datatracker.ietf.org/doc/html/rfc3987) specification.| `$.actor.mbox`|
|LRSAccountId  | A User ID that's contained within a system. (Requires LRSHomepage to be set).| `$.actor.account.name`|
|LRSHomepage   | A homepage for the LRSAccountId (Requires LRSAccountId to be set).| `$.actor.account.homePage`|
|LRSUsernameDisplay | A human readable username display.| `$.actor.name` |
|LRSGameId | A Game ID in the form of an IRI. Follows the [RFC 3987](https://datatracker.ietf.org/doc/html/rfc3987) specification.| `$.object.id`, `$.context.platform`|
|LRSGameDisplay| A human readable display of the game being played.| `$.object.definition.name.en-US`|
|LRSSessionIdentifier| A UUID that uniquely identifies the session being engaged with. This is something that would get set whenever the user initializes a new session.| `$.context.registration`|
|LRSActivityId| Optional ActivityID in the form of an IRI. Follows the [RFC 3987](https://datatracker.ietf.org/doc/html/rfc3987) specification.| `$.object.id`|
|LRSActivityDefinition| human readable activity definition display. | `$.object.definition.name.en-US`|

### Setting up a Scene

Once that state is all setup let's go ahead and setup a scene with the xAPI Publisher. In the package I provided an example file that calls the publisher. Let's set this up in our scene now.

1. Right click on your project hierarchy and create an empty object, You can call it XapiPublisher or anything that fits alongside the hierarchy of your application.
2. Now select the empty object, and in the inspector click on Add Component
3. Navigate to Scripts > X Api Integration. (or if you'd like to directly reference this script it is located at `Packages/unity-xapi-example/Examples/xApiIntegration.cs`)
4. Populate the following variables on the script in the inspector to configure it to talk to the LRS.

### LRS Publisher Configuration Documentation

|Variable Name | Description |
| -----------  | ----------- |
|LRSUrl        | URL of whatever LRS you have setup. |
|LRSKey        | Basic Auth username. Follows the [RFC 7617](https://datatracker.ietf.org/doc/html/rfc7617) specification.|
|LRSSecret     | Basic AUth password. Follows the [RFC 7617](https://datatracker.ietf.org/doc/html/rfc7617) specification.|

5. Once those have been set up, go ahead and give it a test run (just hit play) to see if it works. You should see some stuff in the debug console (I'll configure this to be silent at runtime). Check the LRS to see if the statement with the verb is coming through. If it is, congratulations you have a working integration with an LRS!

## Working with the plugin

Now that we have a working let's discuss the XapiIntegration script is actually calling and how we can utilize it.

### The Example File

```csharp
using UnityEngine;
using LRS.Domain;

public class xApiIntegration : MonoBehaviour
{
    public xApiIntegration() {

    }
    // LRS Credentials
    public string lrsUrl;
    public string lrsKey;
    public string lrsSecret;

    private Publisher publisher{get {return new Publisher(lrsUrl,lrsKey,lrsSecret);}}


    // Start is called before the first frame update
    void Start() {
        // by default, SendStartedStatement can be called with no arguments and uses the configuration from PlayerPrefs to populate it's statements
        // Note that you can set LRSActivityId and LRSActivityDefinition to override the default activity (which is LRSGameId).
        publisher.SendStartedStatement();

        // you can overload SendStartedStatement with a custom ActivityID if you wish to dynamically modify the activity.
        // this overrides $.object.id and $.object.definition.name.en-US):
        publisher.SendStartedStatement("http://video.games/clicker/level/1", "Level 1 of clicking game");
    }

    // Example of something we can call externally via some object callback (Like a GUI button)
    public void OnButtonPress() {
        // similarly, with SendCompletedStatement...
        publisher.SendCompletedStatement();

        // or with overrides...
        publisher.SendCompletedStatement("http://video.games/clicker/level/1", "Level 1 of clicking game");
    }

    void OnApplicationQuit() {
        // Example of sending a statement only configuring the verb.
        // overrides $.verb.id and $.verb.display.en-US
        publisher.SendStatement("http://video.games/verbs/quit", "Quit");

        // Or if you wish to send both a custom verb and activity...
        // overrides $.verb.id, $.verb.display.en-US, $.object.id, and $.object.definition.name.en-US
        publisher.SendStatement("http://video.games/verbs/quit", "Quit", "http://video.games/clicker/level/1", "Level 1 of clicking game");

    }
}
```

Please note that SendStartedStatement and SendCompletedStatement are both firing off statements with predetermined verb IRIs. Started is http://adlnet.gov/expapi/verbs/initialized and Completed http://adlnet.gov/expapi/verbs/completed .

In the OnApplicationQuit call, I showed an example of how to call the publisher with a custom verb. OnApplicationQuit is a hook that will fire when the user quits the application. If you go ahead and attempt to close out of the player you'll see it fire off stuff again.

There's also a way to make fully custom statements but I'm still trying to figure out the best way to make it clear on how to construct the statements from scratch and what gets included in them.


### Technical mumbo jumbo

Keep in mind that these are Async calls, they are spun up on different threads via Microsoft's Task Asynchronous Programming Model. So whatever data is managed can never be returned to the main thread, but if you want to inspect the results of each call you need to wrap whatever your caller is in an Async method. You can read up more on this via Microsoft's [documentation](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/)

### External Service Calls

So there's a side effect in the package async stack. We populate the `context.extensions` with information about the users location. This is done by first getting the hardware's IP (a ping to canhazip.com) , then a call is made to http://ip-api.com where the associated metadata is retrieved and serialized into our statement. This location data is then cached on the Publisher client so it doesn't need to make more calls then necessary.
