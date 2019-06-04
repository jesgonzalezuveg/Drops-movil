using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DragHandeler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public static GameObject itemBeingDragged;
    public static Transform parentItemBeingDragged;
    Vector3 startPosition;
    Transform startParent;


    private void Update() {

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved) {
                this.GetComponent<CanvasGroup>().blocksRaycasts = false;
                Debug.Log("Moviendo");
            }

            if (Input.touchCount == 2) {
                touch = Input.GetTouch(1);

                if (touch.phase == TouchPhase.Ended) {
                    this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    Debug.Log("Levanto movimiento");
                }
            }
        }
    }

    #region IBeginDragHandler implementation

    public void OnBeginDrag(PointerEventData eventData) {
        parentItemBeingDragged = transform.parent;
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GameObject anySlot = GameObject.Find("Respuesta");
        float escala = anySlot.transform.GetChild(0).gameObject.GetComponent<RectTransform>().rect.width/ transform.gameObject.GetComponent<RectTransform>().rect.width;
        if (transform.parent.parent.name != "Respuesta") {
            transform.gameObject.GetComponent<RectTransform>().localScale = new Vector3(escala, escala, escala);
        }
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        //validarEstadoLetra(transform, "");
        //Debug.Log("PRUEBA 1 ES SALIDA");
    }

    #endregion

    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;

        //Debug.Log("PRUEBA 2 MIENTRAS SE MUEVE");
    }

    #endregion

    #region IEndDragHandler implementation

    public void OnEndDrag(PointerEventData eventData) {
        //gameObject.transform.localScale = gameObject.transform.parent.localScale;
        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true ;
        if (transform.parent == startParent) {
            transform.position = startPosition;
        }
        validarEstadoLetra();
        Debug.Log("PRUEBA 3 ENTRO AL CUADRO");
    }
    
    #endregion

    public static void validarEstadoLetra() {
        GameObject panelRes = GameObject.Find("PanelLetras");
        GameObject res = GameObject.Find("Respuesta");
        //Debug.Log("La escala es respuesta: " + res.transform.GetChild(0).gameObject.GetComponent<RectTransform>().rect.width);
        //Debug.Log("La escala es panel respuesta: " + panelRes.transform.GetChild(0).gameObject.GetComponent<RectTransform>().rect.width);
        float escala = res.transform.GetChild(0).gameObject.GetComponent<RectTransform>().rect.width/ panelRes.transform.GetChild(0).gameObject.GetComponent<RectTransform>().rect.width;
        if (panelRes != null && res != null) {
            foreach (Transform hijo in panelRes.transform) {
                if (hijo.gameObject.transform.childCount > 0) {
                    hijo.gameObject.transform.GetChild(0).localScale = hijo.gameObject.transform.localScale;
                    hijo.gameObject.transform.GetChild(0).GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
            }

            Array.Clear(CursoManager.letras, 0, CursoManager.letras.Length);
            foreach (Transform hijo in res.transform) {
                if (hijo.gameObject.transform.childCount > 0) {
                    //Debug.Log("La escala es: "+escala);
                    float escalaAjustada = escala + (escala*0.66f);
                    hijo.gameObject.transform.GetChild(0).localScale = new Vector3(escalaAjustada, escalaAjustada, escalaAjustada);
                    //hijo.gameObject.transform.GetChild(0).localScale = hijo.gameObject.transform.localScale;
                    string letraActual = hijo.gameObject.transform.GetChild(0).name;
                    int index = hijo.gameObject.transform.GetSiblingIndex();
                    CursoManager.letras[index] = letraActual;
                    hijo.gameObject.transform.GetChild(0).GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
            }
            CursoManager.respuestaFraseCompletada = "";
            foreach (string letra in CursoManager.letras) {
                CursoManager.respuestaFraseCompletada += letra;
            }
        } else {
            Debug.Log("DragHandeler linea 75");
            Debug.Log("panelRes: "+panelRes);
            Debug.Log("res: "+res);
        }
    }
}
