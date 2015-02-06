using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour 
{
    public Transform LookAt;
    public float CameraDistance = 10;
    public float Smooth = 0.3f;

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
        tr.LookAt(LookAt,LookAt.up);
        tr.position = Vector3.Slerp(tr.position, TargetDistance(), Smooth);
	}

    private Vector3 TargetDistance()
    {
        // Change to lerp
        return LookAt.position + LookAt.forward * -CameraDistance;
    }
}
