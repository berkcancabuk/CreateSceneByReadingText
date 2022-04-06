using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Water2DTool
{

#if UNITY_EDITOR
    public partial class Water2D_WaterPrefabTracker : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (importedAssets[i].EndsWith(".prefab"))
                {
                    GameObject o = AssetDatabase.LoadAssetAtPath(importedAssets[i], typeof(UnityEngine.Object)) as GameObject;
                    if (o == null) continue;

                    Water2D_Tool[] waterObjects = o.GetComponentsInChildren<Water2D_Tool>();

                    for (int t = 0; t < waterObjects.Length; t++)
                    {
                        MeshFilter filter = waterObjects[t].gameObject.GetComponent<MeshFilter>();

                        if (filter.sharedMesh == null)
                        {
                            waterObjects[t].prefabInstanceIsCreated = true;
                            waterObjects[t].RecreateWaterMesh();
                        }
                    }
                }
            }
        }
    }
#endif
}
