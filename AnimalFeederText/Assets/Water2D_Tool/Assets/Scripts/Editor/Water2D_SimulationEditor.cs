using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Water2DTool
{
    [CustomEditor(typeof(Water2D_Simulation))]
    public class Water2D_SimulationEditor : Editor
    {    
        SerializedProperty springSimulation;
        SerializedProperty springConstant;
        SerializedProperty damping;
        SerializedProperty spread;
        SerializedProperty collisionVelocityScale;
        SerializedProperty waveSpeed;
        SerializedProperty overlapSphereRadius;
        SerializedProperty interactionTime;
        SerializedProperty buoyantForceMode;
        SerializedProperty floatHeight;
        SerializedProperty bounceDamping;
        SerializedProperty linearBFDragCoefficient;
        SerializedProperty linearBFAbgularDragCoefficient;
        SerializedProperty forceScale;
        SerializedProperty forcePositionOffset;
        SerializedProperty clippingMethod;
        SerializedProperty showClippingPlolygon;
        SerializedProperty meshSegmentsPerWaterLineSegment;
        SerializedProperty polygonCorners;
        SerializedProperty dragCoefficient;
        SerializedProperty liftCoefficient;
        SerializedProperty waterDensity;
        SerializedProperty showPolygon;
        SerializedProperty showForces;
        SerializedProperty waterFlow;
        SerializedProperty useAngles;
        SerializedProperty flowDirection;
        SerializedProperty flowAngle;
        SerializedProperty waterFlowForce;
        SerializedProperty animationMethod;
        SerializedProperty animateWaterArea;
        SerializedProperty topEdge;
        SerializedProperty bottomEdge;
        SerializedProperty leftEdge;
        SerializedProperty rightEdge;
        SerializedProperty topEdgeYOffset;
        SerializedProperty bottomEdgeYOffset;
        SerializedProperty leftEdgeXOffset;
        SerializedProperty rightEdgeXOffset;
        SerializedProperty surfaceWaves;
        SerializedProperty sineWavesType;
        SerializedProperty randomValues;
        SerializedProperty sineWaves;
        SerializedProperty maxAmplitude;
        SerializedProperty minAmplitude;
        SerializedProperty maxStretch;
        SerializedProperty minStretch;
        SerializedProperty maxPhaseOffset;
        SerializedProperty minPhaseOffset;
        SerializedProperty waveAmplitude;
        SerializedProperty waveStretch;
        SerializedProperty wavePhaseOffset;
        SerializedProperty sineWaveVelocityScale;
        SerializedProperty timeStep;
        SerializedProperty maxVelocity;
        SerializedProperty minVelocity;
        SerializedProperty neighborVertVelocityScale;
        SerializedProperty characterControllerType;
        SerializedProperty playerBoundingBoxSize;
        SerializedProperty playerBoundingBoxCenter;
        SerializedProperty playerBuoyantForceScale;
        SerializedProperty playerOnExitPSAndSound;
        SerializedProperty playerOnExitRipple;
        SerializedProperty playerOnExitVelocity;
        SerializedProperty rippleWidth;
        SerializedProperty playerOnEnterVelocity;
        SerializedProperty waterType;
        SerializedProperty collisionLayers;
        SerializedProperty collisionDetectionMode;
        SerializedProperty raycastDistance;
        SerializedProperty velocityFilter;
        SerializedProperty interactionRegion;
        SerializedProperty waterDisplacement;
        SerializedProperty constantWaterArea;
        SerializedProperty particleS;
        SerializedProperty particleSystemPosOffset;
        SerializedProperty particleSystemSorting;
        SerializedProperty particleSystemSortingLayerName;
        SerializedProperty particleSystemOrderInLayer;
        SerializedProperty splashSound;

        private void LoadProperties()
        {
            springSimulation                = serializedObject.FindProperty("springSimulation");
            springConstant                  = serializedObject.FindProperty("springConstant");
            damping                         = serializedObject.FindProperty("damping");
            spread                          = serializedObject.FindProperty("spread");
            collisionVelocityScale          = serializedObject.FindProperty("collisionVelocityScale");
            waveSpeed                       = serializedObject.FindProperty("waveSpeed");
            overlapSphereRadius             = serializedObject.FindProperty("overlapSphereRadius");
            interactionTime                 = serializedObject.FindProperty("interactionTime");
            buoyantForceMode                = serializedObject.FindProperty("buoyantForceMode");
            floatHeight                     = serializedObject.FindProperty("floatHeight");
            bounceDamping                   = serializedObject.FindProperty("bounceDamping");
            linearBFDragCoefficient         = serializedObject.FindProperty("liniarBFDragCoefficient");
            linearBFAbgularDragCoefficient  = serializedObject.FindProperty("linearBFAbgularDragCoefficient");
            forceScale                      = serializedObject.FindProperty("forceScale");
            forcePositionOffset             = serializedObject.FindProperty("forcePositionOffset");
            clippingMethod                  = serializedObject.FindProperty("clippingMethod");
            showClippingPlolygon            = serializedObject.FindProperty("showClippingPlolygon");
            meshSegmentsPerWaterLineSegment = serializedObject.FindProperty("meshSegmentsPerWaterLineSegment");
            polygonCorners                  = serializedObject.FindProperty("polygonCorners");
            dragCoefficient                 = serializedObject.FindProperty("dragCoefficient");
            liftCoefficient                 = serializedObject.FindProperty("liftCoefficient");
            waterDensity                    = serializedObject.FindProperty("waterDensity");
            showPolygon                     = serializedObject.FindProperty("showPolygon");
            showForces                      = serializedObject.FindProperty("showForces");
            waterFlow                       = serializedObject.FindProperty("waterFlow");
            useAngles                       = serializedObject.FindProperty("useAngles");
            flowDirection                   = serializedObject.FindProperty("flowDirection");
            flowAngle                       = serializedObject.FindProperty("flowAngle");
            waterFlowForce                  = serializedObject.FindProperty("waterFlowForce");
            animationMethod                 = serializedObject.FindProperty("animationMethod");
            animateWaterArea                = serializedObject.FindProperty("animateWaterArea");
            topEdge                         = serializedObject.FindProperty("topEdge");
            bottomEdge                      = serializedObject.FindProperty("bottomEdge");
            leftEdge                        = serializedObject.FindProperty("leftEdge");
            rightEdge                       = serializedObject.FindProperty("rightEdge");
            topEdgeYOffset                  = serializedObject.FindProperty("topEdgeYOffset");
            bottomEdgeYOffset               = serializedObject.FindProperty("bottomEdgeYOffset");
            leftEdgeXOffset                 = serializedObject.FindProperty("leftEdgeXOffset");
            rightEdgeXOffset                = serializedObject.FindProperty("rightEdgeXOffset");
            surfaceWaves                    = serializedObject.FindProperty("surfaceWaves");
            sineWavesType                   = serializedObject.FindProperty("sineWavesType");
            randomValues                    = serializedObject.FindProperty("randomValues");
            sineWaves                       = serializedObject.FindProperty("sineWaves");
            maxAmplitude                    = serializedObject.FindProperty("maxAmplitude");
            minAmplitude                    = serializedObject.FindProperty("minAmplitude");
            maxStretch                      = serializedObject.FindProperty("maxStretch");
            minStretch                      = serializedObject.FindProperty("minStretch");
            maxPhaseOffset                  = serializedObject.FindProperty("maxPhaseOffset");
            minPhaseOffset                  = serializedObject.FindProperty("minPhaseOffset");
            waveAmplitude                   = serializedObject.FindProperty("waveAmplitude");
            waveStretch                     = serializedObject.FindProperty("waveStretch");
            wavePhaseOffset                 = serializedObject.FindProperty("wavePhaseOffset");
            sineWaveVelocityScale           = serializedObject.FindProperty("sineWaveVelocityScale");
            timeStep                        = serializedObject.FindProperty("timeStep");
            maxVelocity                     = serializedObject.FindProperty("maxVelocity");
            minVelocity                     = serializedObject.FindProperty("minVelocity");
            neighborVertVelocityScale       = serializedObject.FindProperty("neighborVertVelocityScale");
            characterControllerType         = serializedObject.FindProperty("characterControllerType");
            playerBoundingBoxSize           = serializedObject.FindProperty("playerBoundingBoxSize");
            playerBoundingBoxCenter         = serializedObject.FindProperty("playerBoundingBoxCenter");
            playerBuoyantForceScale         = serializedObject.FindProperty("playerBuoyantForceScale");
            playerOnExitPSAndSound          = serializedObject.FindProperty("playerOnExitPSAndSound");
            playerOnExitRipple              = serializedObject.FindProperty("playerOnExitRipple");
            playerOnExitVelocity            = serializedObject.FindProperty("playerOnExitVelocity");
            rippleWidth                     = serializedObject.FindProperty("rippleWidth");
            playerOnEnterVelocity           = serializedObject.FindProperty("playerOnEnterVelocity");
            waterType                       = serializedObject.FindProperty("waterType");
            collisionLayers                 = serializedObject.FindProperty("collisionLayers");
            collisionDetectionMode          = serializedObject.FindProperty("collisionDetectionMode");
            raycastDistance                 = serializedObject.FindProperty("raycastDistance");
            velocityFilter                  = serializedObject.FindProperty("velocityFilter");
            interactionRegion               = serializedObject.FindProperty("interactionRegion");
            waterDisplacement               = serializedObject.FindProperty("waterDisplacement");
            constantWaterArea               = serializedObject.FindProperty("constantWaterArea");
            particleS                       = serializedObject.FindProperty("particleS");
            particleSystemPosOffset         = serializedObject.FindProperty("particleSystemPosOffset");
            particleSystemSorting           = serializedObject.FindProperty("particleSystemSorting");
            particleSystemSortingLayerName  = serializedObject.FindProperty("particleSystemSortingLayerName");
            particleSystemOrderInLayer      = serializedObject.FindProperty("particleSystemOrderInLayer");
            splashSound                     = serializedObject.FindProperty("splashSound");
        }

        private void OnEnable()
        {
            LoadProperties();
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "Modified Inspector");

            Water2D_Simulation water2D_Sim = (Water2D_Simulation)target;

            CustomInspector(water2D_Sim);

            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void CustomInspector(Water2D_Simulation water2D_Sim)
        {
            Water2D_Ripple rippleScript = water2D_Sim.GetComponent<Water2D_Ripple>();

            if (!rippleScript)
            {
                BoldFontStyle(() =>
                {
                    water2D_Sim.showSpringProperties = EditorGUILayout.Foldout(water2D_Sim.showSpringProperties, "Spring");
                });
            }

            if (water2D_Sim.showSpringProperties && !rippleScript)
            {
                InspectorBox(10, () =>
                {
                     
                    EditorGUILayout.PropertyField(springSimulation, new GUIContent("Spring Simulation", "Enables the simulation of springs. This what makes the surface of the water react to objects."));

                    if (water2D_Sim.springSimulation)
                    {
                        EditorGUILayout.PropertyField(springConstant, new GUIContent("Spring Constant", "This value controls the stiffness of the springs. "
                            + "A low spring constant will make the springs loose. This means a force will cause large waves that oscillate slowly. A high spring "
                            + "constant will increase the tension in the spring. Forces will create small waves that oscillate quickly."));

                        EditorGUILayout.PropertyField(damping, new GUIContent("Damping", "The damping slows down the oscillation of the springs. "
                            + " A high dampening value will make the water look thick like molasses, while a low value will allow the waves to oscillate for a long time."));

                        EditorGUILayout.PropertyField(spread, new GUIContent("Spread", "Controls how fast the waves spread."));

                        EditorGUILayout.PropertyField(collisionVelocityScale, new GUIContent("Collision Velocity", "Limits the velocity "
                            + " a spring will receive from a falling object."));

                        EditorGUILayout.PropertyField(waveSpeed, new GUIContent("Wave Speed", "Another variable to control the spread speed of the waves."));

                        Water2D_Tool water2D_Tool = water2D_Sim.GetComponent<Water2D_Tool>();

                        if (water2D_Tool.use3DCollider)
                            EditorGUILayout.PropertyField(overlapSphereRadius, new GUIContent("Overlap Sphere Radius", "The radius of a sphere that will be used to check "
                                + " if there is a 3D collider near a surface vertex."));

                        EditorGUILayout.PropertyField(interactionTime, new GUIContent("Spring Simulation Time", "How many seconds after an object interacted with the water surface and generated a ripple, should the spring simulation stop updating."));
                        water2D_Sim.interactionTime = Mathf.Clamp(water2D_Sim.interactionTime, 0.00001f, 1000);
                    }
                });
            }

            if (water2D_Sim.waterType == Water2D_Type.Dynamic)
            {
                BoldFontStyle(() =>
                {
                    water2D_Sim.showfloatingBuoyantForce = EditorGUILayout.Foldout(water2D_Sim.showfloatingBuoyantForce, "Buoyancy");
                });

                if (water2D_Sim.showfloatingBuoyantForce)
                {
                    InspectorBox(10, () =>
                    {
                        EditorGUILayout.PropertyField(buoyantForceMode, new GUIContent("Buoyant Force", "List of methods to simulate the buoyant force. "
                            + "This is what makes the objects float in the water"));

                        if (water2D_Sim.buoyantForceMode != Water2D_BuoyantForceMode.None)
                        {
                            if (water2D_Sim.buoyantForceMode == Water2D_BuoyantForceMode.Linear)
                            {
                                EditorGUILayout.PropertyField(floatHeight, new GUIContent("Float Height", "Determines how much force should be applied to an object submerged "
                                    + "in the water. A value of 3 means that 3 m under the water the force applied to an object will be 2 times greater than the force applied at the "
                                    + "surface of the water."));
                                water2D_Sim.floatHeight = Mathf.Clamp(water2D_Sim.floatHeight, 0.0001f, 100);

                                EditorGUILayout.PropertyField(bounceDamping, new GUIContent("Bounce Damping", "Slows down the vertical oscillation of the object."));
                                EditorGUILayout.PropertyField(linearBFDragCoefficient, new GUIContent("Drag Coefficient", "Determines how much drag force should be applied to an object."));
                                EditorGUILayout.PropertyField(linearBFAbgularDragCoefficient, new GUIContent("Angular Drag Coefficient", "Slow down the angular rotation of the object."));

                                EditorGUILayout.PropertyField(forceScale, new GUIContent("Force Scale", "A value of 1 will make an object with the mass of "
                                    + " 1kg float at the surface of the water and an object with the mass of 2kg float 3m below the water surface if Float Height "
                                    + "is set to 3m."));

                                EditorGUILayout.PropertyField(forcePositionOffset, new GUIContent("Force Position Offset", "By default the force will "
                                    + " be applied at the center of the object. Use this to offset the position where the force will be applied to an object."));
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(clippingMethod, new GUIContent("Polygon Clipping", "Determines which clipping method will be used to calculate the shape of the polygon that is below the water. "
                                    + "The simple clipping is the cheapest option in terms of performance because the clipping polygon is always a horizontal line."
                                    + "The complex option is best to use when you want the objects to better react to water waves."));

                                if (water2D_Sim.clippingMethod == Water2D_ClippingMethod.Complex)
                                {
                                    EditorGUILayout.PropertyField(showClippingPlolygon, new GUIContent("Show Clipping Polygon", "When enabled will show in the Scene View the shape of the clipping polygon."));
                                    EditorGUILayout.PropertyField(meshSegmentsPerWaterLineSegment, new GUIContent("Water Line Segments", "The number of vertical mesh segments that should fit in a water line segment."));
                                    water2D_Sim.meshSegmentsPerWaterLineSegment = Mathf.Clamp(water2D_Sim.meshSegmentsPerWaterLineSegment, 1, 1000);
                                }

                                EditorGUILayout.PropertyField(polygonCorners, new GUIContent("Polygon Vertices", "When an object with a circleCollider2D "
                                    + "is detected an imaginary regular polygon collider is created based on its radius and position. "
                                    + "Use this to set the number of vertices the regular polygon collider should have."));
                                water2D_Sim.polygonCorners = Mathf.Clamp(water2D_Sim.polygonCorners, 4, 100);

                                EditorGUILayout.PropertyField(dragCoefficient, new GUIContent("Drag Coefficient", "Determines how much drag force should be applied to an object."));
                                water2D_Sim.dragCoefficient = Mathf.Clamp(water2D_Sim.dragCoefficient, 0, 500);
                                EditorGUILayout.PropertyField(liftCoefficient, new GUIContent("Lift Coefficient", "Determines how much lift force should be applied to an object."));
                                water2D_Sim.liftCoefficient = Mathf.Clamp(water2D_Sim.liftCoefficient, 0, 500);

                                EditorGUILayout.PropertyField(waterDensity, new GUIContent("Water Density", "Sets the water density. In a water with low "
                                    + " density the objects will submerge faster and come to the surface slower. If the water density is great the objects will "
                                    + "stay more at the surface of the water and will submerge slower."));

                                EditorGUILayout.PropertyField(showPolygon, new GUIContent("Show Polygon Shape", "When enabled will show in the Scene View the shape of the polygon that is below the waterline."));
                                EditorGUILayout.PropertyField(showForces, new GUIContent("Show Forces", "When enabled will show in the Scene View the velocity direction, drag direction, "
                                    + "lift direction and the normal of a leading edge."));
                            }
                        }
                    });
                }
            }

            BoldFontStyle(() =>
            {            
                water2D_Sim.showFlow = EditorGUILayout.Foldout(water2D_Sim.showFlow, "Flow");
            });

            if (water2D_Sim.showFlow)
            {
                InspectorBox(10, () =>
                {
                    EditorGUILayout.PropertyField(waterFlow, new GUIContent("Water Flow", "When enabled, the water flow will affect the objects in the water."));
               
                    if (water2D_Sim.waterFlow)
                    {
                        EditorGUILayout.PropertyField(useAngles, new GUIContent("Use Angles", "Enable this if you want to control the direction of the water flow using custom angle values."));

                        if (!water2D_Sim.useAngles)
                            EditorGUILayout.PropertyField(flowDirection, new GUIContent("Flow Direction", "The direction of the water flow."));

                        if (water2D_Sim.useAngles)
                            EditorGUILayout.PropertyField(flowAngle, new GUIContent("Flow Angle", "The angle of the water flow. " + "When set to 0 degrees the objects will be pushed to the left, "
                                + "when set to 90 degrees the objects will be pushed down, when set to 180 degrees the objects will "
                                + "be pushed to the right, when set to 270 degrees the objects will be pushed up."));

                        EditorGUILayout.PropertyField(waterFlowForce, new GUIContent("Flow Force", "The force of the water flow."));
                    }
                });
            }

            BoldFontStyle(() =>
            {
                water2D_Sim.showAnimation = EditorGUILayout.Foldout(water2D_Sim.showAnimation, "Animation");
            });

            if (water2D_Sim.showAnimation)
            {
                InspectorBox(10, () =>
                {
                    EditorGUILayout.PropertyField(animationMethod, new GUIContent("Animation Method", "Determines the animation method for the handles position."));

                    if (water2D_Sim.animationMethod != Water2D_AnimationMethod.None)
                    {
                        if (!rippleScript)
                            EditorGUILayout.PropertyField(animateWaterArea, new GUIContent("Animate Water Area", "Enable this "
                                + "if you want to animate the increase or decrease of the total water area."));

                        if (!water2D_Sim.animateWaterArea)
                        {
                            EditorGUILayout.PropertyField(topEdge, new GUIContent("Top Edge", "Place here an animated object "
                                + "you want the water line (the top of the water) to follow."));
                        }

                        if (!rippleScript)
                        {
                            EditorGUILayout.PropertyField(bottomEdge, new GUIContent("Bottom Edge", "Place here an animated object "
                                + "you want the bottom edge of the water to follow."));
                            EditorGUILayout.PropertyField(leftEdge, new GUIContent("Left Edge", "Place here an animated object you "
                                + " want the left edge of the water to follow."));
                            EditorGUILayout.PropertyField(rightEdge, new GUIContent("Right Edge", "Place here an animated object "
                                + "you want the right edge of the water to follow."));
                        }

                        if (water2D_Sim.animationMethod == Water2D_AnimationMethod.Snap)
                        {
                            if (!water2D_Sim.animateWaterArea)
                                EditorGUILayout.PropertyField(topEdgeYOffset, new GUIContent("Top Edge Y Offset", "The offset on the Y axis from the position of a referenced object."));

                            if (!rippleScript)
                            {
                                EditorGUILayout.PropertyField(bottomEdgeYOffset, new GUIContent("Bottom Edge Y Offset", "The offset on the Y axis from the position of a referenced object."));
                                EditorGUILayout.PropertyField(leftEdgeXOffset, new GUIContent("Left Edge X Offset", "The offset on the X axis from the position of a referenced object."));
                                EditorGUILayout.PropertyField(rightEdgeXOffset, new GUIContent("Right Edge X Offset", "The offset on the X axis from the position of a referenced object."));
                            }
                        }

                        if (water2D_Sim.animateWaterArea && water2D_Sim.topEdge != null)
                            water2D_Sim.topEdge = null;
                    }
                });
            }

            if (!rippleScript)
            {
                BoldFontStyle(() =>
                {
                    water2D_Sim.showSurfaceWaves = EditorGUILayout.Foldout(water2D_Sim.showSurfaceWaves, "Surface Waves");
                });
            }

            if (water2D_Sim.showSurfaceWaves && !rippleScript)
            {
                InspectorBox(10, () =>
                {
                    EditorGUILayout.PropertyField(surfaceWaves, new GUIContent("Surface Waves", "List of methods to generate surface waves. Random"
                        + " method generates small random splashes. Sine wave method overlaps a number of sine waves to  get a final wave that changes the velocity of the surface vertices."));

                    if (water2D_Sim.surfaceWaves != Water2D_SurfaceWaves.None)
                    {
                        if (water2D_Sim.surfaceWaves == Water2D_SurfaceWaves.SineWaves)
                        {
                            EditorGUILayout.PropertyField(sineWavesType, new GUIContent("Sine Waves", "The type of sine waves to use. If you want to animate the amplitude and stretch of the sine wave use the Single Sine Wave option."));

                            if (water2D_Sim.sineWavesType == Water2D_SineWaves.MultipleSineWaves)
                                EditorGUILayout.PropertyField(randomValues, new GUIContent("Random Values", "When enabled, the amplitude, stretch and phase offset will be random values for each sine wave."));

                            if (water2D_Sim.sineWavesType == Water2D_SineWaves.MultipleSineWaves)
                            {
                                EditorGUILayout.PropertyField(sineWaves, new GUIContent("Sine Waves Number", "The number of individual sine waves."));
                                water2D_Sim.sineWaves = Mathf.Clamp(water2D_Sim.sineWaves, 1, 100);

                                if (water2D_Sim.randomValues)
                                {
                                    EditorGUILayout.PropertyField(maxAmplitude, new GUIContent("Max Amplitude", "The constant is used to generate a random amplitude value between a Max and a Min. Controls the height of the sine wave."));
                                    EditorGUILayout.PropertyField(minAmplitude, new GUIContent("Min Amplitude", "The constant is used to generate a random amplitude value between a Max and a Min. Controls the height of the sine wave."));

                                    EditorGUILayout.PropertyField(maxStretch, new GUIContent("Max Stretch", "The constant is used to generate a random sine wave stretch value between a Max and a Min. "
                                        + "The bigger the value of the stretch the more compact the waves are."));
                                    EditorGUILayout.PropertyField(minStretch, new GUIContent("Min Stretch", "The constant is used to generate a random sine wave stretch value between a Max and a Min. "
                                        + "The bigger the value of the stretch the more compact the waves are."));

                                    EditorGUILayout.PropertyField(maxPhaseOffset, new GUIContent("Max Phase Offset", "The constant is used to generate a random phase offset value between a Max and a Min. "
                                        + "The bigger the value of the phase offset the faster the waves move to the left (right)."));
                                    EditorGUILayout.PropertyField(minPhaseOffset, new GUIContent("Min Phase Offset", "The constant is used to generate a random phase offset value between a Max and a Min. "
                                        + "The bigger the value of the phase offset the faster the waves move to the left (right)."));
                                }
                                else
                                {
                                    if (water2D_Sim.sineAmplitudes.Count != water2D_Sim.sineWaves)
                                    {
                                        water2D_Sim.sineAmplitudes.Clear();
                                        water2D_Sim.sineStretches.Clear();
                                        water2D_Sim.phaseOffset.Clear();

                                        for (int i = 0; i < water2D_Sim.sineWaves; i++)
                                        {
                                            water2D_Sim.sineAmplitudes.Add(Random.Range(water2D_Sim.minAmplitude, water2D_Sim.maxAmplitude));
                                            water2D_Sim.sineStretches.Add(Random.Range(water2D_Sim.minStretch, water2D_Sim.maxStretch));
                                            water2D_Sim.phaseOffset.Add(Random.Range(water2D_Sim.minPhaseOffset, water2D_Sim.maxPhaseOffset));
                                        }
                                    }

                                    int n = 0;
                                    for (int i = 0; i < water2D_Sim.sineWaves; i++)
                                    {
                                        n = i + 1;
                                        EditorGUILayout.LabelField(new GUIContent("Sine Wave " + n), EditorStyles.boldLabel);

                                        water2D_Sim.sineAmplitudes[i] = EditorGUILayout.FloatField(new GUIContent("Amplitude", "The amplitude of the wave. This value controls the height of the sine wave."), water2D_Sim.sineAmplitudes[i]);
                                        water2D_Sim.sineStretches[i] = EditorGUILayout.FloatField(new GUIContent("Stretch", "The bigger the value of the stretch the more compact the waves are."), water2D_Sim.sineStretches[i]);
                                        water2D_Sim.phaseOffset[i] = EditorGUILayout.FloatField(new GUIContent("Phase Offset", "The bigger the value of the phase offset the faster the waves move to the left (right). "
                                            + " A negative value will make the sine wave move to the right."), water2D_Sim.phaseOffset[i]);
                                    }
                                }
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(waveAmplitude, new GUIContent("Wave Amplitude", "The amplitude of the wave. This value controls the height of the sine wave."));
                                EditorGUILayout.PropertyField(waveStretch, new GUIContent("Wave Stretch", "The bigger the value of the stretch the more compact the waves are."));
                                EditorGUILayout.PropertyField(wavePhaseOffset, new GUIContent("Wave Phase Offset", "The bigger the value of the phase offset the faster the waves move to the left (right)."
                                    + " A negative value will make the sine wave move to the right."));
                            }

                            EditorGUILayout.PropertyField(sineWaveVelocityScale, new GUIContent("Sine Wave Velocity Scale", "Will scale down (up) the velocity that is applied to a vertex from a sine wave."));
                        }

                        if (water2D_Sim.surfaceWaves == Water2D_SurfaceWaves.RandomSplashes)
                        {
                            EditorGUILayout.PropertyField(timeStep, new GUIContent("Wave Time Step", "The time between splashes."));
                            EditorGUILayout.PropertyField(maxVelocity, new GUIContent("Max Velocity", "The constant is used to generate a random velocity between a Max and a Min."));
                            EditorGUILayout.PropertyField(minVelocity, new GUIContent("Min Velocity", "The constant is used to generate a random velocity between a Max and a Min."));
                            EditorGUILayout.PropertyField(neighborVertVelocityScale, new GUIContent("Neighbor Vertex Velocity Scale", "Will scale down (up) the velocity that "
                                + "is applied to the neighbor vertices when RandomWave method is called."));
                        }
                    }
                });
            }

            BoldFontStyle(() =>
            {
                water2D_Sim.showPlayerSettings = EditorGUILayout.Foldout(water2D_Sim.showPlayerSettings, "Player");
            });

            if (water2D_Sim.showPlayerSettings)
            {
                InspectorBox(10, () =>
                {
                    EditorGUILayout.PropertyField(characterControllerType, new GUIContent("Character Controller", "If you are using a physics based character controller select Physics Based option, otherwise select Raycast Based."));
        

                    if (water2D_Sim.characterControllerType == Water2D_CharacterControllerType.PhysicsBased)
                    {
                        EditorGUILayout.PropertyField(playerBoundingBoxSize, new GUIContent("Player BBox Size", "The size for the players bounding box. In most cases the player character will have more than one collider. "
                                + "So to simplify the things, Water2D uses this variable to set the size for an imaginary bounding box that will be used when applying buoyant force. "));

                        EditorGUILayout.PropertyField(playerBoundingBoxCenter, new GUIContent("Player BBox Center", "By default the center of the bounding box will be "
                            + "the transform.position of the object. Use this variable to offset the players bounding box center."));
                        EditorGUILayout.PropertyField(playerBuoyantForceScale, new GUIContent("Player Buoyant Force Scale", "Depending on what character controller you are using, you may have a big character "
                            + "that must have a small mass. As a result the Player will not submerge in the water because of its low mass that results in low density. To resolve this problem use this variable to scale "
                            + "down the buoyant force applied to the Player."));

                        EditorGUILayout.PropertyField(playerOnExitPSAndSound, new GUIContent("On Exit PS and Sound", "When enabled, a particles system will be instantiated and a sound effect will be played when the player exits the water."));
                        EditorGUILayout.PropertyField(playerOnExitRipple, new GUIContent("On Exit Ripple", "When enabled, a ripple will be generated when the player exits the water."));

                        if (!rippleScript && water2D_Sim.playerOnExitRipple)
                            EditorGUILayout.PropertyField(playerOnExitVelocity, new GUIContent("On Exit Velocity", "The velocity that should be applied to a surface vertex when the player exits the water."));
                    }
                    else
                    {
                        if (!rippleScript)
                        {
                            EditorGUILayout.PropertyField(rippleWidth, new GUIContent("Ripple Width", "The initial width of the water wave."));
                            EditorGUILayout.PropertyField(playerOnEnterVelocity, new GUIContent("On Enter Velocity", "The velocity that will be applied to the surface vertices to generate a wave."));

                            if (water2D_Sim.playerOnExitRipple)
                                EditorGUILayout.PropertyField(playerOnExitVelocity, new GUIContent("On Exit Velocity", "The velocity that should be applied to a surface vertex when the player exits the water."));
                        }

                        EditorGUILayout.PropertyField(playerOnExitPSAndSound, new GUIContent("On Exit PS and Sound", "When enabled, a particles system will be instantiated and a sound effect will be played when the player exits the water."));
                        EditorGUILayout.PropertyField(playerOnExitRipple, new GUIContent("On Exit Ripple", "When enabled, a ripple will be generated when the player exits the water."));
                    }
                });
            }

            BoldFontStyle(() =>
            {
                water2D_Sim.showMiscellaneous = EditorGUILayout.Foldout(water2D_Sim.showMiscellaneous, "Miscellaneous");
            });

            if (water2D_Sim.showMiscellaneous)
            {
                InspectorBox(10, () =>
                {
                    EditorGUILayout.PropertyField(waterType, new GUIContent("Water Type", "A list of water types. "
                        + "A dynamic water can be animated and reacts to objects. A decorative water can be animated, but will not react "
                        + " to objects and will not influence their position."));


                    EditorGUILayout.PropertyField(collisionLayers, new GUIContent("Collider Mask"));

                    if (!rippleScript && water2D_Sim.springSimulation)
                    {
                        EditorGUILayout.PropertyField(collisionDetectionMode, new GUIContent("Collider Detection", "When Raycast Based option is selected, for every dynamic object that is near the surface of the water a ray cast "
                            + "if performed for every vertex that is inside the bounding box of that objects collider. This is done to find if a particular vertex is inside the collider and not only his bounding box. If the vertex is inside the collider, his velocity is "
                            + "changed based on the velocity of the dynamic object. If Bounds Based option is selected, no raycast is perfomed and instead the vertex position relative to the objects bounding box is used to determine if a vertex velocity should be changed by an object."));

                        if (water2D_Sim.collisionDetectionMode == Water2D_CollisionDetectionMode.RaycastBased)
                        {
                            EditorGUILayout.PropertyField(raycastDistance, new GUIContent("Raycast Distance", "How far to the left and right from a vertex world position should we look for a collider. "));
                            water2D_Sim.raycastDistance = Mathf.Clamp(water2D_Sim.raycastDistance, 0 , 100);
                        }
                    }

                    Water2D_Tool water2D_Tool = water2D_Sim.GetComponent<Water2D_Tool>();

                    if (water2D_Sim.waterType == Water2D_Type.Decorative)
                    {          
                        if (water2D_Tool.createCollider)
                        {
                            water2D_Tool.createCollider = false;
                            water2D_Tool.RecreateWaterMesh();
                        }
                    }
                    else
                    {
                        if (!water2D_Tool.createCollider)
                        {
                            water2D_Tool.createCollider = true;
                            water2D_Tool.RecreateWaterMesh();
                        }
                    }

                    if (water2D_Sim.waterType == Water2D_Type.Dynamic)
                    {
                        if (!rippleScript)
                        {
                            EditorGUILayout.PropertyField(velocityFilter, new GUIContent("Velocity Filter", "An object with a velocity on the Y axis "
                                + " greater than the value of Velocity Filter will not create splashes."));
                            EditorGUILayout.PropertyField(interactionRegion, new GUIContent("Interaction Region", "The bottom region of a colliders bounding box "
                                + "that can affect the velocity of a vertex. This value is used to limit the ability of the objects with big bounding boxes to affect the "
                                + "velocity of the surface vertices. A value of 1 means that only the first 1m of the bottom of the bounding box will affect the velocity "
                                + "of the surface vertices. "));

                            EditorGUILayout.PropertyField(waterDisplacement, new GUIContent("Water Displacement", "Floating objects will influence the final water area."));
                        }
                    }

                    if (!rippleScript)
                        EditorGUILayout.PropertyField(constantWaterArea, new GUIContent("Constant Water Area", "If the width of the water changes, the height will " +
                            " change too, to keep a constant water Area."));

                    if (water2D_Sim.waterType == Water2D_Type.Dynamic)
                    {
                        EditorGUILayout.PropertyField(particleS, new GUIContent("PS Prefab", "A particle system prefab used to simulate the water splash effect."));
                        EditorGUILayout.PropertyField(particleSystemPosOffset, new GUIContent("PS Position Offset", "Offsets the position where the particle systems are created on the Z axis."));
                        EditorGUILayout.PropertyField(particleSystemSorting, new GUIContent("PS Sorting", "Enable this toggle if you want to set the sorting layer and order in layer of the particle system when it is instantiated."));
                        

                        if (water2D_Sim.particleSystemSorting)
                        {
                            EditorGUILayout.PropertyField(particleSystemSortingLayerName, new GUIContent("PS Sorting Layer Name", "Insert here the sorting layer name for the particle system."));
                            EditorGUILayout.PropertyField(particleSystemOrderInLayer, new GUIContent("PS Order In Layer", "Insert here the order in layer for the particle system."));                      
                        }

                        EditorGUILayout.PropertyField(splashSound, new GUIContent("Sound Effect", "A sound effect generated when an object hits the water surface."));
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
