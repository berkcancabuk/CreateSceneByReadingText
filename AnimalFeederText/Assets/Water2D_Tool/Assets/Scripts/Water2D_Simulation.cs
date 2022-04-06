using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Water2D_ClipperLib;
using Path = System.Collections.Generic.List<Water2D_ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<Water2D_ClipperLib.IntPoint>>;

namespace Water2DTool {
    #region Enumerators

    /// <summary>
    /// Describes how the water handles should be animated.
    /// </summary>
    public enum Water2D_AnimationMethod {
        /// <summary>
        /// The water will not be animated.
        /// </summary>
        None,
        /// <summary>
        /// The animated handle follows the movement of a referenced object.
        /// </summary>
        Follow,
        /// <summary>
        /// The animated handle's global position is set to that of a referenced object.
        /// </summary>
        Snap
    }

    /// <summary>
    /// Describes the direction of the water flow.
    /// </summary>
    public enum Water2D_FlowDirection {
        /// <summary>
        /// The water flow will push the objects up.
        /// </summary>
        Up,
        /// <summary>
        /// The water flow will push the objects down.
        /// </summary>
        Down,
        /// <summary>
        /// The water flow will push the objects to the left.
        /// </summary>
        Left,
        /// <summary>
        /// The water flow will push the objects to the right.
        /// </summary>
        Right
    }

    /// <summary>
    /// Determines which clipping method will be used to calculate the shape of the polygon that is below the water.
    /// </summary>
    public enum Water2D_ClippingMethod {
        /// <summary>
        /// Uses the Sutherland Hodgman polygon clipping algorithm. This is the cheapest option in terms of
        /// performance because the clipping polygon is always a horizontal line.
        /// </summary>
        Simple,
        /// <summary>
        /// Uses the clipper library. This option is best to use when you want the objects to better react to water waves.
        /// </summary>
        Complex,
    }

    /// <summary>
    /// Describes how the Buoyant Force applied to a floating object, should be simulated.
    /// </summary>
    public enum Water2D_BuoyantForceMode {
        /// <summary>
        /// No buoyancy will be applied to objects.
        /// </summary>
        None,
        /// <summary>
        /// A physics based Buoyant Force will be applied to a floating object. This method takes into
        /// account the mass of the object and it's shape. Use this for a more realistic simulation
        /// of Buoyant Forces. It's a little more expensive then the Linear.
        /// </summary>
        PhysicsBased,
        /// <summary>
        /// A linear Buoyant Force will be applied to a floating object. This method does not take into
        /// account the objects shape.
        /// </summary>
        Linear
    }

    /// <summary>
    /// Describes how surface waves like the one created by the wind should be generated.
    /// </summary>
    public enum Water2D_SurfaceWaves {
        /// <summary>
        /// No surface waves will be generated.
        /// </summary>
        None,
        /// <summary>
        /// Generates small random water splashes.
        /// </summary>
        RandomSplashes,
        /// <summary>
        /// Sine waves with random values are used to simulate waves.
        /// </summary>
        SineWaves,
    }

    /// <summary>
    /// Determines the number of sine waves.
    /// </summary>
    public enum Water2D_SineWaves {
        /// <summary>
        /// A single sine wave will be used to change the velocity of the survace vertices.
        /// </summary>
        SingleSineWave,
        /// <summary>
        /// Multiple sine waves will be used to change the velocity of the survace vertices.
        /// </summary>
        MultipleSineWaves
    }

    /// <summary>
    /// Describes the type of the water.
    /// </summary>
    public enum Water2D_Type {
        /// <summary>
        /// The water will react to objects and will influence their position.
        /// </summary>
        Dynamic,
        /// <summary>
        /// The water won't react to objects and won't influence their position, but can be animated.
        /// </summary>
        Decorative
    }

    /// <summary>
    /// Describes the character controller type.
    /// </summary>
    public enum Water2D_CharacterControllerType {
        /// <summary>
        /// The character controller uses the Unity Physics engine to move the player. The Player is a dynamic object.
        /// </summary>
        PhysicsBased,
        /// <summary>
        /// The character controller uses Raycasts to determine and control the player position. The Player is a kinematic object.
        /// </summary>
        RaycastBased
    }

    /// <summary>
    /// Methods used to determine if a surface vertex velocity should be changed by and object.
    /// </summary>
    public enum Water2D_CollisionDetectionMode {
        /// <summary>
        /// Method used to determine if a surface vertex velocity should be changed by and object.
        /// When a vertex is inside the bounding box of an objects collider a raycast is performed to find if that vertex is actually inside the collider.
        /// </summary>
        RaycastBased,
        /// <summary>
        /// Method used to determine if a surface vertex velocity should be changed by and object
        /// A surface vertex velocity can be influenced by an object if the vertex is inside the bounding box of that objects collider.
        /// </summary>
        BoundsBased
    }

    #endregion Enumerators

