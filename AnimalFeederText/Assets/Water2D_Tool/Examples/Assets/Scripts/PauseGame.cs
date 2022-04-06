using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Water2DTool {
    [ExecuteInEditMode]
    public class PauseGame : MonoBehaviour {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Debug.Break();
            }
        }
    }
}
