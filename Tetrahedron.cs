using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), 
	typeof(MeshRenderer), typeof(AudioSource))]
//[RequireComponent(typeof(MeshFilter), 
//	typeof(AudioSource))]
public class Tetrahedron : MonoBehaviour {

//	public Mesh mesh;
	public Material material;
	public float maxRotationSpeed;		//60f = ok range
	public int maxDepth = 0;
	public float childScale;

	public float frequency = 0f;

	[Range(0f,1f)]
	public float amplitude = 0.5f;

	public bool pulse = false;
	public float pulseRate = .5f;


	private float rotationSpeed;
	private int depth;
	private Mesh mesh;
	private float phase = 0.0f;
	private float st = 0.0f;

	private Material[] materials;

	public enum WaveType {
		Sin,
		Saw,
		Square,
		Tri
	}

	private enum Directions {
		FB,
		LR,
		UD
	}

//	private void Awake(){
//		
//	}

	private void Start(){
		
		rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
		if(materials==null)
			InitializeMaterials();

		st = 1.0f / AudioSettings.outputSampleRate;

		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

		Mesh mesh = meshFilter.sharedMesh;


		if(mesh == null){
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
			mesh.name = "Tetrahedron Mesh";
			GetComponent<MeshFilter>().mesh = mesh;
		}
			
		//BASE TRI, bottom left at 0,0,0
		//BOTTOM LEFT
		Vector3 p0 = new Vector3(0,0,0);
		//BOTTOM RIGHT
		Vector3 p1 = new Vector3(1,0,0);
		//BACK END
		Vector3 p2 = new Vector3(0.5f,0,Mathf.Sqrt(0.75f));
		//TOP
		Vector3 p3 = new Vector3(0.5f,Mathf.Sqrt(0.75f),Mathf.Sqrt(0.75f)/3);

		//2nd TRI, sharing the FRONT tri (0,3,1)
		//TRI in -z, opposite direction of BACK END
		Vector3 p4 = new Vector3(0.5f,Mathf.Sqrt(0.75f),(-1f * Mathf.Sqrt(0.75f)));

		//UNDERNEATH, opposite TOP
		Vector3 p5 = new Vector3(0.5f,(-1f * Mathf.Sqrt(0.75f)), Mathf.Sqrt(0.75f)/3);

		Vector3 p6 = new Vector3(0.5f,0,(-1f * Mathf.Sqrt(0.75f)));

//		if(mesh == null){
//			meshFilter.mesh = new Mesh();
//			mesh = meshFilter.sharedMesh;
//		}

		mesh.Clear();
		mesh.vertices = new Vector3[] {
			p0,p1,p2,p3,
			p4,
			p5,
			p6
		};
		mesh.triangles = new int[] {
			0,1,2, 		//BOTTOM
			0,2,3,		//LEFT
			2,1,3,		//RIGHT
			0,3,1,		//FRONT, SHARED

			4,0,3,		//LEFT, -z
			4,1,0,
			3,1,4,

			0,2,5,
			0,5,1,
			2,1,5,

			0,1,6
//			0,4,6
		};
//		mesh.vertices = new Vector3[] {
//			p0,p1,p2,
//			p0,p2,p3,
//			p2,p1,p3,
//			p0,p3,p1
//		};
//		mesh.triangles = new int[] {
//			0,1,2,
//			3,4,5,
//			6,7,8,
//			9,10,11
//		};

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		CreateChild();
	}
	void Update()
	{
		PulseObject();
		transform.Rotate(0f,rotationSpeed * Time.deltaTime, 0f);
	}

	void CreateChild()
	{
		if(depth<maxDepth)
		{
			new GameObject(
				"Tetrahedron Child").AddComponent<Tetrahedron>().Initialize(
					this, Vector3.down);
			new GameObject(
				"Tetrahedron Child").AddComponent<Tetrahedron>().Initialize(
					this, Vector3.right);
			new GameObject(
				"Tetrahedron Child").AddComponent<Tetrahedron>().Initialize(
					this, Vector3.left);
		}
	}

	private void Initialize(Tetrahedron parent, Vector3 direction)
	{
		mesh = parent.mesh;
		materials = parent.materials;
		material = parent.material;
		maxRotationSpeed = parent.maxRotationSpeed;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition = direction * (0.5f + 0.5f * childScale);
	}

	private void InitializeMaterials()
	{
		materials = new Material[maxDepth + 1];
		for(int i=0; i<=maxDepth; i++)
		{
			float t = i / (maxDepth - 1f);
			t *= t;
			materials[i] = new Material(material);
			materials[i].color = Color.Lerp(Color.white,Color.black,(float)i/maxDepth);
		}
		materials[maxDepth].color = Color.magenta;
	}

	private void PulseObject() 
	{
		if(pulse)
		{
			float maxScale = 2.5f;
			float minScale = .5f;
			float scale = (Mathf.Sin(Time.time * (pulseRate * 2 * Mathf.PI)) + 1f)/2f;

			scale = Mathf.Lerp (minScale, maxScale, scale);

			transform.localScale = new Vector3(scale,scale,scale);
		} else {
			return;
		}
	}

	private void OnAudioFilterRead(float[] data, int channels)
	{
		if(st==0.0f)
			return;
		float f = frequency * st;
		for(int n=0; n<data.Length; n+=channels)
		{
			float s = Mathf.Sin(phase * 2.0f * 3.14159726f) * amplitude;
			phase += f;
			phase -= Mathf.Floor(phase);
			for(int i=0; i<channels; i++)
				data[n+1] = s;
		}
	}
//	//TO EQUILATERAL
//	private static float squaresToTriangles = (3f - Mathf.Sqrt(3f)) / 6f;
}
