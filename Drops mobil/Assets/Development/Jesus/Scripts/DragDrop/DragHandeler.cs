using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandeler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;

    #region IBeginDragHandler implementation

    public void OnBeginDrag(PointerEventData eventData) {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        validarEstadoLetra(transform, "");
        Debug.Log("Panel"+ transform.parent.name);
        Debug.Log("PRUEBA 1 ES SALIDA");
    }

    #endregion

    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
        Debug.Log("PRUEBA 2 MIENTRAS SE MUEVE");
    }

    #endregion

    #region IEndDragHandler implementation

    public void OnEndDrag(PointerEventData eventData) {
        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == startParent) {
            transform.position = startPosition;
        }
        validarEstadoLetra(transform, transform.name);
        Debug.Log("PRUEBA 3 ENTRO AL CUADRO");
    }
    
    #endregion

    void validarEstadoLetra(Transform objLetra, string valorLetra) {
        if (objLetra.parent.parent.name == "Respuesta") {
            int index = objLetra.parent.transform.GetSiblingIndex();
            CursoManager.letras[index] = valorLetra;
            CursoManager.respuestaFraseCompletada = "";
            foreach (string letra in CursoManager.letras) {
                CursoManager.respuestaFraseCompletada += letra;
            }
        }
    }
}
