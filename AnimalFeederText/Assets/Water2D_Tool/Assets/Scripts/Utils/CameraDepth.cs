using UnityEngine;

namespace Water2DTool {

    [ExecuteInEditMode]
    public class CameraDepth : MonoBehaviour {

        public bool cameraDepth = true;

        public void Update()
        {
            if (cameraDepth)
            {
                if (Camera.main)
                {
                    Camera.main.depthTextureMode |= DepthTextureMode.Depth;
                }
            }
            else
            {
                Camera.main.depthTextureMode = DepthTextureMode.None;
            }
        }
    }
}
