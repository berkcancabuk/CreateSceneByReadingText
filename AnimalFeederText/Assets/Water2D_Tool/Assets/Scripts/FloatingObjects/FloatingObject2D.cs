using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// Water 2D Tool improvements by
/// Samuel Johansson, www.phaaxgames.com
/// 100% free to use with attribution.

namespace Water2DTool
{
    public class FloatingObject2D : FloatingObject
    {
        private enum ColliderType { Unknown, BoxCollider2D, PolygonCollider2D, CircleCollider2D, CapsuleCollider2D }

        private int pCorners;
        private int prevYAxisDirection = 1;
        private bool playerCollider;
        private bool soundEffectPlayed = false;
        private bool particleSystemInstantiated = false;
        private bool is2DColl;
        private Vector3 previousPosition;
        private Collider2D collider;
        private Rigidbody2D rigidbody;
        private ColliderType colliderType = ColliderType.Unknown;
        private List<Vector2> polygonPoints;
        private Water2D_PolygonClipping polygonCliping;

        public override Transform transform
        {
            get
            {
                return collider.transform;
            }
        }

        public override Bounds bounds
        {
            get
            {
                return collider.bounds;
            }
        }

        public FloatingObject2D(Collider2D col, int polygonCorners, bool player)
        {
            collider = col;
            pCorners = polygonCorners;
            playerCollider = player;
            rigidbody = col.GetComponent<Rigidbody2D>();
            polygonCliping = new Water2D_PolygonClipping();
            is2DColl = true;
            if (col is BoxCollider2D) colliderType = ColliderType.BoxCollider2D;
            if (col is PolygonCollider2D) colliderType = ColliderType.PolygonCollider2D;
            if (col is CircleCollider2D) colliderType = ColliderType.CircleCollider2D;
            if (col is CapsuleCollider2D) colliderType = ColliderType.CapsuleCollider2D;

            if (colliderType == ColliderType.BoxCollider2D)
            {
                polygonPoints = new List<Vector2>();

                for (int i = 0; i < 4; i++)
                    polygonPoints.Add(Vector2.zero);
            }
            else if (colliderType == ColliderType.CircleCollider2D)
            {
                polygonPoints = new List<Vector2>(polygonCorners);

                for (int i = 0; i < polygonCorners; i++)
                    polygonPoints.Add(Vector2.zero);
            }
            else
                polygonPoints = new List<Vector2>();

            if (col is CapsuleCollider2D)
            {
                colliderType = ColliderType.CapsuleCollider2D;

                if (!IsCircle(col as CapsuleCollider2D))
                {
                    pCorners += 2;
                    if (pCorners % 2 != 0)
                        pCorners += 1;
                }

                for (int i = 0; i < pCorners; i++)
                    polygonPoints.Add(Vector2.zero);
            }
        }

        private bool IsCircle(CapsuleCollider2D capsule2D)
        {
            bool circle = false;

            Vector2 capsuleColSize = new Vector2(capsule2D.size.x * capsule2D.transform.localScale.x, capsule2D.size.y * capsule2D.transform.localScale.y);

            if (capsule2D.direction == CapsuleDirection2D.Horizontal)
            {
                if (capsuleColSize.y > capsuleColSize.x)
                    circle = true;
            }
            else
            {
                if (capsuleColSize.x > capsuleColSize.y)
                    circle = true;
            }

            return circle;
        }

        public override List<Vector2> GetPolygon()
        {
            switch (colliderType)
            {
                case ColliderType.BoxCollider2D:
                    return GetBoxVerticesWorldPosition();

                case ColliderType.CircleCollider2D:
                    return GetPolygonVerticesWorldPosition();

                case ColliderType.PolygonCollider2D:
                    return GetPolygonCollider2DPoints();

                case ColliderType.CapsuleCollider2D:
                    return GetCapsuleVerticesWorldPosition();

                default:
                    return null;
            }
        }

