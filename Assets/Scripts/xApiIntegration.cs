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
        publisher.SendStartedStatement();
    }

    public void OnButtonPress() {
        publisher.SendCompletedStatement();
    }

    void OnApplicationQuit() {
        publisher.SendStatement("http://video.games/verbs/quit", "Quit");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
