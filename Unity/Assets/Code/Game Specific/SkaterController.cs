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

        public Vector3 Gravity { get { return GravityMagnitude * Direction; } }

    }

    [System.Serializable]
    public class RaycastSettings
    {
        public float RayCastOffsetY;
        public int RayCastAmount;
    }



    #endregion

    #region Fields

    public MovementSettings MovementSetting;
    public RaycastSettings RayCastSetting;
    public GravitySettings GravitySetting;

    private Vector3 direction;
    private float currentSpeed { get { return rb.velocity.magnitude; } }

    private bool grounded;

    private Rigidbody rb;
    private Transform tr;

    
    #endregion

    #region Start

    void Start () 
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();

        direction = Vector3.forward;
	}

    #endregion

    #region Update

    // Update is called once per frame
	void FixedUpdate () 
    {
        if(grounded)
        {
            rb.useGravity = false;

            rb.velocity = GroundedVelocity();
        }
        else // Airborn
        {
            rb.useGravity = true;
        }
        
        // Reset
        grounded = false;
    }

    private Vector3 GroundedVelocity()
    {
        Vector3 d = direction.normalized;

        Vector3 v0 = rb.velocity;
        Vector3 v0n = v0.normalized;
        
        Vector3 g = GravitySetting.Gravity;
        Vector3 gn = g.normalized;

        float steer = Vector3.Dot(v0n, d) * v0.magnitude;
        float grav = Vector3.Dot(gn, d) * g.magnitude;

        Vector3 st = steer * d;
        Vector3 gt = grav * d;

        Vector3 v1 = st + gt;

        Debug.Log(string.Format("a: {0} = st:{1} * d{2} + gr{3} * d",v1,steer,d,grav));

        return v1;
    }

    #endregion

    public void OnCollisionStay(Collision other)
    {
        // Check if the right type of surface
        grounded = true;
    }

    #region OnSurface WIP

    private bool OnSurface()
    {
        // Raycast around feet
        bool hit = false;

        Vector3 pos = tr.position + new Vector3(0,RayCastSetting.RayCastOffsetY,0);

        //for (int i = 0; i < RayCastSettings.RayCastAmount; i++ )
        //{
        //    Ray ray = new Ray(pos,)
        //}

        return hit;
    }

    #endregion
}
