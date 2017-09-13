using UnityEngine;
using System.Collections;

public class CameraCullDistance : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera camera = GetComponent<Camera>();
		float[] distances = new float[32];
		distances[10] = 100000;
		camera.layerCullDistances = distances;
		Debug.Log(camera.layerCullDistances.GetValue (10));

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
