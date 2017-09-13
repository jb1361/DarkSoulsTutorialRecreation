using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CombatController : MonoBehaviour {
    GameObject player;
    bool combatStance;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void enterCombatStance()
    {
        combatStance = true;
    }
    void exitCombatStance()
    {
        combatStance = false;
    }
}
