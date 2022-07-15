using UnityEngine;
using System;

public class SetPlayerPrefs : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        // I'm guessing that some user identification resolution happens
        // Data then needs to get stored in a persistent session on the client side, i.e. PlayerPrefs
        PlayerPrefs.SetString("LRSEmail","user@example.com");
        PlayerPrefs.SetString("LRSUsernameDisplay","John Doe");

        // As for game metadata I'm guessing this will probably be static and held inside the session also.
        // again all is subject to change.
        PlayerPrefs.SetString("LRSGameId", "http://video.games/button-clicker");
        PlayerPrefs.SetString("LRSGameDisplay", "Button Clicker");

        // session identifier
        PlayerPrefs.SetString("LRSSessionIdentifier",Guid.NewGuid().ToString());
        PlayerPrefs.Save();

    }
}
