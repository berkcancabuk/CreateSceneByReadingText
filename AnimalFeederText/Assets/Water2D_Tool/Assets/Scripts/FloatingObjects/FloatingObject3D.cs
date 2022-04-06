using System;
using UnityEngine;
using System.Collections.Generic;

/// Water 2D Tool improvements by
/// Samuel Johansson, www.phaaxgames.com
/// 100% free to use with attribution.

namespace Water2DTool
{
    public class FloatingObject3D : FloatingObject
    {
        private enum ColliderType { Unknown, BoxCollider, SphereCollider, CapsuleCollider }

        private int prevYAxisDirection = 1;
        private Collider collider;
        private int pCorners;
        private Rigidbody rigidbody;
        private ColliderType colliderType = ColliderType.Unknown;
        private bool playerCollider;
        private bool is2DColl;
        private List<Vector2> polygonPoints;
        private Vector3 previousPosition;

        private bool soundEffectPlayed = false;
        private bool particleSystemInstantiated = false;

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

        public FloatingObject3D(Collider col, int polygonCorners, bool player)
        {
            collider = col;
            pCorners = polygonCorners;
            playerCollider = player;
            is2DColl = false;

            rigidbody = col.GetComponent<Rigidbody>();
            if (col is BoxCollider) colliderType = ColliderType.BoxCollider;
            if (col is SphereCollider) colliderType = ColliderType.SphereCollider;

            if (col is CapsuleCollider)
            {
                colliderType = ColliderType.CapsuleCollider;

                pCorners += 2;
                if (pCorners % 2 != 0)
                    pCorners += 1;
            }

            if (colliderType == ColliderType.BoxCollider)
            {
                polygonPoints = new List<Vector2>();

                for (int i = 0; i < 4; i++)
                    polygonPoints.Add(Vector2.zero);
            }
            else
            {
                polygonPoints = new List<Vector2>(pCorners);

                for (int i = 0; i < pCorners; i++)
                    polygonPoints.Add(Vector2.zero);
            }
        }

        public override bool HasCollider()
        {
            return (collider != null);
        }

        public override bool IsPlayer()
        {
            return playerCollider;
        }

        public override bool HasRigidbody()
        {
            return (rigidbody != null);
        }

        public override List<Vector2> GetPolygon()
        {
            switch (colliderType)
            {
                case ColliderType.BoxCollider:
                    return GetBoxVerticesWorldPosition();

                case ColliderType.SphereCollider:
                    return GetPolygonVerticesWorldPosition();

                case ColliderType.CapsuleCollider:
                    return GetCapsuleVerticesWorldPosition();

                default:
                    return null;
            }
        }

        public override float GetRadius()
        {
            switch (colliderType)
            {
                case ColliderType.BoxCollider:
                    return GetBoxRadius();

                case ColliderType.SphereCollider:
                    return GetSphereRadius();

                case ColliderType.CapsuleCollider:
                    return GetCapsuleRadius();

                default:
                    return 0;
            }
        }

        private float GetBoxRadius()
        {
            BoxCollider box = collider as BoxCollider;
            return box.size.z * Mathf.Abs(collider.transform.localScale.z);
        }

        private float GetSphereRadius()
        {
            SphereCollider sphere = collider as SphereCollider;
            return sphere.radius * Mathf.Abs( collider.transform.localScale.x);
        }
        private float GetCapsuleRadius()
        {
            CapsuleCollider capsule = collider as CapsuleCollider;
            return capsule.radius * Mathf.Abs(collider.transform.localScale.x);
        }


