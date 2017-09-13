using UnityEngine;

public class MouseLock : MonoBehaviour {

	bool locked;
       
	void Start () {
        locked = true;
	}
	
	//this is a simple script that locks and unlocks the cursor when escape is pressed.
	void Update () {
        //get escape input
        if (Input.GetKeyDown(KeyCode.Escape)) locked = !locked;
        //if locked = trues then lock cursor else unlock
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
	}
}
