using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlaybackG : MonoBehaviour {
	
	void Start(){}
	
	void Update(){
		AudioSource audio = GetComponent<AudioSource>();
		Light light = GetComponent<Light>();
		if(Input.GetKeyDown(KeyCode.Semicolon)){
			audio.Play();
			audio.Play(44100);
			light.intensity = 8;
		}
		if(Input.GetKeyUp(KeyCode.Semicolon)){
			audio.Stop();
			light.intensity = 0;
		}
	}
}
