using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Water2DTool
{
    // This is a simplified version of the Sutherland-Hodgman clipping algorithm.
    // http://rosettacode.org/wiki/Sutherland-Hodgman_polygon_clipping
    public class Water2D_PolygonClipping
    {
        List<Vector2> outputList;
        Edge clipEdge;

        public Water2D_PolygonClipping()
        {
            outputList = new List<Vector2>();
            clipEdge = new Edge(Vector2.zero, Vector2.one);
        }
        /// <summary>
        /// This represents a line segment
        /// </summary>
        private class Edge
        {
            public Edge(Vector2 from, Vector2 to)
            {
                this.From = from;
                this.To = to;
            }

            public Vector2 From;
            public Vector2 To;
        }

        /// <summary>
        /// Calculates the intersection between a polygon and a line.
        /// </summary>
        /// <param name="subjectPoly">An Array of polygon points.</param>
        /// <param name="linePoints">Two points that form a line.</param>
        /// <returns>Returns an Array of polygon points.</returns>
        public List<Vector2> GetIntersectedPolygon(List<Vector2> subjectPoly, Vector2[] linePoints, out bool intersecting)
        {
            outputList.Clear();
            intersecting = true;
            //Edge clipEdge = new Edge(linePoints[0], linePoints[1]);

            clipEdge.From = linePoints[0];
            clipEdge.To = linePoints[1];

            Vector2 lastVert = subjectPoly[subjectPoly.Count - 1];
            int len = subjectPoly.Count;

            for (int i = 0; i < len; i++)
            {
                Vector2 vert = subjectPoly[i];

                if (IsInside(clipEdge, vert))
                {
                    if (!IsInside(clipEdge, lastVert))
                    {
                        Vector2? point = GetIntersect(lastVert, vert, clipEdge.From, clipEdge.To);
                        if (point != null)
                        {
                            outputList.Add(point.Value);
                        }
                    }

                    outputList.Add(vert);
                }
                else if (IsInside(clipEdge, lastVert))
                {
                    Vector2? point = GetIntersect(lastVert, vert, clipEdge.From, clipEdge.To);
                    if (point != null)
                    {
                        outputList.Add(point.Value);
                    }
                }

                lastVert = vert;
            }

            if (outputList.Count == 0)
            {
                intersecting = false;
            }
            //	Exit Function
            return outputList;
        }

        private Vector2? GetIntersect(Vector2 line1From, Vector2 line1To, Vector2 line2From, Vector2 line2To)
        {
            Vector2 direction1 = line1To - line1From;
            Vector2 direction2 = line2To - line2From;
            float dotPerp = (direction1.x * direction2.y) - (direction1.y * direction2.x);

            // If it's 0, it means the lines are parallel so have infinite intersection points
            if (IsNearZero(dotPerp))
            {
                return null;
            }

            Vector2 c = line2From - line1From;
            float t = (c.x * direction2.y - c.y * direction2.x) / dotPerp;

            //	Return the intersection point
            return line1From + (t * direction1);
        }

        private bool IsInside(Edge edge, Vector2 test)
        {
            bool? isLeft = IsLeftOf(edge, test);
            if (isLeft == null)
            {
                //	Colinear points should be considered inside
                return true;
            }

            return !isLeft.Value;
        }

        /// <summary>
        /// Tells if the test point lies on the left side of the edge line
        /// </summary>
        private bool? IsLeftOf(Edge edge, Vector2 test)
        {
            Vector2 tmp1 = edge.To - edge.From;
            Vector2 tmp2 = test - edge.To;

            double x = (tmp1.x * tmp2.y) - (tmp1.y * tmp2.x);		//	dot product of perpendicular?

            if (x < 0)
            {
                return false;
            }
            else if (x > 0)
            {
                return true;
            }
            else
            {
                //	Colinear points;
                return null;
            }
        }

        public bool IsClockwise(List<Vector2> polygon)
        {
            for (int cntr = 2; cntr < polygon.Count; cntr++)
            {
                clipEdge.From = polygon[0];
                clipEdge.To = polygon[1];
                bool? isLeft = IsLeftOf(clipEdge, polygon[cntr]);
                if (isLeft != null)		//	some of the points may be colinear.  That's ok as long as the overall is a polygon
                {
                    return !isLeft.Value;
                }
            }

            return true;
        }

        private bool IsNearZero(float testValue)
        {
            return Mathf.Abs(testValue) <= 0.000000001d;
        }
    }
}
