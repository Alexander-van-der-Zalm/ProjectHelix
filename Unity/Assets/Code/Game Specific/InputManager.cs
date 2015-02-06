using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputManager : MonoBehaviour 
{
    public SkaterController Skater;
    public float RotatePerSecond = 360;
    public bool LockCursor = false;

    private PlayerCamera Camera;

    public AnimationCurve Curve;


    public int SmoothFrames=3;

    public List<float> x;
    public List<float> y;


	// Use this for initialization
	void Start () 
    {
        Camera = GetComponent<PlayerCamera>();
        for (int i = 0; i < SmoothFrames; i++)
        {
            x.Add(0);
            y.Add(0);
        }
            
	}
	
	// Update is called once per frame
	void Update () 
    {
        // Add the current input to be averaged
        x.Add(Input.GetAxis("Mouse X"));
        y.Add(Input.GetAxis("Mouse Y"));

        if (x.Count >= SmoothFrames)
            x.RemoveAt(0);
        if (y.Count >= SmoothFrames)
            y.RemoveAt(0);

        float xSmooth = x.Average();
        float ySmooth = y.Average();

        //Skater.Input.Pitch = ySmooth;
        //Skater.Input.Yaw = xSmooth;
        //Skater.Input.
        //Vector2 mouse = mouseInput();
        //Debug.Log(mouse);
	}

    private Vector2 mouseInput()
    {
        Screen.lockCursor = LockCursor;

        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //Vector3 curDir = Controller.Direction;
        //Quaternion yRot = Quaternion.AngleAxis(mouseDelta.y * -RotatePerSecond * Time.deltaTime, Controller.Left);
        //Quaternion xRot = Quaternion.AngleAxis(mouseDelta.x * RotatePerSecond * Time.deltaTime, Controller.Up);

        //Vector3 dir = xRot * yRot * curDir;
        //Controller.SetDirection(dir);
        //pc.SetDistanceFromTarget(dir, -5);

        //Debug.Log(Controller.Up);
    }
}
