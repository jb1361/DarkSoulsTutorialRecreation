using UnityEngine;
using System.Collections;


public class GodsPistol : MonoBehaviour {
	public Camera PlayersCamera;
	public GameObject Explosion;
	public AudioClip exaudio;
//	public bool locked;
	public Component objects;
	// Use this for initialization
	void Start () {
		//locked = true;
		PlayersCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
	//	if (locked == true) {
				//		Screen.lockCursor = true;
			//	} else {
			//			Screen.lockCursor = false;
				//}

		//if (Input.GetKeyDown (KeyCode.Escape)) {
		//	locked = !locked;
			//	}


	if(Input.GetKeyDown(KeyCode.Mouse0)){
			Ray ray = new Ray(PlayersCamera.transform.position,PlayersCamera.transform.forward);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				Debug.DrawRay(ray.origin, hit.transform.position);
				Instantiate(Explosion,hit.transform.position,Quaternion.identity);
				//audio.Play();
				}
		}
		}
	void OnTiggerOver(Collider other){
		Destroy (other.gameObject);
	}
}
