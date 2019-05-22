using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class paquetesManager : MonoBehaviour {

    private appManager manager;             ///< manager referencia al componente appManager
    public Image imagen;                    ///< imagen referencia a la imagen que contendra la imagen del usuario
    public GameObject textoPaquetes;        ///< textoPaquetes referencia al objeto que se muestra u oculta dependiendo si existen paquetes por descargar
    public GameObject listaPaquetes;        ///< listaPaquetes referencia al objeto que contiene los paquetes ya instalados
    public GameObject listaPaquetesNuevos;  ///< listaPaquetesNuevos referencia al objeto que contiene los paquetes nuevos por descargar
    public GameObject configuracionModal;   ///< configuracionModal referencia al modal de configuracion de curso
    public GameObject scrollBar;            ///< scrollBar referencia al scrollbar para seleccionar el numero maximo de preguntas por curso
    private bool bandera = true;            ///< bandera bandera que valida si ya se obtuo la imagen del usuario
    private bool banderaTabs = false;
    public GameObject tabToInstantiate;
    public GameObject panelTabs;
    public GameObject panelTabsBtn;
    public GameObject panelPaquetes;
    public GameObject panelDescargas;
    //public GameObject tabTodos;

    private RectTransform posTabContent;

    public GameObject tabContent;

    GameObject tabActivo;
    GameObject mascota;

    /**
     * Funcion que se manda llamar al inicio de la escena (frame 0)
     * Inicializa la referencia del appManager
     */
    private void Awake() {
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        var preferencias = webServicePreferencias.getPreferenciaSqLite(manager.getUsuario());
        if (preferencias == null) {
            webServicePreferencias.insertarPreferenciaSqLite(manager.getUsuario());
        } else {
            if (preferencias.mascota == "1") {
                manager.mascotaActive = true;
            } else {
                manager.mascotaActive = false;
            }
            manager.setFondo(Int32.Parse(preferencias.fondo));
            manager.numeroPreguntas = Int32.Parse(preferencias.numeroPreguntas);
        }
    }

    /**
     * Funcion que se manda llamar al inicio de la escena (frame 1)
     * set numeroPreguntas al que el usuario ya habia seleccionado
     * Oculta el modal de configuracion
     * obtiene los datos de la BD local
     */
    private void Start() {
        if (manager.isOnline) {
            if (manager.isFirstLogin) {
                manager.isFirstLogin = false;
                StartCoroutine(webServiceCategoria.getCategorias());
                StartCoroutine(webServiceAcciones.getAcciones());
                StartCoroutine(webServiceEjercicio.getEjercicios());
            }
            StartCoroutine(webServicePaquetes.getPaquetes());
        } else {

            var paquetesLocales = webServicePaquetes.getPaquetesSqLite();
            if (paquetesLocales != null) {
                manager.setPaquetes(paquetesLocales.paquete);
            } else {
                fillEmpty(listaPaquetes);
            }
            var categoriasLocal = webServiceCategoria.getCategoriasSql();
            if (categoriasLocal != null) {
                manager.setCategorias(categoriasLocal);
            }
            StartCoroutine(getUserImg());
        }

        scrollBar.GetComponent<Slider>().value = manager.numeroPreguntas;
        setVisibleModal(false);
        manager.setBanderas(true);
        tabActivo = GameObject.Find("tabContentTodos");
        posTabContent = GameObject.Find("tabContentTodos").GetComponent<RectTransform>();
        GameObject.Find("Nombre").GetComponent<Text>().text = manager.getNombre();
        GameObject.Find("Tabs").SetActive(false);
        if (GameObject.FindObjectOfType<appManager>()) {
            GameObject.FindObjectOfType<appManager>().cargando.SetActive(false);
        }
    }

    public void fillPackTabs() {
        if (banderaTabs == false) {
            var categorias = webServiceCategoria.getCategoriasSql();
            if (categorias != null) {
                foreach (var categoria in categorias) {
                    var tab = Instantiate(tabToInstantiate) as GameObject;
                    tab.name = categoria.descripcion + "Tab";
                    tab.transform.SetParent(panelTabs.transform);
                    tab.transform.localPosition = new Vector3(0, 0, 0);
                    tab.GetComponentInChildren<Text>().text = categoria.descripcion;
                    tab.transform.localScale = new Vector3(1, 1, 1);
                    tab.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    addTabEvent(tab, categoria);
                }
                banderaTabs = true;
            } else {
                //Debug.Log("CategoriasVacio");
            }
        }
    }

    void addTabEvent(GameObject tab, webServiceCategoria.categoriaData categoria) {
        var contentTab = Instantiate(tabContent, GameObject.Find("PanelPaquetes").transform, true) as GameObject;
        contentTab.name = "tabContent" + categoria.descripcion;
        contentTab.transform.localScale = new Vector3(1, 1, 1);
        contentTab.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        contentTab.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        contentTab.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        contentTab.GetComponent<RectTransform>().position = posTabContent.position;
        contentTab.GetComponent<RectTransform>().rotation = posTabContent.rotation;
        contentTab.GetComponent<RectTransform>().localScale = posTabContent.localScale;
        contentTab.GetComponent<RectTransform>().sizeDelta = posTabContent.sizeDelta;
        contentTab.SetActive(false);
        tab.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            fillTabContent(contentTab, categoria);
            tabActivo.SetActive(false);
            contentTab.SetActive(true);
            tabActivo = contentTab;
            controlPanel(1);
        });
    }

    void fillTabContent(GameObject content, webServiceCategoria.categoriaData categoria) {
        destruirObjetos(content.GetComponentInChildren<gridScrollLayout>().gameObject);
        var paquetes = webServicePaquetes.getPaquetesByCategoriaSqLite(categoria.id);
        if (paquetes != null) {
            foreach (var paquete in paquetes) {
                var descarga = webServiceDescarga.getDescargaByPaquete(paquete.id);
                if (descarga != null) {
                    if (isActualized(descarga.fechaDescarga, paquete.fechaModificacion)) {
                        newCardJugar(paquete, content.GetComponentInChildren<gridScrollLayout>().gameObject);
                    } else {
                        newCardActualizar(paquete, content.GetComponentInChildren<gridScrollLayout>().gameObject);
                    }
                }
            }
            fillEmpty(content.GetComponentInChildren<gridScrollLayout>().gameObject);
        }
    }

    public bool isActualized(string fechaDescarga, string fechaModificacion) {
        //Formato de fechaDescarga = dd/MM/yyyy HH:mm:ss "PC"
        //Formato de fechaDescarga = MM/dd/yyyy HH:mm:ss "Android"
        var dia = 1;
        var mes = 0;
        var año = 2;
        if (Application.isEditor) {
            dia = 0;
            mes = 1;
            año = 2;
        }
        fechaDescarga = fechaDescarga.Remove(10, fechaDescarga.Length - 10);
        string[] splitDateDescarga = fechaDescarga.Split('/');
        //Formato de fechaModificacion paquete = yyyy-MM-dd HH:mm:ss
        fechaModificacion = fechaModificacion.Remove(10, fechaModificacion.Length - 10);
        string[] splitDatePack = fechaModificacion.Split('/');
        if (Int32.Parse(splitDateDescarga[año]) >= Int32.Parse(splitDatePack[año])) {
            if (Int32.Parse(splitDateDescarga[mes]) >= Int32.Parse(splitDatePack[mes])) {
                if (Int32.Parse(splitDateDescarga[mes]) == Int32.Parse(splitDatePack[mes])) {
                    if (Int32.Parse(splitDateDescarga[dia]) >= Int32.Parse(splitDatePack[dia])) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } else {
                return false;
            }
        } else {
            return false;
        }
        return true;
    }

    /**
     * Funcion que se manda llamar cada frame
     * Si no existe imagen de usuario la inserta
     * verifica si existen paquetes nuevos para descargar
     */
    private void Update() {
        if (manager.getImagen() != "" && bandera) {
            StartCoroutine(getUserImg());
            bandera = false;
        }
        if (listaPaquetesNuevos.transform.childCount <= 0) {
            GameObject.Find("ListaPaquetes").GetComponent<paquetesManager>().textoPaquetes.SetActive(true);
        } else {
            GameObject.Find("ListaPaquetes").GetComponent<paquetesManager>().textoPaquetes.SetActive(false);
        }
    }

    /**
     * Coroutine que obtiene la imagen del usuario
     * no importa si inicio con Facebook o es usuario UVEG
     */
    IEnumerator getUserImg() {
        if (manager.GetComponent<appManager>().getImagen() != null) {
            string path = manager.GetComponent<appManager>().getImagen().Split('/')[manager.GetComponent<appManager>().getImagen().Split('/').Length - 1];
            string url = manager.GetComponent<appManager>().getImagen();
            using (WWW wwwImage = new WWW(url)) {
                yield return wwwImage;

                if (wwwImage.responseHeaders.Count > 0) {
                    foreach (KeyValuePair<string, string> entry in wwwImage.responseHeaders) {
                        if (entry.Key == "STATUS") {
                            //Debug.Log(entry.Value);
                            if (entry.Value == "HTTP/1.1 404 Not Found") {
                                //Debug.Log("No se encontro la imagen");
                                manager.GetComponent<appManager>().setImagen("http://sii.uveg.edu.mx/unity/dropsV2/img/invitado.png");
                                path = manager.GetComponent<appManager>().getImagen().Split('/')[manager.GetComponent<appManager>().getImagen().Split('/').Length - 1];
                            }
                            //Debug.Log(path);
                            if (File.Exists(Application.persistentDataPath + path)) {
                                byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + path);
                                Texture2D texture = new Texture2D(8, 8);
                                texture.LoadImage(byteArray);
                                Rect rec = new Rect(0, 0, texture.width, texture.height);
                                var sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
                                imagen.sprite = sprite;
                            } else {
                                if (manager.isOnline) {
                                    WWW www = new WWW(manager.GetComponent<appManager>().getImagen());
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

    /**
     * Funcion que se manda llamar al tener un paquete listo para jugar
     * Inserta la tarjeta fichaPaqueteJugar en listaPaquetes
     * @pack paqueteData estructura que tiene los datos del paquete a jugar
     */
    public void newCardJugar(webServicePaquetes.paqueteData pack, GameObject lista) {
        var fichaPaquete = Instantiate(Resources.Load("fichaPaqueteJugar") as GameObject);
        fichaPaquete.name = "fichaPack" + pack.id;
        StartCoroutine(llenarFicha(fichaPaquete, pack.urlImagen));
        if (lista == null) {
            fichaPaquete.transform.SetParent(listaPaquetes.transform);
        } else {
            fichaPaquete.transform.SetParent(lista.transform);
        }
        fichaPaquete.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        fichaPaquete.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        fichaPaquete.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        fichaPaquete.GetComponent<packManager>().paquete = pack;
    }

    /**
     * Funcion que se manda llamar al tener un paquete listo para jugar aunque es posible actualizarlo
     * Inserta la tarjeta fichaPaqueteActualizar en listaPaquetes
     * @pack paqueteData estructura que tiene los datos del paquete a jugar
     */
    public void newCardActualizar(webServicePaquetes.paqueteData pack, GameObject lista) {
        var fichaPaquete = Instantiate(Resources.Load("fichaPaqueteActualizar") as GameObject);
        fichaPaquete.name = "fichaPack" + pack.id;
        StartCoroutine(llenarFicha(fichaPaquete, pack.urlImagen));
        if (lista == null) {
            fichaPaquete.transform.SetParent(listaPaquetes.transform);
        } else {
            fichaPaquete.transform.SetParent(lista.transform);
        }
        fichaPaquete.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        fichaPaquete.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        fichaPaquete.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        fichaPaquete.GetComponent<packManager>().paquete = pack;
    }

    /**
     * Funcion que se manda llamar al tener un paquete sin descargar
     * Inserta la tarjeta fichaPaquete en listaPaquetesNuevos
     * @pack paqueteData estructura que tiene los datos del paquete a descargar
     */
    public void newCardDescarga(webServicePaquetes.paqueteData pack) {
        var fichaPaquete = Instantiate(Resources.Load("fichaPaquete") as GameObject);
        fichaPaquete.name = "fichaPack" + pack.id;
        StartCoroutine(llenarFicha(fichaPaquete, pack.descripcion, pack.urlImagen));
        fichaPaquete.transform.SetParent(listaPaquetesNuevos.transform);
        fichaPaquete.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        fichaPaquete.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        fichaPaquete.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        fichaPaquete.GetComponent<packManager>().paquete = pack;
    }

    /**
     * Funcion que se manda llamar cuando termina de insertar todas las 
     * tarjetas de paquetes en sus respectivos lugares
     * Llena los espacios vacios con placeHoldrs para que la cuadricula se vea bien.
     * (Solo se utiliza en en objeto listaPaquetes)
     */
    public void fillEmpty(GameObject contentTab) {
        if (contentTab == null) {
            contentTab = listaPaquetes;
        }
        var hijos = contentTab.GetComponentsInChildren<packManager>(true);
        if (hijos.Length <= 3) {
            var obj = Instantiate(Resources.Load("placeHolder")) as GameObject;
            obj.transform.position = new Vector3(0, 0, 0);
            obj.transform.SetParent(contentTab.transform);
            obj.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            obj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
            obj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            fillEmpty(contentTab);
        }
        contentTab.GetComponent<gridScrollLayout>().bandera = true;
        contentTab.GetComponent<gridScrollLayout>().estaAjustado = false;
        listaPaquetesNuevos.GetComponent<gridScrollLayout>().bandera = true;
    }

    /**
     * Funcion que se manda llamar al hacer click en el btnConfiguracion
     * Apaga el raycastTarget de los botones de paquetes
     * Muestra u oculta el modalConfiguracion
     * @isVisible bool que se encarga de activar o desactivar el modal
     */
    public void setVisibleModal(bool isVisible) {
        configuracionModal.SetActive(isVisible);
        if (isVisible == false) {
            foreach (var ray in gameObject.GetComponentsInChildren<GraphicRaycaster>(true)) {
                ray.enabled = true;
            }
            //gameObject.GetComponent<GraphicRaycaster>().enabled = true;
            manager.numeroPreguntas = scrollBar.GetComponent<Slider>().value;
            webServicePreferencias.updatePreferenciaSqlite(manager.getUsuario(), manager.numeroPreguntas, manager.mascotaActive, manager.getFondo());
        } else {
            foreach (var ray in gameObject.GetComponentsInChildren<GraphicRaycaster>(true)) {
                ray.enabled = false;
            }
            //gameObject.GetComponent<GraphicRaycaster>().enabled = false;
        }
    }

    /**
     * Coroutine que llena los datos de las tarjetas que se insertan dependiendo el paquete
     * @ficha referencia al GameObject de la tarjeta
     * @descripcion descripcion del paquete
     * @urlImagen imagen del paquete que se inserta
     */
    IEnumerator llenarFicha(GameObject ficha, string descripcion, string urlImagen) {
        ficha.transform.GetChild(1).GetComponent<Text>().text = descripcion;
        string path = urlImagen.Split('/')[urlImagen.Split('/').Length - 1];
        if (File.Exists(Application.persistentDataPath + path)) {
            byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + path);
            Texture2D texture = new Texture2D(8, 8);
            texture.LoadImage(byteArray);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            ficha.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
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
            ficha.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        }
    }

    /**
     * Coroutine que llena los datos de las tarjetas que se insertan dependiendo el paquete
     * @ficha referencia al GameObject de la tarjeta
     * @urlImagen imagen del paquete que se inserta
     */
    IEnumerator llenarFicha(GameObject ficha, string urlImagen) {
        string path = urlImagen.Split('/')[urlImagen.Split('/').Length - 1];
        if (File.Exists(Application.persistentDataPath + path)) {
            byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + path);
            Texture2D texture = new Texture2D(8, 8);
            texture.LoadImage(byteArray);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            ficha.GetComponent<Image>().sprite = sprite;
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
            ficha.GetComponent<Image>().sprite = sprite;
        }
    }

    public void activarTodos(GameObject todos) {
        tabActivo.SetActive(false);
        tabActivo = todos;
        tabActivo.SetActive(true);
        controlPanel(1);
    }

    public void destruirObjetos(GameObject contentTab) {
        if (contentTab == null) {
            contentTab = listaPaquetes;
        }
        if (contentTab.transform.childCount > 0) {
            for (var i = 0; i < contentTab.transform.childCount; i++) {
                var objeto = contentTab.transform.GetChild(i);
                DestroyImmediate(objeto.gameObject);
            }
            destruirObjetos(contentTab);
        } else {
            return;
        }
    }

    public void setFondo(int fondo) {
        manager.setFondo(fondo);
        GameObject.FindObjectOfType<fondoManager>().cambiarFondo();
    }


    /**
    * Funcion que se manda llamar al hacer click en el boton salir
    * Cierra la aplicacion de manera segura.
    */
    public void logOut() {
        manager.isFirstLogin = true;
        manager.setUsuario(null);
        manager.setNombre(null);
        manager.setCorreo(null);
        manager.setImagen(null);
        manager.setGradoEstudios(null);
        webServiceRegistro.validarAccionSqlite("Logout", manager.getUsuario(), "Cerrar sesión");
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("mainMenu", "mainMenu"));
    }

    /**
     * Funcion que se manda llamar al hacer click en el boton salir
     * Cierra la aplicacion de manera segura.
     */
    public void salir() {
        Application.Quit();
    }

    /**
     * Funcion que se manda llamar para activar y desactivar paneles de paquetes, nuevos paquetes y tabs
     * Modifica la fecha de termino del log en la base de datos local
     */
    public void controlPanel(int option) {
        if (option == 1) {
            panelPaquetes.SetActive(true);
            panelDescargas.SetActive(false);
            panelTabs.SetActive(false);
            panelTabsBtn.SetActive(true);
            setVisibleModal(false);
        } else if (option == 2) {
            panelPaquetes.SetActive(false);
            panelDescargas.SetActive(true);
            panelTabs.SetActive(false);
            panelTabsBtn.SetActive(false);
            setVisibleModal(false);
        } else if (option == 3) {
            panelPaquetes.SetActive(false);
            panelDescargas.SetActive(false);
            panelTabs.SetActive(true);
            panelTabsBtn.SetActive(false);
        } else {
            panelPaquetes.SetActive(true);
            panelDescargas.SetActive(false);
            panelTabs.SetActive(false);
            panelTabsBtn.SetActive(true);
        }
    }
}
