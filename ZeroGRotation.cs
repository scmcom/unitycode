//==========================================================
// defines 3D zero gravity physics per MOUSE input
//		MOUSE INPUTS - CHANGE SPIN / ROTATIONAL MOMENTUM VECTOR
//
//		mouse around viewscreen || VR look input
//			looking around via mouse
//					OR
//			VR HMD - oculus, carboard, etc
//==========================================================

using UnityEngine;
using System.Collections;

public class ZeroGravityRotation : MonoBehaviour {

	//public variables for mouse inputs
	public string verticalAxisName = "Mouse Y";
	public string horizontalAxisName = "Mouse X";
	//programmer interface - define force amount
	public float force = 10.0f;
	public ForceMode forceMode;

	// Use this for initialization
	void Start () {}

	//FixedUpdate for physics
	void FixedUpdate (){
		//adding torque to force scalar
		GetComponent<Rigidbody>().AddTorque(transform.up * force * Input.GetAxis(horizontalAxisName), forceMode);
		//torque from other direction
		GetComponent<Rigidbody>().AddTorque(-transform.right * force * Input.GetAxis(verticalAxisName), forceMode);
	}
}
