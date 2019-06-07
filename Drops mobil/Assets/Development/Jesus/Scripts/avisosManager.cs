using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class avisosManager : MonoBehaviour
{
    private appManager manager;
    private webServiceAvisos.paqueteDataNuevos[] paquetesRecientes = null;

    public GameObject panelAvisos;
    public Text titulo;
    public Text descripcion;
    public Image imagen;
    public GameObject burbujaDescargas;
    public GameObject avisoDinamico;
    public GameObject avisoEstatico;

    public void setPaquetesMasNuevos(webServiceAvisos.paqueteDataNuevos[]newPacks) {
        paquetesRecientes = newPacks;
    }

    private void Awake() {
        paquetesRecientes = null;
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        panelAvisos.SetActive(false);
        if (manager.isOnline) {
            StartCoroutine(webServiceAvisos.getPaquetesMasNuevos("3"));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.paquetesPorDescargar == 0) {
            if (burbujaDescargas.active == true) {
                burbujaDescargas.SetActive(false);
            }
        } else {
            if (burbujaDescargas.active == false) {
                burbujaDescargas.SetActive(true);
            }
            burbujaDescargas.transform.GetChild(0).gameObject.GetComponent<Text>().text = "" + manager.paquetesPorDescargar;
        }

        if (manager.mostrarAviso == true) {
            mostrarAviso();
            if (manager.getOld == 1) {
                manager.getOld = 2;
                if (manager.isOnline) {
                    StartCoroutine(webServiceAvisos.getPaquetesMasNuevos("90"));
                }
            }
        }
    }

    void mostrarAviso() {
        if (manager.mostrarAviso == true && paquetesRecientes != null) {
            manager.mostrarAviso = false;
            var idUser = manager.getIdUsuario();
            foreach (var paquete in paquetesRecientes) {
                var descargas = webServiceAvisos.isPackDonwload(paquete.id);
                if (descargas == 0) {
                    var res = webServiceAvisos.getLogAvisosLast3DaysSqLite(paquete.id, idUser, paquete.fechaIntervalo);
                    if (res == null) {
                        llenarAviso(paquete.id, paquete.descripcion, paquete.descripcionCategoria, paquete.fechaRegistro, paquete.urlImagen);
                        break;
                    } else {
                        //Debug.Log("El paquete " + paquete.descripcion + " ya fue mostrado en los ultimos 3 dias");
                    }
                } else {
                    //Debug.Log("El paquete " + paquete.descripcion + " ya fue descargado");
                }
            }
        } else {
            //Debug.Log("NO ENTRO A LA VALIDACION PARA MOSTRAR AVISOS");
        }
    }

    void llenarAviso(string idPaquete, string tituloAviso, string descripcionAviso, string fechaRegistro, string urlImagen) {
        var idUser = manager.getIdUsuario();
        if (manager.getOld != 0) {
            titulo.text = "Te recomendamos: '" + tituloAviso + "'. Descargalo ya!!!";
        } else {
            titulo.text = "Nuevo paquete disponible '" + tituloAviso + "'. Descargalo ya!!!";
        }
        descripcion.text = "Descripción del paquete: " + descripcionAviso;
        StartCoroutine(getImagenPaquete(urlImagen));
        var res = webServiceAvisos.insertarLogAvisoSqLite(idPaquete, idUser, fechaRegistro);
        if (res == 0) {
            //Debug.Log("Error al tratar de insertar el log del aviso mostrado");
        } else {
            //Debug.Log("Se inserto el log del aviso mostrado");
        }
        avisoDinamico.SetActive(true);
        avisoEstatico.SetActive(false);
        panelAvisos.SetActive(true);
    }

    public void cerrarAvisos() {
        paquetesRecientes = null;
        panelAvisos.SetActive(false);
    }

    public void cambiarAviso() {
        if (avisoDinamico.active == true) {
            avisoDinamico.SetActive(false);
            avisoEstatico.SetActive(true);
        }else if (avisoEstatico.active == true) {
            avisoDinamico.SetActive(true);
            avisoEstatico.SetActive(false);
        } else {
            avisoDinamico.SetActive(true);
            avisoEstatico.SetActive(false);
        }
    }

    IEnumerator getImagenPaquete(string urlImagen) {
        string path = urlImagen.Split('/')[urlImagen.Split('/').Length - 1];
        Debug.Log(path);
        if (File.Exists(Application.persistentDataPath + path)) {
            byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + path);
            Texture2D texture = new Texture2D(8, 8);
            texture.LoadImage(byteArray);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            imagen.sprite = sprite;
        } else {
            WWW www = new WWW(urlImagen);
            yield return www;
            Texture2D texture = www.texture;
            byte[] bytes;
            if (path.Split('.')[path.Split('.').Length - 1] == "jpg" || path.Split('.')[path.Split('.').Length - 1] == "jpeg") {
                bytes = texture.EncodeToJPG();
            } else {
                bytes = texture.EncodeToPNG();
            }
            File.WriteAllBytes(Application.persistentDataPath + path, bytes);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            imagen.sprite = sprite;
        }
    }
}
