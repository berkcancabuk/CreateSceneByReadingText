using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefInitializer : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerPrefs.HasKey("SavedScene"))
        {
            PlayerPrefs.SetInt("SavedScene", 1);

        }
    }
}
