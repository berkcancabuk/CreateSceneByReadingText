using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace Water2DTool
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class Water2D_Tool : MonoBehaviour
    {
        #region Private fields
        /// <summary>
        /// The renderer component of the front mesh.
        /// </summary>
        private Renderer frontMeshRend;
        /// <summary>
        /// The renderer component of the top mesh.
        /// </summary>
        //private Renderer topMeshRend;
        /// <summary>
        /// The width and height of a render texture pixels in Unity space.
        /// </summary>
        private float rtPixelSize = 1f;
        /// <summary>
        /// Reference to the Water2D_Ripple component.
        /// </summary>
        private Water2D_Ripple ripple;
        /// <summary>
        /// Reference to the Water2D_Simulation component.
        /// </summary>
        //private Water2D_Simulation water2DSim;
#if UNITY_EDITOR
        /// <summary>
        /// Prevents the code responsible for the creation of the Mesh component from being called during play mode.
        /// </summary>
        private bool checkComponentReference = true;
#endif
        /// <summary>
        /// 1 /x . The reverse value of the render texture width in Unity units.
        /// </summary>
        private float rRTWidthInWorldSpace;
        /// <summary>
        /// 1 /x . The reverse value of the render texture height in Unity units.
        /// </summary>
        private float rRTHeightInWorldSpace;
        /// <summary>
        /// Should the bounds and normal be recalculated?.
        /// </summary>
        public bool boundsAndNormals = true;
        /// <summary>
        /// Reference to the front mesh of the water. 
        /// </summary>
        private Mesh parentMesh;
        /// <summary>
        /// Reference to the top mesh of the water. 
        /// </summary>
        private Mesh childMesh;
        /// <summary>
        /// Water2D_Mesh instance used for the front mesh. 
        /// </summary>
        private Water2D_Mesh parentDMesh;
        /// <summary>
        /// Water2D_Mesh instance used for the top mesh. 
        /// </summary>
        private Water2D_Mesh childDMesh;
        /// <summary>
        /// Water2D_Mesh property for the front mesh. 
        /// </summary>
        /// <summary>
        /// The distance on between 2 vertices on the Z axis.
        /// </summary>
        public float zVertDistance = 1f;
        /// <summary>
        /// The width of a horizontal segment. 
        /// </summary>
        private float xVertDistance = 1f;
        /// <summary>
        /// The number of vertical segments. This value must not be changed.
        /// </summary>
        private int ySegments = 1;
        /// <summary>
        /// The Transform component of the water object.
        /// </summary>
        private new Transform transform;
        #endregion

        #region Public fields
#if UNITY_EDITOR
        public bool showVisuals = true;
        public bool showCollider = true;
        public bool showInfo = true;
        public bool showCubeWater = true;
#endif

        /// <summary>
        /// Water2D_Mesh property for the front mesh. 
        /// </summary>
        public Water2D_Mesh ParentDMesh
        {
            get
            {
                if (parentDMesh == null)
                    parentDMesh = new Water2D_Mesh();
                return parentDMesh;
            }
        }
        /// <summary>
        /// Water2D_Mesh property for the top mesh. 
        /// </summary>
        public Water2D_Mesh ChildDMesh
        {
            get
            {
                if (childDMesh == null)
                    childDMesh = new Water2D_Mesh();
                return childDMesh;
            }
        }
        /// <summary>
        /// The max width and height the water can have without tiling. 
        /// </summary>
        public Vector2 unitsPerUV = Vector2.one;
        /// <summary>
        /// The max width and height the water can have without tiling. 
        /// </summary>
        public Vector2 unitsPerUV2 = Vector2.one;
        /// <summary>
        /// 1 / x. The reverse value of unitsPerUV. 
        /// </summary>
        public Vector2 rUnitsPerUV = Vector2.one;
        /// <summary>
        /// 1 / x. The reverse value of unitsPerUV2. 
        /// </summary>
        public Vector2 rUnitsPerUV2 = Vector2.one;
        /// <summary>
        /// The number of horizontal segments. 
        /// </summary>
        public int xSegments = 1;
        /// <summary>
        /// The number of segments on the Z axis. 
        /// </summary>
        public int zSegments = 1;
        /// <summary>
        /// The water width. 
        /// </summary>
        public float width = 1.0f;
        /// <summary>
        /// The water height. 
        /// </summary>
        public float height = 1.0f;
        /// <summary>
        /// The water length. 
        /// </summary>
        public float length = 1.0f;
        /// <summary>
        /// How many pixels should be in one unit of Unity space.
        /// </summary>
        public float pixelsPerUnit = 100;
        /// <summary>
        /// The number of horizontal segments that should fit into one unit of Unity space.
        /// </summary>
        public float segmentsPerUnit = 3f;
        /// <summary>
        /// Shows the water mesh shape in the Scene View.
        /// </summary>
        public bool showMesh = true;
        /// <summary>
        /// Should we generate a collider on Start.
        /// </summary>
        public bool createCollider = true;
        /// <summary>
        /// Colliders top edge offset.
        /// </summary>
        public float colliderYAxisOffset = 0.0f;
        /// <summary>
        /// A list with the local positions of the 4 handles (5 for 2.5D water). Do not change the handles order. 
        /// A lot of Water2D code will not work correctly if you change the order of the handles in the list. 
        /// 0 - top handle.
        /// 1 - bottom handle.
        /// 2 - left handle.
        /// 3 - right handle.
        /// 4 (only for the 2.5D water) - the handle that controls the length of the horizontal mesh.
        /// </summary>
        public List<Vector3> handlesPosition = new List<Vector3>();
        /// <summary>
        /// The value of this field is not used in any important calculations. When creating an animation that animates
        /// the area of the water, use this value as a guide to see how the water area changes between two positions.
        /// </summary>
        public float curentWaterArea = 0f;
        /// <summary>
        /// It's used to decide what collider the water should have. A 2D or a 3D collider.
        /// </summary>
        public bool use3DCollider = false;
        /// <summary>
        /// The offset of the water's Box Collider on the Z axis.
        /// </summary>
        public float overlapingSphereZOffset = 0.5f;
        //public float overlapingSphereZOffset = 0.5f;
        /// <summary>
        /// The size of the water's Box Collider on the Z axis.
        /// </summary>
        public float boxColliderZSize = 1f;
        /// <summary>
        /// The scale of the handle gizmo.
        /// </summary>
        [Range(0.1f, 10f)]
        public float handleScale = 1f;
        /// <summary>
        /// When set to true a horizontal mesh will be added to the vertical mesh.
        /// </summary>
        [SerializeField]
        public bool cubeWater = false;
        /// <summary>
        /// The length on the Z axis the horizontal mesh has.
        /// </summary>
        public float waterLength = 10f;
        /// <summary>
        /// The number of vertices the water line has.
        /// </summary>
        public int frontMeshVertsCount = 0;
        /// <summary>
        /// The height of the water.
        /// </summary>
        public float waterHeight = 6f;
        /// <summary>
        /// The width of the water.
        /// </summary>
        public float waterWidth = 10f;
        /// <summary>
        /// When set to true the water size can be changed using a series of handles.
        /// </summary>
        public bool useHandles = true;
        /// <summary>
        /// The number of vertices the water line has.
        /// </summary>
        public int prevSurfaceVertsCount = 0;
        /// <summary>
        /// The number of horizontal vertices. 
        /// </summary>
        public int xVerts = 2;
        /// <summary>
        /// The number of vertical vertices. 
        /// </summary>
        public int yVerts = 2;
        public int zVerts = 2;
        /// <summary>
        /// The front and top mesh will always be a quad.
        /// </summary>
        public bool squareSegments = false;
        /// <summary>
        /// Displays in the inspector information about the front and top mesh.
        /// </summary>
        public bool showMeshInfo = true;
        /// <summary>
        /// The width af the render texture.
        /// </summary>
        public int renderTextureWidth = 128;
        /// <summary>
        /// The height af the render texture.
        /// </summary>
        public int renderTextureHeight = 128;
        /// <summary>
        /// Set to true if the current object has a Water2D_Ripple script attached to it.
        /// </summary>
        public bool water2DRippleScript = false;
        /// <summary>
        /// Should the front and top mesh be just a quad.
        /// </summary>
        public bool quadMesh = false;
        /// <summary>
        /// Should the number of segments on the Z axis be fixed.
        /// </summary>
        public bool zSegmentsCap = false;
        /// <summary>
        /// The number of segments on the Z axis.
        /// </summary>
        public int zSegmentsSize = 1;
        /// <summary>
        /// Reference to the game object that has the top mesh on it.
        /// </summary>
        public GameObject topMeshGameObject;
        /// <summary>
        /// BoxCollider center offset.
        /// </summary>
        public Vector3 boxColliderCenterOffset = Vector3.zero;
        /// <summary>
        /// BoxCollider size offset.
        /// </summary>
        public Vector3 boxColliderSizeOffset = Vector3.zero;
        /// <summary>
        /// Set to true when a prefab is created from a water object.
        /// </summary>
        public bool prefabInstanceIsCreated = false;
        public float topMeshYOffset = 0f;

        #endregion

        private void GetReferences()
        {
            frontMeshRend = GetComponent<Renderer>();
            //if (cubeWater)
                //topMeshRend = topMeshGameObject.GetComponent<Renderer>();
            transform = GetComponent<Transform>();
            ripple = GetComponent<Water2D_Ripple>();
            if (ripple != null)
                water2DRippleScript = true;
        }

        public void RecreateWaterMesh()
        {

            //Debug.Log("RecreateWaterMesh");

            ParentDMesh.Clear();
            ChildDMesh.Clear();

#if UNITY_EDITOR
            if (!Application.isPlaying)           
                GetReferences();
            
#endif

#if UNITY_EDITOR
            SetTopMeshMaterial();
#endif
            RecalculateMeshParameters();

            if (water2DRippleScript)
                GenerateFrontMeshNoTiling();
            else
                GenerateFrontMeshWithTiling();

            if (cubeWater)
            {
                if (water2DRippleScript)
                    GenerateTopMeshNoTiling();
                else
                    GenerateTopMeshWithTiling();
            }

            if (!use3DCollider)
                UpdateCollider2D();
            else
                UpdateCollider3D();

#if UNITY_EDITOR
            if (checkComponentReference)
                CheckComponentReference();
#endif

            ParentDMesh.Build(ref parentMesh, boundsAndNormals);

            if (cubeWater)
                ChildDMesh.Build(ref childMesh, boundsAndNormals);

            if (prefabInstanceIsCreated)
                prefabInstanceIsCreated = false;
        }

        /// <summary>
        /// Creates a new mesh instance for the current object.
        /// This prevents objects instantiated using a prefab from having the same mesh instance.
        /// </summary>
        public void OnAwakeMeshRebuild()
        {
            Mesh result;
            result = new Mesh();
            result.name = string.Format("{0}{1}-Mesh", gameObject.name, gameObject.GetInstanceID());
            GetComponent<MeshFilter>().sharedMesh = null;
            GetComponent<MeshFilter>().sharedMesh = result;
            parentMesh = GetComponent<MeshFilter>().sharedMesh;

            if (cubeWater)
            {
                result = new Mesh();
                result.name = string.Format("{0}{1}-Mesh", topMeshGameObject.gameObject.name, topMeshGameObject.gameObject.GetInstanceID());
                topMeshGameObject.GetComponent<MeshFilter>().sharedMesh = null;
                topMeshGameObject.GetComponent<MeshFilter>().sharedMesh = result;
                childMesh = topMeshGameObject.GetComponent<MeshFilter>().sharedMesh;
            }
            boundsAndNormals = false;

#if UNITY_EDITOR
            checkComponentReference = false;
#endif
            GetReferences();
            RecreateWaterMesh();
        }

        /// <summary>
        /// Makes sure the references to different components are not null. 
        /// Checks it the current mesh points to the mesh of another object. 
        /// If so a new mesh instance is created for this object.
        /// </summary>
        private void CheckComponentReference()
        {
            parentMesh = GetComponent<MeshFilter>().sharedMesh = GetMesh(true);

            if (cubeWater)
                childMesh = topMeshGameObject.GetComponent<MeshFilter>().sharedMesh = GetMesh(false);
        }

        /// <summary>
        /// Calculates water the width, height and length as well as other 
        /// variables that are necessary to build the water mesh.
        /// </summary>
        private void RecalculateMeshParameters()
        {
            width = Mathf.Abs(handlesPosition[3].x - handlesPosition[2].x);
            height = Mathf.Abs(handlesPosition[0].y - handlesPosition[1].y);
            float rSegmentsPerUnit = 1f / segmentsPerUnit;

            rUnitsPerUV = new Vector2(1f / unitsPerUV.x, 1f / unitsPerUV.y);
            rUnitsPerUV2 = new Vector2(1f / unitsPerUV2.x, 1f / unitsPerUV2.y);

            if (cubeWater)
            {
                length = Mathf.Abs(handlesPosition[0].z - handlesPosition[4].z);

                if (water2DRippleScript)
                {
                    SetRenderTextureVariables();
                    length = rtPixelSize * renderTextureHeight;
                }
            }

            xSegments = (int)(Mathf.Ceil(width * segmentsPerUnit));
            xVertDistance = rSegmentsPerUnit;

            if (squareSegments)
            {
                if (water2DRippleScript)
                {
                    zSegments = (int)(Mathf.Ceil(length * segmentsPerUnit));
                    zVertDistance = rSegmentsPerUnit;
                }
                else
                {
                    zSegments = (int)(Mathf.Ceil(length * segmentsPerUnit));
                    zVertDistance = rSegmentsPerUnit;
                }
            }
            else
            {
                zSegments = zSegmentsSize;
                zVertDistance = length / zSegments;
            }

            zVerts = zSegments + 1;
            xVerts = xSegments + 1;

            if (zSegmentsCap && zSegments > zSegmentsSize)
            {
                zVerts = zSegmentsSize + 1;
                zSegments = zSegmentsSize;

                if (!squareSegments)
                    zVertDistance = length / zSegments;
            }

            if (quadMesh)
            {
                zVerts = 2;
                xVerts = 2;
                zSegments = 1;
                xSegments = 1;
                zVertDistance = length / zSegments;
            }

            if (cubeWater)
                VertexLimit();
        }

        /// <summary>
        /// Makes sure the mesh will not exceed the max number of vertices am single mesh can have.
        /// </summary>
        private void VertexLimit()
        {
            float vertexCount = xVerts * zVerts + xVerts * 2;

            if (vertexCount > 65535)
            {
                while (vertexCount > 65535)
                {
                    zVerts -= 1;
                    zSegments -= 1;
                    vertexCount = xVerts * zVerts + xVerts * 2;
                }

#if UNITY_EDITOR
                Debug.LogWarning("Water 2D Tool Warning. You exceeded the mesh vertex limit of 65535."
                    + " The number of segments on the Z axis was reduced to prevent error generation.");
#endif
            }
        }

        /// <summary>
        /// Generates the front mesh of the water. No texture tiling is applied.
        /// </summary>
        private void GenerateFrontMeshNoTiling()
        {
            if (GetComponent<Renderer>().sharedMaterial == null)
                return;

            SetFrontMeshUnitsPerUV();
            GenerateFrontMeshDataNoTiling();
            ParentDMesh.GenerateTriangles(xSegments, ySegments, xVerts);
        }

        /// <summary>
        /// Generates the front mesh vertices as well as the UV coordinates.
        /// No texture tiling is applied.
        /// </summary>
        private void GenerateFrontMeshDataNoTiling()
        {
            Vector3 vertPos = Vector3.zero;
            Vector2 UV;
            prevSurfaceVertsCount = frontMeshVertsCount / 2;
            frontMeshVertsCount = 0;
            float X, Y;

            for (int y = 0; y < yVerts; y++)
            {
                for (int x = 0; x < xVerts; x++)
                {
                    if (x < xVerts - 1)
                        X = x * xVertDistance + handlesPosition[2].x;
                    else
                        X = handlesPosition[2].x + renderTextureWidth * rtPixelSize;

                    Y = handlesPosition[1].y + y * height;
                    vertPos.x = X;
                    vertPos.y = Y;
                    GetFrontMeshUVNoTiling(x, y, out UV);

                    ParentDMesh.AddVertex(vertPos, UV, true);

                    frontMeshVertsCount++;
                }
            }

            if (water2DRippleScript)
            {
                frontMeshRend.sharedMaterial.SetFloat("_WaterHeight", height);
                frontMeshRend.sharedMaterial.SetFloat("_WaterWidth", width);
            }
        }

        /// <summary>
        /// Generates UV coordinates for a front mesh vertex.
        /// No texture tiling is applied.
        /// </summary>
        private void GetFrontMeshUVNoTiling(int x, int y, out Vector2 UV)
        {
            if (x < xVerts - 1)
                UV.x = (xVertDistance * x) * rRTWidthInWorldSpace;
            else
                UV.x = 1f;

            if (y == 0)
                UV.y = 0;
            else
                UV.y = 1;
        }

        /// <summary>
        /// Generates the top mesh of the water. No texture tiling is applied.
        /// </summary>
        private void GenerateTopMeshNoTiling()
        {
            SetTopMeshUnitsPerUV();
            GenerateTopMeshDataNoTiling();
            ChildDMesh.GenerateTriangles(xSegments, zSegments, xVerts);
        }

        /// <summary>
        /// Generates the top mesh vertices as well as the UV coordinates.
        /// No texture tiling is applied.
        /// </summary>
        private void GenerateTopMeshDataNoTiling()
        {
            Vector3 vertPos;
            Vector2 UV = Vector2.zero;
            float X, Z;
            float tan = 0;

            if (topMeshYOffset > 0)
                tan = topMeshYOffset / length;

            for (int z = 0; z < zVerts; z++)
            {
                for (int x = 0; x < xVerts; x++)
                {
                    if (x < xVerts - 1)
                        X = x * xVertDistance + handlesPosition[2].x;
                    else
                        X = handlesPosition[2].x + renderTextureWidth * rtPixelSize;

                    if (z < zVerts - 1)
                        Z = z * zVertDistance;
                    else
                        Z = renderTextureHeight * rtPixelSize;

                    vertPos = new Vector3(X, handlesPosition[0].y, Z);

                    if (topMeshYOffset > 0 && z > 0)
                        vertPos.y += tan * zVertDistance * z;

                    UV = GetTopMeshUVWithoutTiling(x, z);

                    ChildDMesh.AddVertex(vertPos, UV, false);
                }
            }
        }

        /// <summary>
        /// Calculates the UV coordinates of a vertex for ther top mesh. No texture tiling is applied.
        /// </summary>
        private Vector2 GetTopMeshUVWithoutTiling(int x, int z)
        {
            float U, V;

            if (x < xVerts - 1)
                U = (xVertDistance * x) * rRTWidthInWorldSpace;
            else
                U = 1f;

            if (z < zVerts - 1)
                V = 1f - (zVertDistance * z) * rRTHeightInWorldSpace;
            else
                V = 0;

            return new Vector2(U, V);
        }

        /// <summary>
        /// Generates the front mesh of the water. Texture tiling is applied.
        /// </summary>
        private void GenerateFrontMeshWithTiling()
        {
            if (frontMeshRend.sharedMaterial == null)
                return;

            SetFrontMeshUnitsPerUV();
            GenerateFrontMeshDataWithTiling();
            ParentDMesh.GenerateTriangles(xSegments, ySegments, xVerts);
        }

        /// <summary>
        /// Generates the front mesh vertices as well as the UV coordinates.
        /// Texture tiling is applied.
        /// </summary>
        private void GenerateFrontMeshDataWithTiling()
        {
            Vector3 vertPos = Vector3.zero;
            Vector2 UV = Vector2.zero;
            prevSurfaceVertsCount = frontMeshVertsCount / 2;
            frontMeshVertsCount = 0;
            float posX = transform.position.x;
            float rightHandleX = handlesPosition[3].x;
            float leftHandleX = handlesPosition[2].x;
            float bottomHandleY = handlesPosition[1].y;

            for (int y = 0; y < yVerts; y++)
            {
                for (int x = 0; x < xVerts; x++)
                {
                    if (x == xVerts - 1)
                        vertPos = new Vector3(rightHandleX, bottomHandleY + y * height, 0.0f);
                    else
                        vertPos = new Vector3(x * xVertDistance + leftHandleX, bottomHandleY + y * height, 0.0f);

                    UV = GetFrontMeshUVWithTiling(vertPos, y, posX);
                    ParentDMesh.AddVertex(vertPos, UV, true);
                    frontMeshVertsCount++;
                }
            }
        }

        /// <summary>
        /// Generates UV coordinates for a front mesh vertex.
        /// Texture tiling is applied.
        /// </summary>
        private Vector2 GetFrontMeshUVWithTiling(Vector3 vertPos, int y, float posX)
        {
            float U, V;

            U = (vertPos.x + posX) * rUnitsPerUV.x;
            if (y == 0)
                V = 1 - height * rUnitsPerUV.y;
            else
                V = 1;

            return new Vector2(U, V);
        }

        /// <summary>
        /// Generates the top mesh of the water. Texture tiling is applied.
        /// </summary>
        private void GenerateTopMeshWithTiling()
        {
            SetTopMeshUnitsPerUV();
            GenerateTopMeshDataWithTiling();
            ChildDMesh.GenerateTriangles(xSegments, zSegments, xVerts);
        }

        /// <summary>
        /// Generates the top mesh vertices as well as the UV coordinates.
        /// Texture tiling is applied.
        /// </summary>
        private void GenerateTopMeshDataWithTiling()
        {
            Vector3 vertPos;
            Vector2 UV;
            float topHandleY = handlesPosition[0].y;
            float leftHandleX = handlesPosition[2].x;
            float rightHandleX = handlesPosition[3].x;
            float posX = transform.position.x;
            float tan = 0;

            if (topMeshYOffset > 0)
                tan = topMeshYOffset / length;

            for (int z = 0; z < zVerts; z++)
            {
                for (int x = 0; x < xVerts; x++)
                {
                    if (x < xVerts - 1)
                        vertPos = new Vector3(x * xVertDistance + leftHandleX, topHandleY, z * zVertDistance);
                    else
                        vertPos = new Vector3(rightHandleX, topHandleY, z * zVertDistance);

                    if (topMeshYOffset > 0 && z > 0) {
                        vertPos.y += tan * zVertDistance * z;
                        //float temp = tan * zVertDistance * z;
                        //Debug.Log("temp: " + temp);
                    }
                    

                    UV = GetTopMeshUVWithTiling(vertPos, z, posX);

                    ChildDMesh.AddVertex(vertPos, UV, true);
                }
            }
        }


        /// <summary>
        /// Calculates the UV coordinates of a vertex for ther top mesh. Texture tiling is applied.
        /// </summary>
        private Vector2 GetTopMeshUVWithTiling(Vector3 vertPos, int z, float posX)
        {
            float U, V;

            U = (vertPos.x + posX) * rUnitsPerUV2.x;
            V = 1f - (zVertDistance * z) * rUnitsPerUV2.y;

            return new Vector2(U, V);
        }

        /// <summary>
        /// Recalculates the some parameters of the render texture.
        /// </summary>
        private void SetRenderTextureVariables()
        {
            Water2D_Ripple water2DRipple = GetComponent<Water2D_Ripple>();
            int rtPixelsToUnits = water2DRipple.rtPixelsToUnits;

            renderTextureWidth = (int)(Mathf.Ceil(width * rtPixelsToUnits));
            renderTextureHeight = (int)(Mathf.Ceil(length * rtPixelsToUnits));
            rtPixelSize = 1f / rtPixelsToUnits;

            rRTWidthInWorldSpace = 1f / (renderTextureWidth * rtPixelSize);
            rRTHeightInWorldSpace = 1f / (renderTextureHeight * rtPixelSize);
        }

        /// <summary>
        /// Calculates the number of pixels that should fix in one unit of unity space.
        /// </summary>
        private void SetTopMeshUnitsPerUV()
        {
            Material mat = topMeshGameObject.GetComponent<Renderer>().sharedMaterial;

            if (mat.HasProperty("_MainTex") && mat.mainTexture != null)
            {
                unitsPerUV2.x = mat.mainTexture.width / pixelsPerUnit;
                unitsPerUV2.y = mat.mainTexture.height / pixelsPerUnit;
            }
            else
            {
                unitsPerUV2.x = 512 / pixelsPerUnit;
                unitsPerUV2.y = 512 / pixelsPerUnit;
            }
        }

        public void AddNewWaterMeshInstance()
        {
            parentMesh = null;
            childMesh = null;

            GetComponent<MeshFilter>().sharedMesh = null;

            if (cubeWater)
                topMeshGameObject.GetComponent<MeshFilter>().sharedMesh = null;

            parentMesh = GetComponent<MeshFilter>().sharedMesh = GetMesh(true);

            if (cubeWater)
                childMesh = topMeshGameObject.GetComponent<MeshFilter>().sharedMesh = GetMesh(false);
            RecreateWaterMesh();
        }

        /// <summary>
        /// Adds a BoxCollider2D component if the water doesn't have one and updates its size.
        /// </summary>
        public void UpdateCollider2D()
        {
            if (!createCollider)
            {
                if (GetComponent<BoxCollider2D>() != null)
                {
                    BoxCollider2D collider = GetComponent<BoxCollider2D>();
                    collider.enabled = false;
                }

                return;
            }

            if (GetComponent<BoxCollider2D>() == null)
            {
                gameObject.AddComponent<BoxCollider2D>();
                BoxCollider2D collider = GetComponent<BoxCollider2D>();
                collider.isTrigger = true;
            }

            BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();

            if (boxCollider2D.enabled == false)
                boxCollider2D.enabled = true;

            boxCollider2D.size = new Vector2(width, height + colliderYAxisOffset);

            Vector2 center = handlesPosition[1];
            center.y += height / 2f + colliderYAxisOffset / 2f;

            boxCollider2D.offset = center;
        }

        private void SetFrontMeshUnitsPerUV()
        {
            Material mat = GetComponent<Renderer>().sharedMaterial;

            if (mat.HasProperty("_MainTex") && mat.mainTexture != null)
            {
                unitsPerUV.x = mat.mainTexture.width / pixelsPerUnit;
                unitsPerUV.y = mat.mainTexture.height / pixelsPerUnit;
            }
            else
            {
                unitsPerUV.x = 512 / pixelsPerUnit;
                unitsPerUV.y = 512 / pixelsPerUnit;
            }
        }

        /// <summary>
        /// Adds a BoxCollider component if the water doesn't have one and updates its size.
        /// </summary>
        public void UpdateCollider3D()
        {
            if (!createCollider)
            {
                if (GetComponent<BoxCollider>() != null)
                {
                    BoxCollider collider = GetComponent<BoxCollider>();
                    collider.enabled = false;
                }

                return;
            }

            if (GetComponent<BoxCollider>() == null)
            {
                gameObject.AddComponent<BoxCollider>();
                BoxCollider collider = GetComponent<BoxCollider>();
                collider.isTrigger = true;
            }

            BoxCollider boxCollider = GetComponent<BoxCollider>();

            if (boxCollider.enabled == false)
                boxCollider.enabled = true;

            if (water2DRippleScript)
            {
                boxCollider.size = new Vector3(width, height + colliderYAxisOffset, length);
                Vector3 center = handlesPosition[1];
                center.y += height * 0.5f + colliderYAxisOffset * 0.5f;
                center.z += length * 0.5f;
                boxCollider.center = center;
            }
            else
            {
                if (cubeWater)
                    boxCollider.size = new Vector3(width, height + colliderYAxisOffset, length);
                else
                    boxCollider.size = new Vector3(width, height + colliderYAxisOffset, boxColliderZSize);

                boxCollider.size += boxColliderSizeOffset;
                Vector3 center = handlesPosition[1];
                center.y += height * 0.5f + colliderYAxisOffset * 0.5f;

                if (cubeWater)
                    center.z = length * 0.5f;

                boxCollider.center = center;
                boxCollider.center += boxColliderCenterOffset;
            }
        }


        /// <summary>
        /// Returns a mesh or adds a mesh if the object doesn't have one.
        /// </summary>
        Mesh GetMesh(bool meshForThisObject)
        {
            MeshFilter filter;

            if (meshForThisObject)
                filter = GetComponent<MeshFilter>();
            else
                filter = topMeshGameObject.GetComponent<MeshFilter>();

            string newName = GetMeshName(meshForThisObject);
            Mesh result = filter.sharedMesh;

            if (prefabInstanceIsCreated)
            {
#if UNITY_EDITOR
                if (filter.sharedMesh == null || filter.sharedMesh.name != newName)
                {
                    string path = UnityEditor.AssetDatabase.GetAssetPath(this) + "/WaterMeshes/" + newName + ".asset";
                    Mesh assetMesh = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;
                    if (assetMesh != null)
                    {
                        result = assetMesh;
                    }
                    else
                    {
                        path = Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this)) + "/WaterMeshes";
                        string assetName = "/" + newName + ".asset";
                        result = new Mesh();
                        result.name = newName;

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        try
                        {
                            UnityEditor.AssetDatabase.CreateAsset(result, path + assetName);
                            UnityEditor.AssetDatabase.Refresh();
                        }
                        catch
                        {
                            Debug.LogError("Unable to save water prefab mesh! Likely, you deleted the mesh files, and the prefab is still referencing them. Restarting your Unity editor should solve this minor issue.");
                        }
                    }
                }
#endif
            }
            else
            {
                if (filter.sharedMesh == null || filter.sharedMesh.name != newName)          
                    result = new Mesh();
                
            }
            result.name = newName;
            return result;
        }

        public string GetMeshName(bool nameForThisObject)
        {
            if (prefabInstanceIsCreated)
            {
                string name;
                Transform curr;

                if (nameForThisObject)
                {
                    name = gameObject.name;
                    curr = gameObject.transform.parent;
                }
                else
                {
                    name = topMeshGameObject.gameObject.name;
                    curr = topMeshGameObject.gameObject.transform.parent;
                }

                while (curr != null) { name = curr.name + "." + name; curr = curr.transform.parent; }
                name += "-Mesh";

                return name;
            }
            else
            {
                if (nameForThisObject)
                    return string.Format("{0}{1}-Mesh", gameObject.name, gameObject.GetInstanceID());
                else
                    return string.Format("{0}{1}-Mesh", topMeshGameObject.gameObject.name, topMeshGameObject.gameObject.GetInstanceID());
            }
        }

        /// <summary>
        /// To be used when wanting to update the water properties externally.
        /// </summary>
        /// <param name="wWidth">Water width</param>
        /// <param name="wHeight">Water height</param>
        /// <param name="centerPos">When set to true recenters the pivot point</param>
        public void UpdateWaterMesh(float wWidth, float wHeight, bool centerPos)
        {
            waterWidth = wWidth;
            waterHeight = wHeight;

            float pos = waterHeight / 2f;
            handlesPosition[0] = new Vector2(0, pos);
            handlesPosition[1] = new Vector2(0, -pos);

            pos = waterWidth / 2f;

            handlesPosition[2] = new Vector2(-pos, 0);
            handlesPosition[3] = new Vector2(pos, 0);

            if (cubeWater)
                handlesPosition[4] = new Vector3(0, handlesPosition[0].y, waterLength);

            if (centerPos)
                ReCenterPivotPoint();
        }

        /// <summary>
        /// Moves the object location to the center of the handles. Also offsets the handles locations to match.
        /// </summary>
        public void ReCenterPivotPoint()
        {
            Vector3 center = Vector3.zero;
            transform = GetComponent<Transform>();

            for (int i = 0; i < 4; i++)
                center += handlesPosition[i];

            if (cubeWater)
                center = center / (handlesPosition.Count - 1) + new Vector3(transform.position.x, transform.position.y, 0);
            else
                center = center / handlesPosition.Count + new Vector3(transform.position.x, transform.position.y, 0);

            Vector3 offset = center - new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);

            for (int i = 0; i < 4; i++)
                handlesPosition[i] -= offset;

            if (cubeWater)
                handlesPosition[4] = new Vector3(handlesPosition[0].x, handlesPosition[0].y, handlesPosition[4].z);

            gameObject.transform.position = new Vector3(center.x, center.y, gameObject.transform.position.z);

            RecreateWaterMesh();
        }

        /// <summary>
        /// Sets the position of a handle when the water is created.
        /// </summary>
        /// <param name="pos">Handle position</param>
        public void Add(Vector3 pos)
        {
            handlesPosition.Add(pos);
        }

        /// <summary>
        /// Sets the material for the horizontal mesh. This method is called only once when
        /// the toggle 2.5D Water is set to true in the inspector.
        /// </summary>
        public void SetTopMeshMaterial()
        {
            if (cubeWater && topMeshGameObject.GetComponent<Renderer>().sharedMaterial == null)
            {
                topMeshGameObject.GetComponent<Renderer>().sharedMaterial = GetComponent<Renderer>().sharedMaterial;
            }
        }

        /// <summary>
        /// Sets the initial water material.
        /// </summary>
        public void SetDefaultMaterial()
        {
            Renderer rend = GetComponent<Renderer>();
            Material m = Resources.Load("Materials/Default_Material", typeof(Material)) as Material;
            if (m != null)
            {
                rend.material = m;

                unitsPerUV.x = GetComponent<Renderer>().sharedMaterial.mainTexture.width / pixelsPerUnit;
                unitsPerUV.y = GetComponent<Renderer>().sharedMaterial.mainTexture.height / pixelsPerUnit;
            }
            else
            {
                Debug.LogWarning("The default material was not found. This happened most likely because you " 
                    + " deleted or renamed the Default_Material from the Resources folder. Click on this "
                    + "message to set the name of the default material if you renamed it.");
            }
        }

        /// <summary>
        /// Sets the initial water material.
        /// </summary>
        public void SetGPUWaterDefaultMaterial()
        {
            Renderer fmRend = GetComponent<Renderer>();
            Renderer tmRend = topMeshGameObject.GetComponent<Renderer>();
            Material fmMaterial = Resources.Load("Materials/Default_WaterRippleAdvanced_FM", typeof(Material)) as Material;
            Material tmMaterial = Resources.Load("Materials/Default_WaterRippleAdvanced_TM", typeof(Material)) as Material;
            if (fmMaterial != null && tmMaterial != null)
            {
                fmRend.material = fmMaterial;
                tmRend.material = tmMaterial;

                fmRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                tmRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            else
            {
                Debug.LogWarning("The default material was not found. This happened most likely because" 
                    + " you deleted or renamed the Default material from the Resources folder. Click on this "
                    + "message to set the name of the default material if you renamed it.");
            }
        }
    }
}
