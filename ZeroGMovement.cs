//===================================================
// defines 3D zero gravity physics per KEYBOARD input
//		KEYBOARD INPUTS - CHANGE POSITION VECTOR
//
//		up, down, left-sidestep, right-sidestep, up, back
//			W A S D keys
//				OR
//			< > ^ down (arrow keys)
//===================================================

using UnityEngine;
using System.Collections;

public class ZeroGMovement : MonoBehaviour {

	//public variables for vertical & horizontal axes
	public string forwardAxisName = "Vertical";
	public string horizontalAxisName = "Horizontal";
	public float force = 10.0f;
	public ForceMode forceMode;

	// Use this for initialization
	void Start () {}

	// FixedUpdate for physics
	void FixedUpdate (){
		//direction of gravitation force in 3D space
		Vector3 forceDirection = new Vector3(Input.GetAxis(horizontalAxisName), 0.0f, Input.GetAxis(forwardAxisName)).normalized;
		//get position / transform of rigidbody for player controller in 3D space, apply gravity force
		GetComponent<Rigidbody>().AddForce(transform.rotation * forceDirection*force, forceMode);
	}
}
