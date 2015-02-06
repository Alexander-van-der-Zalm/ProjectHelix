using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour 
{
    public Transform LookAt;
    public float CameraDistance = 10;

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
        tr.LookAt(LookAt);
        tr.position = SetDistanceFromTarget();
	}

    private Vector3 SetDistanceFromTarget()
    {
        // Change to lerp
        return LookAt.position + LookAt.forward * -CameraDistance;
    }
}
