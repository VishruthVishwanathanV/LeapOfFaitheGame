using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingLogScript : MonoBehaviour
{
    public GameObject logGameobject;
    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = logGameobject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("player"))
        {
            if( logGameobject.transform.position.x <0)
            {
                Debug.Log("The value of x is " + logGameobject.transform.position.x + "inside if");
                rigid.AddForce(new Vector3(200, 0, 0), ForceMode.Impulse);
            } else
            {
                Debug.Log("The value of x is " + logGameobject.transform.position.x + "inside else");
                rigid.AddForce(new Vector3(-200, 0, 0), ForceMode.Impulse);
            }
        }
    }
}
