using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Water2DTool {
    [CustomEditor(typeof(Water2D_Ripple))]
    public class Water2D_RippleEditor : Editor {

        SerializedProperty rippleSimulationUpdate;
        SerializedProperty rippleWaterFPSCap;
        SerializedProperty waterDamping;
        SerializedProperty rtPixelsToUnits;
        SerializedProperty bicubicResampling;
        SerializedProperty drawBothSides;
        SerializedProperty waveHeightScale;
        SerializedProperty obstructionType;
        SerializedProperty obstructionTexture;
        SerializedProperty fixedRadius;
        SerializedProperty objectRippleRadius;
        SerializedProperty objectRadiusScale;
        SerializedProperty objectRippleStrength;
        SerializedProperty strengthScale;
        SerializedProperty objectVelocityFilter;
        SerializedProperty objectXAxisRippleOffset;
        SerializedProperty playerRippleRadius;
        SerializedProperty playerRippleStrength;
        SerializedProperty playerVelocityFilter;
        SerializedProperty playerRippleXOffset;
        SerializedProperty rainDrops;
        SerializedProperty rainDropRadius;
        SerializedProperty rainDropStrength;
        SerializedProperty rainDropFrequency;
        SerializedProperty rippleSources;
        SerializedProperty rippleSourcesList;
        SerializedProperty mouseInteraction;
        SerializedProperty mouseRadius;
        SerializedProperty mouseStregth;
        SerializedProperty ambientWaves;
        SerializedProperty amplitudeZAxisFade;
        SerializedProperty amplitudeFadeStart;
        SerializedProperty amplitudeFadeEnd;
        SerializedProperty amplitude1;
        SerializedProperty waveLength1;
        SerializedProperty phaseOffset1;
        SerializedProperty amplitude2;
        SerializedProperty waveLength2;
        SerializedProperty phaseOffset2;
        SerializedProperty amplitude3;
        SerializedProperty waveLength3;
        SerializedProperty phaseOffset3;
        SerializedProperty rtFilterMode;

        private void LoadProperties()
        {
            rippleSimulationUpdate      = serializedObject.FindProperty("rippleSimulationUpdate");
            rippleWaterFPSCap           = serializedObject.FindProperty("rippleWaterFPSCap");
            waterDamping                = serializedObject.FindProperty("waterDamping");
            rtPixelsToUnits             = serializedObject.FindProperty("rtPixelsToUnits");
            bicubicResampling           = serializedObject.FindProperty("bicubicResampling");
            drawBothSides               = serializedObject.FindProperty("drawBothSides");
            waveHeightScale             = serializedObject.FindProperty("waveHeightScale");
            obstructionType             = serializedObject.FindProperty("obstructionType");
            obstructionTexture          = serializedObject.FindProperty("obstructionTexture");
            fixedRadius                 = serializedObject.FindProperty("fixedRadius");
            objectRippleRadius          = serializedObject.FindProperty("objectRippleRadius");
            objectRadiusScale           = serializedObject.FindProperty("objectRadiusScale");
            objectRippleStrength        = serializedObject.FindProperty("objectRippleStrength");
            strengthScale               = serializedObject.FindProperty("strengthScale");
            objectVelocityFilter        = serializedObject.FindProperty("objectVelocityFilter");
            objectXAxisRippleOffset     = serializedObject.FindProperty("objectXAxisRippleOffset");
            playerRippleRadius          = serializedObject.FindProperty("playerRippleRadius");
            playerRippleStrength        = serializedObject.FindProperty("playerRippleStrength");
            playerVelocityFilter        = serializedObject.FindProperty("playerVelocityFilter");
            playerRippleXOffset         = serializedObject.FindProperty("playerRippleXOffset");
            rainDrops                   = serializedObject.FindProperty("rainDrops");
            rainDropRadius              = serializedObject.FindProperty("rainDropRadius");
            rainDropStrength            = serializedObject.FindProperty("rainDropStrength");
            rainDropFrequency           = serializedObject.FindProperty("rainDropFrequency");
            rippleSources               = serializedObject.FindProperty("rippleSources");
            rippleSourcesList           = serializedObject.FindProperty("rippleSourcesList");
            mouseInteraction            = serializedObject.FindProperty("mouseInteraction");
            mouseRadius                 = serializedObject.FindProperty("mouseRadius");
            mouseStregth                = serializedObject.FindProperty("mouseStregth");
            ambientWaves                = serializedObject.FindProperty("ambientWaves");
            amplitudeZAxisFade          = serializedObject.FindProperty("amplitudeZAxisFade");
            amplitudeFadeStart          = serializedObject.FindProperty("amplitudeFadeStart");
            amplitudeFadeEnd            = serializedObject.FindProperty("amplitudeFadeEnd");
            amplitude1                  = serializedObject.FindProperty("amplitude1");
            waveLength1                 = serializedObject.FindProperty("waveLength1");
            phaseOffset1                = serializedObject.FindProperty("phaseOffset1");
            amplitude2                  = serializedObject.FindProperty("amplitude2");
            waveLength2                 = serializedObject.FindProperty("waveLength2");
            phaseOffset2                = serializedObject.FindProperty("phaseOffset2");
            amplitude3                  = serializedObject.FindProperty("amplitude3");
            waveLength3                 = serializedObject.FindProperty("waveLength3");
            phaseOffset3                = serializedObject.FindProperty("phaseOffset3");
            rtFilterMode                = serializedObject.FindProperty("rtFilterMode");
        }

        private void OnEnable()
        {
            LoadProperties();
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "Modified Inspector");

            Water2D_Ripple water2D_Ripple = (Water2D_Ripple)target;

            CustomInspector(water2D_Ripple);

            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void CustomInspector(Water2D_Ripple water2D_Ripple)
        {
            Water2D_Tool water2D = water2D_Ripple.GetComponent<Water2D_Tool>();
            Water2D_Simulation water2DSim = water2D_Ripple.GetComponent<Water2D_Simulation>();

            BoldFontStyle(() =>
            {
                water2D_Ripple.showWaterRipple = EditorGUILayout.Foldout(water2D_Ripple.showWaterRipple, "Water Properties");
            });

            if (water2D_Ripple.showWaterRipple)
            {
                InspectorBox(10, () =>
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(rippleSimulationUpdate, new GUIContent("Run Simulation In"));
                    if (water2D_Ripple.rippleSimulationUpdate == Water2D_RippleSimulationUpdate.FixedUpdateMethod)
                        EditorGUILayout.HelpBox("For smooth looking wave movement it is recommended to use Fixed Update Method with physics based character controllers.", MessageType.Info);

                    if (water2D_Ripple.rippleSimulationUpdate == Water2D_RippleSimulationUpdate.UpdateMethod)
                        EditorGUILayout.HelpBox("For smooth looking wave movement it is recommended to use Update Method with raycast based character controllers.", MessageType.Info);

                    if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                    {
                        water2D_Ripple.rainTimeCounter = 0;
                        water2D_Ripple.heightMapTimeCounter = 0;
                    }

                    if (water2D_Ripple.rippleSimulationUpdate == Water2D_RippleSimulationUpdate.UpdateMethod)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(rippleWaterFPSCap, new GUIContent("Simulation Speed", "Determines how many times per second should the height map be processed through the shader that simulates ripple propagation."));
                        water2D_Ripple.rippleWaterFPSCap = Mathf.Clamp(water2D_Ripple.rippleWaterFPSCap, 1, 240);

                        if (EditorGUI.EndChangeCheck())
                        {
                            water2D_Ripple.heightUpdateMapTimeStep = 1f / water2D_Ripple.rippleWaterFPSCap;
                            water2D_Ripple.heightMapTimeCounter = 0;
                            water2D_Ripple.interactionsTimeCounter = 0;
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(waterDamping, new GUIContent("Damping", "Damping parameter for the water propagation simulation."));
                    water2D_Ripple.waterDamping = Mathf.Clamp(water2D_Ripple.waterDamping, 0, 1);
                    if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                    {
                        water2D_Ripple.UpdateRippleShaderParameters();
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(rtPixelsToUnits, new GUIContent("RT Pixels To Units", "The number of Render Texture pixels that should fit in 1 unit of Unity space."));
                    if (EditorGUI.EndChangeCheck())
                    {
                        water2D.RecreateWaterMesh();
                    }
                    EditorGUILayout.PropertyField(rtFilterMode, new GUIContent("RT Filter Mode"));

                    EditorGUILayout.PropertyField(bicubicResampling, new GUIContent("Bicubic Resampling", "Applies a smoothing effect to the heightmap."
                        + " Eliminates pixelation artifacts. This makes the generated normal map look smoother."));

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(drawBothSides, new GUIContent("Draw Both Sides", "Enabling this will make both sides of the top mesh to be drawn when the camera is below the water line."));
                    if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                    {
                        water2D_Ripple.SetTopMeshCulling();
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(waveHeightScale, new GUIContent("Wave Height Scale", "The scale of the ripples created by objects or rain."));
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (Application.isPlaying)
                        {
                            water2D_Ripple.SetWaveHeightScale();
                        }else
                        {
                            water2D.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveHeightScale", water2D_Ripple.waveHeightScale);
                            water2D.topMeshGameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveHeightScale", water2D_Ripple.waveHeightScale);
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(obstructionType, new GUIContent("Obstruction Type", "List of obstruction methods."));
                    if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                    {
                        water2D_Ripple.UpdateRippleShaderParameters();
                    }

                    if (water2D_Ripple.obstructionType == Water2D_ObstructionType.DynamicObstruction && !water2D.use3DCollider)
                        EditorGUILayout.HelpBox("Dynamic Obstruction only works with objects that have a Box Collider or a Sphere Collider", MessageType.Warning);
                    if (water2D_Ripple.obstructionType == Water2D_ObstructionType.DynamicObstruction)
                        water2D_Ripple.obstructionLayers = (LayerMask)EditorGUILayout.MaskField("Obstruction Layers", water2D_Ripple.obstructionLayers, InternalEditorUtility.layers);

                    if (water2D_Ripple.obstructionType == Water2D_ObstructionType.TextureObstruction)
                        EditorGUILayout.PropertyField(obstructionTexture, new GUIContent("Obstruction Texture"));
                });
            }

            BoldFontStyle(() =>
            {
                water2D_Ripple.showDynamicObjects = EditorGUILayout.Foldout(water2D_Ripple.showDynamicObjects, "Dynamic Objects");
            });

            if (water2D_Ripple.showDynamicObjects)
            {
                InspectorBox(10, () =>
                {
                    if (water2D.use3DCollider)
                        EditorGUILayout.PropertyField(fixedRadius, new GUIContent("Fixed Radius", "When enabled, the radius of the ripple created by dynamic objects will be the same for all objects. "
                            + "When disabled the radius of the ripple depends on the collider of that object."));
     
                    if (!water2D.use3DCollider && !water2D_Ripple.fixedRadius)
                        water2D_Ripple.fixedRadius = true;

                    if (water2D_Ripple.fixedRadius)
                    {
                        EditorGUILayout.PropertyField(objectRippleRadius, new GUIContent("Radius", "The radius of the ripple created by a dynamic object."));
                        water2D_Ripple.objectRippleRadius = Mathf.Clamp(water2D_Ripple.objectRippleRadius, 0.000001f, 100f);
                    }

                    if (!water2D_Ripple.fixedRadius)
                    {
                        EditorGUILayout.PropertyField(objectRadiusScale, new GUIContent("Radius Scale", "Scales up or down the ripple radius. "
                            + "The radius of the ripple created by dynamic objects depends on the size of that objects collider."));
                        water2D_Ripple.objectRadiusScale = Mathf.Clamp(water2D_Ripple.objectRadiusScale, 0.000001f, 100f);
                    }

                    EditorGUILayout.PropertyField(objectRippleStrength, new GUIContent("Strength", "The strength of the ripple created by dynamic objects."));
                    EditorGUILayout.PropertyField(strengthScale, new GUIContent("Y Axis Strength Scale", "Used to scale up or down the strength of the ripples " +
                        "created by dynamic objects when the Abs value of the velocity on the Y axis is greater than the Abs value of the velocity on the X axis."));
                    water2D_Ripple.strengthScale = Mathf.Clamp(water2D_Ripple.strengthScale, 0.000001f, 1000f);

                    EditorGUILayout.PropertyField(objectVelocityFilter, new GUIContent("Velocity Filter", "If the Abs value of a dynamic object velocity on the X and Y axis is smaller than the value of this field, no ripples will be generated by that object."));
                    EditorGUILayout.PropertyField(objectXAxisRippleOffset, new GUIContent("X Axis Relative Offset", "Offsets the ripple position on the X axis based on the width of the colliders bounding box. A value of 0 means that the ripple will be generated at the center of the collider. " +
                        " A value of 0.5f means that the ripple will be positioned at the left or right edge of the colliders bounding box."));
                    water2D_Ripple.objectXAxisRippleOffset = Mathf.Clamp(water2D_Ripple.objectXAxisRippleOffset, 0, 100f);
                });
            }

            EditorGUI.indentLevel = 0;

            BoldFontStyle(() =>
            {
                water2D_Ripple.showPlayer = EditorGUILayout.Foldout(water2D_Ripple.showPlayer, "Player");
            });

            if (water2D_Ripple.showPlayer)
            {
                InspectorBox(10, () =>
                {
                    EditorGUILayout.PropertyField(playerRippleRadius, new GUIContent("Radius", "The radius of the ripple created by the player."));
                    water2D_Ripple.playerRippleRadius = Mathf.Clamp(water2D_Ripple.playerRippleRadius, 0.0000001f, 1000f);
                    EditorGUILayout.PropertyField(playerRippleStrength, new GUIContent("Strength", "The strength of the ripple created by the player."));

                    if (water2DSim.characterControllerType == Water2D_CharacterControllerType.PhysicsBased)
                    {
                        EditorGUILayout.PropertyField(playerVelocityFilter, new GUIContent("Velocity Filter", "If the Abs value of the player velocity on the X and Y axis is smaller than the value of this field, no ripples will be generated by the player."));
                        water2D_Ripple.playerVelocityFilter = Mathf.Clamp(water2D_Ripple.playerVelocityFilter, 0.0000001f, 100f);
                    }
                    EditorGUILayout.PropertyField(playerRippleXOffset, new GUIContent("X Axis Position Offset", "Offsets the position of the ripple created by the player on the X axis."));
                    water2D_Ripple.playerRippleXOffset = Mathf.Clamp(water2D_Ripple.playerRippleXOffset, 0, 1000f);
                });
            }

            EditorGUI.indentLevel = 0;

            BoldFontStyle(() =>
            {
                water2D_Ripple.showRain = EditorGUILayout.Foldout(water2D_Ripple.showRain, "Rain");
            });

            if (water2D_Ripple.showRain)
            {
                InspectorBox(10, () =>
                {
                    EditorGUILayout.PropertyField(rainDrops, new GUIContent("Enable Rain", "Enables the simulation of rain."));

                    if (water2D_Ripple.rainDrops)
                    {
                        EditorGUILayout.PropertyField(rainDropRadius, new GUIContent("Drop Radius", "The radius of a rain drop in Unity space."));
                        EditorGUILayout.PropertyField(rainDropStrength, new GUIContent("Drop Strength", "The strength of a rain drop."));
                        EditorGUILayout.PropertyField(rainDropFrequency, new GUIContent("Drop Frequency", "The number of water drops that should fall in a second."));
                        water2D_Ripple.rainDropFrequency = Mathf.Clamp(water2D_Ripple.rainDropFrequency, 0.0001f, 1000f);
                    }
                });
            }

            BoldFontStyle(() =>
            {
                water2D_Ripple.showRippleSourcesList = EditorGUILayout.Foldout(water2D_Ripple.showRippleSourcesList, "Ripple Sources");
            });

            if (water2D_Ripple.showRippleSourcesList)
            {
                InspectorBox(10, () =>
                {
                    EditorGUILayout.PropertyField(rippleSources, new GUIContent("Enable Ripple Sources"));

                    if (water2D_Ripple.rippleSources)
                    {
                        EditorGUI.indentLevel = 1;
                        EditorGUILayout.PropertyField(rippleSourcesList, new GUIContent("Ripple Sources"), true);
                        EditorGUI.indentLevel = 0;
                    }
                });
            }

            if (water2D.use3DCollider)
            {
                BoldFontStyle(() =>
                {
                    water2D_Ripple.showMouse = EditorGUILayout.Foldout(water2D_Ripple.showMouse, "Mouse Interaction");
                });

                if (water2D_Ripple.showMouse)
                {
                    InspectorBox(10, () =>
                    {
                        EditorGUILayout.PropertyField(mouseInteraction, new GUIContent("Enable Mouse Interaction", "Enables the ability to interact with the water using the mouse arrow."));

                        if (water2D_Ripple.mouseInteraction)
                        {
                            EditorGUILayout.PropertyField(mouseRadius, new GUIContent("Mouse Radius", "The radius of the ripple created by the mouse arrow, in Unity space."));
                            EditorGUILayout.PropertyField(mouseStregth, new GUIContent("Mouse Strength", "The strength of the ripple created by the mouse arrow."));
                        }
                    });
                }
            }

            BoldFontStyle(() =>
            {
                water2D_Ripple.showAmbiantWaves = EditorGUILayout.Foldout(water2D_Ripple.showAmbiantWaves, "Ambient Waves");
            });

            if (water2D_Ripple.showAmbiantWaves)
            {
                InspectorBox(10, () =>
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(ambientWaves, new GUIContent("Enable Ambient Waves", "Enable this to simulate waves created by the wind."));

                    if (water2D_Ripple.ambientWaves)
                    {
                        EditorGUILayout.PropertyField(amplitudeZAxisFade, new GUIContent("Z Axis Amplitude Fade", "When enabled the amplitude of the sine waves will fade along the Z axis."));

                        if (water2D_Ripple.amplitudeZAxisFade)
                        {
                            EditorGUILayout.PropertyField(amplitudeFadeStart, new GUIContent("Fade Start", "The start point of the amplitude fade. Must be a value between 0 and 1. Can also be used to set the direction of the amplitude fade." 
                                + " If the start point value is greater than the end point, the amplitude will fade from back to front."));
                            EditorGUILayout.PropertyField(amplitudeFadeEnd, new GUIContent("Fade End", "The end point of the amplitude fade. Must be a value between 0 and 1. Can also be used to set the direction of the amplitude fade. " 
                                + "If the end point value is greater than the start point, the amplitude will fade towards the back of the water."));
                        }

                        EditorGUILayout.LabelField(new GUIContent("Wave 1"), EditorStyles.boldLabel);

                        EditorGUILayout.PropertyField(amplitude1, new GUIContent("Amplitude", "Sine wave amplitude. The bigger the value the higher the wave top will be."));
                        EditorGUILayout.PropertyField(waveLength1, new GUIContent("Wave Length", "The distance between 2 consecutive points of a sine wave."));
                        EditorGUILayout.PropertyField(phaseOffset1, new GUIContent("Phase Offset", "Sine wave phase offset. The bigger the value of the phase offset the faster the waves move to the left (right)."));

                        EditorGUILayout.LabelField(new GUIContent("Wave 2"), EditorStyles.boldLabel);

                        EditorGUILayout.PropertyField(amplitude2, new GUIContent("Amplitude", "Sine wave amplitude. The bigger the value the higher the wave top will be."));
                        EditorGUILayout.PropertyField(waveLength2, new GUIContent("Wave Length", "The distance between 2 consecutive points of a sine wave."));
                        EditorGUILayout.PropertyField(phaseOffset2, new GUIContent("Phase Offset", "Sine wave phase offset. The bigger the value of the phase offset the faster the waves move to the left (right)."));

                        EditorGUILayout.LabelField(new GUIContent("Wave 3"), EditorStyles.boldLabel);

                        EditorGUILayout.PropertyField(amplitude3, new GUIContent("Amplitude", "Sine wave amplitude. The bigger the value the higher the wave top will be."));
                        EditorGUILayout.PropertyField(waveLength3, new GUIContent("Wave Length", "The distance between 2 consecutive points of a sine wave."));
                        EditorGUILayout.PropertyField(phaseOffset3, new GUIContent("Phase Offset", "Sine wave phase offset. The bigger the value of the phase offset the faster the waves move to the left (right)."));
                    }
                    if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                    {
                        water2D_Ripple.SetAmbientWavesShaderParameters();
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
