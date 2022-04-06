using UnityEngine;

namespace UnityEditor
{
    public class Water2DShaderInspector_TM : ShaderGUI
    {
        MaterialProperty shallowWaterColor = null;
        MaterialProperty deepWaterColor = null;
        MaterialProperty edgeBlend = null;
        MaterialProperty waterDepth = null;
        MaterialProperty underWaterOpacity = null;
        MaterialProperty cubeMapLevel = null;
        MaterialProperty smoothness = null;
        MaterialProperty cube = null;

        MaterialProperty fresnelPower = null;
        MaterialProperty fresnelBias = null;

        MaterialProperty specularToggle = null;
        MaterialProperty specularColor = null;
        MaterialProperty worldLightDir = null;
        MaterialProperty shininess = null;

        MaterialProperty distortion = null;
        MaterialProperty bumpTiling = null;
        MaterialProperty bumpDirection = null;
        MaterialProperty bumpMap = null;

        MaterialProperty foamToggle = null;
        MaterialProperty foamColor = null;
        MaterialProperty foamTex = null;
        MaterialProperty foamGradient = null;
        MaterialProperty foamStrength = null;

        MaterialProperty normals = null;
        MaterialProperty normalsStrength = null;

        MaterialEditor mMatEditor;

        public void FindProperties(MaterialProperty[] mProps)
        {
            shallowWaterColor = FindProperty("_ShallowWaterColor", mProps);
            deepWaterColor = FindProperty("_DeepWaterColor", mProps);
            edgeBlend = FindProperty("_EdgeBlend", mProps);
            waterDepth = FindProperty("_WaterDepth", mProps);
            underWaterOpacity = FindProperty("_UnderWaterOpacity", mProps);
            cubeMapLevel = FindProperty("_CubeMapLevel", mProps);
            cube = FindProperty("_Cube", mProps);

            specularToggle = FindProperty("_SPECULAR", mProps);
            specularColor = FindProperty("_SpecularColor", mProps);
            worldLightDir = FindProperty("_WorldLightDir", mProps);
            shininess = FindProperty("_Shininess", mProps);

            smoothness = FindProperty("_Smoothness", mProps);
            fresnelPower = FindProperty("_FresnelPower", mProps);
            fresnelBias = FindProperty("_FresnelBias", mProps);

            distortion = FindProperty("_Distortion", mProps);
            bumpTiling = FindProperty("_BumpTiling", mProps);
            bumpDirection = FindProperty("_BumpDirection", mProps);
            bumpMap = FindProperty("_BumpMap", mProps);

            foamToggle = FindProperty("_FOAM", mProps);
            foamColor = FindProperty("_FoamColor", mProps);
            foamTex = FindProperty("_FoamTex", mProps);
            foamGradient = FindProperty("_FoamGradient", mProps);
            foamStrength = FindProperty("_FoamStrength", mProps);

            normals = FindProperty("_Normals", mProps);
            normalsStrength = FindProperty("_NormalStrength", mProps);
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
                    mMatEditor.ShaderProperty(edgeBlend, "Edge Blend");
                    mMatEditor.ShaderProperty(waterDepth, "Water Depth");
                    mMatEditor.ShaderProperty(underWaterOpacity, "Surface Opacity Under Water ");
                    mMatEditor.ShaderProperty(smoothness, "Smoothness");
                    mMatEditor.ShaderProperty(cubeMapLevel, "Cube Map Color Tint");
                    mMatEditor.TexturePropertySingleLine(new GUIContent("Cube Map"), cube);
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Fresnel"), EditorStyles.boldLabel);
                    mMatEditor.ShaderProperty(fresnelPower, "Fresnel Power");
                    mMatEditor.ShaderProperty(fresnelBias, "Fresnel Bias");

                });

                EditorGUILayout.Separator();
                
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Specular"), EditorStyles.boldLabel);

                    mMatEditor.ShaderProperty(specularToggle, "Enable Specular Reflection");

                    if (specularToggle.floatValue == 1)
                    {
                        mMatEditor.ShaderProperty(specularColor, "Specular Color");
                        mMatEditor.ShaderProperty(shininess, "Shininess");
                        mMatEditor.ShaderProperty(worldLightDir, "World Light Dir");
                    }
                    
                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Distortion"), EditorStyles.boldLabel);

                    mMatEditor.ShaderProperty(distortion, "Distortion");
                    mMatEditor.ShaderProperty(bumpTiling, "Bump Tiling");
                    mMatEditor.ShaderProperty(bumpDirection, "Bump Direction");
                    mMatEditor.TexturePropertySingleLine(new GUIContent("Bump Map"), bumpMap);

                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Foam"), EditorStyles.boldLabel);

                    mMatEditor.ShaderProperty(foamToggle, "Enable Foam");
                    if (foamToggle.floatValue == 1)
                    {
                        mMatEditor.ShaderProperty(foamColor, "Foam Color");
                        mMatEditor.TexturePropertySingleLine(new GUIContent("Foam Texture"), foamTex);
                        mMatEditor.TexturePropertySingleLine(new GUIContent("Foam Gradient"), foamGradient);
                        mMatEditor.ShaderProperty(foamStrength, "Foam Strength");
                    }

                });

                EditorGUILayout.Separator();
                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Normals"), EditorStyles.boldLabel);

                    mMatEditor.ShaderProperty(normals, "Normals");
                    mMatEditor.ShaderProperty(normalsStrength, "Bump Strength");

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