    [RequireComponent(typeof(Water2D_Tool)), RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class Water2D_Simulation : MonoBehaviour {

        #region Private fields
        private float originalWaterHeight;
        /// <summary>
        /// Surface vertices velocities.
        /// </summary>
        private List<float> velocities;
        /// <summary>
        /// Surface vertices cccelerations.
        /// </summary>
        private List<float> vertYOffsets;
        private List<float> accelerations;
        private List<float> leftDeltas;
        private List<float> rightDeltas;
        /// <summary>
        /// List of Y values created from the overlapping of a number of sine waves.
        /// This values are used to change the velocity of the surface vertices and simulate idle waves.
        /// </summary>
        private List<float> sineY;
        /// <summary>
        /// Water mesh vertices.
        /// </summary>
        private List<Vector3> frontMeshVertices;
        /// <summary>
        /// Water mesh vertices.
        /// </summary>
        private List<Vector3> topMeshVertices;
        /// <summary>
        /// Water mesh UVs.
        /// </summary>
        private List<Vector2> frontMeshUVs;
        /// <summary>
        /// Water mesh UVs.
        /// </summary>
        private List<Vector2> topMeshUVs;
        /// <summary>
        /// A phase to apply to each sine wave.
        /// </summary>
        private float phase = 0;
        /// <summary>
        /// The number of vertices the water surface (top edge of the water) has.
        /// </summary>
        private int surfaceVertsCount;
        /// <summary>
        /// The local position of the water line on the Y axis.
        /// </summary>
        private float waterLineCurrentLocalPos;
        /// <summary>
        /// The local position of the water line on the Y axis at the start of the current frame.
        /// </summary>
        private float waterLinePreviousLocalPos;
        /// <summary>
        /// The scale of the force that should be applied to an object.
        /// </summary>
        private float forceFactor;
        /// <summary>
        /// The position where the upwards force should be applied to an object submerged in the water.
        /// </summary>
        private Vector3 forcePosition;
        /// <summary>
        /// The upward force that should be applied to an object submerged in the water.
        /// </summary>
        private Vector3 upLift;
        /// <summary>
        /// The default water height. This value is set in the Start method.
        /// </summary>
        private float defaultWaterHeight;
        /// <summary>
        /// The default water area. This value is set in the Start method.
        /// </summary>
        private float defaultWaterArea;
        /// <summary>
        /// The position on the Y axis the TopEdge object had on the previous frame.
        /// </summary>
        private float prevTopEdgeYPos;
        /// <summary>
        /// The position on the Y axis the BottomEdge object had on the previous frame.
        /// </summary>
        private float prevBottomEdgeYPos;
        /// <summary>
        /// The position on the X axis the LeftEdge object had on the previous frame.
        /// </summary>
        private float prevLeftEdgeXPos;
        /// <summary>
        /// The position on the X axis the RightEdge object had on the previous frame.
        /// </summary>
        private float prevRightEdgeXPos;
        /// <summary>
        /// The difference between the surface vertices and the current waterline position.
        /// </summary>
        //private float[] vertYOffsets;
        /// <summary>
        /// Used to determine if the water mesh should be recreated from scratch.
        /// </summary>
        private bool recreateWaterMesh = false;
        /// <summary>
        /// Used to determine if the water mesh height should be updated.
        /// </summary>
        private bool updateWaterHeight = false;
        /// <summary>
        /// The amount by which the water height should be increased after all the calculations are done.
        /// </summary>
        private float waterLineYPosOffset = 0;
        /// <summary>
        /// The value that defaultWaterAreaOffset had at the end of the previous frame
        /// (has at the start of the current frame).
        /// </summary>
        private float prevDefaultWaterAreaOffset;
        /// <summary>
        /// Water Mesh.
        /// </summary>
        private Mesh frontMeshFilter;
        /// <summary>
        /// Water Mesh.
        /// </summary>
        private Mesh topMeshFilter;
        /// <summary>
        /// Water2D component.
        /// </summary>
        private Water2D_Tool water2D;
        /// <summary>
        /// Used to generate a random splash.
        /// </summary>
        private bool makeSplash = false;
        /// <summary>
        /// The variable is set to true when a collider with the tag "Player"
        /// is found by the OnTriggerEnter2D method.
        /// </summary>
        private bool onTriggerPlayerDetected = false;
        /// <summary>
        /// The area of the polygon segment that is below the waterline.
        /// </summary>
        private float area = 0;
        /// <summary>
        /// The mass of the water that has the area equal to the area of the polygon segment that is below the waterline.
        /// </summary>
        private float displacedMass = 0;
        /// <summary>
        /// When animating an edge of the water, Water2D compares the difference between the current position
        /// of a referenced object and the one it had on the previous frame to the precisionFactor.
        /// If the difference is bigger than the precisionFactor, than the object moved on the current frame
        /// from the position it had on the previous frame. Comparing the position the object has on the current
        /// frame to the one that it had on the previous frame directly is not a good idea as Unity may round the
        /// numbers differently even if the reference object didn't change its position. Do not change the value of
        /// this field if you are not sure what it does.
        /// </summary>
        private float precisionFactor = 0.001f;
        /// <summary>
        /// The global position of the left handle.
        /// </summary>
        private Vector3 leftHandleGlobalPos;
        /// <summary>
        /// A list of points that form the water line.
        /// </summary>
        private List<Vector2> waterLinePoints;
        /// <summary>
        /// It is used to convert float numbers to int and back.
        /// </summary>
        private float scaleFactor = 100000f;
        /// <summary>
        /// 1 / x. The reverse of scaleFactor value.
        /// </summary>
        private float rScaleFactor;
        /// <summary>
        /// Should mesh vertex and UV date be updated.
        /// </summary>
        private bool updateMeshDate = false;
        /// <summary>
        /// Instance the polygon clipping class.
        /// </summary>
        private Water2D_PolygonClipping polygonClipping;
        /// <summary>
        /// Stores the positions of the 4 corners of a box.
        /// </summary>
        private List<Vector2> boxVertices;
        /// <summary>
        /// List of points that describe the segment of a collider that is below the water line.
        /// </summary>
        private List<Vector2> submergedPolygon;
        /// <summary>
        /// 2 points that form a horizontal line.
        /// </summary>
        private List<Vector2> clipPolygon;
        /// <summary>
        /// Reference to the clipper library.
        /// </summary>
        private Clipper clipper;
        private Paths solutionPath;
        private Path subjPath;
        private Path clipPath;
        /// <summary>
        /// List of points that describe an intersaction poligon.
        /// </summary>
        private List<Vector2> intersectionPolygon;
        /// <summary>
        /// List of points that describe multiple intersaction poligons.
        /// </summary>
        private List<List<Vector2>> intersectionPolygons;
        /// <summary>
        /// 2 points that form a horizontal line.
        /// </summary>
        private Vector2[] linePoints;
        /// <summary>
        /// Array that stores the results of a RaycastNonAlloc.
        /// </summary>
        private RaycastHit2D[] hit2D;
        /// <summary>
        /// Array that stores the results of a OverlapSphereNonAlloc.
        /// </summary>
        private Collider[] hit3D;
        /// <summary>
        /// Should the water be tiled?.
        /// </summary>
        private bool tiling = true;
        /// <summary>
        /// Is the Water2D_Ripple script attached to this object?.
        /// </summary>
        private bool hasRippleScript = false;
        /// <summary>
        /// How much should the top mesh up or down on the Y axis.
        /// </summary>
        private float yAxisOffset = 0f;
        /// <summary>
        /// The renderer component of the front mesh.
        /// </summary>
        private Renderer frontMeshRend;
        /// <summary>
        /// The renderer component of the top mesh.
        /// </summary>
        private Renderer topMeshRend;
        /// <summary>
        /// The Transform component of the water object.
        /// </summary>
        private new Transform transform;
        #endregion Private fields

        #region Public fields

#if UNITY_EDITOR
        public bool showSpringProperties = true;
        public bool showfloatingBuoyantForce = true;
        public bool showAnimation = true;
        public bool showFlow = true;
        public bool showMiscellaneous = true;
        public bool showSurfaceWaves = true;
        public bool showPlayerSettings = true;
#endif

        public Water2D_BuoyantForceMode buoyantForceMode = Water2D_BuoyantForceMode.PhysicsBased;
        public Water2D_SurfaceWaves surfaceWaves = Water2D_SurfaceWaves.None;
        public Water2D_Type waterType = Water2D_Type.Dynamic;
        public Water2D_AnimationMethod animationMethod = Water2D_AnimationMethod.None;
        public Water2D_ClippingMethod clippingMethod = Water2D_ClippingMethod.Simple;
        public Water2D_FlowDirection flowDirection = Water2D_FlowDirection.Right;
        public Water2D_SineWaves sineWavesType = Water2D_SineWaves.SingleSineWave;
        public Water2D_CharacterControllerType characterControllerType = Water2D_CharacterControllerType.PhysicsBased;
        public Water2D_CollisionDetectionMode collisionDetectionMode = Water2D_CollisionDetectionMode.RaycastBased;

        /// <summary>
        /// List of floating objects
        /// </summary>
        public List<FloatingObject> floatingObjects;
        /// <summary>
        /// The velocity that is applied to a surface vertex when the player exits the water.
        /// </summary>
        public float playerOnExitVelocity = 0.2f;
        /// <summary>
        /// Should the player generate a water ripple when it exits the water?.
        /// </summary>
        public bool playerOnExitRipple = false;
        /// <summary>
        /// Should a particle system be instantiated and a sound effect played the player exits the water?.
        /// </summary>
        public bool playerOnExitPSAndSound = false;
        /// <summary>
        /// List of sine wave amplitudes.
        /// </summary>
        public List<float> sineAmplitudes = new List<float>();
        /// <summary>
        /// The amount by which a particular sine wave is stretched.
        /// </summary>
        public List<float> sineStretches = new List<float>();
        /// <summary>
        /// Sine wave phase offset.
        /// </summary>
        public List<float> phaseOffset = new List<float>();
        /// <summary>
        /// This is the variable that should be changed if animateWaterArea is set to true.
        /// </summary>
        public float defaultWaterAreaOffset;
        /// <summary>
        /// The number of Sine waves.
        /// </summary>
        public int sineWaves = 4;
        /// <summary>
        /// The spring constant.
        /// </summary>
        public float springConstant = 0.02f;
        /// <summary>
        /// The damping applied to the surface vertices velocities.
        /// </summary>
        public float damping = 0.04f;
        /// <summary>
        /// It controls how fast the waves spread.
        /// </summary>
        public float spread = 0.03f;
        /// <summary>
        /// Determines how much force should be applied to an object submerged in the water. A value of 3 means
        /// that 3m under the water, the force applied to an object will be 2 times greater than the force applied
        /// at the surface of the water.
        /// </summary>
        public float floatHeight = 3;
        /// <summary>
        /// A bounce damping for the object bounce.
        /// </summary>
        public float bounceDamping = 0.15f;
        /// <summary>
        /// Offsets the position where the upwards force should be applied to an object submerged in the water.
        /// </summary>
        public Vector3 forcePositionOffset;
        /// <summary>
        /// Limits the velocity on the Y axis that is applied to a waterline vertex.
        /// </summary>
        public float collisionVelocityScale = 0.0125f;
        /// <summary>
        /// Used to animate the position of the water line on the Y axis. Assign an animated
        /// object to this field if you want to increase or decrease the water level.
        /// </summary>
        public Transform topEdge;
        /// <summary>
        /// Used to animate the position of the water bottom on the Y axis. Will also affect the waterline position.
        /// </summary>
        public Transform bottomEdge;
        /// <summary>
        /// Used to animate the position of the left edge of the water.
        /// </summary>
        public Transform leftEdge;
        /// <summary>
        /// Used to animate the position of the right edge of the water.
        /// </summary>
        public Transform rightEdge;
        /// <summary>
        /// A particle system prefab for water splash effect.
        /// </summary>
        public GameObject particleS;
        /// <summary>
        /// When set to true will force the default water area to be constant. So if the water width
        /// decreases (increases) the water height will increase (decrease).
        /// </summary>
        public bool constantWaterArea = false;
        /// <summary>
        /// When set to true the objects that are submerged in the water will make the water rise.
        /// </summary>
        public bool waterDisplacement = false;
        /// <summary>
        /// Set this to true if you want to animate the default water area.
        /// </summary>
        public bool animateWaterArea = false;
        /// <summary>
        /// Controls the water wave propagation speed.
        /// </summary>
        public float waveSpeed = 8;
        /// <summary>
        /// Objects with downwards velocity greater than the value of velocityFilter won't create waves.
        /// </summary>
        public float velocityFilter = -2f;
        /// <summary>
        /// The density of the water.
        /// </summary>
        public float waterDensity = 1f;
        /// <summary>
        /// The number of vertices a regular polygon should have.
        /// </summary>
        public int polygonCorners = 8;
        /// <summary>
        /// The maximum drag that should be applied to a colliders edge.
        /// </summary>
        public float maxDrag = 500f;
        /// <summary>
        /// The maximum lift that should be applied to a colliders edge.
        /// </summary>
        public float maxLift = 200f;
        /// <summary>
        /// Drag Coefficient.
        /// </summary>
        public float dragCoefficient = 0.4f;
        /// <summary>
        /// Lift Coefficient.
        /// </summary>
        public float liftCoefficient = 0.25f;
        /// <summary>
        /// How often should a random splash be generated.
        /// </summary>
        public float timeStep = 0.5f;
        /// <summary>
        /// The max value of a random amplitude.
        /// </summary>
        public float maxAmplitude = 0.1f;
        /// <summary>
        /// The min value of a random amplitude.
        /// </summary>
        public float minAmplitude = 0.01f;
        /// <summary>
        /// The max value of a random stretch.
        /// </summary>
        public float maxStretch = 2f;
        /// <summary>
        /// The min value of a random stretch.
        /// </summary>
        public float minStretch = 0.8f;
        /// <summary>
        /// The max value of a random phase offset.
        /// </summary>
        public float maxPhaseOffset = 0.1f;
        /// <summary>
        /// The min value of a random phase offset.
        /// </summary>
        public float minPhaseOffset = 0.02f;
        /// <summary>
        /// The max value of a random velocity.
        /// </summary>
        public float maxVelocity = 0.1f;
        /// <summary>
        /// The min value of a random velocity.
        /// </summary>
        public float minVelocity = -0.1f;
        /// <summary>
        /// The force scale used when Buoyant Force Mode is set to Linear. A value of 1f will make the object with
        /// the mass of 1kg float at the surface of the water.
        /// </summary>
        public float forceScale = 4f;
        /// <summary>
        /// The drag coefficient used when Buoyant Force Mode is set to Linear.
        /// </summary>
        public float liniarBFDragCoefficient = 0.1f;
        /// <summary>
        /// The angular drag coefficient used when Buoyant Force Mode is set to Linear.
        /// </summary>
        public float linearBFAbgularDragCoefficient = 0.01f;
        /// <summary>
        /// The bottom region of a colliders bounding box that can affect the velocity of a vertex.
        /// We use this value to limit the ability of the objects with big bounding boxes to affect
        /// the velocity of the surface vertices. If we don't do this, a long object that falls down
        /// slowly will push the water down for a longer period of time and produce a not very realistic simulation.
        /// </summary>
        public float interactionRegion = 1f;
        /// <summary>
        /// The global position of the water line on the Y axis.
        /// </summary>
        public Vector3 waterLineCurrentWorldPos;
        /// <summary>
        /// Splash sound effect.
        /// </summary>
        public AudioClip splashSound;
        /// <summary>
        /// The size for the players bounding box.
        /// </summary>
        public Vector2 playerBoundingBoxSize = new Vector2(1f, 1f);
        /// <summary>
        /// By default the center of the bounding box will be the transform.position of the
        /// object. Use this variable to offset the players bounding box center.
        /// </summary>
        public Vector2 playerBoundingBoxCenter = Vector2.zero;
        /// <summary>
        /// Use this variable to set the scale for the buoyant force that is applied to
        /// the object with the "Player" tag.
        /// </summary>
        public float playerBuoyantForceScale = 3.0f;
        /// <summary>
        /// When enabled will show in the Scene View the shape of the polygon that is below the waterline.
        /// </summary>
        public bool showPolygon = false;
        /// <summary>
        /// When enabled will show in the Scene View the velocity direction, drag direction,
        /// lift direction and the normal of a leading edge.
        /// </summary>
        public bool showForces = false;
        /// <summary>
        /// Will scale down (up) the velocity that is applied to the neighbor vertices when RandomWave method is called.
        /// </summary>
        public float neighborVertVelocityScale = 0.5f;
        /// <summary>
        /// Will scale down (up) the velocity that is applied to a vertex from a sine wave.
        /// </summary>
        public float sineWaveVelocityScale = 0.05f;
        /// <summary>
        /// The radius of a sphere that will be used to check if there is a collider near a surface vertex.
        /// </summary>
        public float overlapSphereRadius = 0.05f;
        /// <summary>
        /// Should the spring simulation be enabled?.
        /// </summary>
        public bool springSimulation = true;
        /// <summary>
        /// The offset on the Y axis from the position of a referenced object.
        /// </summary>
        public float topEdgeYOffset = 0f;
        /// <summary>
        /// The offset on the Y axis from the position of a referenced object.
        /// </summary>
        public float bottomEdgeYOffset = 0f;
        /// <summary>
        /// The offset on the X axis from the position of a referenced object.
        /// </summary>
        public float leftEdgeXOffset = 0f;
        /// <summary>
        /// The offset on the X axis from the position of a referenced object.
        /// </summary>
        public float rightEdgeXOffset = 0f;
        /// <summary>
        /// Offsets the position where the particle systems are created on the Z axis.
        /// </summary>
        public Vector3 particleSystemPosOffset = Vector3.zero;
        /// <summary>
        /// The sorting layer name for the particle system.
        /// </summary>
        public string particleSystemSortingLayerName = "Default";
        /// <summary>
        /// The order in layer for the particle system.
        /// </summary>
        public int particleSystemOrderInLayer = 0;
        /// <summary>
        /// The number of vertical mesh segments that should fit in a water line segment.
        /// </summary>
        public int meshSegmentsPerWaterLineSegment = 4;
        public bool showClippingPlolygon = false;
        /// <summary>
        /// Should the water flow be enabled?.
        /// </summary>
        public bool waterFlow = false;
        /// <summary>
        /// The angle of the water flow. This value controls the direction of the water flow.
        /// </summary>
        public float flowAngle = 0f;
        /// <summary>
        /// The force of the water flow.
        /// </summary>
        public float waterFlowForce = 5f;
        /// <summary>
        /// Should the direction be controlled using an angle value specified by the developer?.
        /// </summary>
        public bool useAngles = false;
        /// <summary>
        /// The amplitude of a sine wave. This variable controls the height of the sine wave
        /// </summary>
        public float waveAmplitude = 0.1f;
        /// <summary>
        /// The sine wave stretch. The bigger the value of the stretch the more compact the waves are.
        /// </summary>
        public float waveStretch = 1f;
        /// <summary>
        /// Sine wave phase offset. The bigger the value of the phase offset, the faster the waves move to the left (right).
        /// </summary>
        public float wavePhaseOffset = 0.2f;
        /// <summary>
        /// When enabled the sine waves amplitude, stretch and phase offset will be generated randomaly at the start of the game.
        /// </summary>
        public bool randomValues = true;
        /// <summary>
        /// Should the particle system sorting layer and order in layer be set when it is instantiated?
        /// </summary>
        public bool particleSystemSorting = false;
        /// <summary>
        /// The initial width of the water wave.
        /// </summary>
        public float rippleWidth = 1f;
        /// <summary>
        /// The velocity that will be applied to the surface vertices when the player enters the water.
        /// </summary>
        public float playerOnEnterVelocity = -0.2f;
        /// <summary>
        /// Layer Mask that tells the water which colliders should be ignored.
        /// </summary>
        public LayerMask collisionLayers = ~0;
        /// <summary>
        /// How far to the left and right from a vertex world position should we look for a collider.
        /// </summary>
        public float raycastDistance = 0.05f;
        /// <summary>
        /// How many seconds after an object interacted with the water surface and generated a ripple, should the spring simulation stop updating.
        /// </summary>
        public float interactionTime = 5f;
        /// <summary>
        /// The time interval since the last time an object interacted with the surface of the water and generated a ripple
        /// </summary>
        private float interactionTimeCount = 6f;
        /// <summary>
        /// Reference to the Water2D_Ripple component of this object. 
        /// </summary>
        private Water2D_Ripple water2DRipple;

        #endregion Public fields

        #region Class methods

        private void Awake()
        {
            transform = GetComponent<Transform>();
            water2D = GetComponent<Water2D_Tool>();
            water2DRipple = GetComponent<Water2D_Ripple>();
            water2D.OnAwakeMeshRebuild();

            if (water2DRipple)
            {
                water2DRipple.InstantiateRenderTextures();
                springSimulation = false;
                tiling = false;
                hasRippleScript = true;
            }
            frontMeshFilter = GetComponent<MeshFilter>().sharedMesh;

            intersectionPolygon = new List<Vector2>();
            intersectionPolygons = new List<List<Vector2>>();
            submergedPolygon = new List<Vector2>();
            polygonClipping = new Water2D_PolygonClipping();

            clipPolygon = new List<Vector2>();
            clipper = new Clipper();
            solutionPath = new Paths();
            subjPath = new Path();
            clipPath = new Path();
            rScaleFactor = 1f / scaleFactor;

            frontMeshVertices = frontMeshFilter.vertices.ToList();
            surfaceVertsCount = water2D.frontMeshVertsCount / 2;
            frontMeshUVs = frontMeshFilter.uv.ToList();

            if (water2D.cubeWater)
            {
                topMeshFilter = water2D.topMeshGameObject.GetComponent<MeshFilter>().sharedMesh;
                topMeshVertices = topMeshFilter.vertices.ToList();
                topMeshUVs = topMeshFilter.uv.ToList();
            }

            waterLineCurrentWorldPos = transform.TransformPoint(frontMeshVertices[surfaceVertsCount + 1]);
            waterLineCurrentLocalPos = frontMeshVertices[surfaceVertsCount + 1].y;
            waterLinePreviousLocalPos = frontMeshVertices[surfaceVertsCount + 1].y;
            defaultWaterHeight = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
            originalWaterHeight = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
            defaultWaterArea = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y) * Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);

            sineY = new List<float>();
            floatingObjects = new List<FloatingObject>();
            waterLinePoints = new List<Vector2>();

            velocities = new List<float>();
            accelerations = new List<float>();
            leftDeltas = new List<float>();
            rightDeltas = new List<float>();

            boxVertices = new List<Vector2>();
            linePoints = new Vector2[2];
            vertYOffsets = new List<float>();

            if (!water2D.use3DCollider)
                hit2D = new RaycastHit2D[5];
            else
                hit3D = new Collider[5];

            for (int i = 0; i < 4; i++)
                boxVertices.Add(Vector2.zero);

            for (int i = 0; i < surfaceVertsCount; i++)
            {
                velocities.Add(0.0f);
                accelerations.Add(0.0f);
                leftDeltas.Add(0.0f);
                rightDeltas.Add(0.0f);
                sineY.Add(0.0f);
            }

            if (topEdge != null)
                prevTopEdgeYPos = topEdge.transform.position.y;

            if (bottomEdge != null)
                prevBottomEdgeYPos = bottomEdge.transform.position.y;

            if (leftEdge != null)
                prevLeftEdgeXPos = leftEdge.transform.position.x;

            if (rightEdge != null)
                prevRightEdgeXPos = rightEdge.transform.position.x;

            if (sineWavesType == Water2D_SineWaves.MultipleSineWaves && randomValues)
                GenerateSineVariables();

            if (surfaceVertsCount > meshSegmentsPerWaterLineSegment)
            {
                int n = 0;
                while (n < surfaceVertsCount)
                {
                    waterLinePoints.Add(transform.TransformPoint(frontMeshVertices[surfaceVertsCount + n]));
                    n += meshSegmentsPerWaterLineSegment;
                }

                if (meshSegmentsPerWaterLineSegment != 1)
                {
                    Vector2 lastVert = transform.TransformPoint(frontMeshVertices[surfaceVertsCount + surfaceVertsCount - 1]);
                    waterLinePoints.Add(new Vector2(lastVert.x, lastVert.y));
                }
            }
            else
            {
                waterLinePoints.Add(transform.TransformPoint(frontMeshVertices[surfaceVertsCount]));
                Vector2 lastVert = transform.TransformPoint(frontMeshVertices[surfaceVertsCount + surfaceVertsCount - 1]);
                waterLinePoints.Add(new Vector2(lastVert.x, lastVert.y));
            }

            frontMeshFilter.bounds = new Bounds(Vector3.zero, Vector3.one * 2000f);
            if (water2D.cubeWater)
                topMeshFilter.bounds = new Bounds(Vector3.zero, Vector3.one * 2000f);

            frontMeshRend = GetComponent<Renderer>();

            if (water2D.cubeWater)
                topMeshRend = water2D.topMeshGameObject.GetComponent<Renderer>();

            if (surfaceWaves != Water2D_SurfaceWaves.None)
                interactionTimeCount = 0;
        }

