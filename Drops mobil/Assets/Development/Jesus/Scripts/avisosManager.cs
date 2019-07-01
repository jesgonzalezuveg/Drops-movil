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
    public GameObject panelMenuPerfil;
    public GameObject modalPerfil;
    public GameObject modalVista;
    public GameObject nombre;
    public Animator animController;
    public bool anim = false;

    public void setPaquetesMasNuevos(webServiceAvisos.paqueteDataNuevos[]newPacks) {
        paquetesRecientes = newPacks;
    }

    private void Awake() {
        panelMenuPerfil.SetActive(false);
        paquetesRecientes = null;
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        panelAvisos.SetActive(false);
        if (UnityEngine.Application.internetReachability == NetworkReachability.NotReachable) {
            //No hay conexion a internet
        } else {
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

        if (manager.mostrarAviso == true) {
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
            //nombre.SetActive(false);
        } else {
            animController.enabled = true;
            anim = false;
            //nombre.SetActive(true);
        }
    }

    IEnumerator getImagenPaquete(string urlImagen) {
        string path = urlImagen.Split('/')[urlImagen.Split('/').Length - 1];
        string urlModificada = "";
        using (WWW wwwImage = new WWW(urlImagen)) {
            yield return wwwImage;

            if (wwwImage.responseHeaders.Count > 0) {
                foreach (KeyValuePair<string, string> entry in wwwImage.responseHeaders) {
                    if (entry.Key == "STATUS") {
                        Debug.Log(entry.Value);
                        if (entry.Value == "HTTP/1.1 404 Not Found") {
                            Debug.Log("No se encontro la imagen");
                            var spriteObj = Resources.Load("preloadedCoverPacks/portadaPaqueteCarga");
                            Texture2D tex = spriteObj as Texture2D;
                            Rect rec = new Rect(0, 0, tex.width, tex.height);
                            imagen.sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
                        } else {
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
                }
            }
        }
    }
}
