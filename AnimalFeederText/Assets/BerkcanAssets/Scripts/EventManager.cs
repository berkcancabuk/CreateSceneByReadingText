using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OurEventDelegate();
    public static  OurEventDelegate CallColorAlpha;
    public static OurEventDelegate CowTickCowEffect;
    public static OurEventDelegate PlayerOnHand;
    public static OurEventDelegate hitPlayerBox;
    public static OurEventDelegate CowAnimSpeed;
    public static OurEventDelegate AnimalHeadChangeColor;
    public static OurEventDelegate AnimalAnim;
    public static OurEventDelegate DeerAnimal;
    public static OurEventDelegate NextLevel;
    public static OurEventDelegate RemoveList;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
   

}
