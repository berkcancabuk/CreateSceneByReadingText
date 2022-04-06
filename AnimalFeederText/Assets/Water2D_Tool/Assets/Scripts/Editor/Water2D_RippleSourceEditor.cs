using UnityEngine;
using UnityEditor;

namespace Water2DTool
{
    [CustomEditor(typeof(RippleSource))]
    public class Water2D_RippleSourceEditor : Editor
    {

        private bool showOptions = true;
        private static float pathScale = 1f;

        SerializedProperty active;
        SerializedProperty sourceMode;
        SerializedProperty ignoreYAxisPosition;
        SerializedProperty radius;
        SerializedProperty strength;
        SerializedProperty interactionDistance;
        SerializedProperty minPeriod;
        SerializedProperty maxPeriod;
        SerializedProperty frequency;
        SerializedProperty handleScale;
        //SerializedProperty rippleSimulationUpdate;
        //SerializedProperty rippleSimulationUpdate;
        //SerializedProperty rippleSimulationUpdate;

        private void LoadProperties()
        {
            active                      = serializedObject.FindProperty("active");
            sourceMode                  = serializedObject.FindProperty("sourceMode");
            ignoreYAxisPosition         = serializedObject.FindProperty("ignoreYAxisPosition");
            radius                      = serializedObject.FindProperty("radius");
            strength                    = serializedObject.FindProperty("strength");
            interactionDistance         = serializedObject.FindProperty("interactionDistance");
            minPeriod                   = serializedObject.FindProperty("minPeriod");
            maxPeriod                   = serializedObject.FindProperty("maxPeriod");
            frequency                   = serializedObject.FindProperty("frequency");
            handleScale                 = serializedObject.FindProperty("handleScale");
            //rippleSimulationUpdate = serializedObject.FindProperty("rippleSimulationUpdate");
            //rippleSimulationUpdate = serializedObject.FindProperty("rippleSimulationUpdate");
            //rippleSimulationUpdate = serializedObject.FindProperty("rippleSimulationUpdate");
            //rippleSimulationUpdate = serializedObject.FindProperty("rippleSimulationUpdate");
        }

        private void OnEnable()
        {
            LoadProperties();
        }

        private void OnSceneGUI()
        {     
            RippleSource rippleSource = (RippleSource)target;
            Transform targetTransform = rippleSource.GetComponent<Transform>();

            Handles.color = new Color(0,0,1f, 0.1f);
            Handles.DrawSolidDisc(targetTransform.position, Vector3.up, rippleSource.radius);

            Handles.color = Color.blue;
            Vector3 pos = targetTransform.position + new Vector3(0, 0, -rippleSource.radius);
            Vector3 newWorldPos = Handles.Slider(pos, Vector3.back, HandleScale(pos), Handles.SphereHandleCap, 0);
            rippleSource.radius = Mathf.Abs(targetTransform.position.z - newWorldPos.z);

            Handles.color = Color.green;

            pos = targetTransform.position + new Vector3(0, rippleSource.interactionDistance, 0);
            newWorldPos = Handles.Slider(pos, Vector3.up, HandleScale(pos), Handles.CubeHandleCap, 0);

            rippleSource.interactionDistance = Mathf.Abs(targetTransform.position.y - newWorldPos.y);

            pos = targetTransform.position + new Vector3(0, -rippleSource.interactionDistance, 0);
            newWorldPos = Handles.Slider(pos, Vector3.up, HandleScale(pos), Handles.CubeHandleCap, 0);
            rippleSource.interactionDistance = Mathf.Abs(targetTransform.position.y - newWorldPos.y);

            rippleSource.radius = (float)System.Math.Round(rippleSource.radius, 4);
            rippleSource.interactionDistance = (float)System.Math.Round(rippleSource.interactionDistance, 4);
        }

        public static float HandleScale(Vector3 aPos)
        {
            float dist = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.45f : GetCameraDist(aPos);
            return Mathf.Min(0.4f * pathScale, (dist / 5.0f) * 0.4f * pathScale);
        }

        public static float GetCameraDist(Vector3 aPt)
        {
            return Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, aPt);
        }

        public bool IsVisible(Vector3 aPos)
        {
            Transform t = SceneView.lastActiveSceneView.camera.transform;
            if (Vector3.Dot(t.forward, aPos - t.position) > 0)
                return true;
            return false;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "Modified Inspector");

            RippleSource rippleSource = (RippleSource)target;

            CustomInspector(rippleSource);

            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void CustomInspector(RippleSource rippleSource)
        {
            BoldFontStyle(() =>
            {
                showOptions = EditorGUILayout.Foldout(showOptions, "Ripple Properties");
            });

            if (showOptions)
            {
                InspectorBox(10, () =>
                {
                    EditorGUILayout.PropertyField(active, new GUIContent("Enabled", "Enables or disables ripple generation."));

                    if (rippleSource.active)
                    {
                        EditorGUILayout.PropertyField(sourceMode, new GUIContent("Ripple Generation", "List of options that control the behaviour of the sipple source object."));
                        EditorGUILayout.PropertyField(ignoreYAxisPosition, new GUIContent("Ignore Y Axis Position", "When enabled the position of the ripple source object on the Y axis is ignored."));
                        EditorGUILayout.PropertyField(radius, new GUIContent("Radius", "The initial radius of the ripple."));
                        rippleSource.radius = Mathf.Clamp(rippleSource.radius, 0, 100);
                        EditorGUILayout.PropertyField(strength, new GUIContent("Strength", "The strength of the ripple."));

                        if (!rippleSource.ignoreYAxisPosition)
                        {
                            EditorGUILayout.PropertyField(interactionDistance, new GUIContent("Interaction Distance", "If the distance from the water line to the ripple source is greater than the value of this field, ripples are not generated."));
                            rippleSource.interactionDistance = Mathf.Clamp(rippleSource.interactionDistance, 0, 100);
                        }

                        if (rippleSource.sourceMode == RippleSourceMode.RandomInterval)
                        {
                            EditorGUILayout.PropertyField(minPeriod, new GUIContent("Min Delta Time", "Minimum value for a random number that is used to calculate how long to wait before generating a new ripple."));
                            rippleSource.minPeriod = Mathf.Clamp(rippleSource.minPeriod, 0.00001f, 1000);
                            EditorGUILayout.PropertyField(maxPeriod, new GUIContent("Max Delta Time", "Maximum value for a random number that is used to calculate how long to wait before generating a new ripple."));
                            rippleSource.maxPeriod = Mathf.Clamp(rippleSource.maxPeriod, 0.00001f, 1000);
                        }

                        if (rippleSource.sourceMode == RippleSourceMode.FixedInterval)
                        {
                            EditorGUILayout.PropertyField(frequency, new GUIContent("Frequency", "The number of ripples per second."));
                            rippleSource.frequency = Mathf.Clamp(rippleSource.frequency, 0.0001f, 1000);
                        }

                        EditorGUILayout.PropertyField(handleScale, new GUIContent("Handle Scale", "Sets the scale of the water handles."));
                        pathScale = rippleSource.handleScale;
                    }
                });
            }
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

        public void BoldFontStyle(System.Action inside)
        {
            GUIStyle style = EditorStyles.foldout;
            FontStyle previousStyle = style.fontStyle;
            style.fontStyle = FontStyle.Bold;
            inside();
            style.fontStyle = previousStyle;
        }
    }
}
