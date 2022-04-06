using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowControl : MonoBehaviour
{
    public GameObject CowTick, trueTick2;
    [SerializeField] public Animator CowAnim2;
   
    [SerializeField] public GameObject cowThink1, cowThink2, cowThink3, cowThink4, cowThink5;
    [SerializeField] public GameObject cowThink6, cowThink7, cowThink8, cowThink9, cowThink10;
    [SerializeField] public GameObject CowEffect;
    [SerializeField] public Animator CowAnim;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.CowTickCowEffect += CowEffectCowTick;
        EventManager.CowAnimSpeed += AnimalAnimSpeed;
        EventManager.AnimalAnim += AnimalAnim;
        EventManager.CallColorAlpha += ColorAlphaCall;
       
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    
    public void ColorAlphaCall()
    {
        StartCoroutine(ColorAlphaDown1());
        if (Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 8 || Player.LevelValue == 9)
        {
            StartCoroutine(ColorAlphaDown2());
        }
    }
    public void CowEffectCowTick()
    {
        CowEffect.SetActive(true);
        CowTick.SetActive(true);
    }
    public void AnimalAnimSpeed()
    {
        CowAnim.speed = 0;
    }
    public void AnimalAnim()
    {
        if (Player.LevelValue == 4)
        {
            CowAnim.SetBool("Sheep", true);
        }
        else if (Player.LevelValue == 1 || Player.LevelValue == 2 || Player.LevelValue == 3)
        {
            CowAnim.SetBool("Food", true);
        }
        else if (Player.LevelValue == 5)
        {
            CowAnim.SetBool("Goat", true);
        }
        else if (Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 8 || Player.LevelValue == 9)
        {
            CowAnim2.SetBool("Food", true);
        }
    }
    public IEnumerator ColorAlphaDown1()
    {
        cowThink1.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        cowThink2.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        cowThink3.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        cowThink4.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        cowThink5.SetActive(true);

        yield return new WaitForSeconds(2f);
        for (float i = 0.1f; i < 0.4; i += 0.1f)
        {
            cowThink1.GetComponent<SpriteRenderer>().color = new Color(cowThink1.GetComponent<SpriteRenderer>().color.r, cowThink1.GetComponent<SpriteRenderer>().color.g, cowThink1.GetComponent<SpriteRenderer>().color.b, cowThink1.GetComponent<SpriteRenderer>().color.a - i);
            cowThink2.GetComponent<SpriteRenderer>().color = new Color(cowThink2.GetComponent<SpriteRenderer>().color.r, cowThink2.GetComponent<SpriteRenderer>().color.g, cowThink2.GetComponent<SpriteRenderer>().color.b, cowThink2.GetComponent<SpriteRenderer>().color.a - i);
            cowThink3.GetComponent<SpriteRenderer>().color = new Color(cowThink3.GetComponent<SpriteRenderer>().color.r, cowThink3.GetComponent<SpriteRenderer>().color.g, cowThink3.GetComponent<SpriteRenderer>().color.b, cowThink3.GetComponent<SpriteRenderer>().color.a - i);
            cowThink4.GetComponent<SpriteRenderer>().color = new Color(cowThink4.GetComponent<SpriteRenderer>().color.r, cowThink4.GetComponent<SpriteRenderer>().color.g, cowThink4.GetComponent<SpriteRenderer>().color.b, cowThink4.GetComponent<SpriteRenderer>().color.a - i);
            cowThink5.GetComponent<SpriteRenderer>().color = new Color(cowThink5.GetComponent<SpriteRenderer>().color.r, cowThink5.GetComponent<SpriteRenderer>().color.g, cowThink5.GetComponent<SpriteRenderer>().color.b, cowThink5.GetComponent<SpriteRenderer>().color.a - i);
            yield return new WaitForSeconds(0.3f);
        }
    }
    IEnumerator ColorAlphaDown2()
    {
        cowThink6.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        cowThink7.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        cowThink8.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        cowThink9.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        cowThink10.SetActive(true);

        yield return new WaitForSeconds(2f);
        for (float i = 0.1f; i < 0.4; i += 0.1f)
        {
            cowThink6.GetComponent<SpriteRenderer>().color = new Color(cowThink6.GetComponent<SpriteRenderer>().color.r, cowThink6.GetComponent<SpriteRenderer>().color.g, cowThink6.GetComponent<SpriteRenderer>().color.b, cowThink6.GetComponent<SpriteRenderer>().color.a - i);
            cowThink7.GetComponent<SpriteRenderer>().color = new Color(cowThink7.GetComponent<SpriteRenderer>().color.r, cowThink7.GetComponent<SpriteRenderer>().color.g, cowThink7.GetComponent<SpriteRenderer>().color.b, cowThink7.GetComponent<SpriteRenderer>().color.a - i);
            cowThink8.GetComponent<SpriteRenderer>().color = new Color(cowThink8.GetComponent<SpriteRenderer>().color.r, cowThink8.GetComponent<SpriteRenderer>().color.g, cowThink8.GetComponent<SpriteRenderer>().color.b, cowThink8.GetComponent<SpriteRenderer>().color.a - i);
            cowThink9.GetComponent<SpriteRenderer>().color = new Color(cowThink9.GetComponent<SpriteRenderer>().color.r, cowThink9.GetComponent<SpriteRenderer>().color.g, cowThink9.GetComponent<SpriteRenderer>().color.b, cowThink9.GetComponent<SpriteRenderer>().color.a - i);
            cowThink10.GetComponent<SpriteRenderer>().color = new Color(cowThink10.GetComponent<SpriteRenderer>().color.r, cowThink10.GetComponent<SpriteRenderer>().color.g, cowThink10.GetComponent<SpriteRenderer>().color.b, cowThink10.GetComponent<SpriteRenderer>().color.a - i);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
