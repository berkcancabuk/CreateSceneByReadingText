using UnityEngine;

namespace UnityEditor
{
    public class Water2DShaderInspector_FM : ShaderGUI
    {
        MaterialProperty shallowWaterColor = null;
        MaterialProperty deepWaterColor = null;
        MaterialProperty edgeBlend = null;
        MaterialProperty waterDepth = null;
        MaterialProperty opacity = null;
        MaterialProperty depthFogToggle = null;
        MaterialProperty fogDepth = null;
        MaterialProperty fogFalloff = null;
        MaterialProperty fogDensityCap = null;

        MaterialProperty distortion = null;
        MaterialProperty bumpWaves = null;
        MaterialProperty bumpTiling = null;
        MaterialProperty bumpDirection = null;
        MaterialProperty bumpMap = null;

        MaterialProperty waterLineTexture = null;
        MaterialProperty waterLineColor = null;
        MaterialProperty pixelsPerUnits = null;

        MaterialEditor mMatEditor;

        public void FindProperties(MaterialProperty[] mProps)
        {
            
            shallowWaterColor = FindProperty("_ShallowWaterColor", mProps);
            deepWaterColor = FindProperty("_DeepWaterColor", mProps);
            edgeBlend = FindProperty("_EdgeBlend", mProps);
            waterDepth = FindProperty("_WaterDepth", mProps);
            opacity = FindProperty("_Opacity", mProps);
            depthFogToggle = FindProperty("_DEPTHFOG", mProps);
            fogDepth = FindProperty("_FogDepth", mProps);
            fogFalloff = FindProperty("_FogFalloff", mProps);
            fogDensityCap = FindProperty("_FogDensityCap", mProps);

            distortion = FindProperty("_Distortion", mProps);
            bumpWaves = FindProperty("_BumpWaves", mProps);
            bumpTiling = FindProperty("_BumpTiling", mProps);
            bumpDirection = FindProperty("_BumpDirection", mProps);
            bumpMap = FindProperty("_BumpMap", mProps);

            waterLineTexture = FindProperty("_WaterLineTex", mProps);
            waterLineColor = FindProperty("_WaterLineColor", mProps);
            pixelsPerUnits = FindProperty("_PixelsPerUnit", mProps);

        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] mProps)
        {
            mMatEditor = materialEditor;
            Material material = materialEditor.target as Material;

            FindProperties(mProps);
            ShaderPropertiesGUI(material);
        }

        public void ShaderPropertiesGUI(Material material)
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorGUIUtility.fieldWidth = 64f;

                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Color"), EditorStyles.boldLabel);

                    mMatEditor.ShaderProperty(shallowWaterColor, "Shallow Water Color");
                    mMatEditor.ShaderProperty(deepWaterColor, "Deep Water Color");
                    mMatEditor.ShaderProperty(opacity, "Opacity");
                    mMatEditor.ShaderProperty(edgeBlend, "Edge Blend");
                    mMatEditor.ShaderProperty(waterDepth, "Water Depth");
                    mMatEditor.ShaderProperty(depthFogToggle, "Fog");
                    if (depthFogToggle.floatValue == 1)
                    {
                        mMatEditor.ShaderProperty(fogDepth, "Fog Depth");
                        mMatEditor.ShaderProperty(fogFalloff, "Fog Falloff");
                        mMatEditor.ShaderProperty(fogDensityCap, "Fog Density Cap");
                    }
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Distortion"), EditorStyles.boldLabel);

                    mMatEditor.ShaderProperty(distortion, "Distortion");
                    mMatEditor.ShaderProperty(bumpWaves, "Bump Waves");
                    mMatEditor.ShaderProperty(bumpTiling, "Bump Tiling");
                    mMatEditor.ShaderProperty(bumpDirection, "Bump Direction");
                    mMatEditor.TexturePropertySingleLine(new GUIContent("Bump Map"), bumpMap);

                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Water Line"), EditorStyles.boldLabel);

                    mMatEditor.ShaderProperty(waterLineColor, "Water Line Color");
                    mMatEditor.TexturePropertySingleLine(new GUIContent("Water Line Texture"), waterLineTexture);
                    mMatEditor.ShaderProperty(pixelsPerUnits, "Pixels Per Units");

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
    }
}
