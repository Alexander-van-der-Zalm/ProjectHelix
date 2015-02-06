using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour 
{
    public SkaterController Skater;
    public float RotatePerSecond = 360;
    public bool LockCursor = false;

    private PlayerCamera Camera;

    public AnimationCurve Curve;

	// Use this for initialization
	void Start () 
    {
        Camera = GetComponent<PlayerCamera>();
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        Skater.Input.RotationInput = mouseInput();
	}

    private Vector2 mouseInput()
    {
        Screen.lockCursor = LockCursor;
        
        return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        //Vector3 curDir = Controller.Direction;
        //Quaternion yRot = Quaternion.AngleAxis(mouseDelta.y * -RotatePerSecond * Time.deltaTime, Controller.Left);
        //Quaternion xRot = Quaternion.AngleAxis(mouseDelta.x * RotatePerSecond * Time.deltaTime, Controller.Up);

        //Vector3 dir = xRot * yRot * curDir;
        //Controller.SetDirection(dir);
        //pc.SetDistanceFromTarget(dir, -5);

        //Debug.Log(Controller.Up);
    }
}
