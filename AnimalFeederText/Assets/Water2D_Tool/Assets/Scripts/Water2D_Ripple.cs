using UnityEngine;
using System.Collections.Generic;

namespace Water2DTool {
    /// <summary>
    /// Types of water obstruction.
    /// </summary>
    public enum Water2D_ObstructionType {
        /// <summary>
        /// No water obstruction is applied.
        /// </summary>
        None,
        /// <summary>
        /// Obstructions created by dynamic objects are applied to the water.
        /// </summary>
        DynamicObstruction,
        /// <summary>
        /// Texture obstructions are applied to the water.
        /// </summary>
        TextureObstruction
    }

    public enum Water2D_RippleSimulationUpdate {
        /// <summary>
        /// The ripple simulation runs in the Update method.
        /// </summary>
        UpdateMethod,
        /// <summary>
        /// The ripple simulation runs in the Fixed Update method.
        /// </summary>
        FixedUpdateMethod,
    }

    public enum RTFilterMode {
        Point,
        Bilinear,
        Trilinear
    }

    public class Water2D_Ripple : MonoBehaviour {
        #region Private fields

#if UNITY_EDITOR
        public bool showWaterRipple = true;
        public bool showRippleSourcesList = true;
        public bool showPlayer = true;
        public bool showMouse = true;
        public bool showRain = true;
        public bool showDynamicObjects = true;
        public bool showAmbiantWaves = true;
#endif

        private float prevWaterLineWorldPos;
        /// <summary>
        /// Instance of ShaderParam class. Stores the IDs of different shader properties.
        /// </summary>
        private ShaderParam shaderParam;
        /// <summary>
        /// World position of the ripple created when the player exits the water.
        /// </summary>
        public Vector3 playerOnExitRipplePos = Vector3.zero;
        /// <summary>
        /// Temporary Render Texture.
        /// </summary>
        private RenderTexture tempRT;
        /// <summary>
        /// Stores the current state of the heightmap.
        /// </summary>
        private RenderTexture bufferCurrent;
        /// <summary>
        /// Stores the state of the heightmap it had on the previous frame.
        /// </summary>
        private RenderTexture bufferPrev;
        /// <summary>
        /// Stores the final state of the heightmap for the current frame.
        /// </summary>
        private RenderTexture _finalHeightMap;
        /// <summary>
        /// Render Texture format.
        /// </summary>
        private RenderTextureFormat rtFormat = RenderTextureFormat.RFloat;
        /// <summary>
        /// The width of the Render Textures used to store the height map.
        /// </summary>
        private int renderTextureWidth;
        /// <summary>
        /// The height of the Render Textures used to store the height map.
        /// </summary>
        private int renderTextureHeight;
        /// <summary>
        /// A counter that is incremented by 1 every time the height map is processed through the shader that simulates ripple propagation.
        /// </summary>
        private int waterSimCount = 0;
        /// <summary>
        /// The number of water interactions (ripples) that should be generated this frame.
        /// </summary>
        private int waterInteractions = 0;
        /// <summary>
        /// Used to determine when a new raindrop should generated.
        /// </summary>
        public float rainTimeCounter = 0;
        /// <summary>
        /// Used to determine when the height map should be processed through the shader that simulates ripple propagation.
        /// </summary>
        public float heightMapTimeCounter = 0;
        /// <summary>
        /// Used to determine when the interaction with the water should be updated.
        /// </summary>
        public float interactionsTimeCounter = 0;
        /// <summary>
        /// Should a ripple be generated on this frame.
        /// </summary>
        private bool addInteraction = false;
        /// <summary>
        /// List that stores the box colliders that generate dynamic obstructions.
        /// </summary>
        private List<BoxCollider> boxColObstList;
        /// <summary>
        /// List that stores the sphere colliders that generate dynamic obstructions.
        /// </summary>
        private List<SphereCollider> sphereColObstList;
        /// <summary>
        /// List that stores references to RippleSource scripts that are found on kinematic objects.
        /// </summary>
        private List<RippleSource> kinematicRippleSourcesList;
        /// <summary>
        /// Material that uses the shader that generates ripples and simulates ripple propagation.
        /// </summary>
        private Material heightMapMaterial;
        /// <summary>
        /// Material that uses the shader that generates ripples and simulates ripple propagation.
        /// Supports texture obstructions.
        /// </summary>
        private Material heightMapMaterialTexObst;
        /// <summary>
        /// Material that uses the shader that generates ripples and simulates ripple propagation.
        /// Supports dynamic obstructions.
        /// </summary>
        private Material heightMapMaterialDynObst;
        /// <summary>
        /// Material that uses a shader that resamples a texture and applies a smoothing effect to it.
        /// </summary>
        private Material bCubicResamplingMat;
        /// <summary>
        /// Material that uses a shader that adds sine wave patterns to the final state of the current frame height map.
        /// </summary>
        private Material ambientWavesMat;
        /// <summary>
        /// Reference to this object's Water2D_Tool component.
        /// </summary>
        private Water2D_Tool water2DTool;
        /// <summary>
        /// Reference to this object's Water2D_Simulation component.
        /// </summary>
        private Water2D_Simulation water2DSim;
        /// <summary>
        /// Used to remove the stretching effect when the width and length of the water are not equal.
        /// </summary>
        private Vector4 xyAxisScale = new Vector4(1, 1, 0, 0);
        /// <summary>
        /// World position of the left haldle.
        /// </summary>
        private Vector3 leftHandleWorldPos;
        private float texelSize = 0.1f;
        private float waterWidth;
        private float waterLength;
        private BoxCollider waterBoxCollider;
        /// <summary>
        /// The position of the water object on the previous frame.
        /// </summary>
        private Vector3 prevPos;
        /// <summary>
        /// The Transform component of the water object.
        /// </summary>
        private new Transform transform;
        /// <summary>
        /// Main camera Transform component.
        /// </summary>
        private Transform camTransform;
        /// <summary>
        /// The renderer component of the front mesh.
        /// </summary>
        private Renderer frontMeshRend;
        /// <summary>
        /// The renderer component of the top mesh.
        /// </summary>
        private Renderer topMeshRend;
        /// <summary>
        /// 1f / x. Instead of dividing a value by the width of the water we multiply that value by it reverse.
        /// </summary>
        private float rWaterWidth;
        /// <summary>
        /// 1f / x. Instead of dividing a value by the length of the water we multiply that value by it reverse.
        /// </summary>
        private float rWaterLength;
        /// <summary>
        /// Used to find where was the camera on the previous frame relative to the water line.
        /// Holds the value of 1 if the camera is above the water line, -1 if below.
        /// </summary>
        private int prevCamPosRelativeToWaterLine = 1;
        /// <summary>
        /// The time between 2 consecutive interaction updates.
        /// </summary>
        private float interactionsTimeStep = 0.02f;

