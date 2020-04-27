using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camScript : MonoBehaviour
{	
    public GameObject player;
	public float heightOfCamera;
	public float offsetFromPlatyer;
	private Vector3 offset;
    public Player playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Jinu").GetComponent<Player>();
        offset = new Vector3(0.0f,0.0f,offsetFromPlatyer);
    }

    // Update is called once per frame
    void Update()
    {
        float z = player.transform.position.z;

        if ( ! playerScript.cameraTilt ) {
            transform.position = new Vector3(player.transform.position.x, heightOfCamera, z) - offset;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, heightOfCamera, z) - offset;
        }
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
