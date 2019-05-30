using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropArea : MonoBehaviour
{
    public bool empty = true;
    public bool name = false;
    public string nombre;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionStay2D(Collision2D col) {
        if (empty == true) {
            if (col.collider.gameObject.tag == "Items") {
                col.collider.GetComponent<items>().collision = true;
                if (!col.collider.GetComponent<items>().dragging) {
                    col.collider.gameObject.transform.position = transform.position;
                    col.collider.gameObject.GetComponent<Transform>().transform.localScale = this.gameObject.GetComponent<Transform>().transform.localScale;
                    if (name ==false) {
                        nombre = col.collider.gameObject.name;
                        name = true;
                        Debug.Log("Nombre obtenido: "+nombre.ToUpper());
                    }
                    empty = false;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col) {
        col.collider.GetComponent<items>().collision = false;
        empty = true;
        name = false;
        nombre = "";
        Debug.Log("Nombre: " + nombre);
        Debug.Log("Cuadro salio del area sin soltar");
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Cuadro sobre el area sin soltar");
    }
}
