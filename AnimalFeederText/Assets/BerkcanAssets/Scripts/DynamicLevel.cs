using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.UI;
using DG.Tweening;
public class DynamicLevel : MonoBehaviour
{
    public Image backGround, LoadingImage, PurpleBackGround;
    string[] lines = System.IO.File.ReadAllLines("Assets/Resources/TextLevels.txt");
    public List<GameObject> LevelObjects = new List<GameObject>();
    public string[] txt;
    public string[] slash;
    public string[] comma;
    public string[] semicolon;
    public List<GameObject> level1GameObjects = new List<GameObject>();
    public List<GameObject> level2GameObjects = new List<GameObject>();
    
    string txtContents;

    void Start()
    {
        EventManager.NextLevel += CreateLevel;
        EventManager.RemoveList += nextLevelRemoveList;
        TextAsset mytxtData = (TextAsset)Resources.Load("TextLevels");
        txtContents = mytxtData.text;
        //print(" Lines2 : " + lines[2]);
        //print(" Lines3 : " + lines[3]);
        //print(" Lines4 : " + lines[4]);
        //print(" Lines5 : " + lines[5]);
        // if level 1 se 0 yaz ve yükle 1 satýrý okur yazar
        //CreateLevel(0);
        // if level 2 istiyorsak 1 yaz ve yükle 2 satýrý okur yazar
        EventManager.NextLevel();


    }
    public void nextLevelRemoveList()
    {
        for (int i = 0; i < LevelObjects.Count; i++)
        {
            LevelObjects[i].gameObject.SetActive(false);
            LevelObjects.RemoveAt(i);
        }
    }

