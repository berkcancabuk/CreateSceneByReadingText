using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFruitScript : MonoBehaviour
{
    [SerializeField] public GameObject TakeObjectEffect, trueTick,plant;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.hitPlayerBox += Plant;
        EventManager.PlayerOnHand += playerFruitOnHand;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void playerFruitOnHand()
    {
        TakeObjectEffect.SetActive(false);
    }
    public void Plant()
    {
        plant.SetActive(false);
        trueTick.SetActive(true);
        TakeObjectEffect.SetActive(true);
    }
}
