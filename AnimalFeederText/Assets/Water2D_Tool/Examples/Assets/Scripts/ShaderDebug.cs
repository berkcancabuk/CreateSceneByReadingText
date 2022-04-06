using UnityEngine;
using System.Collections;

namespace Water2DTool {
    [ExecuteInEditMode]
    public class ShaderDebug : MonoBehaviour {

        public enum RenderOptions {
            Color = 2,
            HeightMap = 1,
            Normal = 0,
        }

        //public enum ColorMode
        //{
        //    ColorBlend = 1,
        //    DifuseLight = 0,
        //}

        public Material sharedMaterial;
        public RenderOptions renderOptions = RenderOptions.Color;
        //public ColorMode colorMode = ColorMode.ColorBlend;

        public void UpdateShader()
        {
            if (renderOptions == RenderOptions.Color)
            {
                Shader.EnableKeyword("WATER_COLOR");

                Shader.DisableKeyword("WATER_NORMAL");
                Shader.DisableKeyword("WATER_HEIGHTMAP");
            }

            if (renderOptions == RenderOptions.HeightMap)
            {
                Shader.EnableKeyword("WATER_HEIGHTMAP");

                Shader.DisableKeyword("WATER_COLOR");
                Shader.DisableKeyword("WATER_NORMAL");
            }


            if (renderOptions == RenderOptions.Normal)
            {
                Shader.EnableKeyword("WATER_NORMAL");

                Shader.DisableKeyword("WATER_COLOR");
                Shader.DisableKeyword("WATER_HEIGHTMAP");
            }

            //if (colorMode == ColorMode.ColorBlend)
            //{
            //    Shader.EnableKeyword("WATER_COLOR_BLEND");

            //    Shader.DisableKeyword("WATER_DIFUSE");
            //}

            //if (colorMode == ColorMode.DifuseLight)
            //{
            //    Shader.EnableKeyword("WATER_DIFUSE");

            //    Shader.DisableKeyword("WATER_COLOR_BLEND");
            //}
        }

        public void Update()
        {
            if (sharedMaterial)
            {
                UpdateShader();
            }
        }
    }
}
