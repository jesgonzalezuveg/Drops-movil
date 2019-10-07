using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;

public class paquetesManager : MonoBehaviour {

    private appManager manager;             ///< manager referencia al componente appManager
    public Image imagen;                    ///< imagen referencia a la imagen que contendra la imagen del usuario
    public GameObject textoPaquetes;        ///< textoPaquetes referencia al objeto que se muestra u oculta dependiendo si existen paquetes por descargar
    public GameObject listaPaquetes;        ///< listaPaquetes referencia al objeto que contiene los paquetes ya instalados
    public GameObject listaPaquetesNuevos;  ///< listaPaquetesNuevos referencia al objeto que contiene los paquetes nuevos por descargar
    public GameObject configuracionModal;   ///< configuracionModal referencia al modal de configuracion de curso
    public GameObject scrollBar;            ///< scrollBar referencia al scrollbar para seleccionar el numero maximo de preguntas por curso
    public Toggle audioBox;
    private bool bandera = true;            ///< bandera bandera que valida si ya se obtuo la imagen del usuario
    private bool banderaTabs = false;
    public GameObject tabToInstantiate;
    public GameObject panelTabs;
    public GameObject panelTabsBtn;
    public GameObject panelPaquetes;
    public GameObject panelDescargas;
    public GameObject panelDescargaMsj;
    public GameObject modalPaquete;

    public Font f_font1;
    public Font f_font2;

    public webServiceCategoria.categoriaData categoriaTab = null;

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
        manager.vistaLista = true;
        ajustarGridLayout();
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

