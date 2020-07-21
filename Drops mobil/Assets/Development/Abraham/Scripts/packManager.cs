using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;

public class packManager : MonoBehaviour {

    public webServicePaquetes.paqueteData paquete = null;       ///< paquete estructura paqueteData que almacena los datos del paquete al que pertenese esta tarjeta
    public webServiceRespuestas.Data respuestasPaquete = null;       ///< paquete estructura Data que almacena los datos de las respuestas del paquete
    private appManager manager;         ///< manager AppManager 
    private paquetesManager pManager;
    private animationManager aManager;

    /**
     * Funcion que se manda llamar al inicio de la escena (frame 1)
     * Hace referencia al manager y apaga el mensaje
     */
    public void Start() {
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        pManager = GameObject.Find("ListaPaquetes").GetComponent<paquetesManager>(); 
        aManager = GameObject.Find("AnimationManager").GetComponent<animationManager>(); 
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
        manager.validarConexion();
        if (manager.isOnline) {
            gameObject.GetComponentInParent<gridScrollLayout>().estaAjustado = false;
            GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(true, "Descargando paquete");
            webServiceRegistro.validarAccionSqlite("Descarga: " + paquete.descripcion, manager.getUsuario(), "Descargar paquete");
            consultarDatos();
        } else {
            pManager.panelMsj("¡No hay conexión a internet!");
        }
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
        manager.validarConexion();
        if (manager.isOnline) {
            GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(true, "Actualizando paquete");
            webServiceRegistro.validarAccionSqlite("Actualización : " + paquete.descripcion, manager.getUsuario(), "Actualizar paquete");
            consultarDatos();
        } else {
            pManager.panelMsj("¡No hay conexión a internet!");
        }
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
        transform.GetChild(1).GetComponentInChildren<Button>().interactable = false;
        transform.GetChild(4).GetComponentInChildren<Button>().interactable = false;
        //transform.GetComponentsInChildren<Button>()[2].interactable = false;
        manager.packToPlay = paquete;
        var res = webServiceDescarga.deleteDescargaSqLite(paquete.id);
        if (res == 1) {
            dataBaseManager.ejecutarQuery("PRAGMA foreign_keys=off;", "Se desactivaron FKs");
            Debug.Log("Se borro correctamente el paquete");
            var paquetesManager = GameObject.Find("ListaPaquetes").GetComponent<paquetesManager>();
            borrarImagenes(paquete.id);

            res = webServiceRespuestas.deleteRespuestasByPaqueteSqLite(paquete.id);
            if (res == 1) {
                res = webServicePreguntas.deletePreguntasByPaqueteSqLite(paquete.id);
                if (res == 1) {
                    Debug.Log("Se borraron las respuestas y preguntas del paquete " + paquete.id);
                    //res = webServicePaquetes.deletePaqueteSqLite(paquete.id);
                    //if (res == 1) {
                    //    Debug.Log("Se borro por completo el paquete " + paquete.id);
                    //} else {
                    //    Debug.Log("Error al borrar el paquete");
                    //}
                } else {
                    Debug.Log("Se borraron las respuestas del paquete " + paquete.id + "(error al borrar las preguntas)");
                }
            } else {
                Debug.Log("Error al borrar las respuestas del paquete " + paquete.id);
            }

            dataBaseManager.ejecutarQuery("PRAGMA foreign_keys=on;", "Se activaron FKs");
            transform.transform.GetChild(1).GetComponentInChildren<Button>().interactable = true;
            transform.GetChild(4).GetComponentInChildren<Button>().interactable = true;
            paquetesManager.cambiarVistaPaquetes(manager.vistaLista);
        } else {
            Debug.Log("Error al borrar paquete");
            transform.transform.GetChild(1).GetComponentInChildren<Button>().interactable = true;
            transform.GetChild(4).GetComponentInChildren<Button>().interactable = true;
            //transform.GetComponentsInChildren<Button>()[2].interactable = true;
        }
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(false, "");
        pManager.modalPaquete.SetActive(false);
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

    public void mostrarModalPaquete() {
        pManager.modalPaquete.transform.GetChild(1).GetComponent<Image>().color = pManager.modalPaquete.transform.GetChild(1).GetComponent<fondoManager>().colorArray[manager.getFondo()];
        pManager.modalPaquete.GetComponentInChildren<packManager>().paquete = this.GetComponent<packManager>().paquete;
        pManager.modalPaquete.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = this.GetComponent<packManager>().paquete.descripcion;
        pManager.modalPaquete.transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).GetComponentInChildren<Image>().sprite = this.transform.GetChild(0).gameObject.GetComponentInChildren<Image>().sprite;
        StartCoroutine(pManager.llenarFicha(pManager.modalPaquete.transform.GetChild(1).transform.GetChild(1).gameObject, this.GetComponent<packManager>().paquete.urlImagen, this.GetComponent<packManager>().paquete.id));
        pManager.modalPaquete.SetActive(true);
        int dificultad = 1;
        if (manager.numeroPreguntas <= 5) {
            dificultad = 1;
        } else if (manager.numeroPreguntas > 5 && manager.numeroPreguntas <= 10) {
            dificultad = 2;
        } else if (manager.numeroPreguntas > 10 && manager.numeroPreguntas <= 15) {
            dificultad = 3;
        } else if (manager.numeroPreguntas > 15) {
            dificultad = 4;
        }
        string numPreguntas = webServicePreguntas.getPreguntasByPaquete(paquete.id);
        Debug.Log("PREGUNTAS TOTALES DEL PAQUETE: " + numPreguntas);
        string score = webServiceEstadistica.getPuntaje(dificultad, paquete.id, manager.getIdUsuario());
        Debug.Log("Score maximo obtenido: " + score);
        StartCoroutine(aManager.comenzarPuntajeMaxAnim(Convert.ToInt32(score), Convert.ToInt32(numPreguntas), dificultad));
    }

    public void closeDetallePaqute() {
        StartCoroutine(aManager.animClosePanelDetalle());
    }

}
