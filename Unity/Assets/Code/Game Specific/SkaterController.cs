using UnityEngine;
using System.Collections;
using System;

public class SkaterController : MonoBehaviour
{
    #region class

    [System.Serializable]
    public class MovementInfo
    {
        public float MaxSpeed;
        public float AccelerationPerSecond;
    }

    [System.Serializable]
    public class GravitySettings
    {
        public float Magnitude;

        [SerializeField]
        private Vector3 direction;

        public Vector3 Direction { set { direction = value.normalized; } get { return direction.normalized; } }

        public Vector3 Velocity { get { return Magnitude * Direction; } }
    }

    [System.Serializable]
    public class RotationSettings
    {
        public float YawDegPerSec = 360.0f;
        public float PitchDegPerSec = 270.0f;

        public float MaxPitchRange = 90.0f;

        [SerializeField]
        private float yaw;
        public float Yaw { get { return yaw; } set { yaw = value; } }// (360 + value) % 360; } }

        [SerializeField]
        private float pitch;
        public float Pitch { get { return pitch; } set { pitch = Mathf.Clamp(value, -MaxPitchRange, MaxPitchRange); } }
    }

    public class InputContainer
    {
        /// <summary>
        /// spin around the up axis with Yaw[0-1] * turnSpeed degrees per second
        /// </summary>
        public float Yaw { get; set; }
        
        /// <summary>
        /// spin around the right axis with Pitch[0-1] * turnSpeed degrees per second
        /// </summary>
        public float Pitch { get; set; }

        public Vector2 ThrusterInput { get; set; }
    }

    #endregion

    #region Fields

    public Vector3 SurfaceVelocityHack = new Vector3(0, 1, 0);

    public bool GroundedGravity = true;
    public bool GroundedSurface = true;
    public bool GroundedSteering = true;
    public bool GroundedThrusters = true;

    public MovementInfo Movement;
    public RotationSettings Rotation;
    public GravitySettings Gravity;
    public InputContainer Input;

    private Quaternion targetRotation = Quaternion.identity;
    private bool grounded;

    private float currentSpeed { get { return rb.velocity.magnitude; } }

    private Rigidbody rb;
    private Transform tr;

    #endregion

    #region Start

    void Start () 
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        if (Input == null)
            Input = new InputContainer();

