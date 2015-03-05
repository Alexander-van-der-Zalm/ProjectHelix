using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour 
{
    public enum CameraOrientationMode
    {
        Forward,
        Velocity
    }
    
    public Transform Target;
    public float CameraDistance = 10;
    public float Smooth = 0.3f;
    public CameraOrientationMode OrientationMode;

    public float cameraAngleOffset = 15;


    private Camera mc;
    private Transform tr;


	// Use this for initialization
	void Start () 
    {
        mc = Camera.main;
        tr = mc.transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 up;
        Vector3 forward;
        Vector3 side;

        switch(OrientationMode)
        {
            case CameraOrientationMode.Velocity:
                forward = Target.GetComponent<Rigidbody>().velocity.normalized;
                up = new Vector3(0,1,0);//Vector3.Cross(forward,Target.right);
                break;
            default:
                up = Target.up;
                forward = Target.forward;
                break;
        }

        side = Vector3.Cross(forward, up);

        // Look at the target
        tr.LookAt(Target, up);

        Quaternion rotateOffset = Quaternion.AngleAxis(cameraAngleOffset, side);

        // Set the position
        tr.position = Vector3.Slerp(tr.position, Target.position + rotateOffset * forward * -CameraDistance, Smooth);
	}

    //private Vector3 TargetVelocityOrientationDistance()
    //{
    //    // Change to lerp
    //    return Target.rigidbody.velocity.normalized ;
    //}

    //private Vector3 TargetForwardOrientationDistance()
    //{
    //    // Change to lerp
    //    return Target.position + Target.forward * -CameraDistance;
    //}
}
