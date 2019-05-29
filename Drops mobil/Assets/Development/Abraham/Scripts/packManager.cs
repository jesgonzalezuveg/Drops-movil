using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class packManager : MonoBehaviour {

    public webServicePaquetes.paqueteData paquete = null;       ///< paquete estructura paqueteData que almacena los datos del paquete al que pertenese esta tarjeta
    private appManager manager;         ///< manager AppManager 

    /**
     * Funcion que se manda llamar al inicio de la escena (frame 1)
     * Hace referencia al manager y apaga el mensaje
     */
    public void Start() {
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(false, "");
        /*if (GetComponentInChildren<Text>()) {
            GetComponentInChildren<Text>().text = paquete.descripcion;
        }*/
    }

    /**
     * Funcion que se manda llamar cada que el boton descargar paquete es clickeado
     * Enciende el mensaje descargando paquete y inserta el registro de descarga
     */
    public void descargaPaquete() {
        gameObject.GetComponentInParent<gridScrollLayout>().estaAjustado = false;
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(true, "Descargando paquete");
        webServiceRegistro.validarAccionSqlite("Descarga: " + paquete.descripcion, manager.getUsuario(), "Descargar paquete");
        consultarDatos();
    }

    /**
     * Funcion que se manda llamar cada que el boton jugar paquete es clickeado
     * obtiene las preguntas del paquete clickeado y las inserta en manager.preguntas categoria
     * cambia de escena e inserta el registro de inicio de juego
     */
    public void jugarPaquete() {
        manager.preguntasCategoria = webServicePreguntas.getPreguntasByPackSqLiteCurso(paquete.id, manager.numeroPreguntas);
        webServiceRegistro.validarAccionSqlite("Partida: " + paquete.descripcion, manager.getUsuario(), "Comenzó ejercicio");
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("salon",manager.actual));
    }

    /**
     * Funcion que se manda llamar cada que el boton actualizar paquete es clickeado
     * Enciende el mensaje descargando paquete y inserta el registro de actualizar
     */
    public void actualizarPaquete() {
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(true, "Actualizando paquete");
        webServiceRegistro.validarAccionSqlite("Actualización : " + paquete.descripcion, manager.getUsuario(), "Actualizar paquete");
        consultarDatos();
    }

    /**
     * Funcion que se manda llamar al momento de clickear en descargar o actulizar paquete
     * Apaga los botones activos, consulta las preguntas y respuestas desde el SII
     * inserta los nuevos datos y la descarga en local
     */
    void consultarDatos() {
        manager.setPreguntas(null);
        manager.setRespuestas(null);
        foreach (var k in GetComponentsInChildren<Button>()) {
            k.interactable = false;
        }
        manager.packToPlay = paquete;
        StartCoroutine(webServicePreguntas.getPreguntasOfPack(paquete.descripcion));
        StartCoroutine(webServiceRespuestas.getRespuestasByPack(paquete.descripcion));
    }

}
