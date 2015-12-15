//pulled from UnityNativeAudioPlugins
using UnityEngine;
using System.Collections;

public class FullscreenShader : MonoBehaviour
{
    public Material mat;

	void Awake()
	{
		Application.targetFrameRate = 3000;
	}

	void Start(){}

    void Update(){}

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mat);
    }
}
