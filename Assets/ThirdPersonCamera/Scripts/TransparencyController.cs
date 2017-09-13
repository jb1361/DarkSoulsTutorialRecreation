using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class TransparencyController : MonoBehaviour {
   
    GameObject player;
    Material defaultMat;
    [Tooltip("Set how transparent the player is. (Default is 0.7)")]
    public float transparencyAmmount;
    // Use this for initialization
    void Start () {
        transparencyAmmount = 0.7f;
        try
        {
            //Gets our player and it's Mesh Renderer default material.
            player = GameObject.FindGameObjectWithTag("Player");
            defaultMat = player.GetComponentInChildren<SkinnedMeshRenderer>().material;
        }
        catch (Exception e)
        {
            Debug.Log(e);      
            Debug.Log("Ignore this if you disabled transparency otherwise the player is not tagged properly or is missing a SkinnedMeshRenderer.");
        }
    }
	
	//Turns our player transparent
    public void transparent()
    {
        Material mat = player.GetComponentInChildren<SkinnedMeshRenderer>().material;
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        Color newColor = mat.color;
        newColor.a = transparencyAmmount;
        mat.color = newColor;
        }
    //Turns our players material back to it's origional.
    public void solid()
    {
        Material mat = player.GetComponentInChildren<SkinnedMeshRenderer>().material;
        mat.SetFloat("_Mode", 0);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
        player.GetComponentInChildren<SkinnedMeshRenderer>().material = defaultMat;
    }
	
}
