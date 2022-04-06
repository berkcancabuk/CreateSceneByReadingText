using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water2DTool
{
    public class ShaderParam
    {
        /// <summary>
        /// The IDs of shader variables that store the center, radius and strength of a water ripple.
        /// </summary>
        public int[] WaterRippleID;
        /// <summary>
        /// The IDs of shader variables that store the positions of the 4 corners of a dynamic obstruction, in texture space.
        /// </summary>
        public int[] recObstVarIDs;
        /// <summary>
        /// The IDs of shader variables that store the center and radius of a dynamic obstruction caused by a sphere collider, in texture space.
        /// </summary>
        public int waterWidthID;
        public int amplitude1ID;
        public int amplitude2ID;
        public int amplitude3ID;
        public int waveLength1ID;
        public int waveLength2ID;
        public int waveLength3ID;
        public int phaseOffset1ID;
        public int phaseOffset2ID;
        public int phaseOffset3ID;
        public int fadeDistanceID;
        public int fadeStartID;
        public int fadeEndID;
        public int fadeDirectionID;
        public int amplitudeFadeID;

        public int[] cirObstVarIDs;
        public int prevTexID;
        public int faceCullingID;
        public int oneOrZeroID;
        public int waveHeightScaleID;
        public int bottomPosID;
        public int dampingID;
        public int axisScaleID;
        public int applyOffset;

        public ShaderParam()
        {
            WaterRippleID = new int[10];
            recObstVarIDs = new int[5];
            cirObstVarIDs = new int[5];

            for (int i = 0; i < 10; i++)
            {
                int k = i + 1;
                WaterRippleID[i] = Shader.PropertyToID("_WaterRipple" + k);
            }

            for (int i = 0; i < 5; i++)
            {
                int k = i + 1;
                recObstVarIDs[i] = Shader.PropertyToID("_RecObst" + k);
                cirObstVarIDs[i] = Shader.PropertyToID("_CircleObst" + k);
            }

            prevTexID = Shader.PropertyToID("_PrevTex");
            waveHeightScaleID = Shader.PropertyToID("_WaveHeightScale");
            bottomPosID = Shader.PropertyToID("_BottomPos");
            faceCullingID = Shader.PropertyToID("_FaceCulling");
            oneOrZeroID = Shader.PropertyToID("_OneOrZero");
            dampingID = Shader.PropertyToID("_Damping");
            axisScaleID = Shader.PropertyToID("_AxisScale");

            waterWidthID = Shader.PropertyToID("_WaterWidth");
            amplitude1ID = Shader.PropertyToID("_Amplitude1");
            amplitude2ID = Shader.PropertyToID("_Amplitude2");
            amplitude3ID = Shader.PropertyToID("_Amplitude3");
            waveLength1ID = Shader.PropertyToID("_WaveLength1");
            waveLength2ID = Shader.PropertyToID("_WaveLength2");
            waveLength3ID = Shader.PropertyToID("_WaveLength3");
            phaseOffset1ID = Shader.PropertyToID("_PhaseOffset1");
            phaseOffset2ID = Shader.PropertyToID("_PhaseOffset2");
            phaseOffset3ID = Shader.PropertyToID("_PhaseOffset3");
            fadeDistanceID = Shader.PropertyToID("_FadeDistance");
            amplitudeFadeID = Shader.PropertyToID("_AmplitudeFade");
            fadeStartID = Shader.PropertyToID("_FadeStart");
            fadeEndID = Shader.PropertyToID("_FadeEnd");
            fadeDirectionID = Shader.PropertyToID("_FadeDirection");
            applyOffset = Shader.PropertyToID("_ApplyOffset");
        }
    }
}
