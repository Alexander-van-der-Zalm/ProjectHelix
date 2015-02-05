using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraPixelsToUnit : MonoBehaviour 
{
    public float PixelsToUnit = 24;
    public float TargetWidth = 588;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        int height = Mathf.RoundToInt(TargetWidth / (float)Screen.width * Screen.height);
        camera.orthographicSize = height / PixelsToUnit / 2;
	}
}
