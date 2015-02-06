using UnityEngine;
using System.Collections;

public class SkaterController : MonoBehaviour
{
    #region class

    [System.Serializable]
    public class MovementSettings
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
        public float RotationYAxisDPS = 180.0f;
        public float RotationXAxisDPS = 270.0f;
        public float Smooth = 0.3f;
    }

    [System.Serializable]
    public class RaycastSettings
    {
        public float RayCastOffsetY;
        public int RayCastAmount;
    }

    public class InputContainer
    {
        public Vector2 RotationInput = Vector2.zero;
    }

    //private class SmoothRotate
    //{
    //    public float Smooth = 0.5f;

    //    private Quaternion target;

    //    public void RotateRigid(Rigidbody rb,Quaternion newRotate)
    //    {
    //        target *= newRotate;
    //        rb.rotation = Quaternion.Slerp(rb.rotation,target,Smooth);
    //    }
    //}

    #endregion

    #region Fields

    public MovementSettings Movement;
    public RotationSettings Rotation;
    public GravitySettings GravitySetting;


    public InputContainer Input;

    private Quaternion targetRotation = Quaternion.identity;
    private bool grounded;

    private Vector3 direction { get; set; }
    private float currentSpeed { get { return rb.velocity.magnitude; } }

    private Rigidbody rb;
    private Transform tr;

    ////public Vector3 Direction { get { return direction; } }
    //public Vector3 Up { get { return rb.rotation * Vector3.up; } }
    //public Vector3 Forward { get { return rb.rotation * Vector3.forward; } }
    //public Vector3 Left { get { return rb.rotation * Vector3.left; } }

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

        UpdateRotationTarget(dT);

        #endregion

        #region Velocity and rotation

        if (grounded)
        {
            rb.velocity = velocity + dT * GroundedAcceleration();
            rb.rotation = GroundedRotation();
        }
        else // Airborn
        {
            rb.velocity = velocity + dT * (Gravity() + AirAcceleration()) ;
        }

        #endregion

        // Reset
        grounded = false;
    }

    private void UpdateRotationTarget(float deltaTime)
    {        
        Quaternion yRot = Quaternion.AngleAxis(Input.RotationInput.y * -Rotation.RotationYAxisDPS * deltaTime, tr.right);
        Quaternion xRot = Quaternion.AngleAxis(Input.RotationInput.x * Rotation.RotationXAxisDPS * deltaTime, tr.up);

        targetRotation *= xRot * yRot;
    }

    #region Rotation

    private Quaternion GroundedRotation()
    {
        return Quaternion.Slerp(rb.rotation, targetRotation, Rotation.Smooth);
    }

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
        Vector3 d = direction.normalized;

        Vector3 v0 = rb.velocity;
        Vector3 v0n = v0.normalized;

        Vector3 g = GravitySetting.Velocity;
        Vector3 gn = g.normalized;

        float steer = Vector3.Dot(v0n, d) * v0.magnitude;
        float grav = Vector3.Dot(gn, d) * g.magnitude;

        Vector3 st = steer * d;
        Vector3 gt = grav * d;

        Vector3 v1 = st + gt;

        //Debug.Log(string.Format("a: {0} = st:{1} * d{2} + gr{3} * d",v1,steer,d,grav));

        return Vector3.zero;
    }

    #endregion

    #endregion

    //#region Input

    //private void RotateTowards(Quaternion targetRot)
    //{
    //    targetRotation *= targetRot;
    //}

    //#endregion

    public void OnCollisionStay(Collision other)
    {
        // Check if the right type of surface
        grounded = true;
    }

    //#region OnSurface WIP

    //private bool OnSurface()
    //{
    //    // Raycast around feet
    //    bool hit = false;

    //    Vector3 pos = tr.position + new Vector3(0,RayCastSetting.RayCastOffsetY,0);

    //    //for (int i = 0; i < RayCastSettings.RayCastAmount; i++ )
    //    //{
    //    //    Ray ray = new Ray(pos,)
    //    //}

    //    return hit;
    //}

    //#endregion
}
