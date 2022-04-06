using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif 
using System.IO;
using System.Text.RegularExpressions;
using Water2D_ClipperLib;
using Path = System.Collections.Generic.List<Water2D_ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<Water2D_ClipperLib.IntPoint>>;

namespace Water2DTool
{
    public class CreateObstructionTexture : EditorWindow
    {
        private const string windowName = "Texture Obstruction Creator";
        private static string textureName = "NewObstructionTexture1";
        private int texWidth = 128;
        private int texHeight = 128;
        private BoxCollider boxColl;
        private Texture2D tex;
        private Water2D_Tool water2DTool;
        private Vector3 leftHandleGlobalPos;
        private int textureScale = 1;
        private int objectListSize = 1;
        private Color32[] pixelColor;
        private GameObject water2DGameObject;
        private List<GameObject> gameObjectsList = new List<GameObject>();
        private List<Vector2> clipPolygon = new List<Vector2>();
        private List<Vector3> posAndRadiusOnTexture = new List<Vector3>();
        private List<BoxCollider> boxColliderList = new List<BoxCollider>();
        private List<List<Vector2>> pointsInTextureSpace = new List<List<Vector2>>();
        private List<ObstructionPolygon> shapesList = new List<ObstructionPolygon>();
        private Vector2 scrollPos;
        /// <summary>
        /// It is used to convert float numbers to int and back.
        /// </summary>
        private float scaleFactor = 100000f;

        [MenuItem("Window/" + windowName + "")]
        static void Init()
        {
            CreateObstructionTexture window = (CreateObstructionTexture)EditorWindow.GetWindow(typeof(CreateObstructionTexture));
            window.titleContent = new GUIContent(windowName);
            window.minSize = new Vector2(300, 320);
        }

        // main loop
        void OnGUI()
        {
            DrawResetButton();
            DrawNameGUI();

            if (GUILayout.Button(new GUIContent("Create Obstruction Texture", "Creates an obstruction texture."), GUILayout.Height(40)))
            {
                CreateObstructionTextureNow();
            }

            if (GUILayout.Button(new GUIContent("Add Obstruction Objects", "Adds the objects selected in the hierarchy panel to the Obstruction Objects list."), GUILayout.Height(20)))
            {
                FillCollidersList();
            }

            DrawObjectList();
        }

        private void FillCollidersList()
        {
            GameObject[] templistOfObjects = Selection.gameObjects;

            int len = templistOfObjects.Length;

            if (len == 0)
                return;

            if (objectListSize == 1 && gameObjectsList[0] == null)
            {
                gameObjectsList.Clear();
                objectListSize = len;
            }
            else
                objectListSize += len;

            for (int i = 0; i < len; i++)
            {
                gameObjectsList.Add(templistOfObjects[i]);
            }
        }