        //rb.velocity = tr.forward * 10;
	}

    #endregion

    #region Update

    // Update is called once per frame
	void FixedUpdate () 
    {
        float dT = Time.fixedDeltaTime;

        if (grounded)
        {
            GroundedRotation(dT);
            GroundedVelocity(dT);
        }
        else // Airborn
        {
            Debug.Log("Airborn");

            AirbornRotation(dT);
            AirbornVelocity(dT);
        }

        // Reset
        grounded = false;
    }

    #endregion

    #region Rotation

    private void AirbornRotation(float dT)
    {
        UpdateRotationNew(dT);
    }

    private void GroundedRotation(float dT)
    {
        UpdateRotationNew(dT);
    }

    //private Vector3 targetDirection = Vector3.zero;
   // private Vector3 inputV3 = Vector3.zero;

    private void UpdateRotationNew(float deltaTime)
    {
        // Calculate the yaw + pitch additions
        float yaw = deltaTime * Input.Yaw * Rotation.YawDegPerSec;
        float pitch = deltaTime * -Input.Pitch * Rotation.PitchDegPerSec;

        // Add degrees to new rotation
        Rotation.Yaw += yaw;
        Rotation.Pitch += pitch;

        // Calculate the rotation in quaternions
        targetRotation = Quaternion.AngleAxis(Rotation.Yaw, Vector3.up);

        Vector3 right = targetRotation * Vector3.right;
        targetRotation = Quaternion.AngleAxis(Rotation.Pitch, right) * targetRotation;

        rb.rotation = (targetRotation);
    }

    #endregion

    #region Velocity

    private void GroundedVelocity(float dT)
    {
        // Grounded has several physics elements working together: 
        // 1. Gravity (along carve edge)
        // 2. Surface velocity (from the wave "generators" perpendicular to carve edge)
        // 3. Steering (using centrepetal forces)
        // 4. Thrusters:
        //    a. Strafing (whilst moving along the parallel across the surface) 
        //    b. Accel and breaking (along the velocity?)

        #region shared values

        // Find the normalised carve direction
        // For now its the forward of the transform
        // Could be changed to an intersection test thingy?
        Vector3 carveEdge = tr.forward.normalized;

        #endregion

        #region 1. Gravity

        // The force or acceleration of gravity is greater when carving more in line with gravity
        // Hence a dot projection is done between the angle of gravity and the carveEdge 
        // theta = carve dot gravity normalized
        // CarveEdge parallel to Gravity ~ theta == 1 && theta == 0 when carve and gravity are perpendicular 

        // Maybe think about the whole friction thing (for example when standing still but not carving??)

        // The angle difference between the grav dir and the carve dir
        float gravTheta = Vector3.Dot(Gravity.Direction, carveEdge);

        // calculate the gravity component
        Vector3 gravity = gravTheta * Gravity.Magnitude * carveEdge; // (1 - theta) * magnitute * gravityDir along surface for slide??

        #endregion

        #region 2. Surface

        // Get the surface velocity (direction and speed)
        Vector3 surfaceVelocity = SurfaceVelocityHack;

        // Maybe change to a texture lookup based on current position
        
        // Get the angle dot product between the carve edge and the surface velocity
        float surfTheta = 1 - Math.Abs(Vector3.Dot(surfaceVelocity.normalized, carveEdge));

        // calculate the surface component
        Vector3 surface = surfTheta * surfaceVelocity;

        #endregion

        //Debug.Log(string.Format("Grav: {0} theta {1} Surf: {2} theta {3} ", gravity, gravTheta, surface, surfTheta));

        // Do rays
        Debug.DrawRay(rb.position, gravity, Color.blue);
        Debug.DrawRay(rb.position, surface, Color.green);
        Debug.DrawRay(rb.position, rb.velocity, Color.yellow);

        if (GroundedGravity)
            rb.velocity += dT * gravity;

        if(GroundedSurface)
            rb.velocity += dT * surface;

        // Max
        if (rb.velocity.magnitude > Movement.MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * Movement.MaxSpeed;
        }
            
    }


    private void AirbornVelocity(float dT)
    {
        rb.velocity += dT * Gravity.Velocity;
    }

    #endregion

    //private Vector3 SurfaceSpeed()
    //{
    //    return Vector3.zero;
    //}

    //[Range(0,1)]
    //public float BreakAngle = 0.45f;

    //public float BreakFactorSpeed = 1.0f;
    //public float BreakFactorSlow = 10.0f;


    //private Vector3 Braking()
    //{
    //    Vector3 d = tr.forward.normalized;
    //    Vector3 v = rb.velocity;
    //    Vector3 vn = v.normalized;
    //    float vm = v.magnitude;

    //    // No need to brake if there is no velocity
    //    if(vm == 0)
    //    {
    //        return Vector3.zero;
    //    }

    //    // Only break if the direction is sufficiently facing away from the velocity
    //    float dirAngle = Vector3.Dot(vn,d);

    //    Debug.Log(string.Format("d: {0} {3} vn: {1} theta {2}", d, vn, dirAngle, d.normalized));
    //    if(dirAngle == 0 || Mathf.Abs(dirAngle) > BreakAngle)
    //    {
    //        return Vector3.zero;
    //    }

    //    // Calculate the break 
    //    Debug.Log("Braking");

    //    // break Angle Factor
    //    float ba = dirAngle / BreakAngle;

    //    // Fast break factor (does the main bulk of the breaking)
    //    float fb = vm * BreakFactorSpeed;

    //    // Slow break factor (practically only works when the velocity is very low)
    //    float sb = BreakFactorSlow / vm;

    //    //Debug.Log(string.Format("d: {0} v: {1} vn: {2} vm: {3} dir: {4} ba: {5} fb {6} sb {7}",d,v,vn,vm,dirAngle,ba,fb,sb));
        
    //    // negative velocity direction * break angle * (fastbreak + slowbreak)
    //    return -vn * ba * fb;
    //}

    //private Vector3 SurfGravity()
    //{
    //    return Gravity();
        
    //    return Vector3.zero;
        
        
    //}

    //public float SteerPower = 1.0f;

    //private Vector3 Steer(float dT)
    //{
    //    Vector3 d = tr.forward.normalized;

    //    Vector3 v0 = rb.velocity;
    //    Vector3 v0n = v0.normalized;

    //    // Two steps:
    //    // - calculate the new direction
    //    // - subtract an equal magnitude from the velocity

    //    // the dot product of the direction and the velocity
    //    // where 0 = perpendicular and 1 = parralel
    //    float theta = Vector3.Dot(v0n, d);

    //    // Check if backwards is a special case
    //    if (theta < 0)
    //        Debug.Log("Backwards man");

    //    float steerMagnitude = dT * theta * v0.magnitude * SteerPower;

    //    Vector3 steer = d.normalized * steerMagnitude;

    //    Vector3 correction = -v0n * steerMagnitude;//((v0+steer).magnitude - v0.magnitude)* -v0n;

    //    Debug.Log(string.Format("{0} {1} {2}",steer, correction, (steer + correction).magnitude));
      
    //    return steer + correction;
    //}

    //#endregion

    //#region Rotation

    

    //#endregion

    //#region Velocity

    //private Vector3 AirAcceleration()
    //{
    //    return Vector3.zero;
    //}

    ////private Vector3 Gravity()
    ////{
    ////    return GravitySetting.Velocity;
    ////}

    //private Vector3 GroundedAcceleration()
    //{
    //    Vector3 d = tr.forward;

    //    Vector3 v0 = rb.velocity;
    //    Vector3 v0n = v0.normalized;

    //    Vector3 g = Gravity.Velocity;
    //    Vector3 gn = g.normalized;

    //    float steer = Vector3.Dot(v0n, d) * v0.magnitude;
    //    float grav = Vector3.Dot(gn, d) * g.magnitude;

    //    Vector3 st = steer * d;
    //    Vector3 gt = grav * d;

    //    Vector3 v1 = st + gt;

    //    // Check if going backwards

    //    //Debug.Log(string.Format("a: {0} = st:{1} * d + gr{3} * d |||| d{2}",v1,steer,d,grav));

    //    return v1;
    //}

    #region Collision

    public void OnCollisionStay(Collision other)
    {
        // Check if the right type of surface
        grounded = true;
    }

    #endregion

    #region Test

    //public void OnMouseDown()
    //{
    //    StartCoroutine(TestRotation());
    //}

    //private IEnumerator TestRotation()
    //{
    //    Debug.Log("Pitch 1");
    //    Input.Yaw = 0;
    //    Input.Pitch = 1;
    //    yield return new WaitForSeconds(1f);
    //    Input.Yaw = 0;
    //    Input.Pitch = 0;
    //    yield return new WaitForSeconds(.25f);
    //    Debug.Log("Pitch -1");
    //    Input.Yaw = 0;
    //    Input.Pitch = -1;
    //    yield return new WaitForSeconds(1f);
    //    Input.Yaw = 0;
    //    Input.Pitch = 0;
    //    yield return new WaitForSeconds(.25f);

    //    Debug.Log("Yaw 1");
    //    Input.Yaw = 1f;
    //    Input.Pitch = 0f;
    //    yield return new WaitForSeconds(1f);
    //    Input.Yaw = 0;
    //    Input.Pitch = 0;
    //    yield return new WaitForSeconds(.25f);
    //    Debug.Log("Yaw -1");
    //    Input.Yaw = -1f;
    //    Input.Pitch = 0f;
    //    yield return new WaitForSeconds(1f);
    //    Input.Yaw = 0;
    //    Input.Pitch = 0;
    //    yield return new WaitForSeconds(.25f);

    //    Debug.Log("Pitch 1");
    //    Input.Yaw = 0;
    //    Input.Pitch = .5f;
    //    yield return new WaitForSeconds(0.5f);
    //    Input.Yaw = 0;
    //    Input.Pitch = 0;
    //    yield return new WaitForSeconds(.25f);


    //    Debug.Log("Yaw 1");
    //    Input.Yaw = 1f;
    //    Input.Pitch = 0f;
    //    yield return new WaitForSeconds(1f);
    //    Input.Yaw = 0;
    //    Input.Pitch = 0;
    //    yield return new WaitForSeconds(.25f);
    //    Debug.Log("Yaw -1");
    //    Input.Yaw = -1f;
    //    Input.Pitch = 0f;
    //    yield return new WaitForSeconds(1f);
    //    Input.Yaw = 0;
    //    Input.Pitch = 0;
    //    yield return new WaitForSeconds(.25f);
    //}

    #endregion
}
