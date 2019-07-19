using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;

public class avisosManager : MonoBehaviour
{
    private appManager manager;
    private webServiceAvisos.paqueteDataNuevos[] paquetesRecientes = null;
    private paquetesManager packManager;

    public GameObject panelAvisos;
    public Text titulo;
    public Text descripcion;
    public Image imagen;
    public GameObject burbujaDescargas;
    public GameObject avisoDinamico;
    public GameObject avisoEstatico;
    public GameObject panelMenuPerfil;
    public GameObject modalPerfil;
    public GameObject modalVista;
    public Animator animController;
    public bool anim = false;

    public void setPaquetesMasNuevos(webServiceAvisos.paqueteDataNuevos[]newPacks) {
        paquetesRecientes = newPacks;
    }

    private void Awake() {
        panelMenuPerfil.SetActive(false);
        paquetesRecientes = null;
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        packManager = GameObject.Find("ListaPaquetes").GetComponent<paquetesManager>();
        manager.mostrarAviso = true;
        panelAvisos.SetActive(false);
        if (manager.isOnline) {
            StartCoroutine(webServiceAvisos.getPaquetesMasNuevos("3"));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animController.enabled = false;
        anim = false;
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

        if (manager.mostrarAviso == true && panelAvisos.active == false) {
            mostrarAviso();
            if (manager.getOld == 1) {
                manager.getOld = 2;
                if (UnityEngine.Application.internetReachability == NetworkReachability.NotReachable) {
                    //No hay conexion a internet
                } else {
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
                        avisoEstatico.SetActive(true);
                        panelAvisos.SetActive(true);
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

    public void menuPerfil(bool conf) {
        if (conf) {
            panelMenuPerfil.SetActive(false);

        } else {
            if (panelMenuPerfil.active == false) {
                panelMenuPerfil.SetActive(true);
                //modalPerfil.SetActive(true);
                //modalVista.SetActive(false);
                //foreach (var btnImage in modalPerfil.GetComponentsInChildren<fondoManager>()) {
                //    btnImage.gameObject.GetComponent<Image>().color = btnImage.colorArray[manager.getFondo()];
                //}
            } else {
                panelMenuPerfil.SetActive(false);
            }
        }
    }

    public void mostrarModalVista() {
        //modalPerfil.SetActive(false);
        //modalVista.SetActive(true);
        //if (manager.vistaLista == true) {
        //    modalVista.GetComponentsInChildren<Image>()[0].color = new Color(123f, 114f, 114f, 0f);
        //    modalVista.GetComponentsInChildren<Image>()[1].color = new Color(123f, 114f, 114f, 100f);
        //} else {
        //    modalVista.GetComponentsInChildren<Image>()[0].color = new Color(123f, 114f, 114f, 100f);
        //    modalVista.GetComponentsInChildren<Image>()[1].color = new Color(123f, 114f, 114f, 0f);
        //}
    }

    public void animMenu() {
        panelMenuPerfil.SetActive(false);

        if (anim == true) {
            animController.SetBool("anim", true);
        } else {
            animController.SetBool("anim", false);
        }

        if (anim == false) {
            animController.enabled = true;
            anim = true;
        } else {
            animController.enabled = true;
            anim = false;
        }
    }

    IEnumerator getImagenPaquete(string urlImagen) {
        Sprite sprite = null;
        string path = urlImagen.Split('/')[urlImagen.Split('/').Length - 1];
        if (File.Exists(Application.persistentDataPath + path)) {
            byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + path);
            Texture2D texture = new Texture2D(8, 8);
            texture.LoadImage(byteArray);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
        } else {
            if (manager.isOnline) {
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(urlImagen)) {
                    AsyncOperation asyncLoad = www.SendWebRequest();
                    while (!asyncLoad.isDone) {
                        yield return null;
                    }
                    if (www.isNetworkError || www.isHttpError || www.responseCode == 404) {
                        Debug.Log(www.error + "Error al descargar imagen del aviso");
                    } else {
                        Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                        byte[] bytes;
                        if (path.Split('.')[path.Split('.').Length - 1] == "jpg" || path.Split('.')[path.Split('.').Length - 1] == "jpeg") {
                            bytes = texture.EncodeToJPG();
                        } else {
                            bytes = texture.EncodeToPNG();
                        }
                        File.WriteAllBytes(Application.persistentDataPath + path, bytes);
                        Rect rec = new Rect(0, 0, texture.width, texture.height);
                        sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
                    }
                }
            }
        }

        if (sprite == null) {
            sprite = packManager.portadaDefault();
        }

        imagen.sprite = sprite;
    }
}