        private void FixedUpdate()
        {
            waterLineCurrentWorldPos = transform.TransformPoint(water2D.handlesPosition[0]);
            waterLineCurrentLocalPos = water2D.handlesPosition[0].y;

            if (waterDisplacement)
                WaterDisplacement();

            if (surfaceWaves != Water2D_SurfaceWaves.None)
            {
                if (surfaceWaves == Water2D_SurfaceWaves.SineWaves)
                    SineWaves();

                if (surfaceWaves == Water2D_SurfaceWaves.RandomSplashes && !makeSplash)
                    StartCoroutine(RandomWave());
            }

            if (animationMethod != Water2D_AnimationMethod.None)
            {
                if (hasRippleScript)
                    ShaderAnimation();
                else
                    WaterAnimation();
            }

            if (springSimulation)
                WaterWaves();

            WaterMesh();

            if (!springSimulation && surfaceWaves != Water2D_SurfaceWaves.None)
                UpdateWaterLinePoints();

            if (buoyantForceMode != Water2D_BuoyantForceMode.None)
                Buoyancy();

            if (springSimulation || updateMeshDate || surfaceWaves != Water2D_SurfaceWaves.None)
            {
                frontMeshFilter.SetVertices(frontMeshVertices);
                frontMeshFilter.SetUVs(0, frontMeshUVs);

                if (!water2D.cubeWater)
                    updateMeshDate = false;

                if (water2D.cubeWater)
                {
                    topMeshFilter.SetVertices(topMeshVertices);
                    topMeshFilter.SetUVs(0, topMeshUVs);
                    updateMeshDate = false;
                }
            }

            waterLineYPosOffset = 0;
        }


        /// <summary>
        /// Checks if a collider exists or not. Destroys it if it's missing.
        /// </summary>
        public bool ColliderExists(int cIndex)
        {
            if (cIndex >= floatingObjects.Count) return false;
            if (!floatingObjects[cIndex].HasCollider())
            {
                if (floatingObjects[cIndex].IsPlayer())
                    onTriggerPlayerDetected = false;

                floatingObjects.RemoveAt(cIndex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generate a list of random values for the sine amplitudes, stretches and phase offsets.
        /// </summary>
        private void GenerateSineVariables()
        {
            sineAmplitudes.Clear();
            sineStretches.Clear();
            phaseOffset.Clear();

            for (int i = 0; i < sineWaves; i++)
            {
                // Controls the height of the sine wave.
                sineAmplitudes.Add(Random.Range(minAmplitude, maxAmplitude));

                // The bigger the value the more compact the waves are.
                sineStretches.Add(Random.Range(minStretch, maxStretch));

                // The bigger the value the faster the waves move to the left (right).
                phaseOffset.Add(Random.Range(minPhaseOffset, maxPhaseOffset));
            }
        }


        /// <summary>
        /// Applies buoyancy to the objects in the water based on the buoyancy settings.
        /// </summary>
        private void Buoyancy()
        {
            if (buoyantForceMode == Water2D_BuoyantForceMode.PhysicsBased && waterType == Water2D_Type.Dynamic)
                PhysicsBasedBuoyantForce();

            if (buoyantForceMode == Water2D_BuoyantForceMode.Linear && waterType == Water2D_Type.Dynamic)
                LinearBuoyantForce();
        }

        /// <summary>
        /// Calculates the height of the displaced water when an object is submerged in the water.
        /// </summary>
        private void WaterDisplacement()
        {
            // Resets the global and local positions of the waterline to their default values.
            waterLineCurrentLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight;
            waterLineCurrentWorldPos = transform.TransformPoint(new Vector3(0f, waterLineCurrentLocalPos, 0f));

            if (!water2D.use3DCollider)
            {
                int len = floatingObjects.Count;
                for (int i = 0; i < len; i++)
                {
                    ApplyWaterDisplacement(i);
                }
            }
        }

        /// <summary>
        /// Calculates the height of the displaced water when an object is submerged in the water.
        /// </summary>
        /// <param name="oIndex">The index of a floating object.</param>
        /// <remarks>
        /// It's not 100% accurate.
        /// </remarks>
        private void ApplyWaterDisplacement(int oIndex)
        {
            if (!ColliderExists(oIndex)) return;
            submergedPolygon.Clear();
            // Calculates the height of the displaced water by objects.
            if (floatingObjects[oIndex].bounds.min.y < waterLineCurrentWorldPos.y)
            {
                bool isIntersecting = true;
                var polyCorners = floatingObjects[oIndex].GetPolygon();

                // A line from the top left corner of the water to the top right corner.
                linePoints[0] = new Vector2(water2D.handlesPosition[2].x, waterLineCurrentWorldPos.y);
                linePoints[1] = new Vector2(water2D.handlesPosition[3].x, waterLineCurrentWorldPos.y);

                // The segment of the polygon that is below the waterline.
                submergedPolygon = polygonClipping.GetIntersectedPolygon(polyCorners, linePoints, out isIntersecting);
                // The area of the polygon segment that is below the waterline.
                float submergedArea = GetPolygonArea(submergedPolygon);

                // The height of the displaced water for the current object.
                float waterHeightOffset = submergedArea / (Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x));

                // These values are updated so the next iteration can know where the current waterline position is.
                waterLineYPosOffset += waterHeightOffset;
                waterLineCurrentLocalPos += waterHeightOffset;
                waterLineCurrentWorldPos.y += waterHeightOffset;
            }
        }

        /// <summary>
        /// Creates small waves in random places.
        /// </summary>
        private IEnumerator RandomWave()
        {
            makeSplash = true;

            // The index of the vertex whose velocity will be changed.
            int randomVert = Random.Range(0, surfaceVertsCount - 1);
            // A random velocity value.
            float randomVelocity = Random.Range(minVelocity, maxVelocity);

            velocities[randomVert] += randomVelocity;

            // Changing the velocity of a single vertex won't produce a very realistic splash,
            // so we change the velocity of its neighbours too.
            if (randomVert > 0)
                velocities[randomVert - 1] += randomVelocity * neighborVertVelocityScale;

            if (randomVert < surfaceVertsCount - 1)
                velocities[randomVert + 1] += randomVelocity * neighborVertVelocityScale;

            yield return new WaitForSeconds(timeStep);
            makeSplash = false;
        }

        /// <summary>
        /// Updates the Y values of the sine waves for each surface vertex and changes the velocities.
        /// </summary>
        private void SineWaves()
        {
            phase += 1f;
            int len0 = 0;
            int len = 0; ;
            Vector3 temp;

            if (springSimulation)
            {
                len = surfaceVertsCount;

                if (sineWavesType == Water2D_SineWaves.SingleSineWave)
                {
                    for (int i = len0; i < len; i++)
                    {
                        velocities[i] += waveAmplitude * Mathf.Sin(frontMeshVertices[i].x * waveStretch + phase * wavePhaseOffset) * sineWaveVelocityScale;
                    }
                }
                else
                {
                    for (int i = len0; i < len; i++)
                    {
                        sineY[i] = OverlapSineWaves(frontMeshVertices[i].x);
                        velocities[i] += sineY[i] * sineWaveVelocityScale;
                    }
                }
            }
            else
            {
                len = surfaceVertsCount;

                if (sineWavesType == Water2D_SineWaves.SingleSineWave)
                {
                    for (int i = 0; i < len; i++)
                    {
                        float y = waveAmplitude * Mathf.Sin(frontMeshVertices[i].x * waveStretch + phase * wavePhaseOffset);
                        temp = frontMeshVertices[i + len];
                        temp.y = waterLineCurrentLocalPos + y;
                        frontMeshVertices[i + len] = temp;
                    }
                }
                else
                {
                    for (int i = 0; i < len; i++)
                    {
                        sineY[i] = OverlapSineWaves(frontMeshVertices[i].x);
                        temp = frontMeshVertices[i + len];
                        temp.y = waterLineCurrentLocalPos + sineY[i];
                        frontMeshVertices[i + len] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// Overlaps multiple sine waves to achieve a more realistic water wave simulation.
        /// </summary>
        /// <param name="x">The position of a vertex on the X axis.</param>
        private float OverlapSineWaves(float x)
        {
            float y = 0;

            for (int i = 0; i < sineWaves; i++)
            {
                y = y + sineAmplitudes[i] * Mathf.Sin(x * sineStretches[i] + phase * phaseOffset[i]);
            }

            return y;
        }

        /// <summary>
        /// Animates the top edge of the water.
        /// </summary>
        private void ShaderAnimation()
        {
            if (topEdge != null && Mathf.Abs(topEdge.position.y - prevTopEdgeYPos) > precisionFactor)
            {
                if (animationMethod == Water2D_AnimationMethod.Follow)
                {
                    // The default water height is updated based on the difference between the position the
                    // reference object had on the previous frame and the position on the current frame.
                    defaultWaterHeight += topEdge.position.y - prevTopEdgeYPos;
                }
                else
                {
                    defaultWaterHeight = Mathf.Abs(transform.InverseTransformPoint(topEdge.position).y - water2D.handlesPosition[1].y + topEdgeYOffset);
                }

                // Because the default water height has changed, the default water area must be updated too.
                defaultWaterArea = defaultWaterHeight * Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);
                yAxisOffset = defaultWaterHeight - originalWaterHeight;
                prevTopEdgeYPos = topEdge.transform.position.y;

                UpdateTopLeftRightHandles();
                UpdateCollider();

                frontMeshRend.material.SetFloat("_HeightOffset", yAxisOffset);
                topMeshRend.material.SetFloat("_HeightOffset", yAxisOffset);

                float height = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
                frontMeshRend.material.SetFloat("_WaterHeight", height);
            }
        }

        /// <summary>
        /// Animates the top, bottom, left and right edges of the water.
        /// </summary>
        private void WaterAnimation()
        {
            recreateWaterMesh = false;
            updateWaterHeight = false;

            // Updates the top, left and right handles position if waterDisplacement is set to true
            // and waterLineYPosOffset is different from zero (it means there is an object in the water).
            if (waterDisplacement && waterLineYPosOffset != 0)
            {
                // When the water is displaced by an object we don't need to recreate the mesh from scratch,
                // but only update the surface vertices position and the bottom vertices UVs.
                updateWaterHeight = true;
                UpdateTopLeftRightHandles();
            }

            // The default water area is updated based on the difference between the defaultWaterAreaOffset and
            // prevDefaultWaterAreaOffset.
            if (animateWaterArea && Mathf.Abs(defaultWaterAreaOffset - prevDefaultWaterAreaOffset) > precisionFactor)
            {
                defaultWaterArea += defaultWaterAreaOffset - prevDefaultWaterAreaOffset;
                prevDefaultWaterAreaOffset = defaultWaterAreaOffset;

                // Current water width.
                float waterWidth = Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);
                // The default water height is updated based on the new water area.
                defaultWaterHeight = defaultWaterArea / waterWidth;

                UpdateTopLeftRightHandles();
                recreateWaterMesh = true;
            }

            // Updates the position of the top and bottom handles as well as right and left if the referenced objects are not null and
            // their current position changed from the previous frame.

            // The position of the handles is updated by adding to them the difference between the current position
            // of the referenced objects on the Y axis (top, bottom handles), X axis (left, right handles) and the one
            // the referenced objects had on the previous frame. The new handles position is calculated in
            // this way because the position of the referenced objects is global, but the handles values must be local.
            // By doing it this way we don't need to convert from global position to local.
            if (topEdge != null && Mathf.Abs(topEdge.position.y - prevTopEdgeYPos) > precisionFactor)
            {
                if (animationMethod == Water2D_AnimationMethod.Follow)
                {
                    // The default water height is updated based on the difference between the position the
                    // reference object had on the previous frame and the position on the current frame.
                    defaultWaterHeight += topEdge.position.y - prevTopEdgeYPos;
                }
                else
                {
                    defaultWaterHeight = Mathf.Abs(transform.InverseTransformPoint(topEdge.position).y - water2D.handlesPosition[1].y + topEdgeYOffset);
                }
                // Because the default water height has changed, the default water area must be updated too.
                defaultWaterArea = defaultWaterHeight * Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);
                prevTopEdgeYPos = topEdge.transform.position.y;

                // The position on the Y axis of the top, left, right, handles is updated based on the new
                // defaultWaterHeight and waterLineYPosOffset values.
                UpdateTopLeftRightHandles();
                updateWaterHeight = true;
            }

            if (bottomEdge != null && Mathf.Abs(bottomEdge.position.y - prevBottomEdgeYPos) > precisionFactor)
            {
                if (animationMethod == Water2D_AnimationMethod.Follow)
                    water2D.handlesPosition[1] = new Vector2(water2D.handlesPosition[1].x, water2D.handlesPosition[1].y + bottomEdge.position.y - prevBottomEdgeYPos);
                else
                    water2D.handlesPosition[1] = new Vector2(water2D.handlesPosition[1].x, transform.InverseTransformPoint(bottomEdge.position).y + bottomEdgeYOffset);
                UpdateTopLeftRightHandles();
                prevBottomEdgeYPos = bottomEdge.position.y;
                updateWaterHeight = true;
            }

            // Updates the position of the left handler on the X axis.
            if (leftEdge != null && Mathf.Abs(leftEdge.position.x - prevLeftEdgeXPos) > precisionFactor)
            {
                if (animationMethod == Water2D_AnimationMethod.Follow)
                    water2D.handlesPosition[2] = new Vector2(water2D.handlesPosition[2].x + leftEdge.position.x - prevLeftEdgeXPos, water2D.handlesPosition[2].y);
                else
                    water2D.handlesPosition[2] = new Vector2(transform.InverseTransformPoint(leftEdge.position).x + leftEdgeXOffset, water2D.handlesPosition[2].y);

                prevLeftEdgeXPos = leftEdge.transform.position.x;

                float waterWidth = Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);

                // If constantWaterArea is set to true, the default water height must be updated too.
                if (constantWaterArea)
                {
                    defaultWaterHeight = defaultWaterArea / waterWidth;
                    water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[0].x, water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset);
                }

                // The positions on the X axis of the top and bottom handles are updated.
                water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[2].x + waterWidth * 0.5f, water2D.handlesPosition[0].y);
                water2D.handlesPosition[1] = new Vector2(water2D.handlesPosition[2].x + waterWidth * 0.5f, water2D.handlesPosition[1].y);

                // The mesh must be recreated from scratch.
                recreateWaterMesh = true;
            }

            // Updates the position of the right handler on the X axis.
            if (rightEdge != null && Mathf.Abs(rightEdge.position.x - prevRightEdgeXPos) > precisionFactor)
            {
                if (animationMethod == Water2D_AnimationMethod.Follow)
                    water2D.handlesPosition[3] = new Vector2(water2D.handlesPosition[3].x + rightEdge.position.x - prevRightEdgeXPos, water2D.handlesPosition[3].y);
                else
                    water2D.handlesPosition[3] = new Vector2(transform.InverseTransformPoint(rightEdge.position).x + rightEdgeXOffset, water2D.handlesPosition[3].y);

                prevRightEdgeXPos = rightEdge.position.x;

                float waterWidth = Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);

                // If constantWaterArea is set to true, the default water height must be updated too.
                if (constantWaterArea)
                {
                    defaultWaterHeight = defaultWaterArea / waterWidth;
                    water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[0].x, water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset);
                }

