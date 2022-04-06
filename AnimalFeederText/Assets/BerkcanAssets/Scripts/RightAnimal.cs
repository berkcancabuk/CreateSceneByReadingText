using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightAnimal : MonoBehaviour
{
    public GameObject RightPepper;
    public GameObject RightTrueTick;
    public static bool isPepper = false;
    public static bool RightAnimalFood = false;
    private void OnTriggerEnter(Collider other)
    {
        if ((Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 8 || Player.LevelValue == 9) && other.tag == "Player" && isPepper == false)
        {
            print("Right animal girdi mi ");
            RightPepper.SetActive(false);
            RightTrueTick.SetActive(true);
            //player.playerAnim.SetBool("Idle", false);
            //player.playerAnim.SetBool("Run", false);
            //player.playerAnim.SetBool("RunDrop", true);
            Player.isRun = false;
            RightAnimalFood = true;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            isPepper = true;
        }
        

    }
}
