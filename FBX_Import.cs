//FBX IMPORT SCALING AUTOMATION
//FBX_Import.cs
//INSIDE EDITOR FOLDER

using UnityEngine;
using UnityEditor;
using System;

//sets FBX Mesh Scale Factor to 1
public class FBX_Import : AssetPostprocessor
{
	public const float importScale= 1.0f;

	void OnPreprocessModel()
	{
		ModelImporter importer = assetImporter as ModelImporter
		importer.globalScale = importScale;
	}
}