        public override float GetRadius()
        {
            switch (colliderType)
            {
                case ColliderType.BoxCollider2D:
                    return GetRadiusFromBox();

                case ColliderType.CircleCollider2D:
                    return GetRadiusFromCircle();

                case ColliderType.PolygonCollider2D:
                    return GetRadiusFromPolygon();

                default:
                    return 0;
            }
        }

        private float GetRadiusFromBox()
        {
            return collider.bounds.extents.x;
        }

        private float GetRadiusFromCircle()
        {
            CircleCollider2D circle = collider as CircleCollider2D;
            return circle.radius;
        }

        private float GetRadiusFromPolygon()
        {
            return collider.bounds.extents.x;
        }

        /// <summary>
        /// Calculates the global position of a BoxCollider2D vertices.
        /// </summary>
        /// <returns>Returns the global position of a box collider vertices.</returns>
        private List<Vector2> GetBoxVerticesWorldPosition()
        {
            float radians = collider.transform.eulerAngles.z * Mathf.Deg2Rad;
            float angleDegZ = collider.transform.eulerAngles.z;
            Vector2 boundsMin = collider.bounds.min;
            Vector2 boundsMax = collider.bounds.max;
            Vector3 boxCollCenterPos = collider.bounds.center;
            Vector2 boundsExtents = collider.bounds.extents;

            // If the angle of rotation on the Z axis is one of the values (0, 90, 180, 270, 360) we can use the bounding box of the collider to calculate
            // the position of its 4 vertices.
            if (angleDegZ == 0 || angleDegZ == 90 || angleDegZ == 180 || angleDegZ == 270 || angleDegZ == 360)
            {
                // Top left vertex.
                polygonPoints[0] = new Vector2(boundsMin.x, boxCollCenterPos.y + boundsExtents.y);
                // Bottom left vertex.
                polygonPoints[1] = new Vector2(boundsMin.x, boxCollCenterPos.y - boundsExtents.y);
                // Bottom right vertex.
                polygonPoints[2] = new Vector2(boundsMax.x, boxCollCenterPos.y - boundsExtents.y);
                // Top right vertex.
                polygonPoints[3] = new Vector2(boundsMax.x, boxCollCenterPos.y + boundsExtents.y);
            }
            else
            {
                float halfWidth = 0f;
                float halfHeight = 0f;

                halfWidth = ((collider as BoxCollider2D).size.x * collider.transform.localScale.x) * 0.5f;
                halfHeight = ((collider as BoxCollider2D).size.y * collider.transform.localScale.y) * 0.5f;

                // The global position of the box vertices at 0 degrees on the Z axis.
                // Top left vertex.
                polygonPoints[0] = new Vector2(boxCollCenterPos.x - halfWidth, boxCollCenterPos.y + halfHeight);
                // Bottom left vertex.
                polygonPoints[1] = new Vector2(boxCollCenterPos.x - halfWidth, boxCollCenterPos.y - halfHeight);
                // Bottom right vertex.
                polygonPoints[2] = new Vector2(boxCollCenterPos.x + halfWidth, boxCollCenterPos.y - halfHeight);
                // Top right vertex.
                polygonPoints[3] = new Vector2(boxCollCenterPos.x + halfWidth, boxCollCenterPos.y + halfHeight);

                // The global position of the box vertices after rotation around the Z axis.
                for (int i = 0; i < 4; i++)
                {
                    polygonPoints[i] = new Vector2((polygonPoints[i].x - boxCollCenterPos.x) * Mathf.Cos(radians) - (polygonPoints[i].y - boxCollCenterPos.y) * Mathf.Sin(radians) + boxCollCenterPos.x,
                        (polygonPoints[i].x - boxCollCenterPos.x) * Mathf.Sin(radians) + (polygonPoints[i].y - boxCollCenterPos.y) * Mathf.Cos(radians) + boxCollCenterPos.y);
                }
            }

            return polygonPoints;
        }

