using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {
	public GameObject player;

	// Use this for initialization
	void Start () {
		Screen.SetResolution ((int)Screen.width, (int)Screen.height, true);
	}
	
	// Update is called once per frame
	void Update () {		
		float x = player.transform.position.x;
		float z = player.transform.position.z;

		//this.transform.position
	}
}
