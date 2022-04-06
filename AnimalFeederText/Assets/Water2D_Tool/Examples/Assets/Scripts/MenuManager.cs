using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Water2DTool {
    public class MenuManager : MonoBehaviour {

        public GameObject oldDemoScenesUI;
        public GameObject gpuWaterUI;
        public GameObject mainMenuUI;
        private bool inMainMenu = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (inMainMenu)
                    Quit();
                else
                {
                    gpuWaterUI.gameObject.SetActive(false);
                    oldDemoScenesUI.gameObject.SetActive(false);
                    mainMenuUI.gameObject.SetActive(true);
                    inMainMenu = true;
                }
            }
        }

        public void ShowOldDemoScenesMenu()
        {
            mainMenuUI.gameObject.SetActive(false);
            oldDemoScenesUI.gameObject.SetActive(true);
            inMainMenu = false;
        }

        public void ShowGPUWaterMenu()
        {
            mainMenuUI.gameObject.SetActive(false);
            gpuWaterUI.gameObject.SetActive(true);
            inMainMenu = false;
        }

        public void Load2_5DWater()
        {
            SceneManager.LoadScene("2.5D_Water");
        }

        public void Load2DWater()
        {
            SceneManager.LoadScene("2D_Water");
        }

        public void LoadSineWavesScene()
        {
            SceneManager.LoadScene("BigWavesInteraction");
        }

        public void LoadWaterFlowScene()
        {
            SceneManager.LoadScene("WaterFlow");
        }

        public void LoadAnimationOne()
        {
            SceneManager.LoadScene("SandBox_01");
        }

        public void LoadAnimationTwo()
        {
            SceneManager.LoadScene("SandBox_02");
        }

        public void LoadBigLakeUnlit()
        {
            SceneManager.LoadScene("BigLake_Unlit");
        }

        public void LoadSmallLakeUnlit()
        {
            SceneManager.LoadScene("SmallLake_Unlit");
        }

        public void LoadBigLakeLit()
        {
            SceneManager.LoadScene("BigLake_Lit");
        }

        public void LoadSmallLakeLit()
        {
            SceneManager.LoadScene("SmallLake_Lit");
        }

        public void LoadSmallRockPool_Unlit_Mobile()
        {
            SceneManager.LoadScene("SmallLake_Unlit_Basic");
        }

        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}
