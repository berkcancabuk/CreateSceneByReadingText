using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Water2DTool
{
    [CustomEditor(typeof(ObstructionPolygon))]
    public class ObstructionShapeEditor : Editor
    {
        private static float pathScale = 1f;
        private List<int> selectedPoints = new List<int>();
        private bool showProperties = true;
        private bool handleSelected = false;

        void OnEnable()
        {
            selectedPoints.Clear();
        }

        public override void OnInspectorGUI()
        {
            ObstructionPolygon shape = (ObstructionPolygon)target;
            CustomInspector(shape);
        }

        private void CustomInspector(ObstructionPolygon shape)
        {
            Undo.RecordObject(target, "Modified Inspector");
            showProperties = EditorGUILayout.Foldout(showProperties, "Properties");

            if (showProperties)
            {
                EditorGUI.indentLevel = 1;
                InspectorBox(8, () =>
                {
                    shape.handleScale = EditorGUILayout.Slider(new GUIContent("Handle Scale", "Sets the scale of the handles."), shape.handleScale, 0.1f, 5f);
                    pathScale = shape.handleScale;
                });
            }
            EditorGUI.indentLevel = 0;
        }

        void OnSceneGUI()
        {
            ObstructionPolygon path = (ObstructionPolygon)target;

            GUIStyle iconStyle = new GUIStyle();
            iconStyle.alignment = TextAnchor.MiddleCenter;

            // draw the path line
            if (Event.current.type == EventType.Repaint)           
                DrawPath(path);         

            // draw and interact with all the path handles
            UpdateHandles(path, iconStyle);

            if (GUI.changed)            
                EditorUtility.SetDirty(target);
        }

        private void UpdateHandles(ObstructionPolygon shape, GUIStyle iconStyle)
        {
            Quaternion inv = Quaternion.Inverse(shape.transform.rotation);
            Handles.color = new Color(1, 1, 1, 1);

            Vector3 global, tGlobal = Vector3.zero;
            handleSelected = false;

            for (int i = 0; i < shape.handlesPosition.Count; i++)
            {
                Handles.color = new Color(1, 1, 1, 1);

                // global position of a path point
                Vector3 pos = shape.transform.position + Vector3.Scale(new Vector3(shape.handlesPosition[i].x, 0, shape.handlesPosition[i].z), shape.transform.localScale);             
                bool isSelected = selectedPoints.Contains(i);

                if (!handleSelected)
                    handleSelected = selectedPoints.Contains(i);

                global = Handles.FreeMoveHandle(pos, Quaternion.identity, HandleScale(pos), new Vector3(0, 1, 1), Handles.SphereHandleCap);

                if (global != pos)
                {
                    selectedPoints.Clear();
                    selectedPoints.Add(i);
                    isSelected = true;

                    Vector3 local = inv * (global - shape.transform.position);
                    Vector3 relative = new Vector3(local.x / shape.transform.localScale.x, local.y / shape.transform.localScale.y, local.z / shape.transform.localScale.z) - shape.handlesPosition[i];

                    shape.handlesPosition[selectedPoints[0]] += relative;
                }

                // make sure we can add new point at the midpoints!
                if (i < shape.handlesPosition.Count)
                {
                    int index;
                    if (i < shape.handlesPosition.Count - 1)
                        index = i + 1;
                    else
                        index = 0;

                    Vector3 pos2 = shape.transform.position + shape.transform.rotation * Vector3.Scale(new Vector3(shape.handlesPosition[index].x, 0, shape.handlesPosition[index].z), shape.transform.localScale);
                    Vector3 mid = (pos + pos2) / 2;
                    float handleScale = HandleScale(mid) * 0.6f;

                    if (IsVisible(mid))
                        Handles.color = new Color(0, 0.4f, 1, 1);         

                    if (Handles.Button(mid, SceneView.lastActiveSceneView.camera.transform.rotation, handleScale, handleScale, Handles.SphereHandleCap))
                    {
                        Vector3 pt = inv * new Vector3((mid.x - shape.transform.position.x) / shape.transform.localScale.x, 0, (mid.z - shape.transform.position.z) / shape.transform.localScale.z);
                        shape.handlesPosition.Insert(index, pt);
                    }
                }

                // check if we want to remove points
                if (Event.current.alt && shape.handlesPosition.Count > 3)
                {
                    float handleScale = HandleScale(pos);
                    if (IsVisible(pos))                   
                        Handles.color = new Color(1, 0.4f, 0, 1);
                    
                    if (Handles.Button(pos, SceneView.lastActiveSceneView.camera.transform.rotation, handleScale, handleScale, Handles.SphereHandleCap))
                    {
                        if (!isSelected)
                        {
                            selectedPoints.Clear();
                            selectedPoints.Add(i);
                        }
                        for (int s = 0; s < selectedPoints.Count; s++)
                        {
                            shape.handlesPosition.RemoveAt(selectedPoints[s]);

                            if (selectedPoints[s] <= i) i--;

                            for (int u = 0; u < selectedPoints.Count; u++)
                            {
                                if (selectedPoints[u] > selectedPoints[s]) selectedPoints[u] -= 1;
                            }
                        }
                        selectedPoints.Clear();
                        GUI.changed = true;
                    }
                }
            }
        }

        private void DrawPath(ObstructionPolygon path)
        {
            Handles.color = Color.white;
            List<Vector3> verts = path.handlesPosition;
            for (int i = 0; i < verts.Count; i++)
            {
                Vector3 pos, pos2;

                if (i < verts.Count - 1)
                {
                    pos = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(verts[i].x, 0, verts[i].z), path.transform.localScale);
                    pos2 = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(verts[i + 1].x, 0, verts[i + 1].z), path.transform.localScale);
                }
                else
                {
                    pos = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(verts[i].x, 0, verts[i].z), path.transform.localScale);
                    pos2 = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(verts[0].x, 0, verts[0].z), path.transform.localScale);
                }
                Handles.DrawLine(pos, pos2);
            }
        }

        private static float GetCameraDist(Vector3 aPt)
        {
            return Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, aPt);
        }

        private bool IsVisible(Vector3 aPos)
        {
            Transform t = SceneView.lastActiveSceneView.camera.transform;
            if (Vector3.Dot(t.forward, aPos - t.position) > 0)
                return true;
            return false;
        }

        private static float HandleScale(Vector3 aPos)
        {
            float dist = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.45f : GetCameraDist(aPos);
            return Mathf.Min(0.4f * pathScale, (dist / 5.0f) * 0.4f * pathScale);
        }

        public void InspectorBox(int aBorder, System.Action inside, int aWidthOverride = 0, int aHeightOverride = 0)
        {
            Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Width(aWidthOverride));
            if (aWidthOverride != 0)
            {
                r.width = aWidthOverride;
            }
            GUI.Box(r, GUIContent.none);
            GUILayout.Space(aBorder);
            if (aHeightOverride != 0)
                EditorGUILayout.BeginVertical(GUILayout.Height(aHeightOverride));
            else
                EditorGUILayout.BeginVertical();
            GUILayout.Space(aBorder);
            inside();
            GUILayout.Space(aBorder);
            EditorGUILayout.EndVertical();
            GUILayout.Space(aBorder);
            EditorGUILayout.EndHorizontal();
        }
    }
}
