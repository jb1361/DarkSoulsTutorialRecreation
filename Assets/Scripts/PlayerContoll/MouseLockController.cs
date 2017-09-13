using UnityEngine;
using System.Collections;

public class MouseLockController : MonoBehaviour {
	bool locked;
	// Use this for initialization
	void Start () {
		locked = true;
		_lock();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			locked = !locked;

			if (locked) {
				_lock();
			} else {
				unlock();
			}
		}
	}

	void _lock(){
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void unlock(){
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
