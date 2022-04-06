using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water2DTool
{
    public class ObstructionPolygon : MonoBehaviour
    {
        /// <summary>
        /// The local positions of the handles.
        /// </summary>
        public List<Vector3> handlesPosition = new List<Vector3>();
        /// <summary>
        /// The scale of the handle gizmo.
        /// </summary>
        public float handleScale = 1f;

        /// <summary>
        /// Adds a point to the polygon shape.
        /// </summary>
        /// <param name="hP">Node local position.</param>
        public void AddShapePoint(Vector3 hP)
        {
            handlesPosition.Add(hP);
        }

        public List<Vector2> GetShapePointsWorldPos()
        {
            int len = handlesPosition.Count;
            List<Vector2> worldPos = new List<Vector2>();

            for (int i = 0; i < len; i++)
            {
                Vector3 wPos = transform.TransformPoint(handlesPosition[i]);
                worldPos.Add(new Vector2(wPos.x, wPos.z));
            }

            return worldPos;
        }
    }
}
