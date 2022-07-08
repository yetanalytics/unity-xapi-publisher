using UnityEngine;
using System;

public class SetPlayerPrefs : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        // I'm guessing that some login flow happens
        // Data then gets stored inside the session, i.e. PlayerPrefs
        // We'll just have some variables we instruct whoever uses this
        // to populate with the userdata.
        PlayerPrefs.SetString("LRSEmail","user@example.com");
        PlayerPrefs.SetString("LRSUsernameDisplay","John Doe");

        // As for game metadata I'm guessing this will probably be static and held inside the session also.
        // again all is subject to change.
        PlayerPrefs.SetString("LRSGameId", "http://video.games/button-clicker");
        PlayerPrefs.SetString("LRSGameDisplay", "Button Clicker");
        PlayerPrefs.SetString("LRSSessionIdentifier",Guid.NewGuid().ToString());
        PlayerPrefs.Save();

    }
}
