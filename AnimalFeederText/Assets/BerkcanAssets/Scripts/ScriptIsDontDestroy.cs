using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptIsDontDestroy : MonoBehaviour
{
    public static ScriptIsDontDestroy instance = null;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
    }
}
