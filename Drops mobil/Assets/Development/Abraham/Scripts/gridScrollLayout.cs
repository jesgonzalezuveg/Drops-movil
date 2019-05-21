using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gridScrollLayout : MonoBehaviour {

    public float tamañoScroll;
    private GridLayoutGroup layout;     ///< layout layout que acomoda las tarjetas en forma de cuadricula
    private packManager[] hijos;        ///< hijos tarjetas dentro del layout
    public bool estaAjustado;
    public bool bandera;

    /**
     * Funcion que se manda llamar al inicio de la scena(frame 1)
     * se obtiene el componente layout del gameObject
     */
    void Start() {
        layout = gameObject.GetComponent<GridLayoutGroup>();
        estaAjustado = false;
    }

    /**
     * Funcion que se llama cada frame
     * Verifica si la cantidad de hijos es mayor al valor maximo de 
     */
    private void Update() {
        hijos = gameObject.GetComponentsInChildren<packManager>();
        float columnasCount = layout.constraintCount;
        int rowCount = (int)Math.Ceiling(hijos.Length / columnasCount);
        Debug.Log(rowCount);
        if (rowCount != 0) {
            if (!estaAjustado) {
                gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (rowCount - 2) * tamañoScroll);
                Debug.Log(gameObject.GetComponent<RectTransform>().sizeDelta.y / 2);
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, gameObject.GetComponent<RectTransform>().sizeDelta.y / -2);
                estaAjustado = true;
            }
        }
    }
}
