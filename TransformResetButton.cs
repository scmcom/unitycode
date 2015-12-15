using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Transform))]
public class TransformResetButton : Editor {

	public override void OnInspectorGUI()
	{
		if(GUILayout.Button("Reset"))
		{
			var transform = target as Transform;

			transform.position = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.rotation = Quaternion.identity;
		}
		DrawDefaultInspector();
	}
}