        /// <summary>
        /// Generates an imaginary regular polygon based on the circle or sphere colliders center and radius.
        /// </summary>
        /// <returns>Returns the global position of a regular polygon vertices.</returns>
        private List<Vector2> GetPolygonVerticesWorldPosition()
        {
            Vector3 collCenterPos = collider.bounds.center;
            CircleCollider2D circleCollider2D = collider as CircleCollider2D;

            float radius = 0;
            float angleDeg = 0;
            float radians = 0;

            radius = 0.125f + circleCollider2D.radius * (circleCollider2D.transform.localScale.x > circleCollider2D.transform.localScale.y ? circleCollider2D.transform.localScale.x : circleCollider2D.transform.localScale.y);
            radians = circleCollider2D.transform.eulerAngles.z * Mathf.Deg2Rad;
            angleDeg = 360f / pCorners;

            for (int i = 0; i < pCorners; i++)
            {
                radians = angleDeg * i * Mathf.Deg2Rad;
                polygonPoints[i] = new Vector2(collCenterPos.x + radius * Mathf.Cos(radians), collCenterPos.y + radius * Mathf.Sin(radians));

                radians = circleCollider2D.transform.eulerAngles.z * Mathf.Deg2Rad;
                polygonPoints[i] = new Vector2((polygonPoints[i].x - collCenterPos.x) * Mathf.Cos(radians) - (polygonPoints[i].y - collCenterPos.y) * Mathf.Sin(radians) + collCenterPos.x, 
                    (polygonPoints[i].x - collCenterPos.x) * Mathf.Sin(radians) + (polygonPoints[i].y - collCenterPos.y) * Mathf.Cos(radians) + collCenterPos.y);
            }

            return polygonPoints;
        }


        private List<Vector2> GetPolygonCollider2DPoints()
        {
            polygonPoints.Clear();
            polygonPoints = (collider as PolygonCollider2D).points.ToList();

            for (int n = 0; n < polygonPoints.Count; n++)          
                polygonPoints[n] = collider.transform.TransformPoint(polygonPoints[n]);
            
            if (polygonCliping.IsClockwise(polygonPoints))        
                ReverseArray(polygonPoints);     

            return polygonPoints;
        }

        private List<Vector2> ReverseArray(List<Vector2> points)
        {
            int halfLen = points.Count / 2;
            int len = halfLen * 2;
            Vector2 temp;

            for (int i = 0; i < halfLen; i++)
            {
                temp = points[i];
                points[i] = points[len - i - 1];
                points[len - i - 1] = temp;
            }

            return points;
        }

