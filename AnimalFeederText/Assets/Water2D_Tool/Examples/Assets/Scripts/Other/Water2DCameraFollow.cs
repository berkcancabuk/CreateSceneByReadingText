using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water2DTool {
    public class Water2DCameraFollow : MonoBehaviour {

        public Transform target;

        public float smoothSpeed = 0.125f;
        public Vector3 offset;

        void FixedUpdate()
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
            //Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            transform.LookAt(target);
        }
    }
}
