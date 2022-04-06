using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    
    [SerializeField] public Player player;
    
    //public static GameObject trueTick, CowTick,trueTick2;
    
    
    [SerializeField] public static GameObject pepper1, pepper2;
    
    
    //[SerializeField] public GameObject CMCowCam, CMCow2Cam;
    
    // Start is called before the first frame update
    void Start()
    {
        
        //StartCoroutine(ColorAlphaDown1());
        
        player.OneMoreLoop = false;
        //CMCowCam = GameObject.Find("Level2CowCam");
        //CMCowCam.GetComponent<GameObject>();
        
       
        
        
       
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
   
    
    




    
    
    
    
}
