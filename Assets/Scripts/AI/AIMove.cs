using UnityEngine;
using System.Collections;

public class AIMove : MonoBehaviour {

	public Transform goal;

	UnityEngine.AI.NavMeshAgent agent;

	public AIMove(Transform Goal){
		goal = Goal;
	}

	void Start () {
		 agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.destination = goal.position; 
	}

	void Update(){
		//agent.destination = goal.position; 
	}
}