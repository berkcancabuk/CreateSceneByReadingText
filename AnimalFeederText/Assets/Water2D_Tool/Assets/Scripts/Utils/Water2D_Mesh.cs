using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Water2DTool
{
    public class Water2D_Mesh
    {
        #region Fields and Properties
        public List<Vector3> meshVerts;
        private List<int> meshIndices;
        public List<Vector2> meshUVs;
        private List<Vector4> mTans;
        private List<Vector3> mNorms;

        #endregion

        #region Constructor
        public Water2D_Mesh()
        {
            meshVerts = new List<Vector3>();
            meshUVs = new List<Vector2>();
            meshIndices = new List<int>();
            mTans = new List<Vector4>();
            mNorms = new List<Vector3>();
        }
        #endregion

        #region General Methods
        /// <summary>
        /// Clears all verices, indices, uvs, and colors from this mesh, resets color to white.
        /// </summary>
        public void Clear()
        {
            meshVerts.Clear();
            meshIndices.Clear();
            meshUVs.Clear();
            mTans.Clear();
            mNorms.Clear();
        }

        /// <summary>
        /// Clears out the mesh, fills in the data, and recalculates normals and bounds.
        /// </summary>
        /// <param name="mesh">An already existing mesh to fill out.</param>
        public void Build(ref Mesh mesh, bool boundsAndNormals)
        {
            //Profiler.BeginSample("SampleName");
            // round off a few decimal points to try and get better pixel-perfect results
            //for (int i = 0; i < meshVerts.Count; i += 1)
            //    meshVerts[i] = new Vector3(
            //             (float)System.Math.Round(meshVerts[i].x, 3),
            //             (float)System.Math.Round(meshVerts[i].y, 3),
            //             (float)System.Math.Round(meshVerts[i].z, 3));

            mesh.Clear();

            mesh.SetVertices(meshVerts);
            mesh.SetUVs(0, meshUVs);
            mesh.SetTriangles(meshIndices, 0);
            mesh.SetNormals(mNorms);
            mesh.SetTangents(mTans);

            if (boundsAndNormals)
            {
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
            }
        }

        #endregion

        #region Vertex and Face Methods

        /// <summary>
        /// Generates triangles from a list of vertices.
        /// </summary>
        /// <param name="xSegments">The number of horizontal segments.</param>
        /// <param name="ySegments">The number of vertical segments.</param>
        /// <param name="xVertices">The number of horizontal vertices.</param>
        public void GenerateTriangles(int xSegments, int ySegments, int xVertices)
        {
            for (int y = 0; y < (ySegments); y++)
            {
                for (int x = 0; x < xSegments; x++)
                {
                    meshIndices.Add((y * xVertices) + x);
                    meshIndices.Add(((y + 1) * xVertices) + x);
                    meshIndices.Add((y * xVertices) + x + 1);

                    meshIndices.Add(((y + 1) * xVertices) + x);
                    meshIndices.Add(((y + 1) * xVertices) + x + 1);
                    meshIndices.Add((y * xVertices) + x + 1);
                }
            }
        }

        /// <summary>
        /// Adds a vertex to the meshVerts list and a UV point to the meshUVs list.
        /// </summary>
        /// <param name="vertexPoss">The position of a vertex.</param>
        /// <param name="aZ">The position of a vertex on the Z axis.</param>
        /// <param name="aUV">The UV coordinate of the current vertex.</param>
        public void AddVertex(Vector3 vertexPoss, Vector2 aUV, bool frontMesh)
        {
            meshVerts.Add(vertexPoss);
            meshUVs.Add(aUV);

            if (frontMesh)
            {
                mNorms.Add(new Vector3(0, 1, 0));
                mTans.Add(new Vector4(1, 0, 0, -1));
            }
            else
            {
                mNorms.Add(new Vector3(0, 1, 0));
                mTans.Add(new Vector4(1, 0, 0, -1));              
            }
        }

        /// <summary>
        /// Gets the current list of triangles.
        /// </summary>
        /// <param name="startIndex">An offset to start from.</param>
        /// <returns></returns>
        public int[] GetCurrentTriangleList(int startIndex = 0)
        {
            int[] result = new int[meshIndices.Count - startIndex];
            int curr = 0;
            for (int i = startIndex; i < meshIndices.Count; i++)
            {
                result[curr] = meshIndices[i];
                curr += 1;
            }
            return result;
        }
        #endregion
    }
}
