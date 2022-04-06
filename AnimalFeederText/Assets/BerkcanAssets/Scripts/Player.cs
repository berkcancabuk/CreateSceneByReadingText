using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    [SerializeField] public UImanager UI_manager;
    //[SerializeField] GameObject trueTick,CowTick;
    [SerializeField] public static int LevelValue = 0;
    public DynamicJoystick dynamicJoystick;
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed = 0;
    [SerializeField] float turnspeed = 0;
    [SerializeField] public Animator playerAnim;
    //[SerializeField] Animator CowAnim;
    //[SerializeField] GameObject CMvcam1;
    //[SerializeField] GameObject CMCowCam;
    //[SerializeField] GameObject plant;
    //[SerializeField] GameObject plantOnHandPlayer;
    //[SerializeField] GameObject failLevel;
    //[SerializeField] GameObject taptorestart;
    //[SerializeField] GameObject Effect;
    //[SerializeField] GameObject CowEffect;
    bool isCamPosForward = true;
    //[SerializeField] GameObject soil, basket;
    //[SerializeField] GameObject fishHead;
    //[SerializeField] GameObject cowThink1, cowThink2, cowThink3, cowThink4, cowThink5;
    //karakterin birdHouse dönüþüm objeleri
    [SerializeField] GameObject BirdHouse, playerEffect, coverrals, stickman, playerEffect2;
    [SerializeField]
    public GameObject plantOnHandPlantPlayer, plantOnHandRaddishPlayer, plantOnHandBeetRootPlayer,
        plantOnHandPepperPlayer, plantOnHandLemonPlayer, plantOnHandTomatoPlayer, plantOnHandGreenPlayer;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        //LevelValue = PlayerPrefs.GetInt("SavedScene");
        //SceneManager.LoadScene(LevelValue);

        //EventManager.CheckPlayer += CheckGameManager;
        
        dynamicJoystick.StartOpenJoystick();
        //cowThink1 = GetComponent<Image>();
        //cowThink2 = GetComponent<Image>();
        //cowThink3 = GetComponent<Image>();
        //cowThink4 = GetComponent<Image>();
        //cowThink5 = GetComponent<Image>();

        //CMvcam1 = GameObject.Find("CM vcam1");
        //CMvcam1.GetComponent<GameObject>();
        //cowThink1 = GameObject.Find("111");
        //cowThink1.GetComponent<GameObject>();
        //cowThink2 = GameObject.Find("2222");
        //cowThink2.GetComponent<GameObject>();
        //cowThink3 = GameObject.Find("112");
        //cowThink3.GetComponent<GameObject>();
        //cowThink4 = GameObject.Find("444");
        //cowThink4.GetComponent<GameObject>();
        //cowThink5 = GameObject.Find("3333");
        //cowThink5.GetComponent<GameObject>();

        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        //joy = GetComponent<Joystick>();
        //StartCoroutine(CameraDelay());


    }
    //public void CheckGameManager()
    //{
    //    GameObject UImanagerGameObject = GameObject.Find("Canvas");
    //    UI_manager = UImanagerGameObject.GetComponent<UImanager>();
    //    GameObject dynamicjoystickss = GameObject.Find("Dynamic Joystick");
    //    dynamicJoystick = dynamicjoystickss.GetComponent<DynamicJoystick>();
    //}
    //public bool isNextLevel = false;
    private void Update()
    {

        //if (isNextLevel == true)
        //{
        //    GameObject gamemanagerObject = GameObject.Find("GameManager");
        //    game_manager = gamemanagerObject.GetComponent<GameManager>();
        //    print("update");
        //    StartCoroutine(IsNextLevelTrue());
        //}

    }
    //IEnumerator IsNextLevelTrue()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    //isNextLevel = false;
    //}
    //IEnumerator ColorAlphaDown()
    //{
    //    cowThink1.SetActive(true);
    //    yield return new WaitForSeconds(0.3f);
    //    cowThink2.SetActive(true);
    //    yield return new WaitForSeconds(0.3f);
    //    cowThink3.SetActive(true);
    //    yield return new WaitForSeconds(0.3f);
    //    cowThink4.SetActive(true);
    //    yield return new WaitForSeconds(0.3f);
    //    cowThink5.SetActive(true);

    //    yield return new WaitForSeconds(2f);
    //    for (float i = 0.1f; i < 0.4; i += 0.1f)
    //    {
    //        cowThink1.GetComponent<SpriteRenderer>().color = new Color(cowThink1.GetComponent<SpriteRenderer>().color.r, cowThink1.GetComponent<SpriteRenderer>().color.g, cowThink1.GetComponent<SpriteRenderer>().color.b, cowThink1.GetComponent<SpriteRenderer>().color.a - i);
    //        cowThink2.GetComponent<SpriteRenderer>().color = new Color(cowThink2.GetComponent<SpriteRenderer>().color.r, cowThink2.GetComponent<SpriteRenderer>().color.g, cowThink2.GetComponent<SpriteRenderer>().color.b, cowThink2.GetComponent<SpriteRenderer>().color.a - i);
    //        cowThink3.GetComponent<SpriteRenderer>().color = new Color(cowThink3.GetComponent<SpriteRenderer>().color.r, cowThink3.GetComponent<SpriteRenderer>().color.g, cowThink3.GetComponent<SpriteRenderer>().color.b, cowThink3.GetComponent<SpriteRenderer>().color.a - i);
    //        cowThink4.GetComponent<SpriteRenderer>().color = new Color(cowThink4.GetComponent<SpriteRenderer>().color.r, cowThink4.GetComponent<SpriteRenderer>().color.g, cowThink4.GetComponent<SpriteRenderer>().color.b, cowThink4.GetComponent<SpriteRenderer>().color.a - i);
    //        cowThink5.GetComponent<SpriteRenderer>().color = new Color(cowThink5.GetComponent<SpriteRenderer>().color.r, cowThink5.GetComponent<SpriteRenderer>().color.g, cowThink5.GetComponent<SpriteRenderer>().color.b, cowThink5.GetComponent<SpriteRenderer>().color.a - i);
    //        yield return new WaitForSeconds(0.3f);
    //    }
    //}
    //IEnumerator CameraDelay()
    //{
    //    CMvcam1.transform.DOMove(new Vector3(CMvcam1.transform.position.x, CMvcam1.transform.position.y, 14f), 10f);
    //    //yield return new WaitForSeconds(10f);
    //    //CMvcam1.transform.DOMove(new Vector3(CMvcam1.transform.position.x, CMvcam1.transform.position.y, -11.35f), 10f);
    //}
    // Update is called once per frame
    void FixedUpdate()
    {
        touchCont();
        //joystickMovement();
        //Vector3 force = new Vector3(joy.Horizontal * speed, 0f, joy.Vertical * speed);
        //rb.AddForce(force * Time.fixedDeltaTime);
    }
    public void joystickMovement()
    {
        float horizontal = dynamicJoystick.Horizontal;
        float vertical = dynamicJoystick.Vertical;
        Vector3 addedPos = new Vector3(horizontal * speed * Time.deltaTime, 0, vertical * speed * Time.deltaTime);
        transform.position += addedPos;

        Vector3 direction = Vector3.forward * vertical + Vector3.right * horizontal;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnspeed * Time.deltaTime);
    }
    public static bool isRun = true;
    public bool isTouchActive = true;
    public bool didItPullAfterTheFirstTouch = false;
    public void touchCont()
    {
        if (isTouchActive)
        {
            //this.gameObject.GetComponent<BoxCollider>().enabled = true;

            if (Input.touchCount > 0)
            {
                if (isBird == true)
                {
                    StartCoroutine(BirdToHuman());
                    isBird = false;

                }


                if (isCamPosForward)
                {
                    //CMvcam1.transform.DOMove(new Vector3(CMvcam1.transform.position.x, CMvcam1.transform.position.y, 14f), 10f);
                    //StartCoroutine(CameraDelay());
                    isCamPosForward = false;
                }

                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {

                    playerAnim.SetBool("Idle", false);
                    if (isRun == true)
                    {
                        playerAnim.SetBool("Run", true);
                    }
                    else
                    {
                        playerAnim.SetBool("RunDrop", true);
                    }

                    joystickMovement();
                }
                else if ((t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary))
                {
                    didItPullAfterTheFirstTouch = true;
                    playerEffect.SetActive(false);
                    playerEffect2.SetActive(false);
                    isDead = true;
                    //this.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    playerAnim.SetBool("Idle", false);
                    if (isRun == true)
                    {
                        playerAnim.SetBool("Run", true);
                    }
                    else
                    {
                        playerAnim.SetBool("RunDrop", true);
                    }
                    joystickMovement();
                }
                else if (t.phase == TouchPhase.Ended)
                {

                    isDead = false;
                    playerAnim.SetBool("Idle", true);
                    playerAnim.SetBool("Run", false);
                    playerAnim.SetBool("RunDrop", false);
                }
            }
            else
            {

                if (didItPullAfterTheFirstTouch == true)
                {
                    if (isBird == false)
                    {
                        isDead = false;
                        //this.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        //this.gameObject.GetComponent<BoxCollider>().enabled = false;
                        StartCoroutine(BirdFoxTrans());

                    }
                }

                playerAnim.SetBool("Idle", true);
                playerAnim.SetBool("Run", false);
                playerAnim.SetBool("RunDrop", false);

            }
        }


    }
    public bool isBird = false;
    IEnumerator BirdFoxTrans()
    {
        playerEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        coverrals.SetActive(false);
        stickman.SetActive(false);
        BirdHouse.SetActive(true);
        isBird = true;
    }
    IEnumerator BirdToHuman()
    {
        playerEffect2.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        coverrals.SetActive(true);
        stickman.SetActive(true);
        BirdHouse.SetActive(false);
    }
    public static bool isPepper1 = false;
    public static bool isPepper2 = false;
    public bool isDead = false;
    public bool OneMoreLoop = false;
    public bool OneMoreLoopForTwoAnimals = false;
    private void OnTriggerEnter(Collider other)
    {
        print(other.name + "collider çarptýðý objeler");
        if (other.tag == "CowFood")
        {
            EventManager.hitPlayerBox();
            if (LevelValue == 0 || LevelValue == 1 || LevelValue == 2)
            {
                plantOnHandPlantPlayer.SetActive(true);
            }
            else if (LevelValue == 4)
            {
                plantOnHandRaddishPlayer.SetActive(true);
            }
            else if (LevelValue == 5)
            {
                plantOnHandBeetRootPlayer.SetActive(true);
            }
            else if (LevelValue == 10)
            {
                plantOnHandPepperPlayer.SetActive(true);
            }
            playerAnim.SetBool("Idle", false);
            playerAnim.SetBool("Run", false);
            playerAnim.SetBool("RunDrop", true);
            //CMvcam1.transform.DOMove(new Vector3(CMvcam1.transform.position.x, CMvcam1.transform.position.y, -11.35f), 10f);
            isRun = false;
        }
        if (other.tag == "LeftBox" || other.tag == "RightBox")
        {
            plantOnHandPepperPlayer.SetActive(true);
            EventManager.hitPlayerBox();
        }
        if (other.tag == "LeftBoxLevel7")
        {
            plantOnHandLemonPlayer.SetActive(true);
            EventManager.hitPlayerBox();
        }
        if (other.tag == "RightBoxLevel7")
        {
            plantOnHandTomatoPlayer.SetActive(true);
            EventManager.hitPlayerBox();
        }
        if (other.tag == "LeftBoxLevel8")
        {
            plantOnHandGreenPlayer.SetActive(true);
            EventManager.hitPlayerBox();
        }
        if (other.tag == "RightBoxLevel8")
        {
            plantOnHandRaddishPlayer.SetActive(true);
            EventManager.hitPlayerBox();
        }

        if (other.tag == "LeftBoxLevel9")
        {
            plantOnHandPlantPlayer.SetActive(true);
            EventManager.hitPlayerBox();
        }
        if (other.tag == "RightBoxLevel9")
        {
            plantOnHandRaddishPlayer.SetActive(true);
            EventManager.hitPlayerBox();
        }

        if (plantOnHandPlantPlayer.gameObject.activeSelf == true || plantOnHandRaddishPlayer.gameObject.activeSelf == true || plantOnHandBeetRootPlayer.gameObject.activeSelf == true || plantOnHandPepperPlayer.gameObject.activeSelf == true || plantOnHandTomatoPlayer.gameObject.activeSelf == true || plantOnHandLemonPlayer.gameObject.activeSelf == true || plantOnHandGreenPlayer.gameObject.activeSelf == true)
        {
            if (other.tag == "CowFeed")
            {
                //transform.DOLocalMove(new Vector3(-1.1754f, -1.203f, -24.611f), 2f);
                if (OneMoreLoop == false)
                {
                    isTouchActive = false;

                    StartCoroutine(EffectCow());
                    UI_manager.CMvcam1.SetActive(false);
                    UI_manager.CMCowCam.SetActive(true);
                    //StartCoroutine(game_manager.EffectCow());
                    OneMoreLoop = true;
                }

            }
            if (other.tag == "CowFeed2")
            {
                if (OneMoreLoop == false)
                {
                    isTouchActive = false;
                    StartCoroutine(EffectCow());
                    //StartCoroutine(game_manager.EffectCow());
                    OneMoreLoop = true;
                }
            }
            if (other.tag == "SheepFeed")
            {
                if (OneMoreLoop == false)
                {
                    isTouchActive = false;
                    StartCoroutine(EffectCow());
                    //StartCoroutine(game_manager.EffectCow());
                    OneMoreLoop = true;
                }
            }
            if (other.tag == "GoatFeed")
            {
                if (OneMoreLoop == false)
                {
                    isTouchActive = false;
                    StartCoroutine(EffectCow());
                    //StartCoroutine(game_manager.EffectCow());
                    OneMoreLoop = true;
                }
            }
            if (other.tag == "Pepper1" && isPepper1 == false && RightAnimal.RightAnimalFood == true)
            {
                if (OneMoreLoopForTwoAnimals == false)
                {
                    didItPullAfterTheFirstTouch = false;
                    print("girdi pepper");
                    isTouchActive = false;
                    StartCoroutine(EffectCow2());
                    UI_manager.CMvcam1.SetActive(false);
                    UI_manager.CMCowCam.SetActive(true);
                    //StartCoroutine(game_manager.EffectCow2());
                    RightAnimal.isPepper = false;
                    isPepper1 = true;
                    OneMoreLoopForTwoAnimals = true;
                }


            }
            if (other.tag == "Pepper2" && isPepper2 == false && LeftAnimal.LeftAnimalFood == true)
            {
                if (OneMoreLoop == false)
                {
                    didItPullAfterTheFirstTouch = false;
                    print("girdi2 pepper");
                    isPepper2 = true;
                    isTouchActive = false;
                    StartCoroutine(EffectCow());
                    //StartCoroutine(game_manager.EffectCow());
                    RightAnimal.isPepper = false;
                    OneMoreLoop = true;
                }

            }

        }


        if (other.tag == "Fish" && isDead == true)
        {
            EventManager.AnimalHeadChangeColor();
            playerAnim.SetBool("Idle", true);
            playerAnim.SetBool("Run", false);
            playerAnim.SetBool("RunDrop", false);
            UI_manager.failLevel.SetActive(true);
            UI_manager.taptorestart.SetActive(true);
            isTouchActive = false;
        }
        if (other.tag == "Deer" && isDead == true)
        {
            EventManager.AnimalHeadChangeColor();
            playerAnim.SetBool("Idle", true);
            playerAnim.SetBool("Run", false);
            playerAnim.SetBool("RunDrop", false);
            UI_manager.failLevel.SetActive(true);
            UI_manager.taptorestart.SetActive(true);
            isTouchActive = false;
        }
    }
   

    //public void TryButton()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //}

    public IEnumerator EffectCow2()
    {
        if (Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 8 || Player.LevelValue == 9)
        {
            if (Player.LevelValue != 8)
            {
                transform.DOLocalMove(new Vector3(0.394f, -2.72f, -48.866f), 1f);
                transform.DOLocalRotate(new Vector3(0, 135, 0), 1f);
            }
            else if (Player.LevelValue == 8)
            {
                transform.DOLocalMove(new Vector3(1.841f, -1.316f, -23.895f), 1f);
                transform.DOLocalRotate(new Vector3(0, -220.526f, 0), 1f);
            }


            yield return new WaitForSeconds(0.5f);
            playerAnim.SetBool("StandToSit", true);
            EventManager.AnimalAnim();
            UI_manager.CMvcam1.SetActive(false);
            UI_manager.CMCowCam.SetActive(true);

            yield return new WaitForSeconds(1f);
            Player.isPepper1 = false;
            EventManager.CowTickCowEffect();
            yield return new WaitForSeconds(1f);
            plantOnHandPepperPlayer.SetActive(false);
            plantOnHandTomatoPlayer.SetActive(false);
            plantOnHandLemonPlayer.SetActive(false);
            plantOnHandGreenPlayer.SetActive(false);
            plantOnHandRaddishPlayer.SetActive(false);
            plantOnHandPlantPlayer.SetActive(false);
            if (LeftAnimal.LeftAnimalFood == true)
            {
                yield return new WaitForSeconds(1f);
                UI_manager.soil.SetActive(true);
                yield return new WaitForSeconds(1f);
                UI_manager.basket.SetActive(true);
                UI_manager.LevelComplate.SetActive(true);
                UI_manager.TapToContinue.SetActive(true);
            }
            else
            {
                playerAnim.SetBool("StandToSit", false);
                playerAnim.SetBool("Run", false);
                playerAnim.SetBool("RunDrop", false);
                playerAnim.SetBool("Idle", true);
                isTouchActive = true;
                EventManager.PlayerOnHand();
                yield return new WaitForSeconds(0.5f);
                EventManager.CowAnimSpeed();

                UI_manager.CMvcam1.SetActive(true);
                UI_manager.CMCowCam.SetActive(false);

            }

        }
    }
    public IEnumerator EffectCow()
    {
        if (Player.LevelValue == 0)
        {
            transform.DOLocalMove(new Vector3(-1.97f, 0.15f, 0.49f), 1f);
        }
        if (Player.LevelValue == 1 || Player.LevelValue == 10)
        {
            transform.DOLocalMove(new Vector3(-4.156f, -2.72f, -46.603f), 1f);
        }
        if (Player.LevelValue == 3 || Player.LevelValue == 4 || Player.LevelValue == 5)
        {
            print("Kac kere girdi");
            transform.DOLocalMove(new Vector3(-0.616f, -2.697413f, -47.558f), 1f);
            transform.DOLocalRotate(new Vector3(0, 135, 0), 1f);
        }
        if (Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 9)
        {
            transform.DOLocalMove(new Vector3(-3.284f, -3.017283f, -48.974f), 1f);
        }
        if (Player.LevelValue == 8)
        {
            transform.DOLocalMove(new Vector3(-2.229f, -1.371f, -24.004f), 1f);
            transform.DOLocalRotate(new Vector3(0, -145.526f, 0), 1f);
        }
        yield return new WaitForSeconds(0.5f);
        playerAnim.SetBool("StandToSit", true);
        if (Player.LevelValue == 4)
        {

            EventManager.AnimalAnim();
        }
        else if (Player.LevelValue == 1 || Player.LevelValue == 2 || Player.LevelValue == 3)
        {
            EventManager.AnimalAnim();
        }
        else if (Player.LevelValue == 5)
        {
            EventManager.AnimalAnim();
        }
        else if (Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 8 || Player.LevelValue == 9)
        {

            EventManager.AnimalAnim();
            UI_manager.CMCow2Cam.SetActive(true);
        }
        if ((Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 8 || Player.LevelValue == 9) && RightAnimal.RightAnimalFood == false)
        {
            print("Kac kere girdi level 6 oncesi");
            yield return new WaitForSeconds(1f);
            plantOnHandPepperPlayer.SetActive(false);
            plantOnHandTomatoPlayer.SetActive(false);
            plantOnHandLemonPlayer.SetActive(false);
            plantOnHandGreenPlayer.SetActive(false);
            plantOnHandRaddishPlayer.SetActive(false);
            plantOnHandPlantPlayer.SetActive(false);
            playerAnim.SetBool("StandToSit", false);
            playerAnim.SetBool("Run", false);
            playerAnim.SetBool("RunDrop", false);
            playerAnim.SetBool("Idle", true);
            isTouchActive = true;
            EventManager.PlayerOnHand();
            yield return new WaitForSeconds(0.5f);
            EventManager.CowAnimSpeed();
            UI_manager.CMvcam1.SetActive(true);
            UI_manager.CMCow2Cam.SetActive(false);
        }
        else
        {
            if (Player.LevelValue == 6 || Player.LevelValue == 7 || Player.LevelValue == 8 || Player.LevelValue == 9)
            {
                UI_manager.CMvcam1.SetActive(false);
                UI_manager.CMCow2Cam.SetActive(true);
            }
            else
            {
                UI_manager.CMvcam1.SetActive(false);
                UI_manager.CMCowCam.SetActive(true);
            }
            yield return new WaitForSeconds(2f);
            yield return new WaitForSeconds(1f);
            EventManager.CowTickCowEffect();
            UI_manager.soil.SetActive(true);
            yield return new WaitForSeconds(1f);
            UI_manager.basket.SetActive(true);
            UI_manager.LevelComplate.SetActive(true);
            UI_manager.TapToContinue.SetActive(true);
            //player.playerAnim.SetBool("Idle", true);
            //player.playerAnim.SetBool("Run", false);
            //player.playerAnim.SetBool("RunDrop", false);
            //player.playerAnim.SetBool("StandToSit", false);
            //player.isTouchActive = true;
        }


    }
}
