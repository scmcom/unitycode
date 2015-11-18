//Mover.cs
using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
	//desired speed in units/sec -> ie 5 u/s
	public float Speed = 5.0f;

	private Transform ThisTransform = null;

	void Update()
	{
		ThisTransform.position += new Vector3(Speed * Time.deltaTime, 0, 0);
	}
}
