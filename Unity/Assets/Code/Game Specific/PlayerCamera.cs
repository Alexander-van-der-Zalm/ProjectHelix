using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour 
{
    public Transform LookAt;

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
	}

    public void SetDistanceFromTarget(Vector3 direction, float distance)
    {
        // Change to lerp
        tr.position = LookAt.position + direction * distance;
    }
}