        #endregion Private fields

        #region Public fields
        /// <summary>
        /// Should a ripple be generated when the player exits the water?.
        /// </summary>
        public bool playerOnExitRipple = false;
        public float waveHeightScale = 1f;
        /// <summary>
        /// Sine wave amplitude. The bigger the value the higher the wave top will be.
        /// </summary>
        public float amplitude1 = 0.02f;
        /// <summary>
        /// Sine wave amplitude. The bigger the value the higher the wave top will be.
        /// </summary>
        public float amplitude2 = 0.03f;
        /// <summary>
        /// Sine wave amplitude. The bigger the value the higher the wave top will be.
        /// </summary>
        public float amplitude3 = 0.015f;
        /// <summary>
        /// The distance between 2 consecutive points of a sine wave.
        /// </summary>
        public float waveLength1 = 2f;
        /// <summary>
        /// The distance between 2 consecutive points of a sine wave.
        /// </summary>
        public float waveLength2 = 1.5f;
        /// <summary>
        /// The distance between 2 consecutive points of a sine wave.
        /// </summary>
        public float waveLength3 = 1.0f;
        /// <summary>
        /// Sine wave phase offset. The bigger the value of the phase offset the faster the waves move to the left (right).*/
        /// </summary>
        public float phaseOffset1 = 1f;
        /// <summary>
        /// Sine wave phase offset. The bigger the value of the phase offset the faster the waves move to the left (right).
        /// </summary>
        public float phaseOffset2 = 1.5f;
        /// <summary>
        /// Sine wave phase offset. The bigger the value of the phase offset the faster the waves move to the left (right).
        /// </summary>
        public float phaseOffset3 = 2.0f;
        /// <summary>
        /// Amplitude fade start point.
        /// </summary>
        [Range(0f, 1f)]
        public float amplitudeFadeStart = 0.2f;
        /// <summary>
        /// Amplitude fade end point.
        /// </summary>
        [Range(0f, 1f)]
        public float amplitudeFadeEnd = 0.7f;
        /// <summary>
        /// Enables sine waves.
        /// </summary>
        public bool ambientWaves = false;
        /// <summary>
        /// Enables sine wave amplitude on the Z axis.
        /// </summary>
        public bool amplitudeZAxisFade = false;
        /// <summary>
        /// The time between 2 consecutive height map updates.
        /// </summary>
        public float heightUpdateMapTimeStep = 0.015f;
        /// <summary>
        /// Should a smoothing effect be applied to the heightmap.?
        /// </summary>
        public bool bicubicResampling = false;
        /// <summary>
        /// List that stores references to RippleSource scripts.
        /// </summary>
        public List<RippleSource> rippleSourcesList = new List<RippleSource>();
        /// <summary>
        /// Enables the ripple sources option.
        /// </summary>
        public bool rippleSources = false;
        /// <summary>
        /// Determines how many times per second should the height map be processed through the shader that simulates ripple propagation.
        /// </summary>
        [Range(1, 240)]
        public float rippleWaterFPSCap = 60f;
        /// <summary>
        /// Enables rain.
        /// </summary>
        public bool rainDrops = false;
        /// <summary>
        /// The radius of a rain drop in Unity space.
        /// </summary>
        public float rainDropRadius = 0.25f;
        /// <summary>
        /// The strength of a rain drop.
        /// </summary>
        public float rainDropStrength = -0.2f;
        /// <summary>
        /// The number of water drops that should fall in a second.
        /// </summary>
        public float rainDropFrequency = 20f;
        /// <summary>
        /// Damping parameter for the water propagation simulation.
        /// </summary>
        public float waterDamping = 0.05f;
        /// <summary>
        /// The radius of the ripple created by the mouse arrow, in Unity space.
        /// </summary>
        public float mouseRadius = 0.3f;
        /// <summary>
        /// The strength of the ripple created by the mouse arrow.
        /// </summary>
        public float mouseStregth = -0.1f;
        /// <summary>
        /// The number of Render Texture pixels that should fit in 1 unit of Unity space.
        /// </summary>
        public int rtPixelsToUnits = 12;
        /// <summary>
        /// Texture with water obstructions.
        /// </summary>
        public Texture2D obstructionTexture;
        /// <summary>
        /// Enables the ability to interact with the water using the mouse arrow.
        /// </summary>
        public bool mouseInteraction = true;
        /// <summary>
        /// The radius of the ripple created by the player.
        /// </summary>
        public float playerRippleRadius = 0.5f;
        /// <summary>
        /// The strength of the ripple created by the player.
        /// </summary>
        public float playerRippleStrength = -0.08f;
        /// <summary>
        /// If the player has a velocity lower than that of the value of this field than no ripples will be generated by him.
        /// </summary>
        public float playerVelocityFilter = 0.5f;
        /// <summary>
        /// Offsets the position of the ripple created by the player.
        /// </summary>
        public float playerRippleXOffset = 0;
        /// <summary>
        /// The radius of the ripple created by objects that have the script RippleSource attached to them.
        /// </summary>
        public float objectRippleRadius = 0.5f;
        /// <summary>
        /// The strength of the ripple created by objects that have the script RippleSource attached to them.
        /// </summary>
        public float objectRippleStrength = -0.04f;
        public float objectRadiusScale = 1.0f;
        /// <summary>
        /// Used to determine if a dynamic object can generate ripples?.
        /// </summary>
        public float objectVelocityFilter = 0.5f;
        /// <summary>
        /// Offsets the ripple  position on the X axis based on the width of the coliders bounding box. A value of 0 means that the ripple will be generated at the center of the collider. 
        /// A value of 0.5f means that the ripple will be positioned at the left or right edge of the colliders bounding box.
        /// </summary>
        public float objectXAxisRippleOffset = 0.5f;
        /// <summary>
        /// Layer mask.
        /// </summary>
        public LayerMask obstructionLayers;
        public Water2D_ObstructionType obstructionType = Water2D_ObstructionType.None;
        public Water2D_RippleSimulationUpdate rippleSimulationUpdate = Water2D_RippleSimulationUpdate.FixedUpdateMethod;
        public RTFilterMode rtFilterMode = RTFilterMode.Trilinear;
        /// <summary>
        /// When the camera is below the waterline, should both sides of the top mesh be drawn?.
        /// </summary>
        public bool drawBothSides = false;
        /// <summary>
        /// Used to scale up or down the strength of the ripples created by dynamic objects when the Abs value
        /// of the velocity on the Y axis is greater than the Abs value of the velocity on the X axis.
        /// </summary>
        public float strengthScale = 6f;
        /// <summary>
        /// When true, the radius of the ripple created by dynamic objects will be the same for all objects. 
        /// When false the radius of the ripple depends on size of the collider of that object.
        /// </summary>
        public bool fixedRadius = false;


