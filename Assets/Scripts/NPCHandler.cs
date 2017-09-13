using UnityEngine;
using System.Collections;

public class NPCHandler : MonoBehaviour {
	public GameObject NPC;
	// Use this for initialization
	void Start () {
		if (this.gameObject.transform.childCount == 0) {
			GameObject clone = Instantiate (NPC,transform.position, transform.rotation) as GameObject;
			clone.transform.parent = this.gameObject.transform;
		}
	}

}

