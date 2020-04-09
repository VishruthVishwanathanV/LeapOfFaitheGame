using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinRotate : MonoBehaviour
{
	// Update is called once per frame
    void FixedUpdate()
    {
		transform.Rotate( new Vector3(0, 0, 5), Space.Self );
    }
}
