using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionStay2D(Collision2D col) {
        if (col.collider.gameObject.tag == "Items") {
            col.collider.GetComponent<items>().collision = true;
            if (!col.collider.GetComponent<items>().dragging) {
                col.collider.gameObject.transform.position = transform.position;
                Destroy(col.collider.gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col) {
        col.collider.GetComponent<items>().collision = false;
    }
}