                // The positions on the X axis of the top and bottom handles are updated.
                water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[2].x + waterWidth * 0.5f, water2D.handlesPosition[0].y);
                water2D.handlesPosition[1] = new Vector2(water2D.handlesPosition[2].x + waterWidth * 0.5f, water2D.handlesPosition[1].y);

                // The mesh must be recreated from scratch.
                recreateWaterMesh = true;
            }
        }

        /// <summary>
        /// Checks if the water Mesh height must be updated or the mesh must be recreated from scratch.
        /// </summary>
        private void WaterMesh()
        {
            // Recreates the water mesh from scratch based on the new handles positions.
            if (recreateWaterMesh)
            {
                RecreateWaterMesh();
            }

            // Updates the water height based on the new top and bottom handles positions.
            if (updateWaterHeight && !recreateWaterMesh)
            {
                UpdateWaterHeight();
                updateMeshDate = true;
            }
        }

        /// <summary>
        /// Updates the height of the water Mesh and recalculates the UV's for the bottom vertices.
        /// </summary>
        private void UpdateWaterHeight()
        {
            vertYOffsets.Clear();

            // The curent water height.
            float height = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
            Vector3 temp = Vector2.zero;

            for (int i = 0; i < surfaceVertsCount; i++)
            {
                vertYOffsets.Add(frontMeshVertices[surfaceVertsCount + i].y - waterLinePreviousLocalPos);

                float V = height / water2D.unitsPerUV.y;
                // The UVs for the bottom vertices are recalculated to eliminate the texture stretching.

                frontMeshUVs[i] = new Vector2(frontMeshUVs[i].x, tiling && V > 1 ? 0 : 1 - V);
                // The positions on the Y axis of the bottom vertices are reset in case the bottom edge of the water is animated.
                temp.x = frontMeshVertices[i].x;
                temp.y = water2D.handlesPosition[1].y;
                temp.z = 0;

                frontMeshVertices[i] = temp;
                // This updates the surface vertices so that the waves created by objects are not lost.
                temp = frontMeshVertices[surfaceVertsCount + i];
                temp.y = water2D.handlesPosition[0].y + vertYOffsets[i];
                frontMeshVertices[surfaceVertsCount + i] = temp;

                if (water2D.cubeWater)
                {
                    float tan = 0;

                    if (water2D.topMeshYOffset > 0)
                        tan = water2D.topMeshYOffset / water2D.length;

                    for (int j = 0; j < water2D.zSegments + 1; j++)
                    {
                        temp = topMeshVertices[j * surfaceVertsCount + i];
                        temp.y = frontMeshVertices[surfaceVertsCount + i].y;

                        if (water2D.topMeshYOffset > 0 && j > 0)
                            temp.y += tan * water2D.zVertDistance * j;

                        topMeshVertices[j * surfaceVertsCount + i] = temp;
                    }
                }
            }

            if (frontMeshRend.material.HasProperty("_WaterHeight"))
            {
                float waterHeight = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
                frontMeshRend.material.SetFloat("_WaterHeight", waterHeight);
            }

            UpdateCollider();
        }

        private void UpdateCollider()
        {
            float height = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);

            if (!water2D.use3DCollider)
            {
                // Here the size of the BoxCollider2D component is updated.
                BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
                boxCollider2D.size = new Vector2(water2D.width, height + water2D.colliderYAxisOffset);
                Vector2 center = water2D.handlesPosition[1];
                center.y += height / 2f + water2D.colliderYAxisOffset / 2f;
                boxCollider2D.offset = center;
            }
            else
            {
                // Here the size of the BoxCollider2D component is updated.
                BoxCollider boxCollider = GetComponent<BoxCollider>();
                boxCollider.size = new Vector3(water2D.width, height + water2D.colliderYAxisOffset, water2D.length);
                Vector3 center = water2D.handlesPosition[1];
                center.y += height * 0.5f + water2D.colliderYAxisOffset * 0.5f;
                boxCollider.center = center;

                if (water2D.cubeWater)
                    center.z = water2D.length * 0.5f;

                boxCollider.center = center;
                boxCollider.center += water2D.boxColliderCenterOffset;
            }

            waterLinePreviousLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset;
            waterLineCurrentWorldPos = transform.TransformPoint(water2D.handlesPosition[0]);
            waterLineCurrentLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset;
        }

        /// <summary>
        /// Recreates the water Mesh from scratch. This is done if the water width changed.
        /// <summary>
        private void RecreateWaterMesh()
        {
            vertYOffsets.Clear();
            Vector3 temp = Vector3.zero;
            // We store the difference between the current water line position and the surface
            // vertices so that after the mesh is recreated we can update their position based
            // on the new waterline position. If we don't do this after the mesh is recreated
            // the waves created by falling objects will disappear.
            for (int i = 0; i < surfaceVertsCount; i++)
                vertYOffsets.Add(frontMeshVertices[surfaceVertsCount + i].y - waterLinePreviousLocalPos);

            // Recreates the water Mesh from scratch.
            water2D.RecreateWaterMesh();

            // Updates the size of different list variables, if the number of surface vertices changed from the previous frame.
            UpdateVariables();

            int len = vertYOffsets.Count;
            float tan = 0;

            if (water2D.topMeshYOffset > 0)
                tan = water2D.topMeshYOffset / water2D.length;

            // If the surface vertices number is bigger we update their position based on the
            // size of the vertYOffsets list. The new vertices ar left unchanged.
            if (len < surfaceVertsCount || len == surfaceVertsCount)
            {
                for (int i = 0; i < len; i++)
                {
                    temp = frontMeshVertices[surfaceVertsCount + i];
                    temp.y = waterLineCurrentLocalPos + vertYOffsets[i];
                    frontMeshVertices[surfaceVertsCount + i] = temp;

                    if (water2D.cubeWater)
                    {

                        for (int j = 0; j < water2D.zSegments + 1; j++)
                        {
                            temp = topMeshVertices[surfaceVertsCount * j + i];
                            temp.y = frontMeshVertices[surfaceVertsCount + i].y;

                            if (water2D.topMeshYOffset > 0 && j > 0)
                                temp.y += tan * water2D.zVertDistance * j;

                            topMeshVertices[surfaceVertsCount * j + i] = temp;
                        }
                    }
                }

                int vertD = surfaceVertsCount - water2D.prevSurfaceVertsCount;

                for (int i = 0; i < vertD; i++)
                {
                    temp = frontMeshVertices[surfaceVertsCount * 2 - i - 1];
                    temp.y = frontMeshVertices[surfaceVertsCount * 2 - vertD - 1].y;
                    frontMeshVertices[surfaceVertsCount * 2 - i - 1] = temp;

                    if (water2D.cubeWater)
                    {
                        for (int j = 0; j < water2D.zSegments + 1; j++)
                        {
                            temp = topMeshVertices[surfaceVertsCount * j + surfaceVertsCount - i - 1];
                            temp.y = frontMeshVertices[surfaceVertsCount * 2 - vertD - 1].y;

                            if (water2D.topMeshYOffset > 0 && j > 0)
                                temp.y += tan * water2D.zVertDistance * j;

                            topMeshVertices[surfaceVertsCount * j + surfaceVertsCount - i - 1] = temp;
                        }
                    }
                }
            }

            // If the surface vertices number is smaller we update their position based on the
            // size of the surfaceVertsCount list.
            if (len > surfaceVertsCount)
            {
                for (int i = 0; i < surfaceVertsCount; i++)
                {
                    temp = frontMeshVertices[surfaceVertsCount + i];
                    temp.y = waterLineCurrentLocalPos + vertYOffsets[i];
                    frontMeshVertices[surfaceVertsCount + i] = temp;

                    if (water2D.cubeWater)
                    {
                        for (int j = 0; j < water2D.zSegments + 1; j++)
                        {
                            temp = topMeshVertices[surfaceVertsCount * j + i];
                            temp.y = frontMeshVertices[surfaceVertsCount + i].y;

                            if (water2D.topMeshYOffset > 0 && j > 0)
                                temp.y += tan * water2D.zVertDistance * j;

                            topMeshVertices[surfaceVertsCount * j + i] = temp;
                        }
                    }
                }
            }

            if (frontMeshRend.material.HasProperty("_WaterHeight"))
            {
                float height = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
                float width = Mathf.Abs(water2D.handlesPosition[2].y - water2D.handlesPosition[3].y);

                frontMeshRend.material.SetFloat("_WaterHeight", height);
                frontMeshRend.material.SetFloat("_WaterWidth", width);
            }

            CreateWaterLinePoints();
        }

        /// <summary>
        /// Creates a list of points that wil be used to define a clipping polygon.
        /// <summary>
        private void CreateWaterLinePoints()
        {
            waterLinePoints.Clear();

            if (surfaceVertsCount > meshSegmentsPerWaterLineSegment)
            {
                int n = 0;
                while (n < surfaceVertsCount)
                {
                    waterLinePoints.Add(transform.TransformPoint(frontMeshVertices[surfaceVertsCount + n]));
                    n += meshSegmentsPerWaterLineSegment;
                }

                if (meshSegmentsPerWaterLineSegment != 1)
                {
                    Vector2 lastVert = transform.TransformPoint(frontMeshVertices[surfaceVertsCount + surfaceVertsCount - 1]);
                    waterLinePoints.Add(new Vector2(lastVert.x, lastVert.y));
                }
            }
            else
            {
                waterLinePoints.Add(transform.TransformPoint(frontMeshVertices[surfaceVertsCount]));
                Vector2 lastVert = transform.TransformPoint(frontMeshVertices[surfaceVertsCount + surfaceVertsCount - 1]);
                waterLinePoints.Add(new Vector2(lastVert.x, lastVert.y));
            }
        }

        /// <summary>
        /// Updates the position of the Top, Right and Left handles.
        /// </summary>
        private void UpdateTopLeftRightHandles()
        {
            water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[0].x, water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset);
            water2D.handlesPosition[2] = new Vector2(water2D.handlesPosition[2].x, water2D.handlesPosition[1].y + defaultWaterHeight * 0.5f + waterLineYPosOffset * 0.5f);
            water2D.handlesPosition[3] = new Vector2(water2D.handlesPosition[3].x, water2D.handlesPosition[1].y + defaultWaterHeight * 0.5f + waterLineYPosOffset * 0.5f);
        }

        /// <summary>
        /// Simulates water waves generated by falling objects.
        /// </summary>
        private void WaterWaves()
        {
            leftHandleGlobalPos = transform.TransformPoint(water2D.handlesPosition[2]);

            // If the water is not dynamic and there are no objects in the water we do not need to update the velocity of the surface vertices.
            int objCount = floatingObjects.Count;

            if (waterType == Water2D_Type.Dynamic && objCount > 0)
            {
                for (int i = 0; i < objCount; i++)
                    GenerateWaterWaves(i);
            }


            if (surfaceWaves == Water2D_SurfaceWaves.None)
                interactionTimeCount += Time.deltaTime;

            if (interactionTimeCount < interactionTime)
                UpdateVertsPosition();

            if (clippingMethod == Water2D_ClippingMethod.Complex)
                UpdateWaterLinePoints();
        }

        /// <summary>
        /// Updates the position of the water line points. This are the points
        /// that will be used to define a clipping polygon.
        /// </summary>
        private void UpdateWaterLinePoints()
        {
            int n = 0;
            Vector2 vertWorldPos = Vector2.zero;
            int len = waterLinePoints.Count;

            for (int i = 0; i < len - 1; i++)
            {
                vertWorldPos = transform.TransformPoint(frontMeshVertices[surfaceVertsCount + n]);
                waterLinePoints[i] = vertWorldPos;
                n += meshSegmentsPerWaterLineSegment;
            }

            vertWorldPos = transform.TransformPoint(frontMeshVertices[surfaceVertsCount + surfaceVertsCount - 1]);
            waterLinePoints[len - 1] = vertWorldPos;
#if UNITY_EDITOR
            if (showClippingPlolygon)
            {
                for (int j = 0; j < waterLinePoints.Count - 1; j++)
                {
                    Debug.DrawLine(waterLinePoints[j], waterLinePoints[j + 1], Color.cyan);
                }
            }
#endif
        }


        /// <summary>
        /// Generates a surface wave at a given position.
        /// </summary>
        /// <param name="worldPos">World position of a collider.</param>
        /// <param name="width">The width of colliders bounding box.</param>
        /// <param name="minIndex">The index of the surface vertex that is closest to the left edge of the bounding box of a collider.</param>
        /// <param name="maxIndex">The index of the surface vertex that is closest to the right edge of the bounding box of a collider.</param>
        private void GetVertMinMaxIndex(Vector3 worldPos, float width, out int minIndex, out int maxIndex)
        {
            // The distance from the left handle to the center of the wave on the X axis.
            float distance = Mathf.Abs(leftHandleGlobalPos.x - worldPos.x);
            // The current water width.
            float waterWidth = Mathf.Abs(water2D.handlesPosition[3].x - water2D.handlesPosition[2].x);
            // The distance from the left handle to the left side of the wave.
            float minX = distance - width * 0.5f;
            // The distance from the left handle to the right side of the wave.
            float maxX = distance + width * 0.5f;

            if (minX < 0)
                minX = 0;
            if (maxX > waterWidth)
                maxX = waterWidth;

            // The index of the surface vertex that is closest to the left edge of the bounding box of a collider.
            minIndex = (int)Mathf.Floor(minX * water2D.segmentsPerUnit);
            // The index of the surface vertex that is closest to the right edge of the bounding box of a collider.
            maxIndex = (int)Mathf.Floor(maxX * water2D.segmentsPerUnit);

            // We make sure that we don't get out of bounds indexes.
            if (maxIndex > surfaceVertsCount - 1)
                maxIndex = surfaceVertsCount - 1;
            if (minIndex < 0)
                minIndex = 0;
        }

        /// <summary>
        /// Generates a surface wave at a given position.
        /// </summary>
        /// <param name="rippleWorldPos">World position of where the center of the new surface wave will be.</param>
        /// <param name="rippleWidth">The width of the wave.</param>
        /// <param name="vertexVelocity">The velocity a wate line vertex will receive.</param>
        public void GenerateRippleAtPosition(Vector3 rippleWorldPos, float rippleWidth, float vertexVelocity, bool psAndSound)
        {
            int minIndex, maxIndex;

            GetVertMinMaxIndex(rippleWorldPos, rippleWidth, out minIndex, out maxIndex);

            for (int i = minIndex; i <= maxIndex; i++)
                velocities[i] = vertexVelocity;

            if (psAndSound)
            {
                if (particleS != null)
                {
                    // Instantiates a new particle system from a prefab object.
                    GameObject splash;
                    splash = Instantiate(particleS, new Vector3(rippleWorldPos.x + particleSystemPosOffset.x, waterLineCurrentWorldPos.y + particleSystemPosOffset.y, rippleWorldPos.z + particleSystemPosOffset.z), Quaternion.Euler(new Vector3(270f, 0, 0))) as GameObject;

                    if (particleSystemSorting)
                    {
                        // The sorting layer of the particle system is set to the same as the waters.
                        splash.GetComponent<Renderer>().sortingLayerName = particleSystemSortingLayerName;
                        splash.GetComponent<Renderer>().sortingOrder = particleSystemOrderInLayer;
                    }

                    // Makes the water parent of the particle system.
                    splash.transform.parent = transform;
                    // The particle system will be destroyed a second after it was instantiated.
                    Destroy(splash, 1f);
                }

                // Generates a splash sound.
                if (splashSound != null)
                    AudioSource.PlayClipAtPoint(splashSound, rippleWorldPos);
            }
        }

        /// <summary>
        /// Simulates water waves generated by falling objects.
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        private void GenerateWaterWaves(int oIndex)
        {
            if (!CanGenerateRipples(oIndex))
                return;

            int minIndex, maxIndex;

            GetVertMinMaxIndex(floatingObjects[oIndex].bounds.center, floatingObjects[oIndex].bounds.extents.x * 2f, out minIndex, out maxIndex);

            if (collisionDetectionMode == Water2D_CollisionDetectionMode.RaycastBased)
                GenerateRippleBasedOnRaycast(oIndex, minIndex, maxIndex);
            else
                GenerateRippleBasedOnPosition(oIndex, minIndex, maxIndex);
        }


        /// <summary>
        /// Finds the vertices that are inside a collider and updates their velocities. Generates Particles system and sound effect. 
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        /// <param name="minIndex">The index of the surface vertex that is closest to the left edge of the bounding box of a collider.</param>
        /// <param name="maxIndex">The index of the surface vertex that is closest to the right edge of the bounding box of a collider.</param>
        private void GenerateRippleBasedOnRaycast(int oIndex, int minIndex, int maxIndex)
        {
            for (int i = minIndex + 1; i <= maxIndex; i++)
            {
                int len;

                Vector2 vertex1 = frontMeshVertices[i + surfaceVertsCount];
                vertex1 = transform.TransformPoint(vertex1);

                if (!water2D.use3DCollider)
                {
                    vertex1.x -= raycastDistance;
                    len = Physics2D.RaycastNonAlloc(vertex1, new Vector2(1, 0), hit2D, 0.1f, collisionLayers.value);
                }
                else
                {
                    len = Physics.OverlapSphereNonAlloc(new Vector3(vertex1.x, vertex1.y, transform.position.z + water2D.overlapingSphereZOffset), overlapSphereRadius, hit3D, collisionLayers.value);
                }

                for (int j = 0; j < len; j++)
                {
                    if (!water2D.use3DCollider && floatingObjects[oIndex].transform != (hit2D[j].transform))
                        continue;
                    if (water2D.use3DCollider && floatingObjects[oIndex].transform != (hit3D[j].transform))
                        continue;

                    Vector3 objectVelocity = new Vector3(0, 0, 0);
                    objectVelocity = floatingObjects[oIndex].GetVelocity();
                    velocities[i] = objectVelocity.y * collisionVelocityScale;
                    interactionTimeCount = 0;
                    // If on the previous frame in this segment an object was not detected a new particle system will be instantiated.
                    if (particleS != null && objectVelocity.y < velocityFilter - 2f && !floatingObjects[oIndex].HasInstantiatedParticleSystem())
                    {
                        // Instantiates a new particle system from a prefab object.
                        Vector3 pos;

                        if (!water2D.use3DCollider)
                            pos = hit2D[j].transform.position;
                        else
                            pos = hit3D[j].transform.position;

                        InstantiateParticleSystem(oIndex, vertex1.y, pos);
                    }

                    if (splashSound != null && !floatingObjects[oIndex].HasPlayedSoundEffect())
                        PlaySoundEffect(vertex1, oIndex);

                    break;
                }
            }
        }

        /// <summary>
        /// Finds the vertices that are inside a colliders bounding box and updates their velocities. Generates Particles system and sound effect. 
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        /// <param name="minIndex">The index of the surface vertex that is closest to the left edge of the bounding box of a collider.</param>
        /// <param name="maxIndex">The index of the surface vertex that is closest to the right edge of the bounding box of a collider.</param>
        private void GenerateRippleBasedOnPosition(int oIndex, int minIndex, int maxIndex)
        {
            Vector3 objectVelocity = floatingObjects[oIndex].GetVelocity();
            interactionTimeCount = 0;
            for (int i = minIndex + 1; i <= maxIndex; i++)
                velocities[i] = objectVelocity.y * collisionVelocityScale;

            if (splashSound != null && !floatingObjects[oIndex].HasPlayedSoundEffect())
                PlaySoundEffect(floatingObjects[oIndex].transform.position, oIndex);

            if (particleS != null && objectVelocity.y < velocityFilter - 2f && !floatingObjects[oIndex].HasInstantiatedParticleSystem())
                InstantiateParticleSystem(oIndex, waterLineCurrentWorldPos.y, floatingObjects[oIndex].transform.position);
        }

        /// <summary>
        /// Instantiates a new particle system. 
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        /// <param name="yAxisPos">Position on the Y axis where the particle system will be instantiated.</param>
        /// <param name="pos">Position of the object that triggered this particle system.</param>
        private void InstantiateParticleSystem(int oIndex, float yAxisPos, Vector3 pos)
        {
            GameObject splash;

            splash = Instantiate(particleS, new Vector3(
            pos.x + particleSystemPosOffset.x,
            yAxisPos + particleSystemPosOffset.y,
            pos.z + particleSystemPosOffset.z), Quaternion.Euler(new Vector3(270f, 0, 0))) as GameObject;

            if (particleSystemSorting)
            {
                // The sorting layer of the particle system is set to the same as the waters.
                splash.GetComponent<Renderer>().sortingLayerName = particleSystemSortingLayerName;
                splash.GetComponent<Renderer>().sortingOrder = particleSystemOrderInLayer;
            }

            // Makes the water parent of the particle system.
            splash.transform.parent = transform;

            // The particle system will be destroyed a second after it was instantiated.
            Destroy(splash, 1f);
            floatingObjects[oIndex].SetParticleSystemInstantiated(true);
        }

        /// <summary>
        /// Playes a sound effect. 
        /// </summary>
        /// <param name="pos">Position of the object that triggered this sound effect.</param>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        private void PlaySoundEffect(Vector3 pos, int oIndex)
        {
            AudioSource.PlayClipAtPoint(splashSound, pos);
            floatingObjects[oIndex].SetSoundPlayed(true);
        }

        /// <summary>
        /// Checks if an object meets all the necessary requirements for him to be able to interact with the surface of the water.. 
        /// </summary> object
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        private bool CanGenerateRipples(int oIndex)
        {
            Vector2 collBoundsSize;
            float distanceFromWaterLine;

            if (!ColliderExists(oIndex))
                return false;
            else
            {
                collBoundsSize = floatingObjects[oIndex].bounds.size;
                distanceFromWaterLine = collBoundsSize.y * 0.5f - Mathf.Abs(waterLineCurrentWorldPos.y - floatingObjects[oIndex].bounds.center.y);
            }

            if (floatingObjects[oIndex].bounds.center.y < waterLineCurrentWorldPos.y)
                return false;
            else if (distanceFromWaterLine > (interactionRegion > collBoundsSize.y ? collBoundsSize.y : interactionRegion))
                return false;
            else if (!(floatingObjects[oIndex].GetVelocity().y < velocityFilter))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Updates the position of the surface vertices.
        /// </summary>
        /// <remarks>
        /// Based on:
        /// http://gamedevelopment.tutsplus.com/tutorials/make-a-splash-with-2d-water-effects--gamedev-236
        /// </remarks>
        private void UpdateVertsPosition()
        {
            Vector3 temp = Vector3.zero;

            // Applies damping, velocity, and acceleration.
            for (int i = 0; i < surfaceVertsCount; i++)
            {
                float yDisplacement = frontMeshVertices[surfaceVertsCount + i].y - waterLineCurrentLocalPos;
                accelerations[i] = -springConstant * yDisplacement - velocities[i] * damping;
                velocities[i] += accelerations[i];
                yDisplacement += velocities[i];
                temp = frontMeshVertices[surfaceVertsCount + i];
                temp.y = yDisplacement + waterLineCurrentLocalPos;
                frontMeshVertices[surfaceVertsCount + i] = temp;
            }

            // Applies wave motion.
            for (int i = 0; i < surfaceVertsCount; i++)
            {
                float y = frontMeshVertices[surfaceVertsCount + i].y;
                if (i > 0)
                {
                    float leftVertY = frontMeshVertices[surfaceVertsCount + i - 1].y;
                    leftDeltas[i] = spread * (y - leftVertY);
                    velocities[i - 1] += leftDeltas[i] * waveSpeed;
                }

                if (i < surfaceVertsCount - 1)
                {
                    float rightVertY = frontMeshVertices[surfaceVertsCount + i + 1].y;
                    rightDeltas[i] = spread * (y - rightVertY);
                    velocities[i + 1] += rightDeltas[i] * waveSpeed;
                }
            }

            // Updates the neighbour vertices.
            for (int i = 0; i < surfaceVertsCount; i++)
            {
                if (i > 0)
                {
                    temp = frontMeshVertices[surfaceVertsCount + i - 1];
                    temp.y += leftDeltas[i];
                    frontMeshVertices[surfaceVertsCount + i - 1] = temp;
                }

                if (i < surfaceVertsCount - 1)
                {
                    temp = frontMeshVertices[surfaceVertsCount + i + 1];
                    temp.y += rightDeltas[i];
                    frontMeshVertices[surfaceVertsCount + i + 1] = temp;
                }
            }

            if (water2D.cubeWater)
            {
                float tan = 0;

                if (water2D.topMeshYOffset > 0)
                    tan = water2D.topMeshYOffset / water2D.length;

                for (int k = 0; k < water2D.zSegments + 1; k++)
                {
                    for (int i = 0; i < surfaceVertsCount; i++)
                    {
                        temp = topMeshVertices[k * surfaceVertsCount + i];
                        temp.y = frontMeshVertices[surfaceVertsCount + i].y;

                        if (water2D.topMeshYOffset > 0 && k > 0)
                            temp.y += tan * water2D.zVertDistance * k;

                        topMeshVertices[k * surfaceVertsCount + i] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        private void PhysicsBasedBuoyantForce()
        {
            // The global position of the left handle.
            leftHandleGlobalPos = transform.TransformPoint(water2D.handlesPosition[2]);

            int len = floatingObjects.Count;
            for (int i = 0; i < len; i++)
            {
                ApplyPhysicsBasedBuoyantForce(i);
            }
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        private void ApplyPhysicsBasedBuoyantForce(int oIndex)
        {
            if (!ColliderExists(oIndex)) return;

            // The global position of the closest vertex to the center of the collider.
            Vector3 vertWorldPos;
            float distance = Mathf.Abs(leftHandleGlobalPos.x - floatingObjects[oIndex].bounds.center.x);
            bool isIntersecting = true;

            // The index of the surface vertex that is closest to the colliders bounding box center.
            int index = (int)Mathf.Floor(distance * water2D.segmentsPerUnit);

            // We make sure that we don't get out of bounds indexes.
            if (index > surfaceVertsCount - 1)
                index = surfaceVertsCount - 1;

            if (water2DRipple != null)
                vertWorldPos = waterLineCurrentWorldPos;
            else
                vertWorldPos = transform.TransformPoint(frontMeshVertices[index + surfaceVertsCount]);

            area = 0;
            displacedMass = 0;
            // The segment of the polygon that is below the waterline.
            List<Vector2> intersectionPolygon;

            if (floatingObjects[oIndex].bounds.min.y < vertWorldPos.y)
            {
                if (clippingMethod == Water2D_ClippingMethod.Simple)
                {
                    linePoints[0] = new Vector2(water2D.handlesPosition[2].x, vertWorldPos.y);
                    linePoints[1] = new Vector2(water2D.handlesPosition[3].x, vertWorldPos.y);
                }

                if (floatingObjects[oIndex].IsPlayer())
                {
                    // The global position of an imaginary bounding box.
                    boxVertices = GetPlayerBoudingBoxVerticesGlobalPos(oIndex);

                    // If the bottom of the bounding is above the vertex, a buoyant force should not be applied to the object.
                    if (boxVertices[1].y > vertWorldPos.y)
                        return;

                    intersectionPolygon = polygonClipping.GetIntersectedPolygon(boxVertices, linePoints, out isIntersecting);
                    ApplyPhysicsForces(oIndex, intersectionPolygon);

                    // This will make sure the buoyant force is applied only once.
                    // The other colliders for this game object will be ignored.
                    return;
                }

                var vertices = floatingObjects[oIndex].GetPolygon();
                ApplyForcesToObject(vertices, linePoints, oIndex);

            }
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        /// <param name="subjPoly"> The polygon of the object in the water.</param>
        /// <param name="clipPoly"> The clipping polygon.</param>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list object.</param>
        private void ApplyForcesToObject(List<Vector2> subjPoly, Vector2[] clipPoly, int oIndex)
        {
            intersectionPolygon.Clear();
            intersectionPolygons.Clear();
            bool isIntersecting = true;

            if (clippingMethod == Water2D_ClippingMethod.Simple)
            {
                intersectionPolygon = polygonClipping.GetIntersectedPolygon(subjPoly, clipPoly, out isIntersecting);

                if (isIntersecting)
                    ApplyPhysicsForces(oIndex, intersectionPolygon);
            }
            else
            {
                int minIndex = 0;
                int maxIndex = 0;

                GetVertMinMaxIndex(floatingObjects[oIndex].bounds.center, floatingObjects[oIndex].bounds.extents.x * 2f, out minIndex, out maxIndex);

                CalculateIntersectionPolygons(subjPoly, minIndex, maxIndex, out isIntersecting);

                if (isIntersecting)
                {
                    int len = intersectionPolygons.Count;
                    for (int i = 0; i < len; i++)
                    {
                        intersectionPolygon = intersectionPolygons[i];
                        ApplyPhysicsForces(oIndex, intersectionPolygon);
                    }
                }
            }
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        /// <param name="subjectPoly"> The polygon of the object in the water.</param>
        /// <param name="minIndex"> The min index for a value in the "waterLinePoints" list. </param>
        /// <param name="maxIndex"> The max index for a value in the "waterLinePoints" list. </param>
        /// <param name="isIntersecting"> Are the subject and clipping polygon intersecting?. </param>
        private void CalculateIntersectionPolygons(List<Vector2> subjectPoly, int minIndex, int maxIndex, out bool isIntersecting)
        {
            Vector2 bottomHandleGlobalPos = transform.TransformPoint(water2D.handlesPosition[1]);
            clipPolygon.Clear();
            subjPath.Clear();
            clipPath.Clear();
            clipper.Clear();
            int len, len2, min, max;
            isIntersecting = true;

            if (surfaceVertsCount > meshSegmentsPerWaterLineSegment)
            {
                min = (int)Mathf.Floor(minIndex / meshSegmentsPerWaterLineSegment);
                max = (int)Mathf.Floor(maxIndex / meshSegmentsPerWaterLineSegment) + 1;

                if (max > waterLinePoints.Count - 2)
                    max = waterLinePoints.Count - 2;

                for (int i = min; i <= max; i++)
                {
                    clipPolygon.Add(waterLinePoints[i]);
                }

                int last = clipPolygon.Count - 1;
                clipPolygon.Add(new Vector2(clipPolygon[last].x, bottomHandleGlobalPos.y));
                clipPolygon.Add(new Vector2(clipPolygon[0].x, bottomHandleGlobalPos.y));
            }
            else
            {
                Vector2 vertGlobalPos = transform.TransformPoint(frontMeshVertices[surfaceVertsCount]);
                clipPolygon.Add(vertGlobalPos);
                vertGlobalPos = transform.TransformPoint(frontMeshVertices[surfaceVertsCount + surfaceVertsCount - 1]);
                clipPolygon.Add(new Vector2(vertGlobalPos.x, vertGlobalPos.y));

                int last = clipPolygon.Count - 1;
                clipPolygon.Add(new Vector2(clipPolygon[last].x, bottomHandleGlobalPos.y));
                clipPolygon.Add(new Vector2(clipPolygon[0].x, bottomHandleGlobalPos.y));
            }

            if (showClippingPlolygon)
            {
                for (int i = 0; i < clipPolygon.Count; i++)
                {
                    if (i < clipPolygon.Count - 1)
                        Debug.DrawLine(clipPolygon[i], clipPolygon[i + 1], Color.green);
                    else
                        Debug.DrawLine(clipPolygon[i], clipPolygon[0], Color.green);
                }
            }

            len = subjectPoly.Count;
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
                        list.Add(new Vector2(solutionPath[i][j].X * rScaleFactor, solutionPath[i][j].Y * rScaleFactor));
                    }

                    intersectionPolygons.Add(list);
                }
            }
            else
            {
                isIntersecting = false;
            }
        }

        /// <summary>
        /// Calculates the world position for the players bounding box vertices.
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        /// <returns>Returns the world position of a bounding box vertices.</returns>
        public List<Vector2> GetPlayerBoudingBoxVerticesGlobalPos(int oIndex)
        {
            // The current global position of the object.
            Vector3 objPos = floatingObjects[oIndex].transform.position;

            // Top left vertex.
            boxVertices[0] = new Vector2(objPos.x - (playerBoundingBoxSize.x * 0.5f) + playerBoundingBoxCenter.x, objPos.y + (playerBoundingBoxSize.y * 0.5f) + playerBoundingBoxCenter.y);
            // Bottom left vertex.
            boxVertices[1] = new Vector2(objPos.x - (playerBoundingBoxSize.x * 0.5f) + playerBoundingBoxCenter.x, objPos.y - (playerBoundingBoxSize.y * 0.5f) + playerBoundingBoxCenter.y);
            // Bottom right vertex.
            boxVertices[2] = new Vector2(objPos.x + (playerBoundingBoxSize.x * 0.5f) + playerBoundingBoxCenter.x, objPos.y - (playerBoundingBoxSize.y * 0.5f) + playerBoundingBoxCenter.y);
            // Top right vertex.
            boxVertices[3] = new Vector2(objPos.x + (playerBoundingBoxSize.x * 0.5f) + playerBoundingBoxCenter.x, objPos.y + (playerBoundingBoxSize.y * 0.5f) + playerBoundingBoxCenter.y);

            return boxVertices;
        }

        /// <summary>
        /// Performs the cross product on a vector and a scalar.
        /// </summary>
        /// <param name="a">A scalar value.</param>
        /// <param name="b">A 2D vector.</param>
        /// <returns>Returns new 2D vector.</returns>
        private Vector2 Cross(float a, Vector2 b)
        {
            return new Vector2(a * b.y, -a * b.x);
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        /// <param name="intersectionPolygon">List of a polygon vertices.</param>
        /// <remarks>
        /// Based on:
        /// http://www.iforce2d.net/b2dtut/buoyancy
        /// </remarks>
        private void ApplyPhysicsForces(int oIndex, List<Vector2> intersectionPolygon)
        {
            if (!ColliderExists(oIndex)) return;

            int len = intersectionPolygon.Count;

            forcePosition = GetPolygonAreaAndCentroid(intersectionPolygon, out area);
            displacedMass = area * waterDensity;

            // The buoyant force direction is always opposite of the direction of the gravity force.
            if(floatingObjects[oIndex].Is2DCollider())
                upLift = -Physics2D.gravity * displacedMass;
            else
                upLift = -Physics.gravity * displacedMass;

            if (floatingObjects[oIndex].IsPlayer())
                upLift *= playerBuoyantForceScale;

            floatingObjects[oIndex].AddForceAtPosition(upLift, forcePosition);

            //// Uncomment this lines to see where the buoyant force is applied.
            //if (showForces)
            //{
            //    // The buoyant force.
            //    Debug.DrawRay(forcePosition, upLift.normalized, Color.grey);
            //}

            // Here a drag and lift is applied to the object.
            // To simulate a realistic drag and lift we must find the leading edges of the polygon.
            // These are the edges that resist the movement of an object in the water.
            for (int k = 0; k < len; k++)
            {
                // 2 vertices that form a polygon edge.
                Vector2 vertex0 = intersectionPolygon[k];
                Vector2 vertex1 = intersectionPolygon[(k + 1) % len];

                // A point in the middle of the line created by the 2 vertices.
                Vector2 midPoint = 0.5f * (vertex0 + vertex1);

                // The velocity direction at the midPoint position for the current object.
                Vector3 velDir = floatingObjects[oIndex].GetPointVelocity(midPoint);

                // The magnitude of the velocity.
                float vel = velDir.magnitude;

                // We normalize the velocity direction vector so that we can use it in the future calculations.
                // After normalization the vector velDir will have the same direction, but it's magnitude will be 1.
                velDir.Normalize();

                // The difference between the two vertices. This is the vector that describes the edge
                // formed by the two vertices.
                Vector2 edge = vertex1 - vertex0;
                // The lenght of the edge vector.
                float edgeLength = edge.magnitude;
                // Edge vector normalization.
                edge.Normalize();

                // Performing the cross product between the value of -1 and the normalized edge vector we get its normal.
                Vector2 normal = Cross(-1, edge);

                // The dot product between the normal of the normalized edge vector and the normalized velocity
                // vector will tell us if the curent edge is a leading edge.
                float dragDot = Vector2.Dot(normal, velDir);

                if (waterFlow)
                {
                    float angleInRad = 0;

                    if (useAngles)
                    {
                        angleInRad = flowAngle * Mathf.Deg2Rad;
                    }
                    else
                    {
                        switch (flowDirection)
                        {
                            case Water2D_FlowDirection.Up:
                                angleInRad = 270f * Mathf.Deg2Rad;
                                break;

                            case Water2D_FlowDirection.Down:
                                angleInRad = 90f * Mathf.Deg2Rad;
                                break;

                            case Water2D_FlowDirection.Left:
                                angleInRad = 0 * Mathf.Deg2Rad;
                                break;

                            case Water2D_FlowDirection.Right:
                                angleInRad = 180f * Mathf.Deg2Rad;
                                break;
                        }
                    }

                    Vector2 forceDir = new Vector2(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad));

                    float flowDot = Vector2.Dot(normal, forceDir);

                    if (flowDot < 0)
                    {
                        float flowMag = flowDot * edgeLength * waterFlowForce;
                        Vector2 force = flowMag * forceDir;

                        floatingObjects[oIndex].AddForceAtPosition(force, midPoint);
#if UNITY_EDITOR
                        if (showForces)
                        {
                            // The velocity direction.
                            Debug.DrawRay(midPoint, forceDir, Color.green);
                            // The normal of a polygon edge.
                            Debug.DrawRay(midPoint, normal, Color.white);
                        }
#endif
                    }
                }

                // If the dragDot is greater then 0 than the normal of the edge vector has the same direction as the
                // velDir vector so it is not a leading edge.
                if (dragDot > 0)
                    continue;

                // The magnitude of the drag for the current edge.
                float dragMag = dragDot * dragCoefficient * edgeLength * waterDensity * vel * vel;
                // If the dragMag is greater then the max drag that should be applied to an object, the
                // value of dragMag is set to maxDrag value.
                dragMag = Mathf.Min(dragMag, maxDrag);
                // We multiply the dragMag with the normalized velocity direction vector (velDir) to get the drag
                // force that should be applied to the current edge.
                Vector2 dragForce = dragMag * velDir;

                // The drag force is applied at midPoint position.
                floatingObjects[oIndex].AddForceAtPosition(dragForce, midPoint);

                // Now we apply lift to the current edge.
                // Lift is the force applied perpendicular to the direction of movement.

                float liftDot = Vector2.Dot(edge, velDir);
                // The magnitude of the lift for the current edge.
                float liftMag = dragDot * liftDot * liftCoefficient * edgeLength * waterDensity * vel * vel;
                liftMag = Mathf.Min(liftMag, maxLift);
                // We perform the cross product between the value of 1 and the normalized velocity
                // vector to get a vector perpendicular to the velocity vector. That is the lift direction vector.
                Vector2 liftDir = Cross(1, velDir);
                // We multiply the liftMag with the normalized velocity direction vector to get the lift force that should be applied to the current edge.
                Vector2 liftForce = liftMag * liftDir;

                // The lift force is applied at midPoint position.
                floatingObjects[oIndex].AddForceAtPosition(liftForce, midPoint);
#if UNITY_EDITOR
                // Shows in the Scene View the direction of the forces that make an object float in the water.
                if (showForces)
                {
                    // The velocity direction.
                    Debug.DrawRay(midPoint, velDir, Color.green);
                    // The Lift direction.
                    Debug.DrawRay(midPoint, liftDir, Color.blue);
                    // The drag direction.
                    Debug.DrawRay(midPoint, -velDir, Color.red);
                    // The normal of a polygon edge.
                    Debug.DrawRay(midPoint, normal, Color.white);
                }
#endif
            }

#if UNITY_EDITOR
            // Shows the shape of the polygon that is below the waterline.
            if (showPolygon && len > 2)
            {
                Vector3 start, end;
                // Draws lines for all polygon edges.
                for (int j = 0; j < len; j++)
                {
                    if (j < len - 1)
                    {
                        start = new Vector3(intersectionPolygon[j].x, intersectionPolygon[j].y, floatingObjects[oIndex].transform.position.z);
                        end = new Vector3(intersectionPolygon[j + 1].x, intersectionPolygon[j + 1].y, floatingObjects[oIndex].transform.position.z);
                    }
                    else
                    {
                        start = new Vector3(intersectionPolygon[j].x, intersectionPolygon[j].y, floatingObjects[oIndex].transform.position.z);
                        end = new Vector3(intersectionPolygon[0].x, intersectionPolygon[0].y, floatingObjects[oIndex].transform.position.z);
                    }
                    Debug.DrawLine(start, end, Color.red);
                }
            }
#endif
        }

        /// <summary>
        /// Calculates the area of a polygon and its centroid. The vertices order must be counterclockwise.
        /// </summary>
        /// <param name="polygonVertices">The polygon for which to calculate the area and the centroid.</param>
        /// <param name="polygonArea">Link to a variable.</param>
        /// <returns>Returns the polygon centroid.</returns>
        public Vector2 GetPolygonAreaAndCentroid(List<Vector2> polygonVertices, out float polygonArea)
        {
            int count = polygonVertices.Count;
            Vector2 centroidPos = new Vector2(0, 0);
            polygonArea = 0;

            for (int i = 0; i < count; i++)
            {
                Vector2 vert1 = polygonVertices[i];
                Vector2 vert2 = i + 1 < count ? polygonVertices[i + 1] : polygonVertices[0];

                float rArea = (vert1.x * vert2.y) - (vert1.y * vert2.x);
                float triangleArea = 0.5f * rArea;

                polygonArea += triangleArea;

                float cX = (vert1.x + vert2.x) * rArea;
                float cY = (vert1.y + vert2.y) * rArea;

                centroidPos += new Vector2(cX, cY);
            }

            centroidPos *= (1f / (6f * polygonArea));

            if (polygonArea < 0)
                polygonArea = 0;

            return centroidPos;
        }

        /// <summary>
        /// Calculates the area of a polygon. The vertices order must be counterclockwise.
        /// </summary>
        /// <param name="polygonVertices">Polygon vertices.</param>
        /// <returns>Returns the polygon area.</returns>
        public float GetPolygonArea(List<Vector2> polygonVertices)
        {
            int count = polygonVertices.Count;
            float polygonArea = 0;

            for (int i = 0; i < count; i++)
            {
                Vector2 vert1 = polygonVertices[i];
                Vector2 vert2 = i + 1 < count ? polygonVertices[i + 1] : polygonVertices[0];

                float rArea = (vert1.x * vert2.y) - (vert1.y * vert2.x);
                float triangleArea = 0.5f * rArea;

                polygonArea += triangleArea;
            }

            if (polygonArea < 0)
                polygonArea = 0;

            return polygonArea;
        }

        /// <summary>
        /// Applies an upward force, a simple drag and torque on the objects that are in the water.
        /// </summary>
        private void LinearBuoyantForce()
        {
            // The global position of the left handle.
            leftHandleGlobalPos = transform.TransformPoint(water2D.handlesPosition[2]);

            int len = floatingObjects.Count;
            for (int i = 0; i < len; i++)
            {
                ApplyLinearBuoyantForce(i);
            }
        }

        /// <summary>
        /// Applies an upward force, a simple drag and torque on the objects that are in the water.
        /// </summary>
        /// <param name="oIndex">The index of an object stored in the floatingObjects list.</param>
        /// <remarks>
        /// Based on:
        /// https://www.youtube.com/watch?v=mDtnT5fh7Ek
        /// </remarks>
        private void ApplyLinearBuoyantForce(int oIndex)
        {
            if (!ColliderExists(oIndex)) return;

            // The global position of the closest vertex to the center of the the object.
            Vector3 globalPosOfVert;

            forcePosition = floatingObjects[oIndex].bounds.center + floatingObjects[oIndex].transform.TransformDirection(forcePositionOffset);

            // The distance from the left handle to the center of the colider.
            float distance = Mathf.Abs(leftHandleGlobalPos.x - floatingObjects[oIndex].bounds.center.x);

            // The index of the surface vertex that is closest to the colliders bounding box center.
            int index = (int)Mathf.Floor(distance * water2D.segmentsPerUnit);

            // We make sure that we don't get out of bounds indexes.
            if (index > surfaceVertsCount - 1)
                index = surfaceVertsCount - 1;

            globalPosOfVert = transform.TransformPoint(frontMeshVertices[index + surfaceVertsCount]);

            // The idea here is that we want the object to be in a state of equilibrium at the surface of the water.
            // To achieve this, for an object with the mass of 1kg we must apply a force equal to the force of gravity,
            // but in the opposite direction. Applying a constant force will not produce a realistic simulation.
            // so we apply a force based on the position of the object relative the global position of the vertex.
            forceFactor = 1f - ((forcePosition.y - globalPosOfVert.y) / floatHeight);

            // A negative force factor will push the object downwards, something we don't want to happen as gravity does that for us.
            if ((forceFactor > 0f) && (floatingObjects[oIndex].bounds.min.y < globalPosOfVert.y))
            {
                // This is the linear buoyant force that is applied to an object.
                if(floatingObjects[oIndex].Is2DCollider())
                    upLift = -Physics2D.gravity * (forceFactor - floatingObjects[oIndex].GetVelocity().y * bounceDamping) * forceScale;
                else
                    upLift = -Physics.gravity * (forceFactor - floatingObjects[oIndex].GetVelocity().y * bounceDamping) * forceScale;

                // If the collider has the tag "Player" the force is scaled based on the value of playerBuoyantForceScale.
                if (floatingObjects[oIndex].IsPlayer())
                    upLift *= playerBuoyantForceScale;

                // Applies a force that slows the vertical descent of an object and pushes it upwards.
                floatingObjects[oIndex].AddForceAtPosition(upLift, forcePosition);

                if (waterFlow)
                {
                    float angleInRad = 0;

                    if (useAngles)
                    {
                        angleInRad = flowAngle * Mathf.Deg2Rad;
                    }
                    else
                    {
                        switch (flowDirection)
                        {
                            case Water2D_FlowDirection.Up:
                                angleInRad = 90f * Mathf.Deg2Rad;
                                break;

                            case Water2D_FlowDirection.Down:
                                angleInRad = 270f * Mathf.Deg2Rad;
                                break;

                            case Water2D_FlowDirection.Left:
                                angleInRad = 180 * Mathf.Deg2Rad;
                                break;

                            case Water2D_FlowDirection.Right:
                                angleInRad = 0f * Mathf.Deg2Rad;
                                break;
                        }
                    }

                    Vector2 forceDir = new Vector2(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad));
                    Vector2 force = waterFlowForce * forceDir;

                    floatingObjects[oIndex].AddForceAtPosition(force, forcePosition);
                }

                // The velocity of the object at the center of the bounding box.
                Vector3 velDir = floatingObjects[oIndex].GetPointVelocity(floatingObjects[oIndex].bounds.center);

                // Velocity magnitude.
                float vel = velDir.magnitude;
                // The velocity direction vector is normalized so that we can use it to calculate the drag magnitude.
                velDir.Normalize();
                // The magnitude of the drag.
                float dragMag = liniarBFDragCoefficient * vel * vel;
                // The drag force direction must be opposite of direction of movement.
                Vector2 dragForce = dragMag * -velDir;

                // A drag is applied in the oposite direction of the objects movement.
                floatingObjects[oIndex].AddForceAtPosition(dragForce, floatingObjects[oIndex].bounds.center);

                // A torque is applied to stop the rotation of the object.
                float angularDrag = linearBFAbgularDragCoefficient * -floatingObjects[oIndex].GetAngularVelocity();
                floatingObjects[oIndex].AddTorque(angularDrag);
            }
        }

        /// <summary>
        /// Updates different variables after the water mesh was recreated from scratch.
        /// </summary>
        private void UpdateVariables()
        {
            frontMeshVertices = water2D.ParentDMesh.meshVerts;
            frontMeshUVs = water2D.ParentDMesh.meshUVs;

            surfaceVertsCount = water2D.frontMeshVertsCount / 2;

            int prevSurfaceVertsCount = water2D.prevSurfaceVertsCount;

            if (water2D.cubeWater)
            {
                topMeshVertices = water2D.ChildDMesh.meshVerts;
                topMeshUVs = water2D.ChildDMesh.meshUVs;
            }

            waterLinePreviousLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset;
            waterLineCurrentWorldPos = transform.TransformPoint(frontMeshVertices[surfaceVertsCount + 1]);
            waterLineCurrentLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset;

            // If the number of surface vertices changed the lists are updated.
            if (prevSurfaceVertsCount != surfaceVertsCount)
            {
                // If the total number of vertices increased new values are added to the list so we don't get out of bounds errors.
                if (prevSurfaceVertsCount < surfaceVertsCount)
                {
                    int vertD = surfaceVertsCount - prevSurfaceVertsCount;
                    int index = velocities.Count - 1;

                    for (int i = 0; i < vertD; i++)
                    {
                        velocities.Add(velocities[index]);
                        accelerations.Add(accelerations[index]);
                        leftDeltas.Add(leftDeltas[index]);
                        rightDeltas.Add(rightDeltas[index]);
                        sineY.Add(sineY[index]);
                    }
                }

                // If the total number of vertices decreased, the list values that no longer reference to a surface vertex are deleted.
                if (prevSurfaceVertsCount > surfaceVertsCount)
                {
                    int vertD = prevSurfaceVertsCount - surfaceVertsCount;

                    for (int i = 0; i < vertD; i++)
                    {
                        int last = velocities.Count - 1;

                        velocities.RemoveAt(last);
                        accelerations.RemoveAt(last);
                        leftDeltas.RemoveAt(last);
                        rightDeltas.RemoveAt(last);
                        sineY.RemoveAt(last);
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates a particle system and plays a sound effect
        /// </summary>
        private void GenerateParticleSystemAndSoundEffect(Vector3 pos)
        {
            if (particleS != null)
                InstantiateParticleSystem(floatingObjects.Count - 1, waterLineCurrentWorldPos.y, pos);
            if (splashSound != null)
                PlaySoundEffect(pos, floatingObjects.Count - 1);
        }

        /// <summary>
        /// Used for resetting some variables after the mesh is recreated. Not part of the Water 2D Tool main code.
        /// </summary>
        public void ResetVariables()
        {
            frontMeshVertices = water2D.ParentDMesh.meshVerts;
            surfaceVertsCount = water2D.frontMeshVertsCount / 2;
            frontMeshUVs = water2D.ParentDMesh.meshUVs;
            frontMeshFilter.bounds = new Bounds(Vector3.zero, Vector3.one * 2000f);
            topMeshFilter.bounds = new Bounds(Vector3.zero, Vector3.one * 2000f);

            if (water2D.cubeWater)
            {
                topMeshVertices = water2D.ChildDMesh.meshVerts;
                topMeshUVs = water2D.ChildDMesh.meshUVs;
                defaultWaterHeight = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
                originalWaterHeight = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);

                frontMeshRend.material.SetFloat("_HeightOffset", 0);
                topMeshRend.material.SetFloat("_HeightOffset", 0);
            }
        }

        public Vector3 GetTopLeftVertexWorldPosition()
        {
            return transform.TransformPoint(frontMeshVertices[surfaceVertsCount]);
        }

        public Vector3 GetTopRightVertexWorldPosition()
        {
            return transform.TransformPoint(frontMeshVertices[surfaceVertsCount * 2 - 1]);
        }

        public Vector3 GetBottomLeftVertexWorldPosition()
        {
            return transform.TransformPoint(frontMeshVertices[0]);
        }

        public Vector3 GetBottomRightVertexWorldPosition()
        {
            return transform.TransformPoint(frontMeshVertices[surfaceVertsCount - 1]);
        }


        public Vector3 GetTopLeftVertexLocalPosition()
        {
            return frontMeshVertices[surfaceVertsCount];
        }

        public Vector3 GetTopRightVertexLocalPosition()
        {
            return frontMeshVertices[surfaceVertsCount * 2 - 1];
        }

        public Vector3 GetBottomLeftVertexLocalPosition()
        {
            return frontMeshVertices[0];
        }

        public Vector3 GetBottomRightVertexLocalPosition()
        {
            return frontMeshVertices[surfaceVertsCount - 1];
        }

        /// <summary>
        /// Instantiates a particle system and plays a sound effect when the player exits the water.
        /// </summary>
        private void OnTriggerExitPlayer(Vector3 pos)
        {
            float width, vel;

            if (hasRippleScript)
            {
                if (playerOnExitPSAndSound)
                    GenerateParticleSystemAndSoundEffect(pos);

                if (playerOnExitRipple)
                {
                    water2DRipple.playerOnExitRipple = true;
                    water2DRipple.playerOnExitRipplePos = pos;
                }
            }
            else
            {
                if (!playerOnExitRipple && playerOnExitPSAndSound)
                    GenerateParticleSystemAndSoundEffect(pos);

                if (playerOnExitRipple)
                {
                    if (characterControllerType == Water2D_CharacterControllerType.PhysicsBased)
                    {
                        width = playerBoundingBoxSize.x;
                        vel = playerOnExitVelocity;
                    }
                    else
                    {
                        width = rippleWidth;
                        vel = -playerOnEnterVelocity;
                    }
                    interactionTimeCount = 0;
                    GenerateRippleAtPosition(pos, width, vel, playerOnExitPSAndSound);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsInLayerMask(collisionLayers.value, other.gameObject.layer) && !floatingObjects.Exists(o => o.Equals(other)))
            {
                // This makes sure only the first collider with the "Player" tag
                // that is detected is added to the floatingObjects list.
                if (other.tag == "Player" && onTriggerPlayerDetected)
                    return;

                if (other.tag == "Player" && !onTriggerPlayerDetected)
                {
                    onTriggerPlayerDetected = true;

                    var fObj = new FloatingObject2D(other, polygonCorners, true);
                    floatingObjects.Add(fObj);

                    if (characterControllerType == Water2D_CharacterControllerType.PhysicsBased)
                    {
                        if (hasRippleScript && other.transform.position.y > waterLineCurrentWorldPos.y)
                            GenerateParticleSystemAndSoundEffect(other.transform.position);
                    }
                    else
                    {
                        if (other.transform.position.y > waterLineCurrentWorldPos.y)
                        {
                            if (hasRippleScript)
                                GenerateParticleSystemAndSoundEffect(other.transform.position);
                            else
                            {
                                interactionTimeCount = 0;
                                GenerateRippleAtPosition(other.transform.position, rippleWidth, playerOnEnterVelocity, true);
                            }
                        }
                    }
                }
                else
                {
                    var fObj = new FloatingObject2D(other, polygonCorners, false);
                    floatingObjects.Add(fObj);

                    if (other.transform.position.y > waterLineCurrentWorldPos.y)
                        if ((hasRippleScript && Mathf.Abs(floatingObjects[floatingObjects.Count - 1].GetVelocity().y) > 0) || (!springSimulation && surfaceWaves == Water2D_SurfaceWaves.SineWaves))
                            GenerateParticleSystemAndSoundEffect(other.transform.position);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player" && floatingObjects.Exists(o => o.Equals(other)))
            {
                onTriggerPlayerDetected = false;
                if(other.transform.position.y > waterLineCurrentWorldPos.y)
                    OnTriggerExitPlayer(other.transform.position);
            }

            floatingObjects.RemoveAll(o => o.Equals(other));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsInLayerMask(collisionLayers.value, other.gameObject.layer) && !floatingObjects.Exists(o => o.Equals(other)))
            {
                // This makes sure only the first collider with the "Player" tag
                // that is detected is added to the floatingObjects list.
                if (other.tag == "Player" && onTriggerPlayerDetected)
                    return;

                if (other.tag == "Player" && !onTriggerPlayerDetected)
                {
                    onTriggerPlayerDetected = true;

                    var fObj = new FloatingObject3D(other, polygonCorners, true);
                    floatingObjects.Add(fObj);

                    if (characterControllerType == Water2D_CharacterControllerType.PhysicsBased)
                    {
                        if (hasRippleScript && other.transform.position.y > waterLineCurrentWorldPos.y)
                            GenerateParticleSystemAndSoundEffect(other.transform.position);
                    }
                    else
                    {
                        if (other.transform.position.y > waterLineCurrentWorldPos.y)
                        {
                            if (hasRippleScript)
                                GenerateParticleSystemAndSoundEffect(other.transform.position);
                            else
                            {
                                interactionTimeCount = 0;
                                GenerateRippleAtPosition(other.transform.position, rippleWidth, playerOnEnterVelocity, true);
                            }
                        }
                    }
                }
                else
                {
                    var fObj = new FloatingObject3D(other, polygonCorners, false);
                    floatingObjects.Add(fObj);

                    if(other.transform.position.y > waterLineCurrentWorldPos.y)
                        if ((hasRippleScript && Mathf.Abs(floatingObjects[floatingObjects.Count - 1].GetVelocity().y) > 0) || (!springSimulation && surfaceWaves == Water2D_SurfaceWaves.SineWaves))
                            GenerateParticleSystemAndSoundEffect(other.transform.position);

                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player" && floatingObjects.Exists(o => o.Equals(other)))
            {
                onTriggerPlayerDetected = false;

                if (other.transform.position.y > waterLineCurrentWorldPos.y)
                    OnTriggerExitPlayer(other.transform.position);           
            }

            floatingObjects.RemoveAll(o => o.Equals(other));
        }

        public static bool IsInLayerMask(int layerMask, int objectLayerIndex)
        {
            int objLayerMask = (1 << objectLayerIndex);

            return (layerMask & objLayerMask) != 0;
        }

        #endregion Class methods
    }
}