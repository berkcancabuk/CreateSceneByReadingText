using UnityEngine;
using System.Collections.Generic;

/// Water 2D Tool improvements by
/// Samuel Johansson, www.phaaxgames.com
/// 100% free to use with attribution.

namespace Water2DTool
{
    /// <summary>
    /// Abstract base class for anything that floats.
    /// </summary>

    public abstract class FloatingObject
    {
        public abstract Transform transform { get; }
        public abstract Bounds bounds { get; }

        public abstract bool IsPlayer();
        public abstract bool Is2DCollider();
        public abstract bool HasCollider();
        public abstract bool HasRigidbody();
        public abstract bool HasPlayedSoundEffect();
        public abstract bool HasInstantiatedParticleSystem();

        public abstract int GetPreviousDirectionOnYAxis();
        public abstract float GetRadius();
        public abstract float GetAngularVelocity();
        public abstract void AddForce(Vector3 force);
        public abstract void AddForceAtPosition(Vector3 force, Vector3 position);
        public abstract void AddTorque(float torque);
        public abstract void SetSoundPlayed(bool soundPlayed);
        public abstract void SetParticleSystemInstantiated(bool instantiated);
        public abstract void SetPreviousPosition();
        public abstract void SetDirectionOnYAxis(int dir);
        public abstract List<Vector2> GetPolygon();
        public abstract Vector3 GetVelocity();
        public abstract Vector2 GetPointVelocity(Vector2 point);
        public abstract Vector3 GetPreviousPosition();

        // Override Equals and GetHashCode so we can compare against colliders directly.
        public abstract override bool Equals(object other);
        public abstract override int GetHashCode();
    }
}