        /// <summary>
        /// Generates an imaginary polygon based on a CapsuleCollider2D.
        /// </summary>
        /// <returns>Returns the global position of a polygon vertices.</returns>
        private List<Vector2> GetCapsuleVerticesWorldPosition()
        {
            CapsuleCollider2D capsuleCollider = collider as CapsuleCollider2D;
            Vector3 capsuleLocalScale = capsuleCollider.transform.localScale;
            Vector3 capsuleCCPos = capsuleCollider.bounds.center;
            float radius, offset, edgeDeg, radians, corners, height = 0;
            bool circle = false;
            int direction, half;

            circle = IsCircle(capsuleCollider);
            Vector2 capsuleColSize = new Vector2(capsuleCollider.size.x * capsuleCollider.transform.localScale.x, capsuleCollider.size.y * capsuleCollider.transform.localScale.y);

            if (capsuleCollider.direction == CapsuleDirection2D.Horizontal)
            {
                if (capsuleColSize.y > capsuleColSize.x)
                {
                    radius = capsuleColSize.y * 0.5f;
                    height = radius;
                }
                else
                {
                    radius = capsuleColSize.y * 0.5f;
                    height = capsuleColSize.x;
                }
                direction = 0;
            }
            else
            {
                if (capsuleColSize.x > capsuleColSize.y)
                {
                    radius = capsuleColSize.x * 0.5f;
                    height = radius;
                }
                else
                {
                    radius = capsuleColSize.x * 0.5f;
                    height = capsuleColSize.y;
                }

                direction = 1;
            }

            radians = capsuleCollider.transform.eulerAngles.z * Mathf.Deg2Rad;
            corners = pCorners;
            half = pCorners / 2;
            edgeDeg = 360f / corners;

            if (circle)
                offset = 0;
            else
                offset = ((height * capsuleCollider.transform.localScale.y) - (2 * radius)) * 0.5f + 0.1f;

            for (int i = 0; i < corners; i++)
            {
                radians = ((edgeDeg * i) + (edgeDeg * 0.5f)) * Mathf.Deg2Rad;

                polygonPoints[i] = new Vector2(capsuleCCPos.x + radius * Mathf.Cos(radians), capsuleCCPos.y + (i < half ? offset : -offset) + radius * Mathf.Sin(radians));

                if (direction == 1)
                    radians = capsuleCollider.transform.eulerAngles.z * Mathf.Deg2Rad;
                else
                    radians = (capsuleCollider.transform.eulerAngles.z + 90f) * Mathf.Deg2Rad;

                polygonPoints[i] = new Vector2((polygonPoints[i].x - capsuleCCPos.x) * Mathf.Cos(radians) - (polygonPoints[i].y - capsuleCCPos.y) * Mathf.Sin(radians) + capsuleCCPos.x,
                    (polygonPoints[i].x - capsuleCCPos.x) * Mathf.Sin(radians) + (polygonPoints[i].y - capsuleCCPos.y) * Mathf.Cos(radians) + capsuleCCPos.y);
            }

            return polygonPoints;
        }

        public override bool HasCollider()
        {
            return (collider != null);
        }

        public override bool HasRigidbody()
        {
            return (rigidbody != null);
        }

        public override bool IsPlayer()
        {
            return playerCollider;
        }

        public override bool HasPlayedSoundEffect()
        {
            return soundEffectPlayed;
        }

        public override bool HasInstantiatedParticleSystem()
        {
            return particleSystemInstantiated;
        }

        public override void SetParticleSystemInstantiated(bool instantiated)
        {
            particleSystemInstantiated = instantiated;
        }

        public override void SetSoundPlayed(bool soundPlayed)
        {
            soundEffectPlayed = soundPlayed;
        }

        public override int GetPreviousDirectionOnYAxis()
        {
            return prevYAxisDirection;
        }

        public override void SetDirectionOnYAxis(int dir)
        {
            prevYAxisDirection = dir;
        }

        public override Vector3 GetPreviousPosition()
        {
            return previousPosition;
        }

        public override void SetPreviousPosition()
        {
            previousPosition = transform.position;
        }

        public override void AddForce(Vector3 force)
        {
            if (!HasRigidbody()) return;
            rigidbody.AddForceAtPosition(force, collider.bounds.center);
        }

        public override void AddForceAtPosition(Vector3 force, Vector3 position)
        {
            if (!HasRigidbody()) return;
            rigidbody.AddForceAtPosition(force, position);
        }

        public override void AddTorque(float torque)
        {
            if (!HasRigidbody()) return;
            rigidbody.AddTorque(torque);
        }

        public override Vector3 GetVelocity()
        {
            if (!HasRigidbody()) return Vector3.zero;
            return rigidbody.velocity;
        }

        public override Vector2 GetPointVelocity(Vector2 point)
        {
            if (!HasRigidbody()) return Vector2.zero;
            return rigidbody.GetPointVelocity(point);
        }

        public override float GetAngularVelocity()
        {
            if (!HasRigidbody()) return 0;
            return rigidbody.angularVelocity;
        }

        public override bool Equals(object other)
        {
            return collider.Equals(other);
        }

        public override int GetHashCode()
        {
            return collider.GetHashCode();
        }

        public override bool Is2DCollider()
        {
            return is2DColl;
        }
    }
}