using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fondoManager : MonoBehaviour {

    public Color[] colorArray;

    appManager manager;
    int fondo = 0;

    public void Start() {
        manager = GameObject.FindObjectOfType<appManager>();
        fondo = manager.getFondo();
        if (fondo == -1) {
            fondo = Random.Range(0, 7);
        }
        gameObject.GetComponent<Image>().color = colorArray[fondo];
    }


    IEnumerator FadeTo(float aValue, float aTime) {
        var color = gameObject.GetComponent<Image>().color;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
            Color newColor = new Color(Mathf.Lerp(color.r, colorArray[fondo].r, t), Mathf.Lerp(color.g, colorArray[fondo].g, t), Mathf.Lerp(color.b, colorArray[fondo].b, t), Mathf.Lerp(color.a, colorArray[fondo].a, t));
            gameObject.GetComponent<Image>().color = newColor;
            yield return null;
        }
        yield return new WaitForSeconds(.05f);
    }

    public void cambiarFondo(float tiempo) {
        manager = GameObject.FindObjectOfType<appManager>();
        fondo = manager.getFondo();
        if (fondo == -1) {
            fondo = Random.Range(0, 7);
        }
        StartCoroutine(FadeTo(0.0f, tiempo));
    }

}