        void DrawResetButton()
        {
            GUILayout.BeginHorizontal("", GUIStyle.none);
            GUILayout.Label("Texture Settings", EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent("Default", "Resets the settings to their default values."), GUILayout.Width(62)))
            {
                // reset values to default
                textureName = "NewObstructionTexture1";
                textureScale = 1;
                objectListSize = 1;
                gameObjectsList.Clear();
                water2DGameObject = null;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        void DrawNameGUI()
        {
            GUILayout.BeginHorizontal("", GUIStyle.none);
            textureName = EditorGUILayout.TextField("Name", textureName);
            if (GUILayout.Button(new GUIContent("+", "Increment counter"), GUILayout.Width(20)))
            {
                textureName = IncrementNameCounter(textureName);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("", GUIStyle.none);
            textureScale = Mathf.Clamp(EditorGUILayout.IntField("Texture Scale", textureScale), 1, 4096);
            EditorGUILayout.EndHorizontal();
        }

        void DrawObjectList()
        {

            if (gameObjectsList.Count == 0)
                gameObjectsList.Add(null);

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal("", GUIStyle.none);
            water2DGameObject = EditorGUILayout.ObjectField(new GUIContent("Water Object"), water2DGameObject, typeof(GameObject), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal("", GUIStyle.none);
            GUILayout.Label("Obstruction Objects", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("", GUIStyle.none);

            objectListSize = Mathf.Clamp(EditorGUILayout.IntField("List size", objectListSize), 1, 500);
            EditorGUILayout.EndHorizontal();

            if (objectListSize != gameObjectsList.Count)
            {
                if (objectListSize > gameObjectsList.Count)
                {
                    while (objectListSize > gameObjectsList.Count)
                    {
                        gameObjectsList.Add(null);
                    }
                }
                else
                {
                    while (objectListSize < gameObjectsList.Count)
                    {
                        int last = gameObjectsList.Count - 1;
                        gameObjectsList.RemoveAt(last);
                    }
                }
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            int len = gameObjectsList.Count;

            for (int i = 0; i < len; i++)
            {
                if (gameObjectsList.Count < i)
                    break;

                GUILayout.BeginHorizontal("", GUIStyle.none);
                int obj = i + 1;
                gameObjectsList[i] = EditorGUILayout.ObjectField(new GUIContent("" + obj), gameObjectsList[i], typeof(GameObject), true) as GameObject;

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        // finds number value from string and increments it by 1
        string IncrementNameCounter(string currentName)
        {
            string nameCounter = Regex.Match(currentName, @"\d+").Value;
            if (nameCounter == "") nameCounter = "0";
            return currentName.Replace(nameCounter.ToString(), "") + (int.Parse(nameCounter) + 1).ToString();
        }

        void CreateObstructionTextureNow()
        {
            if (water2DGameObject == null || water2DGameObject.GetComponent<Water2D_Tool>() == null)
            {
                Debug.LogError("Please place a Water 2D Object in the Water Object field.");
                return;
            }

            water2DGameObject.GetComponent<Water2D_Tool>();
            water2DTool = water2DGameObject.GetComponent<Water2D_Tool>(); ;
            leftHandleGlobalPos = water2DGameObject.transform.TransformPoint(water2DTool.handlesPosition[2]);

            texWidth = (int)(water2DTool.renderTextureWidth * textureScale);
            texHeight = (int)(water2DTool.renderTextureHeight * textureScale);

            Texture2D tex = new Texture2D(texWidth, texHeight, TextureFormat.RGB24, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Trilinear;

            int count = 0;
            Color color = Color.white;
            pixelColor = new Color32[texWidth * texHeight];

            FillColliderLists();
            SetClipPolygon();
            WorldPointsToTexture();

            for (int y = 0; y < texHeight; y++)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    pixelColor[count] = color;
                    color = Color.white;
                    count++;
                }
            }

            FillObstructionColor();

            tex.SetPixels32(pixelColor);
            tex.Apply();

            if (textureName.Length < 1) textureName = "NewObstructionTexture";
            var bytes = tex.EncodeToPNG();

            string path = Application.dataPath + "/" + textureName + ".png";
            File.WriteAllBytes(path, bytes);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            UnityEngine.Object.DestroyImmediate(tex);
        }

        private void FillObstructionColor()
        {
            int len = pointsInTextureSpace.Count;
            bool pointInsidePolygon = false;
            Color color = Color.white;
            Vector2[] polyPoints;
            int minX, maxX, minY, maxY;

            for (int i = 0; i < len; i++)
            {
                polyPoints = pointsInTextureSpace[i].ToArray();
                int len2 = polyPoints.Length;

                minX = (int)polyPoints[0].x;
                maxX = (int)polyPoints[0].x;
                minY = (int)polyPoints[0].y;
                maxY = (int)polyPoints[0].y;

                for (int j = 0; j < len2; j++)
                {
                    if (polyPoints[j].x < minX)
                        minX = (int)polyPoints[j].x;

                    if (polyPoints[j].x > maxX)
                        maxX = (int)polyPoints[j].x;

                    if (polyPoints[j].y < minY)
                        minY = (int)polyPoints[j].y;

                    if (polyPoints[j].y > maxY)
                        maxY = (int)polyPoints[j].y;
                }

                if (minX < 0)
                    minX = 0;
                if (maxX > texWidth)
                    maxX = texWidth;
                if (minY < 0)
                    minY = 0;
                if (maxY > texHeight)
                    maxY = texHeight;

                for (int y = minY; y < maxY; y++)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        pointInsidePolygon = PointInsideComplexPolygon(x, y, polyPoints);

                        if (pointInsidePolygon)
                        {
                            pixelColor[texWidth * y + x] = Color.black;
                        }
                    }
                }
            }

            len = posAndRadiusOnTexture.Count;

            for (int i = 0; i < len; i++)
            {
                minX = (int)(posAndRadiusOnTexture[i].x - posAndRadiusOnTexture[i].z);
                maxX = (int)(posAndRadiusOnTexture[i].x + posAndRadiusOnTexture[i].z);
                minY = (int)(posAndRadiusOnTexture[i].y - posAndRadiusOnTexture[i].z);
                maxY = (int)(posAndRadiusOnTexture[i].y + posAndRadiusOnTexture[i].z);

                if (minX < 0)
                    minX = 0;
                if (maxX > texWidth)
                    maxX = texWidth;
                if (minY < 0)
                    minY = 0;
                if (maxY > texHeight)
                    maxY = texHeight;

                for (int y = minY; y < maxY; y++)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        pointInsidePolygon = InsideCircle(new Vector2(x, y), new Vector2(posAndRadiusOnTexture[i].x, posAndRadiusOnTexture[i].y), posAndRadiusOnTexture[i].z);

                        if (pointInsidePolygon)
                        {
                            pixelColor[texWidth * y + x] = Color.black;
                        }
                    }
                }
            }
        }

        private void FillColliderLists()
        {
            int len = gameObjectsList.Count;
            boxColliderList.Clear();
            shapesList.Clear();
            posAndRadiusOnTexture.Clear();
            Vector3 topHandleGlobalPos = water2DTool.transform.TransformPoint(water2DTool.handlesPosition[0]);
            Vector3 leftHandleGlobalPos = water2DTool.transform.TransformPoint(water2DTool.handlesPosition[2]);
            Vector3 rightHandleGlobalPos = water2DTool.transform.TransformPoint(water2DTool.handlesPosition[3]);

            for (int i = 0; i < len; i++)
            {
                if (gameObjectsList[i] != null)
                {
                    BoxCollider box = gameObjectsList[i].GetComponent<BoxCollider>();
                    SphereCollider sphere = gameObjectsList[i].GetComponent<SphereCollider>();
                    ObstructionPolygon shape = gameObjectsList[i].GetComponent<ObstructionPolygon>();
                    if (box != null)
                    {
                        float boxMaxPoint = box.bounds.center.y + box.bounds.extents.y;
                        float boxMinPoint = box.bounds.center.y - box.bounds.extents.y;

                        if (boxMaxPoint > topHandleGlobalPos.y && boxMinPoint < topHandleGlobalPos.y)
                            boxColliderList.Add(box);
                    }

                    if (sphere != null)
                    {
                        Vector3 sphereC = sphere.bounds.center;

                        float sphereYMaxPoint = sphere.bounds.center.y + sphere.bounds.extents.y;
                        float sphereYMinPoint = sphere.bounds.center.y - sphere.bounds.extents.y;

                        if (sphereYMaxPoint > topHandleGlobalPos.y && sphereYMinPoint < topHandleGlobalPos.y)
                            if (sphereC.x > leftHandleGlobalPos.x && sphereC.x < rightHandleGlobalPos.x && sphereC.z > topHandleGlobalPos.z && sphereC.z < topHandleGlobalPos.z + water2DTool.length)
                                posAndRadiusOnTexture.Add(GetPosAndRadiusOnTexture(sphere));
                    }

                    if (shape != null)
                    {
                        shapesList.Add(shape);
                    }
                }
            }
        }

        private Vector3 GetPosAndRadiusOnTexture(SphereCollider sphere)
        {
            Vector2 pos;
            float radius;
            float scale;
            float d;

            Vector3 topHandleGlobalPos = water2DTool.transform.TransformPoint(water2DTool.handlesPosition[0]);

            scale = sphere.transform.localScale.x;

            if (sphere.transform.localScale.y > sphere.transform.localScale.x)
                scale = sphere.transform.localScale.y;

            if (sphere.transform.localScale.z > sphere.transform.localScale.y && sphere.transform.localScale.z > sphere.transform.localScale.x)
                scale = sphere.transform.localScale.y;

            radius = sphere.radius * scale;
            d = Mathf.Abs(sphere.bounds.center.y - topHandleGlobalPos.y);
            radius = Mathf.Sqrt(radius * radius - d * d);

            pos = new Vector2(sphere.bounds.center.x, sphere.bounds.center.z);
            pos = (PointWorldPosToTexture(pos));
            radius = (int)Mathf.Floor((radius / water2DTool.width) * texWidth);

            return new Vector3(pos.x, pos.y, radius);
        }

        bool InsideCircle(Vector2 pixelPos, Vector2 circleCenter, float radius)
        {
            bool insideCircle = false;
            float radiusPowerTwo = radius * radius;

            float value = (pixelPos.x - circleCenter.x) * (pixelPos.x - circleCenter.x) + (pixelPos.y - circleCenter.y) * (pixelPos.y - circleCenter.y);

            if (value < radiusPowerTwo)
                insideCircle = true;

            return insideCircle;
        }

        private List<List<Vector2>> GetIntersectionPolygon(Vector2[] subjectPoly, out bool isIntersecting)
        {
            List<List<Vector2>> intersectionPoly = new List<List<Vector2>>();

            Clipper clipper = new Clipper();
            Paths solutionPath = new Paths();
            Path subjPath = new Path();
            Path clipPath = new Path();
            int len, len2;
            isIntersecting = true;

            len = subjectPoly.Length;
            for (int i = 0; i < len; i++)
            {
                subjPath.Add(new IntPoint(subjectPoly[i].x * scaleFactor, subjectPoly[i].y * scaleFactor));
            }

            len = clipPolygon.Count;
            for (int i = 0; i < len; i++)
            {
                clipPath.Add(new IntPoint(clipPolygon[i].x * scaleFactor, clipPolygon[i].y * scaleFactor));
            }

            clipper.AddPath(subjPath, PolyType.ptSubject, true);
            clipper.AddPath(clipPath, PolyType.ptClip, true);
            clipper.Execute(ClipType.ctIntersection, solutionPath, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            if (solutionPath.Count != 0)
            {
                len = solutionPath.Count;

                for (int i = 0; i < len; i++)
                {
                    len2 = solutionPath[i].Count;
                    List<Vector2> list = new List<Vector2>();

                    for (int j = 0; j < len2; j++)
                    {
                        list.Add(new Vector2(solutionPath[i][j].X / scaleFactor, solutionPath[i][j].Y / scaleFactor));
                    }

                    intersectionPoly.Add(list);
                }
                return intersectionPoly;
            }
            else
            {
                isIntersecting = false;
                return null;
            }
        }


        //  Globals which should be set before calling this function:
        //
        //  int    polyCorners  =  how many corners the polygon has
        //  float  polyX[]      =  horizontal coordinates of corners
        //  float  polyY[]      =  vertical coordinates of corners
        //  float  x, y         =  point to be tested
        //
        //  (Globals are used in this example for purposes of speed.  Change as
        //  desired.)
        //
        //  The function will return YES if the point x,y is inside the polygon, or
        //  NO if it is not.  If the point is exactly on the edge of the polygon,
        //  then the function may return YES or NO.
        //
        //  Note that division by zero is avoided because the division is protected
        //  by the "if" clause which surrounds it.

        private bool PointInsideComplexPolygon(float x, float y, Vector2[] polygon)
        {
            int polyCorners = polygon.Length;
            int i, j = polyCorners - 1;
            bool oddNodes = false;

            for (i = 0; i < polyCorners; i++)
            {
                if ((polygon[i].y < y && polygon[j].y >= y
                || polygon[j].y < y && polygon[i].y >= y)
                && (polygon[i].x <= x || polygon[j].x <= x))
                {
                    oddNodes ^= (polygon[i].x + (y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < x);
                }
                j = i;
            }

            return oddNodes;
        }

        private void SetClipPolygon()
        {
            clipPolygon.Clear();

            clipPolygon.Add(new Vector2(leftHandleGlobalPos.x, leftHandleGlobalPos.z + water2DTool.length));
            clipPolygon.Add(new Vector2(leftHandleGlobalPos.x + water2DTool.width, leftHandleGlobalPos.z + water2DTool.length));
            clipPolygon.Add(new Vector2(leftHandleGlobalPos.x + water2DTool.width, leftHandleGlobalPos.z));
            clipPolygon.Add(new Vector2(leftHandleGlobalPos.x, leftHandleGlobalPos.z));
        }

        private void WorldPointsToTexture()
        {
            int size = boxColliderList.Count;
            Vector2[] points;
            List<List<Vector2>> intersactionPoly = new List<List<Vector2>>();
            pointsInTextureSpace.Clear();
            bool isIntersecting = true;

            for (int i = 0; i < size; i++)
            {
                points = GetBoxVerticesGlobalPosition(boxColliderList[i]);
                intersactionPoly = GetIntersectionPolygon(points, out isIntersecting);

                if (!isIntersecting)
                    continue;

                int len = intersactionPoly.Count;

                for (int j = 0; j < len; j++)
                    pointsInTextureSpace.Add(PolyPointsWorldPosToTexture(intersactionPoly[j]));

            }
            size = shapesList.Count;

            for (int i = 0; i < size; i++)
            {
                points = shapesList[i].GetShapePointsWorldPos().ToArray();
                intersactionPoly = GetIntersectionPolygon(points, out isIntersecting);


                if (!isIntersecting)
                    continue;

                int len = intersactionPoly.Count;

                for (int j = 0; j < len; j++)
                    pointsInTextureSpace.Add(PolyPointsWorldPosToTexture(intersactionPoly[j]));
            }
        }

        private List<Vector2> PolyPointsWorldPosToTexture(List<Vector2> worldPos)
        {
            int size = worldPos.Count;

            for (int i = 0; i < size; i++)
            {
                worldPos[i] = PointWorldPosToTexture(worldPos[i]);
            }

            return worldPos;
        }

        private Vector2 PointWorldPosToTexture(Vector2 point)
        {
            float distanceX = Mathf.Abs(leftHandleGlobalPos.x - point.x);
            float distanceZ = Mathf.Abs(leftHandleGlobalPos.z - point.y);

            point = new Vector2((int)Mathf.Floor((distanceX / water2DTool.width) * texWidth), (int)Mathf.Floor((distanceZ / water2DTool.length) * texHeight));

            return point;
        }

        private Vector2[] GetBoxVerticesGlobalPosition(BoxCollider collider)
        {
            Vector2[] boxVertices = new Vector2[4];

            float radians = -collider.transform.eulerAngles.y * Mathf.Deg2Rad;
            Vector3 boxCollCenterPos = collider.bounds.center;
            float angleDegY = collider.transform.eulerAngles.y;
            Vector3 boundsMin = collider.bounds.min;
            Vector3 boundsMax = collider.bounds.max;
            Vector3 boundsExtents = collider.bounds.extents;

            if (angleDegY == 0 || angleDegY == 90 || angleDegY == 180 || angleDegY == 270 || angleDegY == 360)
            {
                // Top left vertex.
                boxVertices[0] = new Vector2(boundsMin.x, boxCollCenterPos.z + boundsExtents.z);
                // Bottom left vertex.
                boxVertices[1] = new Vector2(boundsMin.x, boxCollCenterPos.z - boundsExtents.z);
                // Bottom right vertex.
                boxVertices[2] = new Vector2(boundsMax.x, boxCollCenterPos.z - boundsExtents.z);
                // Top right vertex.
                boxVertices[3] = new Vector2(boundsMax.x, boxCollCenterPos.z + boundsExtents.z);
            }
            else
            {
                float halfWidth = 0f;
                float halfLenght = 0f;

                halfWidth = ((collider as BoxCollider).size.x * collider.transform.localScale.x) / 2f;
                halfLenght = ((collider as BoxCollider).size.z * collider.transform.localScale.z) / 2f;


                // Top left vertex.
                boxVertices[0] = new Vector2(boxCollCenterPos.x - halfWidth, boxCollCenterPos.z + halfLenght);
                // Bottom left vertex.
                boxVertices[1] = new Vector2(boxCollCenterPos.x - halfWidth, boxCollCenterPos.z - halfLenght);
                // Bottom right vertex.
                boxVertices[2] = new Vector2(boxCollCenterPos.x + halfWidth, boxCollCenterPos.z - halfLenght);
                // Top right vertex.
                boxVertices[3] = new Vector2(boxCollCenterPos.x + halfWidth, boxCollCenterPos.z + halfLenght);

                // The global position of the box vertices after rotation around the Z axis.
                for (int i = 0; i < 4; i++)
                {
                    boxVertices[i] = new Vector2((boxVertices[i].x - boxCollCenterPos.x) * Mathf.Cos(radians) - (boxVertices[i].y - boxCollCenterPos.z) * Mathf.Sin(radians) + boxCollCenterPos.x,
                        (boxVertices[i].x - boxCollCenterPos.x) * Mathf.Sin(radians) + (boxVertices[i].y - boxCollCenterPos.z) * Mathf.Cos(radians) + boxCollCenterPos.z);
                }
            }

            return boxVertices;
        }
    }
}
