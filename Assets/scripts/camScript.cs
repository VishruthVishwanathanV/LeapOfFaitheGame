using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camScript : MonoBehaviour
{	public GameObject player;
	public float heightOfCamera;
	public float offsetFromPlatyer;
	private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
		offset = new Vector3(0.0f,0.0f,offsetFromPlatyer);
    }

    // Update is called once per frame
    void Update()
    {
		float x = player.transform.position.x;
		float z = player.transform.position.z;

		transform.position = new Vector3(transform.position.x,heightOfCamera,z)-offset;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public void setPosition( Vector3 position )
    {
        transform.position = position;
    }
}
