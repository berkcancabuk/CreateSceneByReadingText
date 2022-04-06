using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DeerScript : MonoBehaviour
{
    [SerializeField] public GameObject AnimalHead;
    public Animator deerAnim;
    public Player player;
    public float RotateSpeed = 5f;
    public float rotationSpeed;
    public float Radius = 0.1f;

    private Vector3 _centre;
    public float _angle;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AnimalHeadChangeColor += AnimalHeadChangeAnotherColor;

        //StartCoroutine(DeerPose());
        _centre = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        _angle += RotateSpeed * Time.deltaTime;

        var offset = new Vector3(Mathf.Sin(_angle) * Radius * Mathf.Rad2Deg, transform.position.y, Mathf.Cos(_angle) * Radius * Mathf.Rad2Deg);
        transform.position = _centre + offset;
        Quaternion toRotation = Quaternion.LookRotation(offset, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, -rotationSpeed * Time.deltaTime);
        //StartCoroutine(DeerPose());
        //this.gameObject.transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
    }
    public void AnimalHeadChangeAnotherColor()
    {
        AnimalHead.GetComponent<SpriteRenderer>().color = Color.red;
    }
    public bool TryAgain = false;
    //public IEnumerator DeerPose()
    //{
    //    if (Player.LevelValue != 8)
    //    {
    //        if (TryAgain == false)
    //        {
    //            TryAgain = true;
    //            this.gameObject.transform.DOLocalMove(new Vector3(3.26f, transform.position.y, -9.844561f), 3f);
    //            yield return new WaitForSeconds(2.5f);
    //            this.gameObject.transform.DORotate(new Vector3(0, 169.24f, 0), 0.5f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.transform.DOLocalMove(new Vector3(3.5f, transform.position.y, -11.024561f), 1f);
    //            yield return new WaitForSeconds(1f);
    //            this.transform.DORotate(new Vector3(0, 255, 0), 0.5f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.transform.DOLocalMove(new Vector3(-4.57f, transform.position.y, -13.57f), 3f);
    //            yield return new WaitForSeconds(2.5f);
    //            this.transform.DORotate(new Vector3(0, 332.22f, 0), 0.5f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.transform.DOLocalMove(new Vector3(-4.57f, transform.position.y, -14.18f), 1f);
    //            yield return new WaitForSeconds(0.8f);
    //            this.transform.DORotate(new Vector3(0, 75, 0), 0.5f);
    //            TryAgain = false;
    //        }
    //    }
    //    else if(Player.LevelValue == 8)
    //    {
    //        if (TryAgain == false)
    //        {
    //            TryAgain = true;
    //            print("kaç kere giriyoruz");
    //            this.gameObject.transform.DOLocalMove(new Vector3(-0.57f, transform.position.y, 22.77f), 2f);
    //            yield return new WaitForSeconds(1.5f);
    //            this.gameObject.transform.DORotate(new Vector3(0, -47.94f, 0), 0.5f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.gameObject.transform.DOLocalMove(new Vector3(-3.44f, transform.position.y, 25.36f), 1f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.gameObject.transform.DORotate(new Vector3(0, -152.7f, 0), 0.5f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.gameObject.transform.DOLocalMove(new Vector3(-3.16f, transform.position.y, 24.5f), 1f);
    //            this.gameObject.transform.DORotate(new Vector3(0, -217.1f, 0), 0.5f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.gameObject.transform.DOLocalMove(new Vector3(-0.7220308f, transform.position.y, 22.6513f), 1f);
    //            this.gameObject.transform.DORotate(new Vector3(0, -195.16f, 0), 0.5f);
    //            yield return new WaitForSeconds(1f);
    //            this.gameObject.transform.DOLocalMove(new Vector3(-0.79f, transform.position.y, 6.31f), 3.5f);
    //            this.gameObject.transform.DORotate(new Vector3(0, -181.517f, 0), 0.5f);
    //            yield return new WaitForSeconds(4f);
    //            this.gameObject.transform.DOLocalMove(new Vector3(1.74f, transform.position.y, 2.4f), 1f);
    //            this.gameObject.transform.DORotate(new Vector3(0, -239.035f, 0), 0.5f);
    //            yield return new WaitForSeconds(1f);
    //            this.gameObject.transform.DORotate(new Vector3(0, -298.3f, 0), 0.5f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.gameObject.transform.DORotate(new Vector3(0, -33f, 0), 0.5f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.gameObject.transform.DOLocalMove(new Vector3(-0.15f, transform.position.y, 4.76f), 0.5f);
    //            this.gameObject.transform.DORotate(new Vector3(0, -2.1f, 0), 0.5f);
    //            yield return new WaitForSeconds(0.5f);
    //            this.gameObject.transform.DOLocalMove(new Vector3(-0.29f, transform.position.y, 15.25f), 3f);
    //            yield return new WaitForSeconds(3f);
    //            TryAgain = false;
    //        }
    //    }

    //    }
}
