using UnityEngine;
using UnityEditor;
using System.Collections;

public class SceneLoader : Editor 
{

	[MenuItem( "Open Scene/ResolutionChanger" )]
	public static void OpenResolutionChanger()
	{
		OpenScene( "ResolutionChanger" );
	}

	[MenuItem( "Open Scene/MainMenu" )]
	public static void OpenMainMenu()
	{
		OpenScene( "MainMenu" );
	}

	[MenuItem( "Open Scene/HostGame" )]
	public static void OpenHostGame()
	{
		OpenScene( "HostGame" );
	}

	[MenuItem( "Open Scene/Scenes/" )]
	public static void OpenScenes()
	{
		OpenScene( "Scenes" );
	}

	static void OpenScene(string name)
	{
		if(EditorApplication.SaveCurrentSceneIfUserWantsTo())
		{
			EditorApplication.OpenScene( "Assets/Scenes/" + name + ".unity" );
		}
	}
}
