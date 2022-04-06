using UnityEngine;
using Water2DTool;
using UnityEngine.UI;
//using UnityEngine.PostProcessing;

namespace Water2DTool {
    public class WaterSettings : MonoBehaviour {

        private Water2D_Tool water2D;
        private Water2D_Simulation sim;
        private Water2D_Ripple ripple;
        public GameObject waterObject;
        public Renderer topMeshRenderer;

        public Text rainDropFrequency;

        public Slider rainDropSlider;
        public Text segmentsToUnitsTextValue;
        public Text topMeshVertices;
        public Text rtValue;
        public Text rtResolution;
        public Text helpInfo;

        public GameObject obstructionObjects;

        public Button lockFPSButton;
        public Button unlockFPSButton;

        public Button hideUI;
        public Button showUI;
        public GameObject leftSideUI;
        public Text controlsInfo;
        public Toggle amplitudeFadeToggle;
        public Dropdown obstructionsDropdown;
        public Animator waterLineAnimtor;
        public Slider meshQualitySlider;
        public Slider rtResolutionSlider;

        private void Start()
        {
            water2D = waterObject.GetComponent<Water2D_Tool>();
            sim = waterObject.GetComponent<Water2D_Simulation>();
            ripple = waterObject.GetComponent<Water2D_Ripple>();

            rtValue.text = ripple.rtPixelsToUnits.ToString();
            ShowRenderTextureResolution();
            ShowVerticesNumber();
        }

        public void EnableHeightAnimation(bool anim)
        {
            waterLineAnimtor.enabled = anim;

            if (anim)
            {
                meshQualitySlider.gameObject.SetActive(false);
                rtResolutionSlider.gameObject.SetActive(false);
            }
            else
            {
                meshQualitySlider.gameObject.SetActive(true);
                rtResolutionSlider.gameObject.SetActive(true);
            }
        }

        public void EnableRain(bool rain)
        {
            ripple.rainDrops = rain;
            rainDropSlider.gameObject.SetActive(rain);
        }

        public void EnableAmbiantWaves(bool ambiantWaves)
        {
            ripple.ambientWaves = ambiantWaves;
            amplitudeFadeToggle.gameObject.SetActive(ambiantWaves);

        }

        public void EnableAmplitudeFade(bool amplitudeFade)
        {
            ripple.amplitudeZAxisFade = amplitudeFade;
            ripple.SetAmbientWavesShaderParameters();
        }

        public void SetSegmentsToUnits(float seg)
        {
            water2D.segmentsPerUnit = seg;
            water2D.RecreateWaterMesh();
            sim.ResetVariables();
            ripple.InstantiateRenderTextures();
            segmentsToUnitsTextValue.text = seg.ToString();
            ShowVerticesNumber();
        }

        public void SetDropsPerSecond(float drops)
        {
            ripple.rainDropFrequency = drops;
        }

        public void SetRainDropFrequency(float frequency)
        {
            frequency = (int)frequency;
            ripple.rainDropFrequency = frequency;
            rainDropFrequency.text = frequency.ToString();
        }

        private void ShowVerticesNumber()
        {
            float vertices = water2D.xVerts * water2D.zVerts + water2D.xVerts * 2;
            topMeshVertices.text = "Mesh Vertices: " + vertices;
        }

        private void ShowRenderTextureResolution()
        {
            rtResolution.text = "RT Resolution: " + water2D.renderTextureWidth + " x " + water2D.renderTextureHeight;
        }

        public void SetRenderTexturePixelsToUnits(float pixelsTOUnits)
        {
            ripple.rtPixelsToUnits = (int)pixelsTOUnits;
            water2D.RecreateWaterMesh();
            sim.ResetVariables();
            ripple.InstantiateRenderTextures();
            rtValue.text = ripple.rtPixelsToUnits.ToString();
            ShowRenderTextureResolution();
        }

        public void ObstructionsDropDownmenu(int index)
        {
            if (index == 0)
            {
                obstructionObjects.SetActive(false);
                ripple.obstructionType = Water2D_ObstructionType.None;

            }
            else if (index == 1)
            {
                obstructionObjects.SetActive(true);
                ripple.obstructionType = Water2D_ObstructionType.DynamicObstruction;
            }
            else
            {
                obstructionObjects.SetActive(true);
                ripple.obstructionType = Water2D_ObstructionType.TextureObstruction;
            }

            ripple.UpdateRippleShaderParameters();
        }

        public void EnableTextureObstruction(bool obstruction)
        {
            obstructionObjects.SetActive(obstruction);

            if (obstruction)
                ripple.obstructionType = Water2D_ObstructionType.TextureObstruction;
            else
                ripple.obstructionType = Water2D_ObstructionType.None;

            ripple.UpdateRippleShaderParameters();
        }

        public void OnSliderSegmentsToUnits()
        {
            helpInfo.gameObject.SetActive(true);
            helpInfo.text = "Controls the quality of the mesh. The bigger the value the more vertices the mesh has.";
            controlsInfo.gameObject.SetActive(false);

        }

        public void OnSliderSimulationSpeed()
        {
            helpInfo.gameObject.SetActive(true);
            helpInfo.text = "Controls the speed of wave propagation. If the games FPS is greater than the value of Simulation Speed, the water will  be updated " + "" +
                 "based on the value of Simulation Speed (basically limits the number of times the water is updated per second). " +
                 " If the game FPS is lower than the Simulation Speed, than the water will be updated based on the games current FPS.";
            controlsInfo.gameObject.SetActive(false);
        }

        public void OnSliderRTPixelsToUnits()
        {
            helpInfo.gameObject.SetActive(true);
            helpInfo.text = "The number of pixels that should fit in one unit of Unity space." +
                " A value of 15 means that if the water has a width (X Axis) and length (Z axis) of 10,"
                + " the resolution of the render texture will be 150 x 150 pixels";

            controlsInfo.gameObject.SetActive(false);

        }

        public void OnRainDropFrequency()
        {
            helpInfo.gameObject.SetActive(true);
            helpInfo.text = "Controls how many rain drops should fall in a second.";
            controlsInfo.gameObject.SetActive(false);
        }

        public void DisableHelpText()
        {
            helpInfo.gameObject.SetActive(false);
            controlsInfo.gameObject.SetActive(true);
        }

        public void LockFPS()
        {
            Application.targetFrameRate = 60;
            lockFPSButton.gameObject.SetActive(false);
            unlockFPSButton.gameObject.SetActive(true);
        }

        public void UnlockFPS()
        {
            lockFPSButton.gameObject.SetActive(true);
            unlockFPSButton.gameObject.SetActive(false);
            Application.targetFrameRate = 1000;
        }

        public void WaterObjectSetActive(bool enabled)
        {
            waterObject.gameObject.SetActive(enabled);
        }

        public void ShowUI()
        {
            leftSideUI.gameObject.SetActive(true);
            showUI.gameObject.SetActive(false);
            hideUI.gameObject.SetActive(true);
        }
        public void HideUI()
        {
            leftSideUI.gameObject.SetActive(false);
            hideUI.gameObject.SetActive(false);
            showUI.gameObject.SetActive(true);
        }

        public void PostProcessingSetActive(bool enabled)
        {
            //Camera.main.GetComponent<PostProcessingBehaviour>().enabled = enabled;
        }
    }
}