    public void ajustarGridLayout() {
        if (manager.vistaLista != true) {
            listaPaquetes.GetComponent<GridLayoutGroup>().padding.top = 50;
            listaPaquetes.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            listaPaquetes.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
            listaPaquetes.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            listaPaquetes.GetComponent<GridLayoutGroup>().cellSize = new Vector2(150f, 150f);
            listaPaquetes.GetComponent<GridLayoutGroup>().constraintCount = 3;
            listaPaquetes.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 20);

            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().padding.top = 10;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().cellSize = new Vector2(280f, 280f);
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().constraintCount = 2;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 20);
        } else {
            //Vista en lista
            //listaPaquetes.GetComponent<GridLayoutGroup>().padding.top = 50;
            //listaPaquetes.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            //listaPaquetes.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
            //listaPaquetes.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            //listaPaquetes.GetComponent<GridLayoutGroup>().cellSize = new Vector2(560f, 100f);
            //listaPaquetes.GetComponent<GridLayoutGroup>().constraintCount = 1;
            //listaPaquetes.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 20);

            //listaPaquetesNuevos.GetComponent<GridLayoutGroup>().padding.top = 10;
            //listaPaquetesNuevos.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            //listaPaquetesNuevos.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
            //listaPaquetesNuevos.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            //listaPaquetesNuevos.GetComponent<GridLayoutGroup>().cellSize = new Vector2(560f, 100f);
            //listaPaquetesNuevos.GetComponent<GridLayoutGroup>().constraintCount = 1;
            //listaPaquetesNuevos.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 20);

            listaPaquetes.GetComponent<GridLayoutGroup>().padding.top = 50;
            listaPaquetes.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            listaPaquetes.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
            listaPaquetes.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            listaPaquetes.GetComponent<GridLayoutGroup>().cellSize = new Vector2(280f, 280f);
            listaPaquetes.GetComponent<GridLayoutGroup>().constraintCount = 2;
            listaPaquetes.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 20);

            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().padding.top = 10;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().cellSize = new Vector2(280f, 280f);
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().constraintCount = 2;
            listaPaquetesNuevos.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 20);
        }

        foreach (var panel in panelPaquetes.GetComponentsInChildren<GridLayoutGroup>()) {
            if (manager.vistaLista != true) {
                panel.padding.top = 50;
                panel.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                panel.startCorner = GridLayoutGroup.Corner.UpperLeft;
                panel.startAxis = GridLayoutGroup.Axis.Horizontal;
                panel.cellSize = new Vector2(150f, 150f);
                panel.constraintCount = 3;
                panel.spacing = new Vector2(10, 20);
            } else {
                //Vista Lista
                //panel.padding.top = 50;
                //panel.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                //panel.startCorner = GridLayoutGroup.Corner.UpperLeft;
                //panel.startAxis = GridLayoutGroup.Axis.Horizontal;
                //panel.cellSize = new Vector2(560f, 100f);
                //panel.constraintCount = 1;
                //panel.spacing = new Vector2(10, 20);
                panel.padding.top = 50;
                panel.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                panel.startCorner = GridLayoutGroup.Corner.UpperLeft;
                panel.startAxis = GridLayoutGroup.Axis.Horizontal;
                panel.cellSize = new Vector2(280f, 280f);
                panel.constraintCount = 2;
                panel.spacing = new Vector2(10, 20);
            }
        }
    }

    public void fillPaquetesLocal() {
        var paquetesLocales = webServicePaquetes.getPaquetesSqLite();
        if (paquetesLocales != null) {
            manager.setPaquetes(paquetesLocales.paquete);
        } else {
            newCardEmpty(listaPaquetes);
        }
        var categoriasLocal = webServiceCategoria.getCategoriasSql();
        if (categoriasLocal != null) {
            manager.setCategorias(categoriasLocal);
        }
        StartCoroutine(getUserImg());
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

            fillPaquetesLocal();
        }

        switch (manager.numeroPreguntas) {
            case 5:
                scrollBar.GetComponent<Slider>().value = 1;
                break;
            case 10:
                scrollBar.GetComponent<Slider>().value = 2;
                break;
            case 15:
                scrollBar.GetComponent<Slider>().value = 3;
                break;
            case 20:
                scrollBar.GetComponent<Slider>().value = 4;
                break;
            default:
                scrollBar.GetComponent<Slider>().value = 1;
                break;
        }
        audioBox.isOn = manager.mascotaActive;
        setVisibleModal(false);
        manager.setBanderas(true);
        tabActivo = GameObject.Find("tabContentTodos");
        posTabContent = GameObject.Find("tabContentTodos").GetComponent<RectTransform>();
        GameObject.Find("Nombre").GetComponent<Text>().text = manager.getNombre();
        GameObject.Find("Tabs").SetActive(false);
        controlPanel(1);

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
            categoriaTab = categoria;
            fillTabContent(contentTab, categoria);
            tabActivo.SetActive(false);
            contentTab.SetActive(true);
            tabActivo = contentTab;
            ajustarGridLayout();
            controlPanel(1);
        });
    }

    void fillTabContent(GameObject content, webServiceCategoria.categoriaData categoria) {
        Debug.Log("CATEGORIA" + categoria.descripcion);
        destruirObjetos(content.GetComponentInChildren<gridScrollLayout>().gameObject);
        var paquetes = webServicePaquetes.getPaquetesByCategoriaSqLite(categoria.id);
        if (paquetes != null) {
            foreach (var paquete in paquetes) {
                var descarga = webServiceDescarga.getDescargaByPaquete(paquete.id);
                if (descarga != null) {
                    Debug.Log(descarga.fechaDescarga);
                    Debug.Log("------------------------");
                    Debug.Log(paquete.fechaModificacion);
                    if (isActualized(descarga.fechaDescarga, paquete.fechaModificacion)) {
                        Debug.Log("Esta actualizado el paquete " + paquete.descripcion);
                        newCardJugar(paquete, content.GetComponentInChildren<gridScrollLayout>().gameObject);
                    } else {
                        Debug.Log("No se encuentra actualizado el paquete " + paquete.descripcion);
                        newCardActualizar(paquete, content.GetComponentInChildren<gridScrollLayout>().gameObject);
                    }
                }
            }
            newCardEmpty(content.GetComponentInChildren<gridScrollLayout>().gameObject);
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
        Sprite sprite = null;
        string path = manager.GetComponent<appManager>().getImagen().Split('/')[manager.GetComponent<appManager>().getImagen().Split('/').Length - 1];
        string url = manager.GetComponent<appManager>().getImagen();
        if (File.Exists(Application.persistentDataPath + path)) {
            byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + path);
            Texture2D texture = new Texture2D(8, 8);
            texture.LoadImage(byteArray);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            imagen.GetComponent<Image>().sprite = sprite;
        } else {
            if (manager.isOnline) {
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url)) {
                    AsyncOperation asyncLoad = www.SendWebRequest();
                    while (!asyncLoad.isDone) {
                        yield return null;
                    }
                    if (www.isNetworkError || www.isHttpError || www.responseCode == 404) {
                        Debug.Log(www.error);
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
            var spriteObj = Resources.Load("preloadedCoverPacks/invitado");
            Texture2D tex = spriteObj as Texture2D;
            Rect rec = new Rect(0, 0, tex.width, tex.height);
            sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
        }
        imagen.GetComponent<Image>().sprite = sprite;
    }

    /**
     * Funcion que se manda llamar al tener un paquete listo para jugar
     * Inserta la tarjeta fichaPaqueteJugar en listaPaquetes
     * @pack paqueteData estructura que tiene los datos del paquete a jugar
     */
    public void newCardJugar(webServicePaquetes.paqueteData pack, GameObject lista) {
        GameObject fichaPaquete;
        if (manager.vistaLista == true) {
            //fichaPaquete = Instantiate(Resources.Load("PaqueteListaFix") as GameObject);
            //fichaPaquete.GetComponent<Image>().color = fichaPaquete.GetComponent<fondoManager>().colorArray[manager.getFondo()];
            fichaPaquete = Instantiate(Resources.Load("fichaPaqueteOption") as GameObject);
        } else {
            fichaPaquete = Instantiate(Resources.Load("fichaPaqueteJugar") as GameObject);
            fichaPaquete.GetComponentInChildren<fondoManager>().transform.gameObject.GetComponent<Image>().color = fichaPaquete.GetComponentInChildren<fondoManager>().colorArray[manager.getFondo()];
        }
        fichaPaquete.name = "fichaPack" + pack.id;
        StartCoroutine(llenarFicha(fichaPaquete, pack.urlImagen, pack.id));
        if (lista == null) {
            fichaPaquete.transform.SetParent(listaPaquetes.transform);
        } else {
            fichaPaquete.transform.SetParent(lista.transform);
        }
        fichaPaquete.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        fichaPaquete.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        fichaPaquete.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        fichaPaquete.GetComponent<packManager>().paquete = pack;
        //Vista
        //if (manager.vistaLista == true) {
        //    fichaPaquete.transform.GetChild(4).gameObject.SetActive(false);
        //}

        reajustarGridLayout(listaPaquetes, 50);
    }

    /**
     * Funcion que se manda llamar al tener un paquete listo para jugar aunque es posible actualizarlo
     * Inserta la tarjeta fichaPaqueteActualizar en listaPaquetes
     * @pack paqueteData estructura que tiene los datos del paquete a jugar
     */
    public void newCardActualizar(webServicePaquetes.paqueteData pack, GameObject lista) {
        GameObject fichaPaquete;
        if (manager.vistaLista == true) {
            //fichaPaquete = Instantiate(Resources.Load("PaqueteLista") as GameObject);
            //fichaPaquete.GetComponent<Image>().color = fichaPaquete.GetComponent<fondoManager>().colorArray[manager.getFondo()];
            fichaPaquete = Instantiate(Resources.Load("fichaPaqueteOptionUpdate") as GameObject);
        } else {
            fichaPaquete = Instantiate(Resources.Load("fichaPaqueteActualizar") as GameObject);
            fichaPaquete.GetComponentInChildren<fondoManager>().transform.gameObject.GetComponent<Image>().color = fichaPaquete.GetComponentInChildren<fondoManager>().colorArray[manager.getFondo()];
        }
        fichaPaquete.name = "fichaPack" + pack.id;
        StartCoroutine(llenarFicha(fichaPaquete, pack.urlImagen, pack.id));
        if (lista == null) {
            fichaPaquete.transform.SetParent(listaPaquetes.transform);
        } else {
            fichaPaquete.transform.SetParent(lista.transform);
        }
        fichaPaquete.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        fichaPaquete.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        fichaPaquete.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        fichaPaquete.GetComponent<packManager>().paquete = pack;
        //vista
        //if (manager.vistaLista == true) {
        //    fichaPaquete.transform.GetChild(4).gameObject.SetActive(false);
        //}

        reajustarGridLayout(listaPaquetes, 50);
    }

    /**
     * Funcion que se manda llamar al tener un paquete sin descargar
     * Inserta la tarjeta fichaPaquete en listaPaquetesNuevos
     * @pack paqueteData estructura que tiene los datos del paquete a descargar
     */
    public void newCardDescarga(webServicePaquetes.paqueteData pack) {
        GameObject fichaPaquete;
        if (manager.vistaLista == true) {
            //fichaPaquete = Instantiate(Resources.Load("PaqueteListaDescarga") as GameObject);
            //fichaPaquete.GetComponent<Image>().color = fichaPaquete.GetComponent<fondoManager>().colorArray[manager.getFondo()];
            fichaPaquete = Instantiate(Resources.Load("fichaPaqueteDownload") as GameObject);
        } else {
            fichaPaquete = Instantiate(Resources.Load("fichaPaquete") as GameObject);
            Debug.Log("LINEA 423 paquetesManager IMAGEN DEL PREFAB PARA NUEVOS PAQUETES");
            //fichaPaquete.GetComponentInChildren<fondoManager>().transform.gameObject.GetComponent<Image>().color = fichaPaquete.GetComponentInChildren<fondoManager>().colorArray[manager.getFondo()];
        }
        fichaPaquete.name = "fichaPack" + pack.id;
        StartCoroutine(llenarFicha(fichaPaquete, pack.descripcion, pack.urlImagen, pack.id));
        fichaPaquete.transform.SetParent(listaPaquetesNuevos.transform);
        fichaPaquete.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        fichaPaquete.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        fichaPaquete.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        fichaPaquete.GetComponent<packManager>().paquete = pack;

        reajustarGridLayout(listaPaquetesNuevos, 330);
    }

    public void newCardEmpty(GameObject obj) {
        GameObject fichaPaquete;
        if (manager.vistaLista == true) {
            //fichaPaquete = Instantiate(Resources.Load("PaqueteListaEmpty") as GameObject);
            //fichaPaquete.GetComponent<Image>().color = fichaPaquete.GetComponent<fondoManager>().colorArray[manager.getFondo()];
            fichaPaquete = Instantiate(Resources.Load("placeHolder") as GameObject);
        } else {
            fichaPaquete = Instantiate(Resources.Load("placeHolder") as GameObject);
        }
        fichaPaquete.name = "fichaPackEmpty";
        fichaPaquete.transform.SetParent(obj.transform);
        fichaPaquete.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        fichaPaquete.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        fichaPaquete.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

        reajustarGridLayout(obj, 0);
    }

    /**
     * Funcion que se manda llamar cuando termina de insertar todas las 
     * tarjetas de paquetes en sus respectivos lugares
     * Llena los espacios vacios con placeHoldrs para que la cuadricula se vea bien.
     * (Solo se utiliza en en objeto listaPaquetes)
     */
    //public void fillEmpty(GameObject contentTab) {
    //    if (contentTab == null) {
    //        contentTab = listaPaquetes;
    //    }
    //    var hijos = contentTab.GetComponentsInChildren<packManager>(true);
    //    if (hijos.Length <= 3) {
    //        var obj = Instantiate(Resources.Load("placeHolder")) as GameObject;
    //        obj.transform.position = new Vector3(0, 0, 0);
    //        obj.transform.SetParent(contentTab.transform);
    //        obj.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    //        obj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
    //        obj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
    //        fillEmpty(contentTab);
    //    }

    //    if (manager.vistaLista == true) {
    //        contentTab.GetComponent<gridScrollLayout>().tamañoScroll = 100;
    //    } else {
    //        contentTab.GetComponent<gridScrollLayout>().tamañoScroll = 330;
    //    }
    //    contentTab.GetComponent<gridScrollLayout>().bandera = true;
    //    contentTab.GetComponent<gridScrollLayout>().estaAjustado = false;
    //    listaPaquetesNuevos.GetComponent<gridScrollLayout>().bandera = true;
    //}

    /**
     * Funcion que se manda llamar al hacer click en el btnConfiguracion
     * Apaga el raycastTarget de los botones de paquetes
     * Muestra u oculta el modalConfiguracion
     * @isVisible bool que se encarga de activar o desactivar el modal
     */
    public void setVisibleModal() {
        configuracionModal.SetActive(!configuracionModal.active);
        configuracionModal.GetComponent<Image>().color = configuracionModal.GetComponent<fondoManager>().colorArray[manager.getFondo()];
        if (configuracionModal.active == false) {
            if (panelPaquetes.active == false) {
                panelDescargas.SetActive(false);
                panelPaquetes.SetActive(true);
                panelTabsBtn.SetActive(true);
                foreach (var ficha in panelPaquetes.GetComponentsInChildren<fondoManager>()) {
                    ficha.transform.gameObject.GetComponent<Image>().color = ficha.colorArray[manager.getFondo()];
                }
            }
            foreach (var ray in gameObject.GetComponentsInChildren<GraphicRaycaster>(true)) {
                ray.enabled = true;
            }
            //gameObject.GetComponent<GraphicRaycaster>().enabled = true;
            switch (scrollBar.GetComponent<Slider>().value) {
                case 1:
                    manager.numeroPreguntas = 5;
                    break;
                case 2:
                    manager.numeroPreguntas = 10;
                    break;
                case 3:
                    manager.numeroPreguntas = 15;
                    break;
                case 4:
                    manager.numeroPreguntas = 20;
                    break;
                default:
                    manager.numeroPreguntas = 5;
                    break;
            }
            manager.mascotaActive = audioBox.isOn;
            webServicePreferencias.updatePreferenciaSqlite(manager.getUsuario(), manager.numeroPreguntas, manager.mascotaActive, manager.getFondo());
        } else {
            if (panelDescargas.active == true) {
                panelDescargas.SetActive(false);
            } else if (panelPaquetes.active == true) {
                panelPaquetes.SetActive(false);
            }

            if (panelTabs.active == true) {
                panelTabs.SetActive(false);
            }
            if (panelTabsBtn.active == true) {
                panelTabsBtn.SetActive(false);
            }

            foreach (var ray in gameObject.GetComponentsInChildren<GraphicRaycaster>(true)) {
                ray.enabled = false;
            }
            //gameObject.GetComponent<GraphicRaycaster>().enabled = false;
        }
    }

    public void setVisibleModal(bool isVisible) {
        configuracionModal.SetActive(isVisible);
        configuracionModal.GetComponent<Image>().color = configuracionModal.GetComponent<fondoManager>().colorArray[manager.getFondo()];
        if (isVisible == false) {
            foreach (var ray in gameObject.GetComponentsInChildren<GraphicRaycaster>(true)) {
                ray.enabled = true;
            }
            //gameObject.GetComponent<GraphicRaycaster>().enabled = true;
            switch (scrollBar.GetComponent<Slider>().value) {
                case 1:
                    manager.numeroPreguntas = 5;
                    break;
                case 2:
                    manager.numeroPreguntas = 10;
                    break;
                case 3:
                    manager.numeroPreguntas = 15;
                    break;
                case 4:
                    manager.numeroPreguntas = 20;
                    break;
                default:
                    manager.numeroPreguntas = 5;
                    break;
            }
            manager.mascotaActive = audioBox.isOn;
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
    IEnumerator llenarFicha(GameObject ficha, string descripcion, string urlImagen, string id) {
        Sprite sprite = null;
        ficha.GetComponentsInChildren<Text>()[0].text = descripcion;
        if (descripcion.Length>18) {
            ficha.GetComponentsInChildren<Text>()[0].fontSize = 110;
        } else if(descripcion.Length < 7){
            ficha.GetComponentsInChildren<Text>()[0].fontSize = 220;
        } else {
            ficha.GetComponentsInChildren<Text>()[0].fontSize = 150;
        }
        ficha.GetComponentsInChildren<Text>()[0].text = descripcion;
        if (Int32.Parse(id) > 10) {
            string path = urlImagen.Split('/')[urlImagen.Split('/').Length - 1];
            if (File.Exists(Application.persistentDataPath + path)) {
                //Debug.Log("ENCONTRO EL ARCHIVO");
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
                            Debug.Log("Error al tratar de llenar la ficha de paquetes descargables sin public");
                            Debug.Log(www.error);
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
        } else {
            var splitUrl = urlImagen.Split('.');
            var spriteObj = Resources.Load("preloadedCoverPacks/" + splitUrl[0]);
            Texture2D tex = spriteObj as Texture2D;
            Rect rec = new Rect(0, 0, tex.width, tex.height);
            sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
        }

        if (sprite == null) {
            sprite = portadaDefault();
        }
        ficha.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
    }

    /**
     * Coroutine que llena los datos de las tarjetas que se insertan dependiendo el paquete
     * @ficha referencia al GameObject de la tarjeta
     * @urlImagen imagen del paquete que se inserta
     */
    public IEnumerator llenarFicha(GameObject ficha, string urlImagen, string id) {
        Sprite sprite = null;
        if (Int32.Parse(id) > 10) {
            string path = urlImagen.Split('/')[urlImagen.Split('/').Length - 1];
            if (File.Exists(Application.persistentDataPath + path)) {
                //Debug.Log("ENCONTRO EL ARCHIVO");
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
                            Debug.Log("Error al tratar de llenar la ficha de paquetes descargables con public");
                            Debug.Log(www.error);
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
        } else {
            var splitUrl = urlImagen.Split('.');
            var spriteObj = Resources.Load("preloadedCoverPacks/" + splitUrl[0]);
            Texture2D tex = spriteObj as Texture2D;
            Rect rec = new Rect(0, 0, tex.width, tex.height);
            sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
        }
        if (sprite == null) {
            sprite = portadaDefault();
        }

        if (manager.vistaLista == true) {
            ficha.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        } else {
            ficha.GetComponent<Image>().sprite = sprite;
        }
    }

    public Sprite portadaDefault(){
        var spriteObj = Resources.Load("preloadedCoverPacks/portadaPaqueteCarga");
        Texture2D tex = spriteObj as Texture2D;
        Rect rec = new Rect(0, 0, tex.width, tex.height);
        return Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
    }


    public void activarTodos(GameObject todos) {
        categoriaTab = null;
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
        Debug.Log(fondo);
        foreach(var i in GameObject.FindObjectsOfType<fondoManager>()){
            i.cambiarFondo(0.5f);
        }
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
        manager.mostrarAviso = true;
        manager.getOld = 0;
        manager.paquetesPorDescargar = 0;
        int res = webServiceUsuario.updateAllSesionStatusSqlite(0);
        if (res == 0) {
            //Debug.Log("Error en paquetesManager en linea 470");
            Debug.Log("Error al modificar los status de sesion de los usuarios");
        }
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
            panelTabsBtn.GetComponent<Image>().color = panelTabsBtn.GetComponent<fondoManager>().colorArray[manager.getFondo()];
            foreach (var i in listaPaquetes.GetComponentsInChildren<fondoManager>()) {
                i.gameObject.GetComponent<Image>().color = i.colorArray[manager.getFondo()];
            }
            foreach (var btn in GameObject.FindGameObjectsWithTag("btnEliminar")) {
                btn.SetActive(false);
                btn.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
                btn.transform.parent.transform.GetChild(2).gameObject.SetActive(true);
            }
            Color col = new Color(panelPaquetes.GetComponentInChildren<Image>().color.r, panelPaquetes.GetComponentInChildren<Image>().color.g, panelPaquetes.GetComponentInChildren<Image>().color.b, 0f);
            panelPaquetes.GetComponentInChildren<Image>().color = col;
            if (GameObject.Find("PanelPaquetesBtn") && GameObject.Find("PanelDescargasBtn")) {
                GameObject.Find("PanelPaquetesBtn").GetComponentInChildren<Text>().font = f_font1;
                GameObject.Find("PanelDescargasBtn").GetComponentInChildren<Text>().font = f_font2;
            }
        } else if (option == 2) {
            panelPaquetes.SetActive(false);
            panelDescargas.SetActive(true);
            panelTabs.SetActive(false);
            panelTabsBtn.SetActive(false);
            setVisibleModal(false);
            foreach (var i in listaPaquetesNuevos.GetComponentsInChildren<fondoManager>()) {
                i.gameObject.GetComponent<Image>().color = i.colorArray[manager.getFondo()];
            }
            Color col = new Color(panelDescargas.GetComponentInChildren<Image>().color.r, panelDescargas.GetComponentInChildren<Image>().color.g, panelDescargas.GetComponentInChildren<Image>().color.b, 0f);
            panelDescargas.GetComponentInChildren<Image>().color = col;
            if (GameObject.Find("PanelPaquetesBtn") && GameObject.Find("PanelDescargasBtn")) {
                GameObject.Find("PanelPaquetesBtn").GetComponentInChildren<Text>().font = f_font2;
                GameObject.Find("PanelDescargasBtn").GetComponentInChildren<Text>().font = f_font1;
            }
        } else if (option == 3) {
            panelPaquetes.SetActive(false);
            panelDescargas.SetActive(false);
            panelTabs.SetActive(true);
            panelTabsBtn.SetActive(false);
            foreach (var i in panelTabs.GetComponentsInChildren<fondoManager>()) {
                i.gameObject.GetComponent<Image>().color = i.colorArray[manager.getFondo()];
            }
        } else {
            panelPaquetes.SetActive(true);
            panelDescargas.SetActive(false);
            panelTabs.SetActive(false);
            panelTabsBtn.SetActive(true);
        }
    }

    public void recargarPanelDescargas() {
        manager.validarConexion();
        if (manager.isOnline) {
            panelDescargaMsj.SetActive(false);
            cambiarVistaPaquetes(manager.vistaLista);
            controlPanel(2);
        } else {
            if (panelDescargaMsj.active == true) {
                panelDescargaMsj.SetActive(false);
            } else {
                panelDescargaMsj.SetActive(true);
                panelDescargaMsj.GetComponentInChildren<Text>().text = "¡No hay conexión a internet!";
            }
        }
    }

    public void panelMsj(string msj) {
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(false, "");
        panelDescargaMsj.SetActive(true);
        Debug.Log("MENSAJE DE PANELMSJ");
        panelDescargaMsj.GetComponentInChildren<Text>().text = msj;
    }

    public void cerrarPanelMsj() {
        if(panelDescargaMsj.active == true) {
            if (panelDescargas.active == true) {
                recargarPanelDescargas();
                panelDescargaMsj.SetActive(false);
            } else {
                panelDescargaMsj.SetActive(false);
            }
        }
    }



    public void cambiarVistaPaquetes(bool vista) {
        bool panelActivo = panelDescargas.active;
        manager.vistaLista = vista;
        panelDescargas.SetActive(true);
        panelPaquetes.SetActive(true);

        while (panelPaquetes.GetComponentInChildren<GridLayoutGroup>().transform.childCount > 0) {
            for (var i = 0; i < panelPaquetes.GetComponentInChildren<GridLayoutGroup>().transform.childCount; i++) {
                var objeto = panelPaquetes.GetComponentInChildren<GridLayoutGroup>().transform.GetChild(i);
                DestroyImmediate(objeto.gameObject);
            }
        }


        if (categoriaTab != null) {
            fillTabContent(tabActivo, categoriaTab);
        }

        while (listaPaquetesNuevos.transform.childCount>0) {
            for (var i = 0; i < listaPaquetesNuevos.transform.childCount; i++) {
                var objeto = listaPaquetesNuevos.transform.GetChild(i);
                DestroyImmediate(objeto.gameObject);
            }
        }
        ajustarGridLayout();
        manager.banderaPaquetes = true;
        if (panelActivo == true) {
            controlPanel(2);
        } else {
            controlPanel(1);
        }
    }

    public void reajustarGridLayout(GameObject obj, float size) {
        if (manager.vistaLista == true) {
            //obj.GetComponent<gridScrollLayout>().tamañoScroll = 100;
            obj.GetComponent<gridScrollLayout>().tamañoScroll = 330;
        } else {
            if (size == 50) {
                obj.GetComponent<gridScrollLayout>().tamañoScroll = 50;
            } else {
                obj.GetComponent<gridScrollLayout>().tamañoScroll = 330;
            }
        }
        obj.GetComponent<gridScrollLayout>().bandera = true;
        obj.GetComponent<gridScrollLayout>().estaAjustado = false;
        obj.GetComponent<gridScrollLayout>().bandera = true;
    }

}
