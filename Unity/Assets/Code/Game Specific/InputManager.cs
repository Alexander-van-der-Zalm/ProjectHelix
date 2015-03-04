using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputManager : MonoBehaviour
{
    #region Fields

    public SkaterController Skater;
    public MouseInput Mouse;

    public ControlScheme scheme;

    private PlayerCamera Camera;

    #endregion

    #region Start

    // Use this for initialization
	void Start () 
    {
        Camera = GetComponent<PlayerCamera>();
        
        Mouse.Start(); 
	}

    #endregion

    #region Update

    void Update()
    {
        Mouse.Update();

        if(Skater != null && Skater.Input != null)
        {
            Skater.Input.Pitch = Mouse.MouseY;
            Skater.Input.Yaw = Mouse.MouseX;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Break();
        // Todo add camera scroll
    }

    #endregion
}

#region Mouse class

[System.Serializable]
public class MouseInput
{
    public bool LockCursor = false;

    public float MouseSensitivity = 3.0f;

    public int SmoothFrames = 3;

    public float MouseX { get; set; }
    public float MouseY { get; set; }

    private List<float> x;
    private List<float> y;

    public void Start()
    {
        x = new List<float>();
        y = new List<float>();

        for (int i = 0; i < SmoothFrames; i++)
        {
            x.Add(0);
            y.Add(0);
        }
    }

    public void Update()
    {
        // Move mouse shit to its own class
        Screen.lockCursor = LockCursor;

        // Add the current input to be averaged
        x.Add(Input.GetAxis("Mouse X"));
        y.Add(Input.GetAxis("Mouse Y"));

        if (x.Count >= SmoothFrames)
            x.RemoveAt(0);
        if (y.Count >= SmoothFrames)
            y.RemoveAt(0);

        MouseX = x.Average() * MouseSensitivity;
        MouseY = y.Average() * MouseSensitivity;
    }
}

#endregion