        #endregion Public fields

        #region Class methods

        private void Awake()
        {
            kinematicRippleSourcesList = new List<RippleSource>();

            heightMapMaterial = new Material(Shader.Find("Hidden/Water_CircularRippleWaves"));
            heightMapMaterialDynObst = new Material(Shader.Find("Hidden/Water_CircularRippleWaves_DynamicObs"));
            heightMapMaterialTexObst = new Material(Shader.Find("Hidden/Water_CircularRippleWaves_TextureObs"));

            if (obstructionTexture != null)
                heightMapMaterialTexObst.SetTexture("_ObstructionTex", obstructionTexture);

            ambientWavesMat = new Material(Shader.Find("Hidden/AmbientWaves"));
            bCubicResamplingMat = new Material(Shader.Find("Hidden/BSplineResampling"));

            transform = GetComponent<Transform>();
        }

        public void InstantiateRenderTextures()
        {
            bufferCurrent = null;
            bufferPrev = null;
            _finalHeightMap = null;

            water2DSim = GetComponent<Water2D_Simulation>();
            water2DTool = GetComponent<Water2D_Tool>();

            renderTextureWidth = water2DTool.renderTextureWidth;
            renderTextureHeight = water2DTool.renderTextureHeight;

            bufferCurrent = new RenderTexture(renderTextureWidth, renderTextureHeight, 0, rtFormat, RenderTextureReadWrite.Linear);

            if (rtFilterMode == RTFilterMode.Trilinear)
                bufferCurrent.filterMode = FilterMode.Trilinear;
            else if (rtFilterMode == RTFilterMode.Bilinear)
                bufferCurrent.filterMode = FilterMode.Bilinear;
            else
                bufferCurrent.filterMode = FilterMode.Point;

            bufferPrev = new RenderTexture(renderTextureWidth, renderTextureHeight, 0, rtFormat, RenderTextureReadWrite.Linear);
            bufferPrev.filterMode = bufferCurrent.filterMode;

            _finalHeightMap = new RenderTexture(renderTextureWidth, renderTextureHeight, 0, rtFormat, RenderTextureReadWrite.Linear);
            _finalHeightMap.filterMode = bufferCurrent.filterMode;


            RenderTexture.active = bufferPrev;
            GL.Clear(true, true, Color.grey);
            RenderTexture.active = null;

            RenderTexture.active = bufferCurrent;
            GL.Clear(true, true, Color.grey);
            RenderTexture.active = null;

            RenderTexture.active = _finalHeightMap;
            GL.Clear(true, true, Color.grey);
            RenderTexture.active = null;

            GetComponent<Renderer>().material.SetTexture("_MainTex", _finalHeightMap);
            water2DTool.topMeshGameObject.GetComponent<Renderer>().material.mainTexture = _finalHeightMap;

            camTransform = Camera.main.GetComponent<Transform>();
            if (water2DTool.cubeWater)
                waterBoxCollider = GetComponent<BoxCollider>();

            if (water2DTool.use3DCollider)
                fixedRadius = true;

            shaderParam = new ShaderParam();
        }

        private void Start()
        {
            xyAxisScale = new Vector4(1, 1, 0, 0);

            if (renderTextureWidth > renderTextureHeight)
                xyAxisScale = new Vector4((float)renderTextureWidth / (float)renderTextureHeight, 1f, 0, 0);

            if (renderTextureWidth < renderTextureHeight)
                xyAxisScale = new Vector4(1f, (float)renderTextureHeight / (float)renderTextureWidth, 0, 0);

            prevPos = transform.position;
            leftHandleWorldPos = transform.TransformPoint(water2DTool.handlesPosition[2]);

            if (water2DTool.width > water2DTool.length)
                texelSize = 1f / water2DTool.length;
            else
                texelSize = 1f / water2DTool.width;

            boxColObstList = new List<BoxCollider>();
            sphereColObstList = new List<SphereCollider>();

            waterWidth = water2DTool.width;
            waterLength = water2DTool.length;

            rWaterWidth = 1f / waterWidth;
            rWaterLength = 1f / waterLength;

            interactionsTimeStep = 1f / 60f;

            frontMeshRend = GetComponent<Renderer>();
            topMeshRend = water2DTool.topMeshGameObject.GetComponent<Renderer>();
            SetAmbientWavesShaderParameters();
            UpdateRippleShaderParameters();

            if (camTransform.position.y < water2DSim.waterLineCurrentWorldPos.y)
                prevCamPosRelativeToWaterLine = -1;
            else
                prevCamPosRelativeToWaterLine = 1;

            SetAmbientWavesShaderParameters();

            frontMeshRend.material.SetFloat(shaderParam.bottomPosID, water2DTool.handlesPosition[1].y);
            topMeshRend.material.SetFloat(shaderParam.bottomPosID, water2DTool.handlesPosition[1].y);

            SetWaveHeightScale();

            prevWaterLineWorldPos = transform.TransformPoint(water2DTool.handlesPosition[0]).y;

            Camera.main.depthTextureMode = DepthTextureMode.Depth;
            SetWaterLinePosition();

            topMeshRend.material.SetInt(shaderParam.applyOffset, 1);
            frontMeshRend.material.SetInt(shaderParam.applyOffset, 1);
        }

        public void SetWaveHeightScale()
        {
            frontMeshRend.material.SetFloat(shaderParam.waveHeightScaleID, waveHeightScale);
            topMeshRend.material.SetFloat(shaderParam.waveHeightScaleID, waveHeightScale);
        }

        private void OnDestroy()
        {
            Destroy(bufferCurrent);
            Destroy(bufferPrev);
            Destroy(_finalHeightMap);
            bufferCurrent = null;
            bufferPrev = null;
            _finalHeightMap = null;
        }

        private void Update()
        {
            WaterObjectMoved();
            TopMeshCulling();

            if (rippleSimulationUpdate == Water2D_RippleSimulationUpdate.UpdateMethod)
                RippleWaterSimulationUpdate();
        }

        private void FixedUpdate()
        {
            if (rippleSimulationUpdate == Water2D_RippleSimulationUpdate.FixedUpdateMethod)
                RippleWaterSimulationFixedUpdate();
        }


        /// <summary>
        /// Runs the ripple water simulation in the Update method.
        /// </summary>
        private void RippleWaterSimulationUpdate()
        {
            if (obstructionType == Water2D_ObstructionType.DynamicObstruction)
                UpdateDynamicObstructions();

            AddRain();

            interactionsTimeCounter += Time.deltaTime;

            if (interactionsTimeCounter > interactionsTimeStep)
            {
                interactionsTimeCounter -= interactionsTimeStep;
                UpdateWaterInteractions();
                UpdateHeightMapWithWaterInteractions();
            }

            heightMapTimeCounter += Time.deltaTime;
            if (heightMapTimeCounter > heightUpdateMapTimeStep)
            {
                int loops = 0;
                while (heightMapTimeCounter > heightUpdateMapTimeStep)
                {
                    heightMapTimeCounter -= heightUpdateMapTimeStep;
                    UpdateHeightMap();
                    loops++;

                    if (loops > 5)
                        break;
                }
            }
        }

