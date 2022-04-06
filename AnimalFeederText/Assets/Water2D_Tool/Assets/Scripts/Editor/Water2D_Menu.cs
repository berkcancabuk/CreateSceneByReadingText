using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Water2DTool
{
    public class Water2D_Menu
    {
        [MenuItem("GameObject/2D Water/CPU Water/Water With 2D Collider", false, 0)]
        static void MenuAddWater2D_Collider2D()
        {
            GameObject obj = CreateWater2D(true);
            Selection.activeGameObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        [MenuItem("GameObject/2D Water/CPU Water/Water With 3D Collider", false, 0)]
        static void MenuAddWater2D_Collider3D()
        {
            GameObject obj = CreateWater2D(false);
            Selection.activeGameObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        [MenuItem("GameObject/2D Water/GPU Water/2.5D Water With 2D Collider", false, 0)]
        static void MenuAddWater2DRippleShader_Collider2D()
        {
            GameObject obj = CreateGPUWater(true);
            Selection.activeGameObject = obj;
            EditorGUIUtility.PingObject(obj);
        }


        [MenuItem("GameObject/2D Water/GPU Water/2.5D Water With 3D Collider", false, 0)]
        static void MenuAddWater2DRippleShader_Collider3D()
        {
            GameObject obj = CreateGPUWater(false);
            Selection.activeGameObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        [MenuItem("GameObject/2D Water/GPU Water/Obstruction Polygon", false, 11)]
        static void MenuAddObstructionPolygon()
        {
            GameObject obj = new GameObject("New ObstructionPolygon");
            ObstructionPolygon shape = obj.AddComponent<ObstructionPolygon>();

            shape.AddShapePoint(new Vector3(-3f, 0f, 3f));
            shape.AddShapePoint(new Vector3(-3f, 0f,-3f));
            shape.AddShapePoint(new Vector3(3f, 0f, -3f));
            shape.AddShapePoint(new Vector3(3f, 0f, 3f));

            obj.transform.position = GetSpawnPos();

            Selection.activeGameObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        [MenuItem("GameObject/2D Water/GPU Water/Ripple Source", false, 12)]
        static void MenuAddRippleSource()
        {
            GameObject obj = new GameObject("New RippleSource");
            obj.AddComponent<RippleSource>();

            obj.transform.position = GetSpawnPos();

            Selection.activeGameObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        static GameObject CreateWater2D(bool collider2D)
        {
            GameObject obj = new GameObject("New Water2D_FM");
            Water2D_Tool water = obj.AddComponent<Water2D_Tool>();
            obj.AddComponent<Water2D_Simulation>();

            water.Add(new Vector2(0, 3));
            water.Add(new Vector2(0, -3));
            water.Add(new Vector2(-5, 0));
            water.Add(new Vector2(5, 0));

            if (!collider2D)
                water.use3DCollider = true;

            water.SetDefaultMaterial();
            water.RecreateWaterMesh();
            
            obj.transform.position = GetSpawnPos();

            return obj;
        }

        static GameObject CreateGPUWater(bool collider2D)
        {
            GameObject fmObject = new GameObject("New Water2D_FM");
            Water2D_Tool water2DTool = fmObject.AddComponent<Water2D_Tool>();
            fmObject.AddComponent<Water2D_Simulation>();
            fmObject.AddComponent<Water2D_Ripple>();

            water2DTool.Add(new Vector3(0, 3, 0));
            water2DTool.Add(new Vector3(0, -3, 0));
            water2DTool.Add(new Vector3(-8, 0, 0));
            water2DTool.Add(new Vector3(8, 0, 0));
            water2DTool.Add(new Vector3(0, 3, 10));

            if (!collider2D)
                water2DTool.use3DCollider = true;

            water2DTool.squareSegments = true;
            water2DTool.segmentsPerUnit = 4;
            water2DTool.cubeWater = true;

            GameObject tmObject = new GameObject("New Water2D_TM");
            tmObject.AddComponent<MeshRenderer>();
            tmObject.AddComponent<MeshFilter>();
            tmObject.AddComponent<Water2D_PlanarReflection>();
            tmObject.transform.position = water2DTool.transform.position;
            tmObject.transform.SetParent(water2DTool.transform);
            water2DTool.topMeshGameObject = tmObject;


            water2DTool.SetGPUWaterDefaultMaterial();   
            water2DTool.RecreateWaterMesh();

            fmObject.transform.position = GetSpawnPos();

            return fmObject;
        }



        static Vector3 GetSpawnPos()
        {
            Plane plane = new Plane(new Vector3(0, 0, -1), 0);
            float dist = 0;
            Vector3 result = new Vector3(0, 0, 0);
            Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
            if (plane.Raycast(ray, out dist))
            {
                result = ray.GetPoint(dist);
            }
            return new Vector3(result.x, result.y, 0);
        }
    }
}
