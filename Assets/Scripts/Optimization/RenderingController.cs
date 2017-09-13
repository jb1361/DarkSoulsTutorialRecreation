using UnityEngine;
using System.Collections;

public class RenderingController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		var distances = new float[32];
		distances[0] = 1000;
		distances[10] =100000;
		GetComponent<Camera>().layerCullDistances = distances;




	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
