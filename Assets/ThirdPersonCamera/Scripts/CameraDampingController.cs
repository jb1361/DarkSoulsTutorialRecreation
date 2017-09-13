using System;
using UnityEngine;


public class CameraDampingController : MonoBehaviour {
	Camera mainCamera;
	Transform pivotPoint;
	Transform CameraPosition;
	//current values for our camera
	float currentHeight;
	float currentx;
	float currentz;

    [Tooltip("Set how much the camera y movement is dampened. (Default is 10)")]
    public float dampingy;
    [Tooltip("Set how much the camera x movement is dampened. (Default is 15)")]
    public float dampingx;
    [Tooltip("Set how much the camera z movement is dampened. (Default is 15)")]
    public float dampingz;

    //initialize variables that have not been set
	void Start () {
		try{          
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			pivotPoint = GameObject.FindGameObjectWithTag("pivotPoint").transform;
            CameraPosition = GameObject.FindGameObjectWithTag("cameraPosition").transform;
        }
        catch(Exception e){
            Debug.LogError(e);
            Debug.LogError("MainCamera or pivotPoint or cameraPosition not found. Make sure tags are set properly.");
		}
        if (dampingx == 0) dampingx = 15.0f;
        if (dampingy == 0) dampingy = 10.0f;
        if (dampingz == 0) dampingz = 15.0f;
    }
	
	
	void FixedUpdate () {

		currentHeight = Mathf.Lerp (currentHeight, CameraPosition.position.y, dampingy * Time.deltaTime);
		currentx = Mathf.Lerp (currentx, CameraPosition.position.x, dampingx * Time.deltaTime);
		currentz = Mathf.Lerp (currentz, CameraPosition.position.z, dampingz * Time.deltaTime);
		mainCamera.transform.position = new Vector3 (currentx, currentHeight, currentz);
		mainCamera.transform.LookAt (pivotPoint.transform.position);
	
	}
}