    IEnumerator LoadingValueIncrease()
    {
        yield return new WaitForSeconds(0.5f);
        LoadingImage.DOFillAmount(loadingValue, 1f);
        loadingValue += 1 / createObjectValue;
        print(loadingValue);
        yield return new WaitForSeconds(2f);
        if (loadingValue >= 0.94f)
        {
            loadingValue = 1;
            if (loadingValue == 1)
            {
                LoadingImage.gameObject.SetActive(false);
                backGround.gameObject.SetActive(false);
                PurpleBackGround.gameObject.SetActive(false);
                //EventManager.CheckPlayer();

            }
        }

    }
    public int level;
    float loadingValue= 0;
    float createObjectValue = 24.0f;
    public void CreateLevel()
    {
        print("asd");
        //loadingValue = 1/createObjectValue;

        level = Player.LevelValue;
        for (int i = 0; i < lines[Player.LevelValue].Length; i++)
        {
            
            print(createObjectValue);
            print(1 / createObjectValue);
            StartCoroutine(LoadingValueIncrease());
            
            
            slash = lines[Player.LevelValue].Split('/');
            if (slash[i].Contains("bos") == true)
            {
                i++;
                continue;
            }
            else if (slash[i].Contains(";") && slash[i].Contains(" ") != true)
            {
                for (int j = 0; j < slash[i].Split(';').Length; j++)
                {
                    semicolon = slash[i].Split(';');
                    for (int k = 0; k < semicolon[j].Split(',').Length; k++)
                    {
                        comma = semicolon[j].Split(',');
                        InstantiateLevel1GameObjects(i, j);
                        break;
                    }
                }
            }
            else if (slash[i].Contains(";") != true && slash[i].Contains(" ") != true && slash[i].Contains(",") == true)
            {
                for (int j = 0; j < slash[i].Split(',').Length; j++)
                {
                    comma = slash[i].Split(',');
                    InstantiateLevel1GameObjects(i, j);
                    break;

                }
            }

        }
    }
    public void InstantiateLevel1GameObjects(int value,int value2)
    {
        if (level == 0)
        {
            if (value == 1)
            {
               GameObject sceneObje = Instantiate(level1GameObjects[0], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObje);

            }
            if (value == 3)
            {
                 GameObject sceneObject1 = Instantiate(level1GameObjects[1], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObject1);
            }
            if (value == 5)
            {
                GameObject sceneObject2 = Instantiate(level1GameObjects[3], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObject2);

            }
            if (value == 7)
            {
                GameObject sceneObject2 = Instantiate(level1GameObjects[4], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.Euler(0, 30, 0));
                LevelObjects.Add(sceneObject2);

            }
            if (value == 9)
            {
                GameObject sceneObject1 = Instantiate(level1GameObjects[5], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.Euler(0, 90, 0));
                LevelObjects.Add(sceneObject1);
            }
            if (value == 11)
            {
                GameObject sceneObject1 = Instantiate(level1GameObjects[6], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.Euler(0, -161.388f, 0));
                LevelObjects.Add(sceneObject1);

            }
            if (value == 13)
            {
                GameObject sceneObject1 = Instantiate(level1GameObjects[7], new Vector3(int.Parse(comma[0]), 0.08f, int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObject1);

            }
            if (value == 15)
            {
                GameObject sceneObject1 = Instantiate(level1GameObjects[8], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObject1);

            }
            if (value == 17)
            {
                GameObject sceneObject1 = Instantiate(level1GameObjects[9], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObject1);

            }
            if (value == 19)
            {
                GameObject sceneObject1 = Instantiate(level1GameObjects[10], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObject1);

            }

            if (value == 21)
            {
                GameObject sceneObject1 = Instantiate(level1GameObjects[11], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObject1);
            }
            if (value == 23)
            {
                GameObject sceneObject1 = Instantiate(level1GameObjects[12], new Vector3(float.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObject1);
            }
            if (value == 25)
            {
                GameObject sceneObject1 = Instantiate(level1GameObjects[13], new Vector3(int.Parse(comma[0]), 0.1f, int.Parse(comma[2])), Quaternion.identity);
                LevelObjects.Add(sceneObject1);
            }

            if (value == 27)
            {
                if (value2 == 0)
                {
                    GameObject sceneObject1 = Instantiate(level1GameObjects[14], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                    LevelObjects.Add(sceneObject1);
                }

                if (value2 == 1)
                {
                    GameObject sceneObject1 = Instantiate(level1GameObjects[15], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                    LevelObjects.Add(sceneObject1);
                }

            }
        }
        if (level == 1)
        {
            if (value == 1)
            {
                Instantiate(level2GameObjects[0], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);

            }
            if (value == 3)
            {
                Instantiate(level2GameObjects[1].transform, new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
            }
            //if (value == 5)
            //{
            //    Instantiate(level2GameObjects[3], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);

            //}
            if (value == 7)
            {
                Instantiate(level2GameObjects[2], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.Euler(0, 30, 0));

            }
            if (value == 9)
            {
                Instantiate(level2GameObjects[3].transform, new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.Euler(0, 90, 0));
            }
            if (value == 11)
            {
                Instantiate(level2GameObjects[4], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.Euler(0, -161.388f, 0));

            }
            //if (value == 13)
            //{
            //    Instantiate(level2GameObjects[7], new Vector3(int.Parse(comma[0]), 0.08f, int.Parse(comma[2])), Quaternion.identity);

            //}
            if (value == 15)
            {
                Instantiate(level2GameObjects[5], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);

            }
            if (value == 17)
            {
                Instantiate(level2GameObjects[6], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);

            }
            if (value == 19)
            {
                Instantiate(level2GameObjects[7], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);

            }

            if (value == 21)
            {
                Instantiate(level2GameObjects[8], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
            }
            //if (value == 23)
            //{
            //    Instantiate(level2GameObjects[12].transform, new Vector3(float.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
            //}
            //if (value == 25)
            //{
            //    Instantiate(level2GameObjects[13].transform, new Vector3(int.Parse(comma[0]), 0.1f, int.Parse(comma[2])), Quaternion.identity);
            //}

            if (value == 25)
            {
                if (value2 == 0)
                {
                    Instantiate(level2GameObjects[9], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                }

                if (value2 == 1)
                {
                    Instantiate(level2GameObjects[10], new Vector3(int.Parse(comma[0]), int.Parse(comma[1]), int.Parse(comma[2])), Quaternion.identity);
                }
            }
        }
    }

}
