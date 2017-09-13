using UnityEngine;
using System.Collections;

public class AspectRatio : MonoBehaviour {


	public Vector2 scaleRatio = new Vector2(0.1f,0.1f);
	private Transform mytrans;
	private float whratio;
	// Use this for initialization
	void Start () {
		mytrans = transform;
		SetScale ();

	}
	
	// Update is called once per frame
	void Update () {

	}

	void SetScale(){
		whratio = (float)Screen.width / Screen.height;

		mytrans.localScale = new Vector3 (scaleRatio.x, whratio * scaleRatio.y, 1);

	}


}
