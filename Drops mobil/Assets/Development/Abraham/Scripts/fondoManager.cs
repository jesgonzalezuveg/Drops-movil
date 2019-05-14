using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fondoManager : MonoBehaviour {

    public Texture[] textures;
    /*public Texture[] noche;
    public Texture[] tarde;
    public Texture[] dia;*/

    appManager manager;
    int fondo = 0;

    public void Start() {
        manager = GameObject.FindObjectOfType<appManager>();
        fondo = manager.getFondo();
        if (fondo == -1) {
            fondo = Random.Range(0, 7);
        }
        #region cambiar por hora
        /*switch (System.DateTime.Now.TimeOfDay.Hours) {
            case int n when (n >= 20 || n <= 5):
                Debug.Log(System.DateTime.Now.TimeOfDay.Hours);
                Debug.Log("caso 1");
                gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", noche[manager.fondo]);
                break;
            case int n when (n >= 6 && n <= 11):
                Debug.Log(System.DateTime.Now.TimeOfDay.Hours);
                Debug.Log("caso 2");
                gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", dia[manager.fondo]);
                break;
            case int n when (n >= 12 && n <= 19):
                Debug.Log(System.DateTime.Now.TimeOfDay.Hours);
                Debug.Log("caso 3");
                gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", tarde[manager.fondo]);
                break;
        }*/
        #endregion
        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", textures[fondo]);
    }

    IEnumerator FadeTo(float aValue, float aTime) {
        var color = gameObject.GetComponent<Renderer>().material.color;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
            Color newColor = new Color(Mathf.Lerp(color.r, aValue, t), Mathf.Lerp(color.g, aValue, t), Mathf.Lerp(color.b, aValue, t), Mathf.Lerp(color.a, aValue, t));
            gameObject.GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }
        yield return new WaitForSeconds(.5f);
        color = gameObject.GetComponent<Renderer>().material.color;
        Debug.Log("Cambiando a fondo #: " + fondo);
        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", textures[fondo]);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / (aTime * 2) ) {
            Color newColor = new Color(Mathf.Lerp(color.r, 1.0f, t), Mathf.Lerp(color.g, 1.0f, t), Mathf.Lerp(color.b, 1.0f, t), Mathf.Lerp(color.a, 1.0f, t));
            gameObject.GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }
    }

    public void cambiarFondo() {
        fondo = manager.getFondo();
        if (fondo == -1) {
            fondo = Random.Range(0, 7);
        }
        StartCoroutine(FadeTo(0.0f, 0.5f));
    }

}
