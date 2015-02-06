using UnityEngine;
using System.Collections;

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
        public float GravityMagnitude;
        public Vector3 Direction;

        public Vector3 Velocity { get { return GravityMagnitude * Direction; } }
    }

    [System.Serializable]
    public class RotationSettings
    {
        public float YawDegPerSec = 180.0f;
        public float PitchDegPerSec = 270.0f;
        //public float Smooth = 0.3f;



        [SerializeField]
        private float yaw;
        public float Yaw { get { return yaw; } set { yaw = value; } }// (360 + value) % 360; } }

        [SerializeField]
        private float pitch;
        public float Pitch { get { return pitch; } set { pitch = Mathf.Clamp(value, -180, 180); } }
    }

    [System.Serializable]
    public class RaycastSettings
    {
        public float RayCastOffsetY;
        public int RayCastAmount;
    }

    public class InputContainer
    {
        //private Vector2 rotationInput = Vector2.zero;

        public Vector2 RotationInput { get; set; }

        /// <summary>
        /// spin around the up axis with Yaw[0-1] * turnSpeed degrees per second
        /// </summary>
        public float Yaw { get; set; }
        
        /// <summary>
        /// spin around the right axis with Pitch[0-1] * turnSpeed degrees per second
        /// </summary>
        public float Pitch { get; set; }
    }

    #endregion

    #region Fields

    public MovementInfo Movement;
    public RotationSettings Rotation;
    public GravitySettings GravitySetting;


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
	}

    #endregion

    #region Update

    // Update is called once per frame
	void FixedUpdate () 
    {
        float dT = Time.fixedDeltaTime;
        Vector3 velocity = rb.velocity;

        #region Input

        UpdateRotationNew(dT);
        //UpdateRotationByVectorsSmoothed(dT);
        #endregion

        #region Velocity and rotation

        if (grounded)
        {
            rb.velocity = velocity + dT * GroundedAcceleration();
            //rb.rotation = GroundedRotation();
        }
        else // Airborn
        {
            rb.velocity = velocity + dT * (Gravity() + AirAcceleration()) ;
            //rb.rotation = GroundedRotation();
        }

        #endregion

        // Reset
        grounded = false;
    }

    private Vector3 targetDirection = Vector3.zero;
    private Vector3 inputV3 = Vector3.zero;

    private void UpdateRotationByVectorsSmoothed(float deltaTime)
    {
        inputV3.x = Input.RotationInput.x;
        inputV3.y = Input.RotationInput.y;

        float yaw = deltaTime * Input.Yaw * Rotation.PitchDegPerSec;
        float pitch = deltaTime * Input.Pitch * Rotation.YawDegPerSec;

        //Quaternion yawRot = Quaternion.AngleAxis()

        //targetDirection = deltaTime * (tr.forward + inputV3.normalized).normalized;

        //Debug.Log(tr.forward + inputV3.normalized);

        //Vector3 slerped = Vector3.Slerp(tr.forward, targetDirection, 0.5f);

        //rb.rotation = Quaternion.LookRotation(targetDirection,Vector3.up);
        //rb.MoveRotation();
    }

    //private void UpdateRotationByVectorsInsta(float deltaTime)
    //{
    //    targetDirection = 
    //}

    private void UpdateRotation(float deltaTime)
    {
        float yaw = deltaTime * Input.Yaw * Rotation.PitchDegPerSec;
        float pitch = deltaTime * Input.Pitch * Rotation.YawDegPerSec;

        tr.RotateAround(tr.position, tr.right, pitch);
        tr.RotateAround(tr.position, Vector3.up, yaw);
    }

    private void UpdateRotationNew(float deltaTime)
    {
        float yaw = deltaTime * Input.Yaw * Rotation.PitchDegPerSec;
        float pitch = deltaTime * Input.Pitch * Rotation.YawDegPerSec;

        //tr.rotation = Quaternion.identity;

        Rotation.Yaw += yaw;
        Rotation.Pitch += pitch;

        targetRotation = Quaternion.AngleAxis(Rotation.Yaw, Vector3.up);

        Vector3 right = targetRotation * Vector3.right;
        targetRotation = Quaternion.AngleAxis(Rotation.Pitch, right) * targetRotation;

        //targetRotation *= Quaternion.AngleAxis(Movement.Pitch, tr.right);


        Debug.Log(targetRotation + " P: " + Rotation.Pitch + " y: " + Rotation.Yaw + " right: " + tr.right + " up: " + Vector3.up);
        rb.rotation = (targetRotation);

        //tr.RotateAround(Vector3.zero, tr.right, Movement.Pitch);
        //tr.RotateAround(Vector3.zero, Vector3.up, Movement.Yaw);
    }

    #region Rotation

    //private Quaternion GroundedRotation()
    //{
    //    return Quaternion.Slerp(rb.rotation, targetRotation, Rotation.Smooth);
    //}

    #endregion

    #region Velocity

    private Vector3 AirAcceleration()
    {
        return Vector3.zero;
    }

    private Vector3 Gravity()
    {
        return GravitySetting.Velocity;
    }

    private Vector3 GroundedAcceleration()
    {
        Vector3 d = tr.forward;

        Vector3 v0 = rb.velocity;
        Vector3 v0n = v0.normalized;

        Vector3 g = GravitySetting.Velocity;
        Vector3 gn = g.normalized;

        float steer = Vector3.Dot(v0n, d) * v0.magnitude;
        float grav = Vector3.Dot(gn, d) * g.magnitude;

        Vector3 st = steer * d;
        Vector3 gt = grav * d;

        Vector3 v1 = st + gt;

        // Check if going backwards

        //Debug.Log(string.Format("a: {0} = st:{1} * d{2} + gr{3} * d",v1,steer,d,grav));

        return Vector3.zero;
    }

    #endregion

    #endregion

    public void OnCollisionStay(Collision other)
    {
        // Check if the right type of surface
        grounded = true;
    }

    public void OnMouseDown()
    {
        StartCoroutine(TestRotation());
    }

    private IEnumerator TestRotation()
    {
        Debug.Log("Pitch 1");
        Input.Yaw = 0;
        Input.Pitch = 1;
        yield return new WaitForSeconds(1f);
        Input.Yaw = 0;
        Input.Pitch = 0;
        yield return new WaitForSeconds(.25f);
        Debug.Log("Pitch -1");
        Input.Yaw = 0;
        Input.Pitch = -1;
        yield return new WaitForSeconds(1f);
        Input.Yaw = 0;
        Input.Pitch = 0;
        yield return new WaitForSeconds(.25f);

        Debug.Log("Yaw 1");
        Input.Yaw = 1f;
        Input.Pitch = 0f;
        yield return new WaitForSeconds(1f);
        Input.Yaw = 0;
        Input.Pitch = 0;
        yield return new WaitForSeconds(.25f);
        Debug.Log("Yaw -1");
        Input.Yaw = -1f;
        Input.Pitch = 0f;
        yield return new WaitForSeconds(1f);
        Input.Yaw = 0;
        Input.Pitch = 0;
        yield return new WaitForSeconds(.25f);

        Debug.Log("Pitch 1");
        Input.Yaw = 0;
        Input.Pitch = .5f;
        yield return new WaitForSeconds(0.5f);
        Input.Yaw = 0;
        Input.Pitch = 0;
        yield return new WaitForSeconds(.25f);


        Debug.Log("Yaw 1");
        Input.Yaw = 1f;
        Input.Pitch = 0f;
        yield return new WaitForSeconds(1f);
        Input.Yaw = 0;
        Input.Pitch = 0;
        yield return new WaitForSeconds(.25f);
        Debug.Log("Yaw -1");
        Input.Yaw = -1f;
        Input.Pitch = 0f;
        yield return new WaitForSeconds(1f);
        Input.Yaw = 0;
        Input.Pitch = 0;
        yield return new WaitForSeconds(.25f);
    }
}