        /// <summary>
        /// Calculates the global position of a BoxCollider2D vertices.
        /// </summary>
        /// <returns>Returns the global position of a box collider vertices.</returns>
        private List<Vector2> GetBoxVerticesWorldPosition()
        {
            float radians = collider.transform.eulerAngles.z * Mathf.Deg2Rad;
            Vector3 boxCollCenterPos = collider.bounds.center;
            float angleDegZ = collider.transform.eulerAngles.z;
            Vector2 boundsMin = collider.bounds.min;
            Vector2 boundsMax = collider.bounds.max;
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

                halfWidth = ((collider as BoxCollider).size.x * collider.transform.localScale.x) * 0.5f;
                halfHeight = ((collider as BoxCollider).size.y * collider.transform.localScale.y) * 0.5f;

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
        /// <returns>Returns the world position of a regular polygon vertices.</returns>
        private List<Vector2> GetPolygonVerticesWorldPosition()
        {
            Vector3 collCenterPos = collider.bounds.center;
            SphereCollider sphereCollider = collider as SphereCollider;
            Vector3 sphereLocalScale = sphereCollider.transform.localScale;

            float angleDeg = 0;
            float radians = 0;
            float radius = 0.125f + sphereCollider.radius * sphereLocalScale.x;

            if (sphereLocalScale.y > sphereLocalScale.x && sphereLocalScale.y > sphereLocalScale.z)
                radius = 0.125f + sphereCollider.radius * sphereLocalScale.y;
            if (sphereLocalScale.z > sphereLocalScale.x && sphereLocalScale.z > sphereLocalScale.y)
                radius = 0.125f + sphereCollider.radius * sphereLocalScale.z;

            radians = sphereCollider.transform.eulerAngles.z * Mathf.Deg2Rad;
            angleDeg = 360f / pCorners;

            for (int i = 0; i < pCorners; i++)
            {
                radians = angleDeg * i * Mathf.Deg2Rad;
                polygonPoints[i] = new Vector2(collCenterPos.x + radius * Mathf.Cos(radians), collCenterPos.y + radius * Mathf.Sin(radians));

                radians = sphereCollider.transform.eulerAngles.z * Mathf.Deg2Rad;
                polygonPoints[i] = new Vector2((polygonPoints[i].x - collCenterPos.x) * Mathf.Cos(radians) - (polygonPoints[i].y - collCenterPos.y) * Mathf.Sin(radians) + collCenterPos.x,
                    (polygonPoints[i].x - collCenterPos.x) * Mathf.Sin(radians) + (polygonPoints[i].y - collCenterPos.y) * Mathf.Cos(radians) + collCenterPos.y);
            }

            return polygonPoints;
        }

        /// <summary>
        /// Generates an imaginary polygon based on a CapsuleCollider center and radius.
        /// </summary>
        /// <returns>Returns the global position of a polygon vertices.</returns>
        private List<Vector2> GetCapsuleVerticesWorldPosition()
        {
            CapsuleCollider capsuleCollider = collider as CapsuleCollider;
            Vector3 capsuleLocalScale = capsuleCollider.transform.localScale;
            Vector3 capsuleCCPos = capsuleCollider.bounds.center;
            float radius;

            radius = 0.125f + capsuleCollider.radius * capsuleCollider.transform.localScale.x;

            if (capsuleLocalScale.y > capsuleLocalScale.x && capsuleLocalScale.y > capsuleLocalScale.z)
                radius = 0.125f + capsuleCollider.radius * capsuleLocalScale.y;
            if (capsuleLocalScale.z > capsuleLocalScale.x && capsuleLocalScale.z > capsuleLocalScale.y)
                radius = 0.125f + capsuleCollider.radius * capsuleLocalScale.z;

            //float angleDeg;
            float radians = capsuleCollider.transform.eulerAngles.z * Mathf.Deg2Rad;
            int direction = capsuleCollider.direction;

            int half = pCorners / 2;
            float offset;
            float edgeDeg = 360f / pCorners;

            offset = ((capsuleCollider.height * capsuleCollider.transform.localScale.y) - (2 * radius)) * 0.5f + 0.1f;

            for (int i = 0; i < pCorners; i++)
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
            rigidbody.AddTorque(0, torque, 0, ForceMode.Force);
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
            return rigidbody.angularVelocity.z;
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