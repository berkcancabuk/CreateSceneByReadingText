using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water2DTool {
    public class CameraModes : MonoBehaviour {

        private MaxCamera maxCamera;
        private Water2DCameraFollow cameraFollow;
        private bool active = true;

        // Use this for initialization
        void Start()
        {
            maxCamera = GetComponent<MaxCamera>();
            cameraFollow = GetComponent<Water2DCameraFollow>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                active = !active;
                if (active)
                {
                    maxCamera.enabled = false;
                    cameraFollow.enabled = true;
                }
                else
                {
                    maxCamera.enabled = true;
                    cameraFollow.enabled = false;
                }
            }
        }
    }
}
