using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GravityCenter : MonoBehaviour {

//	Rigidbody rigidbody;
	public Rigidbody rb;
	public Rigidbody rb2;
//	public Transform pi;
//	public Transform twopi;
//	public float orbitTime = 1.0f;
//	private float startTime;

	void Start()
	{
//		startTime = Time.time;
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		//origin for the center of gravity / BLACK HOLE!
		Vector3 gravityOrigin = new Vector3(0.0f,0.5f,0.0f);
		Vector3 toGravityOriginFromObject = gravityOrigin - gameObject.transform.position;

		toGravityOriginFromObject.Normalize();

		float accelertaionDueToGravity = 9.8f;
		toGravityOriginFromObject *= (
			accelertaionDueToGravity * gameObject.GetComponent<Rigidbody>().mass * Time.deltaTime);
		//apply accel
		gameObject.GetComponent<Rigidbody>().AddForce(toGravityOriginFromObject, ForceMode.Acceleration);
		rb.transform.RotateAround(Vector3.zero, Vector3.up, 20 * Time.deltaTime);
	}

	void FixedUpdate()
	{

//		Vector3 center = (pi.position + twopi.position) * 0.5f;
//		center -= new Vector3(0,1,0);
//		Vector3 inRelCenter = pi.position - center;
//		Vector3 outRelCenter = twopi.position - center;
//		float fracComplete = (Time.time - startTime) / orbitTime;
//		transform.position = Vector3.Slerp(inRelCenter, outRelCenter, fracComplete);
//		transform.position += center;
	}
}
