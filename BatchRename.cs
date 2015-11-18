//BatchRename.cs
//select a bunch of GameObjects in heirarchy and rename them via
// Edit > Batch Rename

using EnityEngine;
using UnityEditor;
using System.Collections;

public class BatchRename : ScriptableWizard 
{

	//base name
	public string BaseName = "MyObject_";

	//start count
	public int StartNumber = 0;

	//increment
	public int Increment = 1;

	[MenuItem("Edit/Batch Rename...")]

	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("Batch Rename", typeof(BatchRename),"Rename");
	}

	//called when the window first appears
	void OnEnable()
	{
		UpdateSelectionHelper();
	}

	//function called when selection changes in scene
	void OnSelectionChange()
	{
		UpdateSelectionHelper();
	}

	//update selection counter
	void UpdateSelectionHelper()
	{
		helpString = "";

		if(Selection.objects != null)
			helpString = "Number of objects selected: " + Selection.objects.Length;
	}

	//rename
	void OnWizardCreate()
	{
		//if selection empty, then exit
		if(Selection.objects == null)
			return;

		//current increment
		int PostFix = StartNumber;

		//cycle and rename
		foreach(Object O in Selection.objects)
		{
			O.name = BaseName + PostFix;
			PostFix += Increment;
		}
	}
}
