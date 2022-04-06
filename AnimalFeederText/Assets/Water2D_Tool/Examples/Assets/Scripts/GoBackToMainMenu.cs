using UnityEngine;
using UnityEngine.SceneManagement;

namespace Water2DTool {
    public class GoBackToMainMenu : MonoBehaviour {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadScene("MainMenu");
        }
    }
}
