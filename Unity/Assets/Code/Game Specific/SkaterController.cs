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
        public float MinTurnRadius;
        public float MaxTurnRadius;
        public AnimationCurve TurnRadiusTransition;

        [Range(0,10.0f)]
        public float LeanTransitionTime = 0.2f;
        

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
        public float Steer { get; set; }
        
        /// <summary>
        /// spin around the right axis with Pitch[0-1] * turnSpeed degrees per second
        /// </summary>
        public float ForwardLean { get; set; }

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

    public float RayCastLength;

    private float delayedLean = 0;

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

        Vector3 surfaceNormal = GetSurfaceNormal();

        if (surfaceNormal == Vector3.zero)
        {
            grounded = false;
            Debug.Log("False test");
        }
        else
            grounded = true;
            

        if (grounded)
        {
            GroundedVelocity(dT, surfaceNormal);
            GroundedRotation(dT, surfaceNormal);
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

    private Vector3 GetSurfaceNormal()
    {  
        // Get the surface normal through an raycast
        Ray ray = new Ray(tr.position, tr.up.normalized * -1);
        RaycastHit hit;

        Debug.DrawRay(tr.position, tr.up.normalized * -RayCastLength, Color.cyan);

        // Handle edge case when there is no contact?????
        if (!Physics.Raycast(ray, out hit, RayCastLength))
        {
            //Debug.LogError("Exception in surface raycast when grounded");
            return Vector3.zero;
        }

        return hit.normal.normalized;
    }

    #endregion

    #region Rotation

    private void AirbornRotation(float dT)
    {
        UpdateRotationV1(dT);
    }

    private void GroundedRotation(float dT, Vector3 surfaceNormal)
    {
        //UpdateRotationV1(dT);
        UpdateGroundedRotation(dT, surfaceNormal);
    }

    #region V1

    private void UpdateRotationV1(float deltaTime)
    {
        UpdateYawAndPitch(deltaTime);

        // Calculate the rotation in quaternions
        targetRotation = Quaternion.AngleAxis(Rotation.Yaw, Vector3.up);

        Vector3 right = targetRotation * Vector3.right;
        targetRotation = Quaternion.AngleAxis(Rotation.Pitch, right) * targetRotation;

        rb.rotation = (targetRotation);
    }

    private void UpdateYawAndPitch(float deltaTime)
    {
        // Calculate the yaw + pitch additions
        float yaw = deltaTime * Input.Steer * Rotation.YawDegPerSec;
        float pitch = deltaTime * -Input.ForwardLean * Rotation.PitchDegPerSec;

        // Add degrees to new rotation
        Rotation.Yaw += yaw;
        Rotation.Pitch += pitch;
    }

    #endregion

    private void UpdateGroundedRotation(float dT, Vector3 surfaceNormal)
    {
        // 1. Find and rotate towards up and forward vector
        // 2. Add a small delay to reach the target up and forward
        // 3. Solve correction from jumps:
        //      a. to fall or rotate up again
        // 4. When landing do COG/foot swing corrections

        // SurfaceNormal direction is the target up (not gravity)
        Vector3 targetUp = surfaceNormal;
        Vector3 forward;// 
        Vector3 side = tr.right;

        #region Yaw

        // Yaw for driftbutton - otherwise lean
        float yawInput = dT * Input.Steer * Rotation.YawDegPerSec;

        // Add rotation (Yaw) to target
        Quaternion yaw = Quaternion.AngleAxis(yawInput, targetUp);
        side = yaw * side;
        forward = Vector3.Cross(side, targetUp);

        #endregion

        // Add forward lean (Pitch) to target
        if(Rotation.LeanTransitionTime > 0)
        {
            // Delayed Input/ Leaning
            if (Input.Steer != 0) // Add more leaning
                delayedLean = Mathf.Clamp(delayedLean + Input.Steer * dT / Rotation.LeanTransitionTime, -1, 1);
            else if (delayedLean != 0)
            {
                float sign = Mathf.Sign(delayedLean);
                delayedLean -= sign * dT / Rotation.LeanTransitionTime;
                if (sign != Mathf.Sign(delayedLean))
                    delayedLean = 0;
            }
        }
        else
        {
            // Direct leaning
            delayedLean = Input.Steer;
        }

        Debug.Log(delayedLean);

        //Quaternion pitch = Quaternion.AngleAxis()

        // Max 

        // Side lean (Roll)
        // 

        // Do rotation towards
        rb.rotation = Quaternion.LookRotation(forward, targetUp);
    }

    #endregion

    #region Velocity

    private void GroundedVelocity(float dT, Vector3 surfaceNormal)
    {
        // Grounded has several physics elements working together: 
        // 1. Gravity (along carve edge)
        // 2. Surface velocity (from the wave "generators" perpendicular to carve edge)
        // 3. Steering (using centrepetal forces)
        // 4. Thrusters:
        //    a. Strafing (whilst moving along the parallel across the surface) 
        //    b. Accel and breaking (along the velocity?)

        #region Shared values (carve edge, surface normal, etc.)

        // velocity
        Vector3 v = GetComponent<Rigidbody>().velocity;
        float vm = v.magnitude;

        // Find the normalised carve direction

        // Handle edge case when perp (this would mean no carving in theory)
        Vector3 carveEdge;

        if(tr.up == surfaceNormal)
        {
            carveEdge = tr.forward;
        }
        else 
        {
            // Cross product of the up vector of the feet (up of transform for now) and the surface normal
            carveEdge = Vector3.Cross(tr.up.normalized, surfaceNormal).normalized;//tr.forward.normalized;
        }

       
 
        #endregion

        #region 1. Gravity (carving)

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

        #region 2. Surface (carving)

        // Get the surface velocity (direction and speed)
        Vector3 surfaceVelocity = SurfaceVelocityHack;

        // Maybe change to a texture lookup based on current position
        
        // Get the angle dot product between the carve edge and the surface velocity
        float surfTheta = 1 - Math.Abs(Vector3.Dot(surfaceVelocity.normalized, carveEdge));

        // calculate the surface component
        Vector3 surface = surfTheta * surfaceVelocity;

        #endregion

        #region 3. Steering (using centripetal forces)

        // Determine whether steering left or right (via input)

        // Find the current turn radius
        // min and max interpolate between

        float radius = 1;
        // direction of centripetal foce (perp to velocity)
        //Vector3 r = Vector3.Cross()
        //Vector3 centripetal = (vm * vm) / radius;

        #endregion

        //Debug.Log(string.Format("Grav: {0} theta {1} Surf: {2} theta {3} ", gravity, gravTheta, surface, surfTheta));

        // Do rays
        Debug.DrawRay(rb.position, carveEdge, Color.red);
        Debug.DrawRay(rb.position, gravity, Color.blue);
        Debug.DrawRay(rb.position, surface, Color.green);
        Debug.DrawRay(rb.position, rb.velocity, Color.yellow);
        //Debug.DrawRay(rb.position, surfaceNormal, Color.white);
        //Debug.DrawRay(rb.position, tr.forward, Color.magenta);

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
