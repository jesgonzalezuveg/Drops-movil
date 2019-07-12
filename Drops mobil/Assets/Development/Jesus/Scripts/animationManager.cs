﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class animationManager : MonoBehaviour
{
    public Text puntaje;
    public Text rango;
    public Animator controllerEstralla1;
    public Animator controllerEstralla2;
    public Animator controllerEstralla3;
    public Animator controllerPanelDetallePaquete;


    public Color col;
    public Color bronce;
    public Color plata;
    public Color oro;
    public Color esmeralda;
    //public Image estrellaVacia;
    private bool stopAnimation;
    // Start is called before the first frame update
    void Start()
    {
        stopAnimation = false;
        controllerEstralla1.enabled = false;
        controllerEstralla2.enabled = false;
        controllerEstralla3.enabled = false;
        controllerPanelDetallePaquete.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator comenzarPuntajeMaxAnim(int score, int numPreguntas, int dificultad) {
        if (stopAnimation == false) {
            controllerPanelDetallePaquete.enabled = true;
            if (dificultad == 1) {
                col = bronce;
                rango.text = "Bronce";
            } else if (dificultad == 2) {
                col = plata;
                rango.text = "Plata";
            } else if (dificultad == 3) {
                col = oro;
                rango.text = "Oro";
            } else if (dificultad == 4) {
                col = esmeralda;
                rango.text = "Diamante";
            }
            colorStar();

            int count = 0;
            while (count < 20) {
                int num = Mathf.RoundToInt(Random.Range(100f, 7200f));
                puntaje.text = num + "";
                yield return new WaitForSeconds(0.01f);
                count++;
            }
            puntaje.text = score + "";
            if (numPreguntas > 20) {
                numPreguntas = 20;
            }

            float puntajeMax = ((numPreguntas - 4) * 400) + 700;
            float puntePorEstrella = puntajeMax / 10;
            if (score < 1100) {
                //Esta seccion es para cuando no se consigue niguna estrella
            } else if (score >= 1100 && score <= (puntePorEstrella * 6)) {
                controllerEstralla1.enabled = true;
                yield return new WaitForSeconds(0.3f);
            } else if (score > (puntePorEstrella * 6) && score < (puntePorEstrella * 8.5)) {
                controllerEstralla1.enabled = true;
                yield return new WaitForSeconds(0.3f);
                controllerEstralla2.enabled = true;
                yield return new WaitForSeconds(0.3f);
            } else if (score > (puntePorEstrella * 8.5)) {
                controllerEstralla1.enabled = true;
                yield return new WaitForSeconds(0.3f);
                controllerEstralla2.enabled = true;
                yield return new WaitForSeconds(0.3f);
                controllerEstralla3.enabled = true;
                yield return new WaitForSeconds(0.3f);
            }
            controllerEstralla1.enabled = false;
            controllerEstralla2.enabled = false;
            controllerEstralla3.enabled = false;
            stopAnimation = true;
        }
    }

    public void colorStar() {
        controllerEstralla1.gameObject.GetComponent<Image>().color = col;
        controllerEstralla2.gameObject.GetComponent<Image>().color = col;
        controllerEstralla3.gameObject.GetComponent<Image>().color = col;
        var spriteObj = Resources.Load("star3");
        Texture2D tex = spriteObj as Texture2D;
        Rect rec = new Rect(0, 0, tex.width, tex.height);
        controllerEstralla1.gameObject.GetComponent<Image>().sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
        controllerEstralla2.gameObject.GetComponent<Image>().sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
        controllerEstralla3.gameObject.GetComponent<Image>().sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
    }

    public IEnumerator animClosePanelDetalle() {
        if (stopAnimation == true) {
            controllerPanelDetallePaquete.SetBool("Close", true);
            yield return new WaitForSeconds(0.6f);
            controllerPanelDetallePaquete.SetBool("Close", false);
            controllerPanelDetallePaquete.enabled = false;
            controllerPanelDetallePaquete.gameObject.SetActive(false);
            foreach (var estrella in controllerPanelDetallePaquete.gameObject.transform.GetChild(1).transform.GetChild(3).GetComponentsInChildren<Image>()) {
                estrella.color = new Color(255f, 255f, 255f, 0f);
            }
            stopAnimation = false;
        }
    }
}