        /// <summary>
        /// Runs the ripple water simulation in the FixedUpdate method.
        /// </summary>
        private void RippleWaterSimulationFixedUpdate()
        {
            if (obstructionType == Water2D_ObstructionType.DynamicObstruction)
                UpdateDynamicObstructions();

            AddRain();
            UpdateWaterInteractions();
            UpdateHeightMapWithWaterInteractions();
            UpdateHeightMap();
        }

        /// <summary>
        /// Runs the current height map through a shader that adds ripples caused by rain or by objects.
        /// </summary>
        private void UpdateHeightMapWithWaterInteractions()
        {
            if (addInteraction)
            {
                if (waterSimCount % 2 == 0)
                {
                    RenderTexture tempRT = RenderTexture.GetTemporary(bufferCurrent.width, bufferCurrent.height, 0, rtFormat, RenderTextureReadWrite.Linear);
                    tempRT.filterMode = bufferCurrent.filterMode;

                    switch (obstructionType)
                    {
                        case Water2D_ObstructionType.None:
                            Graphics.Blit(bufferCurrent, tempRT, heightMapMaterial, 0);
                            break;

                        case Water2D_ObstructionType.TextureObstruction:
                            Graphics.Blit(bufferCurrent, tempRT, heightMapMaterialTexObst, 0);
                            break;

                        case Water2D_ObstructionType.DynamicObstruction:
                            Graphics.Blit(bufferCurrent, tempRT, heightMapMaterialDynObst, 0);
                            break;
                    }

                    Graphics.Blit(tempRT, bufferCurrent);
                    RenderTexture.ReleaseTemporary(tempRT);
                }
                else
                {
                    RenderTexture tempRT = RenderTexture.GetTemporary(bufferPrev.width, bufferPrev.height, 0, rtFormat, RenderTextureReadWrite.Linear);
                    tempRT.filterMode = bufferPrev.filterMode;

                    switch (obstructionType)
                    {
                        case Water2D_ObstructionType.None:
                            Graphics.Blit(bufferPrev, tempRT, heightMapMaterial, 0);
                            break;

                        case Water2D_ObstructionType.TextureObstruction:
                            Graphics.Blit(bufferPrev, tempRT, heightMapMaterialTexObst, 0);
                            break;

                        case Water2D_ObstructionType.DynamicObstruction:
                            Graphics.Blit(bufferPrev, tempRT, heightMapMaterialDynObst, 0);
                            break;
                    }

                    Graphics.Blit(tempRT, bufferPrev);
                    RenderTexture.ReleaseTemporary(tempRT);
                }
            }

            ResetWaterInteractionParameters();

        }

        /// <summary>
        /// Runs the current height map through a shader pass that simulates the propagation of ripples.
        /// </summary>
        private void UpdateHeightMap()
        {
            RenderTexture bufferOne = (waterSimCount % 2 == 0) ? bufferCurrent : bufferPrev;
            RenderTexture bufferTwo = (waterSimCount % 2 == 0) ? bufferPrev : bufferCurrent;

            waterSimCount++;

            if (bicubicResampling || ambientWaves)
            {
                tempRT = RenderTexture.GetTemporary(bufferCurrent.width, bufferCurrent.height, 0, rtFormat, RenderTextureReadWrite.Linear);
                tempRT.filterMode = bufferCurrent.filterMode;
            }
            else
            {
                tempRT = null;
            }

            switch (obstructionType)
            {
                case Water2D_ObstructionType.None:
                    heightMapMaterial.SetTexture(shaderParam.prevTexID, bufferTwo);
                    Graphics.Blit(bufferOne, _finalHeightMap, heightMapMaterial, 1);
                    break;

                case Water2D_ObstructionType.TextureObstruction:
                    heightMapMaterialTexObst.SetTexture(shaderParam.prevTexID, bufferTwo);
                    Graphics.Blit(bufferOne, _finalHeightMap, heightMapMaterialTexObst, 1);
                    break;

                case Water2D_ObstructionType.DynamicObstruction:
                    heightMapMaterialDynObst.SetTexture(shaderParam.prevTexID, bufferTwo);
                    Graphics.Blit(bufferOne, _finalHeightMap, heightMapMaterialDynObst, 1);
                    break;
            }

            if (bicubicResampling)
            {
                Graphics.Blit(_finalHeightMap, tempRT, bCubicResamplingMat);
                Graphics.Blit(tempRT, _finalHeightMap);
            }

            Graphics.Blit(_finalHeightMap, bufferTwo);

            if (bicubicResampling || ambientWaves)
            {
                RenderTexture.active = tempRT;
                GL.Clear(true, true, Color.grey);
                RenderTexture.active = null;
            }

            if (ambientWaves)
            {
                Graphics.Blit(_finalHeightMap, tempRT, ambientWavesMat);
                Graphics.Blit(tempRT, _finalHeightMap);
            }

            if (bicubicResampling || ambientWaves)
                RenderTexture.ReleaseTemporary(tempRT);
        }

        /// <summary>
        /// Controls which sides of polygons should be culled (not drawn). 
        /// Which side should be culled is decided based on the position of
        /// the camera relative to the water line position on the Y axis.
        /// </summary>
        private void TopMeshCulling()
        {
            int camPosRelativeToWaterLine = 1;
            bool setCulling = false;

            if (camTransform.position.y < water2DSim.waterLineCurrentWorldPos.y)
                camPosRelativeToWaterLine = -1;
            else
                camPosRelativeToWaterLine = 1;

            if (camPosRelativeToWaterLine != prevCamPosRelativeToWaterLine)
            {
                prevCamPosRelativeToWaterLine = camPosRelativeToWaterLine;
                setCulling = true;
            }

            if (setCulling)
                SetTopMeshCulling();
        }

        /// <summary>
        /// Sets top mesh culling to Front, Back or Off. 
        /// </summary>
        public void SetTopMeshCulling()
        {
            if (camTransform.position.y < water2DSim.waterLineCurrentWorldPos.y)
            {
                int culling = drawBothSides ? 0 : 1;
                topMeshRend.material.SetInt(shaderParam.faceCullingID, culling);
                topMeshRend.material.SetInt(shaderParam.oneOrZeroID, 1);
                
                // For LWRP and URP 
                //topMeshRend.material.renderQueue = 2500;
            }
            else
            {
                topMeshRend.material.SetInt(shaderParam.faceCullingID, 2);
                topMeshRend.material.SetInt(shaderParam.oneOrZeroID, 0);

                // For LWRP and URP 
                //topMeshRend.material.renderQueue = 3000;
            }
        }

