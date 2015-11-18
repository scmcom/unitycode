using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PCGSphereTester : MonoBehaviour {

	public int subdivisions = 0;
	public float radius = 1f;

	private void Awake(){
		GetComponent<MeshFilter>().mesh = PCGSphereCreator.Create(subdivisions, radius);
	}
}
