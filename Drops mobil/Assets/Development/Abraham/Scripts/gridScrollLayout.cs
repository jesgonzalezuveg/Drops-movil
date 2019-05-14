using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gridScrollLayout : MonoBehaviour {

    public int maxHorizontalTags;       ///< maxHorizontalTags Valor maximo de tarjetas que contendra el grid layout
    public int scrollCount;             ///< scrollCount numero de tarjetas que avanzará con cada click en los botones
    public bool bandera = true;         ///< bandera bandera que verifica si ya se realizo el acomo inicial del grid scroll
    private GridLayoutGroup layout;     ///< layout layout que acomoda las tarjetas en forma de cuadricula
    private packManager[] hijos;        ///< hijos tarjetas dentro del layout
    private int count = 0;              ///< count valor inicial del hijo que debe mostrar, va desde 0 hasta (hijos.lenght - maxHorizontalTags)
    public bool isVertical;
    public bool estaAjustado = false;

    public bool rayCastersOn = true;

    

    /**
     * Funcion que se manda llamar al inicio de la scena(frame 1)
     * se obtiene el componente layout del gameObject
     */
    void Start() {
        layout = gameObject.GetComponent<GridLayoutGroup>();
    }

    /**
     * Funcion que se llama cada frame
     * Verifica si la cantidad de hijos es mayor al valor maximo de 
     */
    private void Update() {
        if (isVertical) {
            if (!estaAjustado) {
                hijos = gameObject.GetComponentsInChildren<packManager>();
                float columnasCount = layout.constraintCount;
                int rowCount = (int)Math.Ceiling(hijos.Length / columnasCount);
                if (rowCount <= 2) {
                    rowCount = 2;
                } else {
                    gameObject.GetComponent<RectTransform>().localPosition += new Vector3(0, (rowCount - 2) * -3.4f, 0);
                    gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, (rowCount) * 6.58f);
                    estaAjustado = true;
                }
            }
        } else {
            if (!estaAjustado) {
                hijos = gameObject.GetComponentsInChildren<packManager>();
                gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2((hijos.Length) * 5.30f, gameObject.GetComponent<RectTransform>().sizeDelta.y);
                if (hijos.Length <= 5) {
                    
                } else {
                    gameObject.GetComponent<RectTransform>().localPosition += new Vector3((hijos.Length - 5) * 2.65f, 0, 0);
                    estaAjustado = true;
                }
            }
        }
    }
}
