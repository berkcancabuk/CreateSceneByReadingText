using UnityEngine;

namespace Water2DTool {
    public enum RippleSourceMode {
        WhenMoving,
        RandomInterval,
        FixedInterval,
    }

    public class RippleSource : MonoBehaviour {
        /// <summary>
        /// The position the ripple source had on the previous frame. 
        /// </summary>
        private Vector3 prevPos;
        private Water2D_Simulation sim;
        private float precisionFactor = 0.0001f;
        /// <summary>
        /// The time between ripples. 
        /// </summary>
        private float currentPeriod = 0.1f;
        /// <summary>
        /// Time counter to determine when to generate a new ripple. 
        /// </summary>
        private float timeCount = 0.1f;
        /// <summary>
        /// The initial radius of the ripple
        /// </summary>
        public float radius = 0.3f;
        /// <summary>
        /// Ripple stregth
        /// </summary>
        public float strength = -0.15f;
        /// <summary>
        /// The number of ripples per second.
        /// </summary>
        public float frequency = 5f;
        /// <summary>
        /// Minimum value for a random number that is used to calculate how long to wait before generating a new ripple.
        /// </summary>
        public float minPeriod = 0.5f;
        /// <summary>
        /// Maximum value for a random number that is used to calculate how long to wait before generating a new ripple.
        /// </summary>
        public float maxPeriod = 1.0f;
        /// <summary>
        /// Water line world position on the x Axis.
        /// </summary>
        public float waterLineYAxisWorldPosition = 0.0f;
        /// <summary>
        /// Should this object generate ripples?.
        /// </summary>
        public bool active = true;
        /// <summary>
        /// Tells the Water2D_Ripple script that this object should generate a new ripple.
        /// </summary>
        public bool newRipple = false;
        /// <summary>
        /// The scale of the handle gizmo.
        /// </summary>
        [Range(0.01f, 1f)]
        public float handleScale = 0.3f;
        /// <summary>
        /// When set to true, the position on the Y axis of the ripple source is ignored.
        /// </summary>
        public bool ignoreYAxisPosition = false;
        public RippleSourceMode sourceMode = RippleSourceMode.RandomInterval;
        /// <summary>
        /// If the distance from the water line to the ripple source is greater than the value of this field, ripples are not generated
        /// </summary>
        public float interactionDistance = 1.0f;
        public new Transform transform;

        private void Start()
        {
            transform = GetComponent<Transform>();
        }

        private void Update()
        {
            if (active)
            {
                if (sim != null)
                    waterLineYAxisWorldPosition = sim.waterLineCurrentWorldPos.y;

                switch (sourceMode)
                {
                    case RippleSourceMode.WhenMoving:
                        WhenMoving();
                        break;

                    case RippleSourceMode.RandomInterval:
                        RandomInterval();
                        break;

                    case RippleSourceMode.FixedInterval:
                        FixedInterval();
                        break;
                }
            }
        }

        private void WhenMoving()
        {
            Vector3 offset = prevPos - transform.position;
            float sqrLen = offset.sqrMagnitude;

            if (ignoreYAxisPosition)
            {
                WhenMovingRipple(sqrLen);
            }
            else
            {
                if (Mathf.Abs(transform.position.y - waterLineYAxisWorldPosition) <= interactionDistance && transform.position.y > waterLineYAxisWorldPosition - interactionDistance)
                {
                    WhenMovingRipple(sqrLen);
                }
            }
        }

        private void RandomInterval()
        {
            if (ignoreYAxisPosition)
            {
                RandomIntervalRipple();
            }
            else
            {
                if (Mathf.Abs(transform.position.y - waterLineYAxisWorldPosition) <= interactionDistance && transform.position.y > waterLineYAxisWorldPosition - interactionDistance)
                {
                    RandomIntervalRipple();
                }
            }
        }

        private void FixedInterval()
        {
            if (ignoreYAxisPosition)
            {
                FixedIntervalRipple();
            }
            else
            {
                if (Mathf.Abs(transform.position.y - waterLineYAxisWorldPosition) <= interactionDistance && transform.position.y > waterLineYAxisWorldPosition - interactionDistance)
                {
                    FixedIntervalRipple();
                }
            }
        }

        private void WhenMovingRipple(float sqrLen)
        {
            if (sqrLen > precisionFactor * precisionFactor)
                newRipple = true;

            prevPos = transform.position;
        }

        private void RandomIntervalRipple()
        {
            timeCount += Time.deltaTime;

            if (timeCount > currentPeriod)
            {
                timeCount = 0;
                newRipple = true;
                NewPeriod();
            }
        }

        private void FixedIntervalRipple()
        {
            float period = 1f / frequency;
            if (timeCount > period)
            {
                timeCount -= period;
                newRipple = true;
            }

            timeCount += Time.deltaTime;
        }

        public void NewPeriod()
        {
            currentPeriod = Random.Range(minPeriod, maxPeriod);
        }

        void OnDrawGizmosSelected()
        {
            if (transform == null)
                transform = GetComponent<Transform>();

            Gizmos.color = new Color(0, 1, 0, 1.0f);
            Gizmos.DrawLine(transform.position - new Vector3(0, interactionDistance, 0), transform.position + new Vector3(0, interactionDistance, 0));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            sim = collision.GetComponent<Water2D_Simulation>();

            if (sim != null)
            {
                active = true;
                waterLineYAxisWorldPosition = sim.waterLineCurrentWorldPos.y;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<Water2D_Simulation>() == sim)
            {
                active = false;
                sim = null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            sim = other.GetComponent<Water2D_Simulation>();

            if (sim != null)
            {
                active = true;
                waterLineYAxisWorldPosition = sim.waterLineCurrentWorldPos.y;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.GetComponent<Water2D_Simulation>() == sim)
            {
                active = false;
                sim = null;
            }
        }
    }
}

