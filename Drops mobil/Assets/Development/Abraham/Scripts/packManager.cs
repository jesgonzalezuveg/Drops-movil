using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class packManager : MonoBehaviour {

    public webServicePaquetes.paqueteData paquete = null;       ///< paquete estructura paqueteData que almacena los datos del paquete al que pertenese esta tarjeta
    public webServiceRespuestas.Data respuestasPaquete = null;       ///< paquete estructura Data que almacena los datos de las respuestas del paquete
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
     * Funcion que se manda llamar cuando el boton eliminar paquete es clickeado
     * Enciende el mensaje eliminando paquete y elimina todo el contenido del paquete
     */
    public void eliminarPaquete() {
        Debug.Log("ELIMINANDO PAQUETE");
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(true, "Eliminando paquete");
        //webServiceRegistro.validarAccionSqlite("Eliminar : " + paquete.descripcion, manager.getUsuario(), "Borrar paquete");
        borrarDatos();
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

    /**
     * Funcion que se manda llamar al momento de clickear en eliminar paquete
     * elimina los nuevos datos y la descarga en local
     */
    void borrarDatos() {
        manager.setPreguntas(null);
        manager.setRespuestas(null);
        transform.GetComponentsInChildren<Button>()[1].interactable = false;
        transform.GetComponentsInChildren<Button>()[2].interactable = false;
        manager.packToPlay = paquete;
        var res = webServiceDescarga.deleteDescargaSqLite(paquete.id);
        if (res == 1) {
            Debug.Log("Se borro correctamente el paquete");
            var paquetesManager = GameObject.Find("ListaPaquetes").GetComponent<paquetesManager>();
            borrarImagenes(paquete.id);
            paquetesManager.cambiarVistaPaquetes(manager.vistaLista);
        } else {
            Debug.Log("Error al borrar paquete");
            transform.GetComponentsInChildren<Button>()[1].interactable = true;
            transform.GetComponentsInChildren<Button>()[2].interactable = true;
        }
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(false, "");
    }

    void borrarImagenes(string idPaquete) {
        respuestasPaquete = webServiceRespuestas.getRespuestasByPaqueteSqLite(idPaquete);
        if (respuestasPaquete!= null) {
            foreach (var respuesta in respuestasPaquete.respuestas) {
                if (respuesta.urlImagen != null && respuesta.urlImagen != "") {
                    string path = respuesta.urlImagen.Split('/')[respuesta.urlImagen.Split('/').Length - 1];
                    string file = Application.persistentDataPath + path;
                    if (File.Exists(file)) {
                        File.Delete(file);
                        Debug.Log("La imagen " + file + " fue eliminada.");
                    } else {
                        Debug.Log("La imagen " + file + " no existe.");
                    }
                }
            }
        }
    }

    public void showDeleteButton() {
        GameObject eliminar = this.gameObject.transform.GetChild(4).gameObject;
        bool st = eliminar.active;
        //foreach (var btns in GameObject.FindGameObjectsWithTag("btnPlayUpdate")) {
        //    btns.SetActive(true);
        //}
        foreach (var btn in GameObject.FindGameObjectsWithTag("btnEliminar")) {
            btn.SetActive(false);
            btn.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
            btn.transform.parent.transform.GetChild(2).gameObject.SetActive(true);
        }
        Debug.Log("Entro a la funcion");
        if (st == true) {
            eliminar.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
        } else {
            eliminar.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }
    }

}
