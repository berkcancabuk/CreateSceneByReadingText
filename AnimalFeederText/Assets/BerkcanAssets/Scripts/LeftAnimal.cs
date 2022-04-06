using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftAnimal : MonoBehaviour
{
    public GameObject LeftPepper;
    public GameObject LeftTrueTick;
    public static bool LeftAnimalFood = false;
    private void OnTriggerEnter(Collider other)
    {
        if ((Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 8 || Player.LevelValue == 9 || Player.LevelValue == 10) && other.tag == "Player" && RightAnimal.isPepper == false)
        {
            print("left animal girdi mi ");
            LeftTrueTick.SetActive(true);
            LeftPepper.SetActive(false);
            //Player.playerAnim.SetBool("Idle", false);
            //Player.playerAnim.SetBool("Run", false);
            //Player.playerAnim.SetBool("RunDrop", true);
            //GameManager.TakeObjectEffect.SetActive(true);
            Player.isRun = false;
            LeftAnimalFood = true;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            RightAnimal.isPepper = true;

        }
    }
}
