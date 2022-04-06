using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Water2DTool;

[ExecuteInEditMode]
public class WaterCulling : MonoBehaviour
{

//    public bool drawBothSides = false;

//    // Update is called once per frame
//    public void OnWillRenderObject()
//    {

//#if UNITY_EDITOR
//        SetCulling();
//#endif
//    }

//    private void SetCulling()
//    {
//        Renderer rend;
//        rend = GetComponent<Renderer>();
//        Water2D_Tool water2d = GetComponentInParent<Water2D_Tool>();
//        Vector3 waterLineCurrentGlobalPos = transform.TransformPoint(water2d.handlesPosition[0]);
        
//        if (SceneView.currentDrawingSceneView)
//        {
//            Vector3 camPos = SceneView.currentDrawingSceneView.camera.transform.position;

//            if (camPos.y < waterLineCurrentGlobalPos.y)
//            {
//                int culling = drawBothSides ? 0 : 1;
//                rend.sharedMaterial.SetInt("_FaceCulling", culling);
//                rend.sharedMaterial.SetInt("_Culling", 1);
//            }
//            else
//            {
//                rend.sharedMaterial.SetInt("_FaceCulling", 2);
//                rend.sharedMaterial.SetInt("_Culling", 2);
//            }
//        }
//    }
}
