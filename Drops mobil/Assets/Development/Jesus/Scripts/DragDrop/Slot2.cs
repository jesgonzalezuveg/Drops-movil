using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot2 : MonoBehaviour, IDropHandler
{
    int intentos = 3;
    int index = 0;
    GameObject panelCentral;

    void Start() {
        panelCentral = GameObject.Find("PanelCentral");
    }

    public GameObject item {
        get {
            //Debug.Log("Metodo item");
            //Debug.Log(gameObject.name+"->");
            for (var i = 0; i < transform.childCount; i ++) {
                if (transform.GetChild(i).gameObject.transform.childCount == 0) {
                    index = i;
                    return transform.GetChild(i).gameObject;
                } 
            }
            //if (transform.childCount>0) {
            //    return transform.GetChild (0).gameObject;
            //}
            return null;
        }
    }

    #region IDropHandler implementation

    public void OnDrop(PointerEventData eventData) {
        if (item == null) {
            //DragHandeler.itemBeingDragged.transform.SetParent(transform);
        } else {
            if (DragHandeler.itemBeingDragged.transform.name == panelCentral.GetComponent<CursoManager>().fraseACompletarPublica[index] + "") {
                DragHandeler.itemBeingDragged.transform.SetParent(item.transform);
                DragHandeler.itemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = false;
                intentos = 3;
                panelCentral.GetComponent<CursoManager>().textoCompletado.text = "Intentos " + intentos;
                panelCentral.GetComponent<CursoManager>().respuestaFraseCompletada += DragHandeler.itemBeingDragged.transform.name;
                Debug.Log(panelCentral.GetComponent<CursoManager>().respuestaFraseCompletada);
            } else {
                intentos--;
                panelCentral.GetComponent<CursoManager>().textoCompletado.text = "Intentos "+intentos;
                if (intentos == 0) {
                    panelCentral.GetComponent<CursoManager>().intentosCompletarPalabra(intentos);
                }
                //Debug.Log("Te quedan " + intentos + " intentos");
            }
            //if (item.transform.parent.name != DragHandeler.parentItemBeingDragged.name) {
            //    item.transform.SetParent(DragHandeler.parentItemBeingDragged);
            //    DragHandeler.itemBeingDragged.transform.SetParent(transform);
            //}
        }
    }

    #endregion
}
