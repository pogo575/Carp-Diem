using UnityEngine;
using System.Collections;

public class FishArt : MonoBehaviour {

	// Use this for initialization

	public Material[] fishskin;

	public int skinCount;
	public int iC;

	public SkinnedMeshRenderer mesh;

	void Start () {
		skinCount = fishskin.Length;
	}
	
	// Update is called once per frame
	public void SelectRandomskin () {
		mesh.material = fishskin[Random.Range(0,skinCount)];
	}


}
