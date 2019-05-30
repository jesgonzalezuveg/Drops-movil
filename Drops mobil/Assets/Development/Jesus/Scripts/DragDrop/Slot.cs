using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public Vector3 size;
    public GameObject item {
        get {
            if (transform.childCount>0) {
                return transform.GetChild (0).gameObject;
            }
            return null;
        }
    }

    #region IDropHandler implementation

    public void OnDrop(PointerEventData eventData) {
        if (!item) {
            DragHandeler.itemBeingDragged.transform.SetParent(transform);
        }
        //Obtener nombre del item 
        //Debug.Log("Nombre del item "+item.name);
        if (item.transform.parent.parent.name == "Respuesta") {
            item.transform.localScale = this.gameObject.GetComponent<Transform>().transform.lossyScale;
            // Debug.Log("Es el panel respuesta");
            //Debug.Log("ORDEN DEL ITEM: " + item.transform.parent.transform.GetSiblingIndex());
        } else {

        }
        //Debug.Log("Nombre del padre "+item.transform.parent.name);
    }

    #endregion
}
