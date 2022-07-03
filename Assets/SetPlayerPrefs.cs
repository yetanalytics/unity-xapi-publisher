using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerPrefs : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        PlayerPrefs.SetString("LRSEmail","user@example.com");
        PlayerPrefs.SetString("LRSUsernameDisplay","John Doe");
        PlayerPrefs.SetString("LRSGameId", "http://video.games/button-clicker");
        PlayerPrefs.SetString("LRSGameDisplay", "Button Clicker");
    }
}
