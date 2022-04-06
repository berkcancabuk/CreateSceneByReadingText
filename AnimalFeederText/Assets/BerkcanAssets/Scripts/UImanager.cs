using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UImanager : MonoBehaviour
{
    
    [SerializeField] public GameObject soil, basket;
    [SerializeField] TextMeshProUGUI TextlevelValue, TextLevel;
    int ActiveSceneValue;
    [SerializeField] public GameObject taptorestart;
    [SerializeField] public GameObject failLevel;
    [SerializeField] public GameObject TapToContinue;
    [SerializeField] public GameObject LevelComplate;
    [SerializeField] public Player player;
    [SerializeField] public GameObject CMCowCam, CMCow2Cam, CMvcam1;
    // Start is called before the first frame update
    void Start()
    {
        GameObject playergameobject = GameObject.Find("Player");
        player = playergameobject.GetComponent<Player>();
        ActiveSceneValue = SceneManager.GetActiveScene().buildIndex+1;
        TextlevelValue.text = "" + ActiveSceneValue;
        TextLevel.text = "LEVEL ";
        //EventManager.CheckPlayer += checkplayer;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    //public void checkplayer()
    //{
    //    GameObject playergameobject = GameObject.Find("Player");
    //    player = playergameobject.GetComponent<Player>();
    //}
    public void TryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //StartCoroutine(checkGameManagers());
        CMvcam1.SetActive(true);
        CMCowCam.SetActive(false);
        failLevel.SetActive(false);
        player.isDead = false;
        player.OneMoreLoop = false;
        player.OneMoreLoopForTwoAnimals = false;
        Player.isPepper1 = false;
        Player.isPepper2 = false;
        Player.isRun = true;
        RightAnimal.isPepper = false;
        RightAnimal.RightAnimalFood = false;
        LeftAnimal.LeftAnimalFood = false;
        taptorestart.SetActive(false);
        player.didItPullAfterTheFirstTouch = false;
        player.isTouchActive = true;
        //player.isNextLevel = true;
        LevelComplate.SetActive(false);
        TapToContinue.SetActive(false);
        soil.SetActive(false);
        basket.SetActive(false);
        player.UI_manager.failLevel.SetActive(false);
        player.UI_manager.taptorestart.SetActive(false);
        if (Player.LevelValue ==1 || Player.LevelValue == 8)
        {
            player.transform.localPosition = new Vector3(0, -1.276707f, -23.05456f);
        }
        if (Player.LevelValue == 2 || Player.LevelValue == 3 || Player.LevelValue == 4 || Player.LevelValue == 5 || Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 9)
        {
            player.transform.localPosition = new Vector3(0.4446331f, -2.697414f, -46.97912f);
        }
        if (Player.LevelValue == 8)
        {
            player.transform.localPosition = new Vector3(-0.223274f, -1.487283f, -23.50282f);
        }
        player.playerAnim.SetBool("Idle", true); player.playerAnim.SetBool("Run", false); player.playerAnim.SetBool("RunDrop", false);
        player.plantOnHandPlantPlayer.SetActive(false); Player.isRun = true;player.plantOnHandRaddishPlayer.SetActive(false); 
        player.plantOnHandBeetRootPlayer.SetActive(false); player.plantOnHandTomatoPlayer.SetActive(false);player.plantOnHandLemonPlayer.SetActive(false);


    }
    public void NextLevelButton()
    {
        EventManager.RemoveList();
        Player.LevelValue++;
        
        EventManager.NextLevel();
        
        //SceneManager.LoadScene(Player.LevelValue-1);
        //ActiveSceneValue = Player.LevelValue;
        //PlayerPrefs.SetInt("SavedScene", Player.LevelValue);
        print(Player.LevelValue +"level value");
        TextlevelValue.text = "" + ActiveSceneValue;
        //StartCoroutine(checkGameManagers());
        CMvcam1.SetActive(true);
        CMCowCam.SetActive(false);
        player.isDead = false;
        player.OneMoreLoop = false;
        player.OneMoreLoopForTwoAnimals = false;
        player.didItPullAfterTheFirstTouch = false;
        Player.isPepper1 = false;
        Player.isPepper2 = false;
        Player.isRun = true;
        RightAnimal.isPepper = false;
        RightAnimal.RightAnimalFood = false;
        LeftAnimal.LeftAnimalFood = false;
        LevelComplate.SetActive(false);
        TapToContinue.SetActive(false);
        soil.SetActive(false);
        basket.SetActive(false);
        if (Player.LevelValue != 8)
        {
            player.transform.position = new Vector3(0, 0, 0);
        }
        else if (Player.LevelValue == 8)
        {
            player.transform.position = new Vector3(0, 0, 0);
        }
        player.playerAnim.SetBool("Idle", true); player.playerAnim.SetBool("Run", false); player.playerAnim.SetBool("RunDrop", false); player.playerAnim.SetBool("StandToSit", false);
        player.plantOnHandPlantPlayer.SetActive(false);
        player.plantOnHandRaddishPlayer.SetActive(false);
        player.plantOnHandBeetRootPlayer.SetActive(false);
        Player.isRun = true;
        player.isTouchActive = true;
        player.plantOnHandPlantPlayer.SetActive(false); Player.isRun = true; player.plantOnHandRaddishPlayer.SetActive(false);
        player.plantOnHandBeetRootPlayer.SetActive(false); player.plantOnHandTomatoPlayer.SetActive(false); player.plantOnHandLemonPlayer.SetActive(false);
    }
    //IEnumerator checkGameManagers()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    //player.CheckGameManager();
    //}
}