        /// <summary>
        /// Tells the ripple sources the global position of the water line for the current object. 
        /// </summary>
        private void SetWaterLinePosition()
        {
            int size = rippleSourcesList.Count;
            for (int i = 0; i < size; i++)
            {
                if (rippleSourcesList[i] != null)
                {
                    rippleSourcesList[i].waterLineYAxisWorldPosition = water2DSim.waterLineCurrentWorldPos.y;
                }
            }

            size = kinematicRippleSourcesList.Count;
            for (int i = 0; i < size; i++)
            {
                if (kinematicRippleSourcesList[i] != null)
                {
                    kinematicRippleSourcesList[i].waterLineYAxisWorldPosition = water2DSim.waterLineCurrentWorldPos.y;
                }
            }

            prevWaterLineWorldPos = water2DSim.waterLineCurrentWorldPos.y;

        }

        /// <summary>
        /// Adds rain ripples. 
        /// </summary>
        private void AddRain()
        {
            if (rainDrops)
            {
                if (rippleSimulationUpdate == Water2D_RippleSimulationUpdate.UpdateMethod)
                    rainTimeCounter += Time.deltaTime;
                else
                    rainTimeCounter += Time.fixedDeltaTime;

                int loops = 0;
                float rainPeriod = 1f / rainDropFrequency;

                if (rainTimeCounter > rainPeriod)
                {
                    float dropRadiusToUV;
                    addInteraction = true;

                    while (rainTimeCounter > rainPeriod)
                    {
                        rainTimeCounter -= rainPeriod;
                        dropRadiusToUV = WorldSpaceValueToUV(rainDropRadius);

                        Vector2 dropUVPos = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                        AddRippleToShader(waterInteractions, dropUVPos, dropRadiusToUV, rainDropStrength);
                        waterInteractions++;
                        loops++;

                        if (loops > 6)
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Updates some variables used by the shader responsible for generating the heightmap.
        /// </summary>
        public void UpdateRippleShaderParameters()
        {
            switch (obstructionType)
            {
                case Water2D_ObstructionType.None:
                    heightMapMaterial.SetFloat(shaderParam.dampingID, waterDamping);
                    heightMapMaterial.SetVector(shaderParam.axisScaleID, xyAxisScale);
                    break;

                case Water2D_ObstructionType.TextureObstruction:
                    heightMapMaterialTexObst.SetFloat(shaderParam.dampingID, waterDamping);
                    heightMapMaterialTexObst.SetVector(shaderParam.axisScaleID, xyAxisScale);
                    break;

                case Water2D_ObstructionType.DynamicObstruction:
                    heightMapMaterialDynObst.SetFloat(shaderParam.dampingID, waterDamping);
                    heightMapMaterialDynObst.SetVector(shaderParam.axisScaleID, xyAxisScale);
                    break;
            }
        }

        /// <summary>
        /// Calculates the properties of the ripples caused by objects.
        /// </summary>
        private void UpdateWaterInteractions()
        {
            if (Mathf.Abs(prevWaterLineWorldPos - water2DSim.waterLineCurrentWorldPos.y) > 0.0001f)
                SetWaterLinePosition();

            DynamicObjectsInteraction();
            if (water2DTool.use3DCollider)
                MouseOnWater();
            RippleSourcesInteraction();

            if (playerOnExitRipple)
            {
                AddRippleAtPosition(playerOnExitRipplePos, playerRippleRadius, playerRippleStrength * strengthScale);
                playerOnExitRipple = false;
            }
        }

        /// <summary>
        /// Calculates the properties of the ripples caused by dynamic objects.
        /// </summary>
        private void DynamicObjectsInteraction()
        {
            if (water2DSim.floatingObjects.Count > 0)
            {
                int len = water2DSim.floatingObjects.Count;

                for (int i = 0; i < len; i++)
                {
                    if (!water2DSim.ColliderExists(i))
                        continue;

                    float topPoint = GetColliderTopPoint(water2DSim.floatingObjects[i]);

                    if (topPoint > water2DSim.waterLineCurrentWorldPos.y)
                    {
                        if (water2DSim.floatingObjects[i].IsPlayer())
                            PlayerInteraction(i);
                        else
                            ObjectInteraction(i);
                    }
                }
            }
        }

        private void PlayerInteraction(int oIndex)
        {
            if (water2DSim.characterControllerType == Water2D_CharacterControllerType.PhysicsBased)
            {
                float scale;
                Vector3 vel = water2DSim.floatingObjects[oIndex].GetVelocity();

                if (CanGenerateRipple(vel, oIndex, out scale))
                    PhysicsBasedCharacterController(oIndex, vel, scale);
            }
            else
            {
                RaycastBasedCharacterController(oIndex);
            }
        }


        /// <summary>
        /// Calculates the properties of the ripples caused by Player.
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        /// <param name="vel"> Player velocity.</param>
        /// <param name="scale">The index of an object stored in the floatingObjects list.</param>
        public void PhysicsBasedCharacterController(int oIndex, Vector3 vel, float scale)
        {
            Vector2 uv;
            float radiusToUV;
            Vector3 pos = Vector3.zero;
            Vector3 posOffset = Vector3.zero;

            pos = water2DSim.floatingObjects[oIndex].transform.position;
            addInteraction = true;

            if (Mathf.Abs(vel.x) > playerVelocityFilter)
                posOffset.x = playerRippleXOffset;

            radiusToUV = WorldSpaceValueToUV(playerRippleRadius);

            if (vel.x > 0)
                uv = GetUVFromPosition(pos + new Vector3(posOffset.x, 0, 0));
            else
                uv = GetUVFromPosition(pos - new Vector3(posOffset.x, 0, 0));

            AddRippleToShader(waterInteractions, uv, radiusToUV, playerRippleStrength * scale);
            waterInteractions++;
        }

        /// <summary>
        /// Calculates the properties of the ripples caused by Player.
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        private void RaycastBasedCharacterController(int oIndex)
        {
            float x, y;
            int currentYDir;
            Vector3 posOffset = Vector3.zero;

            Vector3 currentPos = water2DSim.floatingObjects[oIndex].transform.position;
            Vector3 previousPos = water2DSim.floatingObjects[oIndex].GetPreviousPosition();

            x = Mathf.Abs(currentPos.x - previousPos.x);
            y = Mathf.Abs(currentPos.y - previousPos.y);

            if (x < 0.0001f && y < 0.0001f)
            {
                water2DSim.floatingObjects[oIndex].SetPreviousPosition();
                return;
            }

            if (currentPos.x > previousPos.x)
                posOffset = new Vector3(playerRippleXOffset, 0, 0);
            else
                posOffset = new Vector3(-playerRippleXOffset, 0, 0);

            if (x > y)
            {
                AddRippleAtPosition(currentPos + posOffset, playerRippleRadius, playerRippleStrength);
            }
            else
            {
                if (currentPos.y > previousPos.y)
                    currentYDir = 1;
                else
                    currentYDir = -1;

                if (water2DSim.floatingObjects[oIndex].GetPreviousDirectionOnYAxis() != currentYDir)
                {
                    water2DSim.floatingObjects[oIndex].SetDirectionOnYAxis(currentYDir);
                    AddRippleAtPosition(currentPos + posOffset, playerRippleRadius, strengthScale * playerRippleStrength);
                }
            }

            water2DSim.floatingObjects[oIndex].SetPreviousPosition();
        }

        /// <summary>
        /// Checks if an object is moving or not.
        /// </summary>
        private bool CanGenerateRipple(Vector3 vel, int oIndex, out float scale)
        {
            float x, y;
            float velFilter = 0;

            x = Mathf.Abs(vel.x);
            y = Mathf.Abs(vel.y);
            scale = 1f;

            if (water2DSim.floatingObjects[oIndex].IsPlayer())
                velFilter = playerVelocityFilter;
            else
                velFilter = objectVelocityFilter;

            if (x > velFilter)
                return true;

            if (y > velFilter)
            {
                int currentDir;

                if (vel.y > 0)
                    currentDir = 1;
                else
                    currentDir = -1;
                if (water2DSim.floatingObjects[oIndex].GetPreviousDirectionOnYAxis() != currentDir)
                {
                    water2DSim.floatingObjects[oIndex].SetDirectionOnYAxis(currentDir);
                    scale = strengthScale;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Generates a ripple at the specified position. 
        /// If the specified position is not inside the X, Z bounding box of the water, no ripple will be generated.
        /// </summary>
        /// <param name="pos">World space postion of the ripple. Only the X a Z values are used</param>
        /// <param name="radius">The radius of the ripple in Unity units.</param>
        /// <param name="strength">The strength of the ripple.</param>
        public void AddRippleAtPosition(Vector3 pos, float radius, float strength)
        {
            if (IsInside(pos))
            {
                Vector2 uv;
                float radiusToUV;

                addInteraction = true;
                radiusToUV = WorldSpaceValueToUV(radius);
                uv = GetUVFromPosition(pos);

                AddRippleToShader(waterInteractions, uv, radiusToUV, strength);
                waterInteractions++;
            }
        }

        /// <summary>
        /// Calculates the properties of the ripples caused by a dynamic object.
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        private void ObjectInteraction(int oIndex)
        {
            float scale;
            Vector3 vel = water2DSim.floatingObjects[oIndex].GetVelocity();

            if (!CanGenerateRipple(vel, oIndex, out scale))
                return;

            Vector2 uv;
            float radius;
            float radiusToUV;
            Vector3 pos = Vector3.zero;
            Vector3 posOffset = Vector3.zero;

            pos = water2DSim.floatingObjects[oIndex].bounds.center;
            addInteraction = true;

            if (fixedRadius)
                radius = objectRippleRadius;
            else
                radius = water2DSim.floatingObjects[oIndex].GetRadius() * objectRadiusScale;

            radiusToUV = WorldSpaceValueToUV(radius);

            if (Mathf.Abs(vel.x) > objectVelocityFilter)
                posOffset.x = water2DSim.floatingObjects[oIndex].bounds.extents.x * objectXAxisRippleOffset * 2f;

            if (vel.x > 0)
                uv = GetUVFromPosition(pos + new Vector3(posOffset.x, 0, 0));
            else
                uv = GetUVFromPosition(pos - new Vector3(posOffset.x, 0, 0));

            AddRippleToShader(waterInteractions, uv, radiusToUV, objectRippleStrength * scale);
            waterInteractions++;
        }

        /// <summary>
        /// Calculates the properties of the ripples caused by objects that have the RippleSource script attached to them.
        /// </summary>
        private void RippleSourcesInteraction()
        {
            float rippleSourceUnitsToUV;
            Vector2 uv;
            int len;

            if (rippleSources)
            {
                len = rippleSourcesList.Count;

                for (int i = 0; i < len; i++)
                {
                    if (rippleSourcesList[i] != null && rippleSourcesList[i].newRipple && IsInside(rippleSourcesList[i].transform.position))
                    {
                        rippleSourceUnitsToUV = WorldSpaceValueToUV(rippleSourcesList[i].radius);

                        uv = GetUVFromPosition(rippleSourcesList[i].transform.position);
                        AddRippleToShader(waterInteractions, uv, rippleSourceUnitsToUV, rippleSourcesList[i].strength);

                        rippleSourcesList[i].newRipple = false;
                        addInteraction = true;
                        waterInteractions++;
                    }
                }
            }

            len = kinematicRippleSourcesList.Count;

            for (int i = 0; i < len; i++)
            {
                if (kinematicRippleSourcesList[i] != null && kinematicRippleSourcesList[i].newRipple && IsInside(kinematicRippleSourcesList[i].transform.position))
                {
                    rippleSourceUnitsToUV = WorldSpaceValueToUV(kinematicRippleSourcesList[i].radius);

                    uv = GetUVFromPosition(kinematicRippleSourcesList[i].transform.position);
                    AddRippleToShader(waterInteractions, uv, rippleSourceUnitsToUV, kinematicRippleSourcesList[i].strength);

                    kinematicRippleSourcesList[i].newRipple = false;
                    addInteraction = true;
                    waterInteractions++;
                }
            }
        }

        /// <summary>
        /// Checks if a point is inside the waters bound. The position of the point on the Y axis is ignored. 
        /// </summary>
        /// <param name="point">Point in world space coordinates.</param>
        /// <returns> Returns true if the point inside the waters bounds. False if not inside.</returns>
        private bool IsInside(Vector3 point)
        {
            if (point.x > leftHandleWorldPos.x && point.x < (leftHandleWorldPos.x + waterWidth) && point.z > leftHandleWorldPos.z && point.z < (leftHandleWorldPos.z + waterLength))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calculates the properties of the ripples caused by the mouse arrow.
        /// </summary>
        private void MouseOnWater()
        {
            if (mouseInteraction && Input.GetMouseButton(0))
            {
                var cam = Camera.main;
                RaycastHit info;

                if (waterBoxCollider.Raycast(cam.ScreenPointToRay(Input.mousePosition), out info, float.MaxValue))
                {
                    Vector2 uv;
                    float mouseRadiusToUV;

                    addInteraction = true;
                    mouseRadiusToUV = WorldSpaceValueToUV(mouseRadius);
                    uv = GetUVFromPosition(info.point);

                    AddRippleToShader(waterInteractions, uv, mouseRadiusToUV, mouseStregth);
                    waterInteractions++;
                }
            }
        }


        /// <summary>
        /// Resets the variables used by the shader to add new ripples the height map.
        /// </summary>
        private void ResetWaterInteractionParameters()
        {
            if (addInteraction)
            {
                int len = waterInteractions <= 10 ? waterInteractions : 10;

                for (int i = 1; i <= len; i++)
                    AddRippleToShader(i, Vector2.zero, 0, 0);

                addInteraction = false;
                waterInteractions = 0;
            }
        }

        /// <summary>
        /// Finds a colliders bounding box top point position on the Y axis.
        /// </summary>
        private float GetColliderTopPoint(FloatingObject floatingObj)
        {
            return floatingObj.bounds.extents.y + floatingObj.bounds.center.y;
        }

        /// <summary>
        /// Converts a world space position to a texture position.
        /// </summary>
        /// <param name="worldPos"> A Point in world space. </param>
        private Vector2 GetUVFromPosition(Vector3 worldPos)
        {
            Vector2 uv;

            float u = Mathf.Abs(worldPos.x - leftHandleWorldPos.x) * rWaterWidth;
            float v = Mathf.Abs(worldPos.z - transform.position.z) * rWaterLength;

            uv = new Vector2(u, 1 - v);

            return uv;
        }

        /// <summary>
        /// Converts a world space radius to a texture radius.
        /// </summary>
        /// <param name="radius"> A world space radius value </param>
        /// <returns> Returns a UV space value.</returns>
        private float WorldSpaceValueToUV(float radius)
        {
            return texelSize * radius;
        }

        /// <summary>
        /// Stores the properties of a water ripple in a shader variable.
        /// </summary>
        /// <param name="index"> The index of the variable where the ripple properties should be stored. </param>
        /// <param name="uvPos"> The center position of the ripple in UV space. </param>
        /// <param name="uvRadius"> The ripple radius in UV space. </param>
        /// <param name="strength"> The ripple strength. </param>
        private void AddRippleToShader(int index, Vector2 uvPos, float uvRadius, float strength)
        {
            if (index < 10)
            {
                switch (obstructionType)
                {
                    case Water2D_ObstructionType.None:
                        heightMapMaterial.SetVector(shaderParam.WaterRippleID[index], new Vector4(uvPos.x, uvPos.y, uvRadius, strength));
                        break;

                    case Water2D_ObstructionType.TextureObstruction:
                        heightMapMaterialTexObst.SetVector(shaderParam.WaterRippleID[index], new Vector4(uvPos.x, uvPos.y, uvRadius, strength));
                        break;

                    case Water2D_ObstructionType.DynamicObstruction:
                        heightMapMaterialDynObst.SetVector(shaderParam.WaterRippleID[index], new Vector4(uvPos.x, uvPos.y, uvRadius, strength));
                        break;
                }
            }
        }

        /// <summary>
        /// Udates the varibles that store the dynamic obstructions.
        /// </summary>
        private void UpdateDynamicObstructions()
        {
            int recColLen = boxColObstList.Count;
            int sphereCollen = sphereColObstList.Count;

            if (recColLen > 5)
                recColLen = 5;
            if (sphereCollen > 5)
                sphereCollen = 5;

            for (int i = 0; i < 5; i++)
            {
                Vector4 rectangleObstruction = new Vector4(0, 0, 0, 0);
                heightMapMaterialDynObst.SetVector(shaderParam.recObstVarIDs[i], rectangleObstruction);
            }

            for (int i = 0; i < 5; i++)
            {
                Vector3 rectangleObstruction = new Vector3(0, 0, 0);
                heightMapMaterialDynObst.SetVector(shaderParam.cirObstVarIDs[i], rectangleObstruction);
            }

            for (int i = 0; i < recColLen; i++)
            {
                if (boxColObstList[i] == null)
                {
                    boxColObstList.RemoveAt(i);
                    recColLen--;
                    continue;
                }

                Vector4 rectangleObstruction = GetRectangleObstruction(boxColObstList[i]);
                heightMapMaterialDynObst.SetVector(shaderParam.recObstVarIDs[i], rectangleObstruction);
            }

            for (int i = 0; i < sphereCollen; i++)
            {
                if (sphereColObstList[i] == null)
                {
                    sphereColObstList.RemoveAt(i);
                    sphereCollen--;
                    continue;
                }

                Vector4 rectangleObstruction = GetCicleObstruction(sphereColObstList[i]);
                heightMapMaterialDynObst.SetVector(shaderParam.cirObstVarIDs[i], rectangleObstruction);
            }
        }

        /// <summary>
        /// Calculates the center and radius in texture space for a sphere collider.
        /// </summary>
        private Vector3 GetCicleObstruction(SphereCollider sphere)
        {
            Vector3 TopHandleGlobalPos = transform.TransformPoint(water2DTool.handlesPosition[0]);
            Vector3 obstaclePos = sphere.transform.position;
            float centerU, centerV;

            float scale = sphere.transform.localScale.x;

            if (sphere.transform.localScale.y > sphere.transform.localScale.x)
                scale = sphere.transform.localScale.y;

            if (sphere.transform.localScale.z > sphere.transform.localScale.y && sphere.transform.localScale.z > sphere.transform.localScale.x)
                scale = sphere.transform.localScale.y;

            float radius = sphere.radius * scale;
            float d = Mathf.Abs(obstaclePos.y - TopHandleGlobalPos.y);
            float r = Mathf.Sqrt(radius * radius - d * d);

            if (leftHandleWorldPos.x < obstaclePos.x && leftHandleWorldPos.x + waterWidth > obstaclePos.x)
            {
                centerU = (Mathf.Abs(leftHandleWorldPos.x - obstaclePos.x)) * rWaterWidth;
            }
            else
            {
                if (leftHandleWorldPos.x >= obstaclePos.x)
                    centerU = 0;
                else
                    centerU = 1;
            }

            if (leftHandleWorldPos.z < obstaclePos.z && leftHandleWorldPos.z + waterLength > obstaclePos.z)
            {
                centerV = 1 - (Mathf.Abs(leftHandleWorldPos.z - obstaclePos.z)) * rWaterLength;
            }
            else
            {
                if (leftHandleWorldPos.z >= obstaclePos.z)
                    centerV = 1;
                else
                    centerV = 0;

            }

            r *= rWaterLength;

            return new Vector3(centerU, centerV, r);
        }

        /// <summary>
        /// Calculates rectangles 4 corners position in texture space.
        /// </summary>
        private Vector4 GetRectangleObstruction(Collider other)
        {
            float minX, maxX, minY, maxY;
            Vector3 obstaclePos = other.transform.position;
            Vector3 obstacleColExtents = other.bounds.extents;
            Vector3 topHandleGlobalPos = transform.TransformPoint(water2DTool.handlesPosition[0]);

            if (other.bounds.center.y + other.bounds.extents.y < topHandleGlobalPos.y)
                return new Vector4(0, 0, 0, 0);

            if (leftHandleWorldPos.x < obstaclePos.x && leftHandleWorldPos.x + waterWidth > obstaclePos.x)
            {
                minX = (Mathf.Abs(leftHandleWorldPos.x - obstaclePos.x) - obstacleColExtents.x) * rWaterWidth;
                maxX = (Mathf.Abs(leftHandleWorldPos.x - obstaclePos.x) + obstacleColExtents.x) * rWaterWidth;
            }
            else
            {
                if (leftHandleWorldPos.x >= obstaclePos.x)
                {
                    minX = 0;
                    maxX = (Mathf.Abs(obstacleColExtents.x - (leftHandleWorldPos.x - obstaclePos.x))) * rWaterWidth;
                }
                else
                {
                    minX = (Mathf.Abs(obstacleColExtents.x - (obstaclePos.x - leftHandleWorldPos.x + waterLength))) * rWaterWidth;
                    maxX = 1;
                }
            }

            if (leftHandleWorldPos.z < obstaclePos.z && leftHandleWorldPos.z + waterLength > obstaclePos.z)
            {
                maxY = 1 - (Mathf.Abs(leftHandleWorldPos.z - obstaclePos.z) - obstacleColExtents.z) * rWaterLength;
                minY = 1 - (Mathf.Abs(leftHandleWorldPos.z - obstaclePos.z) + obstacleColExtents.z) * rWaterLength;
            }
            else
            {
                if (leftHandleWorldPos.z >= obstaclePos.z)
                {
                    minY = 1 - (Mathf.Abs(obstacleColExtents.z - (leftHandleWorldPos.z - obstaclePos.z))) * rWaterLength;
                    maxY = 1;
                }
                else
                {
                    maxY = 1 - (Mathf.Abs(obstacleColExtents.z - (obstaclePos.z - leftHandleWorldPos.z))) * rWaterLength;
                    minY = 0;
                }
            }

            if (minX < 0)
                minX = 0;
            if (minY < 0)
                minY = 0;

            if (maxX > 1)
                maxX = 1;
            if (maxY > 1)
                maxY = 1;


            return new Vector4(minX, minY, maxX, maxY);
        }

        /// <summary>
        /// Sets the values for the variables used to generate sine waves.
        /// </summary>
        public void SetAmbientWavesShaderParameters()
        {
            ambientWavesMat.SetFloat(shaderParam.waterWidthID, water2DTool.width);
            ambientWavesMat.SetFloat(shaderParam.amplitude1ID, amplitude1);
            ambientWavesMat.SetFloat(shaderParam.amplitude2ID, amplitude2);
            ambientWavesMat.SetFloat(shaderParam.amplitude3ID, amplitude3);
            ambientWavesMat.SetFloat(shaderParam.waveLength1ID, waveLength1);
            ambientWavesMat.SetFloat(shaderParam.waveLength2ID, waveLength2);
            ambientWavesMat.SetFloat(shaderParam.waveLength3ID, waveLength3);
            ambientWavesMat.SetFloat(shaderParam.phaseOffset1ID, phaseOffset1);
            ambientWavesMat.SetFloat(shaderParam.phaseOffset2ID, phaseOffset2);
            ambientWavesMat.SetFloat(shaderParam.phaseOffset3ID, phaseOffset3);
            ambientWavesMat.SetFloat(shaderParam.fadeDistanceID, 1f / Mathf.Abs(amplitudeFadeEnd - amplitudeFadeStart));
            ambientWavesMat.SetFloat(shaderParam.fadeEndID, amplitudeFadeEnd);

            if (amplitudeFadeStart < amplitudeFadeEnd)
                ambientWavesMat.SetFloat(shaderParam.fadeDirectionID, 1);
            else
                ambientWavesMat.SetFloat(shaderParam.fadeDirectionID, 0);

            if (amplitudeZAxisFade)
                ambientWavesMat.SetFloat(shaderParam.amplitudeFadeID, 1);
            else
                ambientWavesMat.SetFloat(shaderParam.amplitudeFadeID, 0);
        }

        private void WaterObjectMoved()
        {
            if (prevPos != transform.position)
            {
                leftHandleWorldPos = transform.TransformPoint(water2DTool.handlesPosition[2]);
                prevPos = transform.position;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            RippleSource ripple = other.GetComponent<RippleSource>();

            if (ripple)
                kinematicRippleSourcesList.Add(ripple);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            RippleSource ripple = other.GetComponent<RippleSource>();

            if (ripple)
            {
                if (kinematicRippleSourcesList.Contains(ripple))
                    kinematicRippleSourcesList.Remove(ripple);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsInLayerMask(obstructionLayers.value, other.gameObject.layer))
            {
                BoxCollider boxColl = other.GetComponent<BoxCollider>();
                if (boxColl != null && !boxColObstList.Contains(boxColl))
                {
                    boxColObstList.Add(boxColl);
                }

                SphereCollider sphere = other.GetComponent<SphereCollider>();
                if (sphere != null && !sphereColObstList.Contains(sphere))
                {
                    sphereColObstList.Add(sphere);
                }
            }

            RippleSource ripple = other.GetComponent<RippleSource>();

            if (ripple)
            {
                kinematicRippleSourcesList.Add(ripple);
            }
        }

        void OnTriggerExit(Collider other)
        {
            BoxCollider boxColl = other.GetComponent<BoxCollider>();

            if (boxColl != null && boxColObstList.Contains(boxColl))
            {
                boxColObstList.Remove(boxColl);
            }

            SphereCollider sphere = other.GetComponent<SphereCollider>();
            if (sphere != null && sphereColObstList.Contains(sphere) && other.GetComponent<SphereCollider>())
            {
                sphereColObstList.Remove(sphere);
            }


            RippleSource ripple = other.GetComponent<RippleSource>();

            if (ripple)
            {
                if (kinematicRippleSourcesList.Contains(ripple))
                    kinematicRippleSourcesList.Remove(ripple);
            }
        }

        /// <summary>
        /// Checks whether the current object layer is enabled on a layer mask.
        /// </summary>
        public static bool IsInLayerMask(int layerMask, int objectLayerIndex)
        {
            if (objectLayerIndex == 4 || objectLayerIndex == 5)
                objectLayerIndex -= 1;

            if (objectLayerIndex > 5)
                objectLayerIndex -= 3;

            int objLayerMask = (1 << objectLayerIndex);

            return (layerMask & objLayerMask) != 0;
        }

        #endregion Class methods
    }
}