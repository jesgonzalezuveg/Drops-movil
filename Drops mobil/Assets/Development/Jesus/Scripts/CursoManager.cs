using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Networking;

public class CursoManager : MonoBehaviour {
    bool animPar1 = false;
    bool animPar2 = false;
    bool animPar3 = false;
    public Color[] colores;
    public RuntimeAnimatorController[] animaciones;
    public string[] letras =  new string[25];
    public string respuestaFraseCompletada = "";
    public string fraseACompletarPublica = "";


    private float time = 30.0f;
    private float timeDelete;
    private float timeClockPerSecond;
    private float tiempo;
    //private bool aumento = false;
    private bool comenzarPregunta = false;
    public Text Tiempo;
    SyncroManager sicroManager;
    // public GameObject panelCompletarPalabra;
    public Text textoRachaMax;
    public Text textoAciertos;
    public Text textoRacha;

    public Text textoMultiplicador;
    public Text textoPuntajeObtenido;
    public Text textoPuntaje;
    public Text textoPuntajeMarcador;
    public Text textoCompletado;
    public GameObject correctoimg;
    public GameObject incorrectoimg;
    public Text preguntaText;

    private AudioClip bien;
    private UnityEngine.Object bienSp;
    private AudioClip genial;
    private UnityEngine.Object genialSp;
    private AudioClip asombroso;
    private UnityEngine.Object asombrosoSp;
    private AudioClip excelente;
    private UnityEngine.Object excelenteSp;
    private AudioClip perfect;
    private UnityEngine.Object perfectSp;
    private AudioClip woow;
    private UnityEngine.Object woowSp;
    private AudioClip increible;
    private UnityEngine.Object increibleSp;
    private AudioClip sigueAsi;
    private UnityEngine.Object sigueAsiSp;
    private AudioClip ops;
    private UnityEngine.Object opsSp;

    public GameObject respuestaNormal;
    public GameObject respuestaRelacionar;
    public GameObject respuestaCompletar;
    public GameObject respuestaCompletarNueva;
    public GameObject btnValidarPalabra;
    public GameObject respuestaTexto;
    public GameObject canvasParentOfAnswers;
    public Image PanelTiempo;

    //Variables para prefabs Drag&Drop
    public GameObject itemDrag;
    public GameObject Slot;
    public GameObject NoSlot;

    //Variables para Paneles Drag&Drop
    public GameObject Respuesta;
    public GameObject PanelLetras;

    public GameObject scoreFinal;
    public GameObject PanelScore;
    public GameObject PanelPregunta;
    public GameObject PanelRespuestas;

    private List<GameObject> objsRespuestas = new List<GameObject>();
    private List<string> relacionesPares = new List<string>();
    private List<int> posRespuestas = new List<int>();

    public Image imagenPack;
    public Text NombrePaquete;

    webServicePreguntas.preguntaData[] preguntas = null;
    appManager manager;

    string descripcionTipoEjercicio;

    int countPreguntas = 0;
    int score;
    int mayorRacha = 0;
    int racha = 0;
    int aciertos = 0;
    int multiplicador;

    int contador = 0;

    int numPreguntas = 0;
    int maxPuntosPorPartida = 0;
    int correctasAContestar = 0;
    int correctas = 0;
    string parUno = "";
    string parDos = "";
    bool seleccion = false;
    string fraseACompletar = "";
    string fraseCompletada = "";

    string idIntento = "";
    string idPregunta = "";
    string idRespuesta = "";



    IEnumerator putImagenPack(string urlImagen) {
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
            var spriteObj = Resources.Load("preloadedCoverPacks/portadaPaqueteCarga");
            Texture2D tex = spriteObj as Texture2D;
            Rect rec = new Rect(0, 0, tex.width, tex.height);
            sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
        }

        imagenPack.sprite = sprite;
    }

    void Awake() {
        timeClockPerSecond = 1 / time;
        timeDelete = time-1;
        respuestaFraseCompletada = "";
        tiempo = time;
        fraseACompletar = "";
        fraseCompletada = "";
        animPar1 = false;
        animPar2 = false;
        animPar3 = false;
    }

    void Start() {
        textoCompletado.text = "-";
        scoreFinal.SetActive(false);

        bien = Resources.Load("audios/Correct") as AudioClip;
        bienSp = Resources.Load("UserInterface/Dab");

        genial = Resources.Load("audios/Correct") as AudioClip;
        genialSp = Resources.Load("UserInterface/Dab");

        asombroso = Resources.Load("audios/Correct") as AudioClip;
        asombrosoSp = Resources.Load("UserInterface/Dab");

        excelente = Resources.Load("audios/Correct") as AudioClip;
        excelenteSp = Resources.Load("UserInterface/Dab");

        perfect = Resources.Load("audios/Correct") as AudioClip;
        perfectSp = Resources.Load("UserInterface/Dab");

        woow = Resources.Load("audios/Correct") as AudioClip;
        woowSp = Resources.Load("UserInterface/Dab");

        increible = Resources.Load("audios/Correct") as AudioClip;
        increibleSp = Resources.Load("UserInterface/Dab");

        sigueAsi = Resources.Load("audios/Correct") as AudioClip;
        sigueAsiSp = Resources.Load("UserInterface/Dab");

        ops = Resources.Load("audios/wrong") as AudioClip;
        opsSp = Resources.Load("UserInterface/mal");

        mayorRacha = 0;
        racha = 0;
        aciertos = 0;
        maxPuntosPorPartida = 0;
        multiplicador = 1;
        textoPuntajeObtenido.text = "";
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        preguntas = manager.preguntasCategoria;
        numPreguntas = preguntas.Length;
        var urlImagen = webServicePaquetes.getPaquetesByDescripcionSqLite(preguntas[0].descripcionPaquete).urlImagen;
        NombrePaquete.text = preguntas[0].descripcionPaquete;
        StartCoroutine(putImagenPack(urlImagen));
        maxPuntosPorPartida = 700 + ((numPreguntas - 4) * 400);
        var idUsuario = webServiceUsuario.consultarIdUsuarioSqLite(manager.getUsuario());
        var idLog = webServiceLog.getLastLogSqLite(idUsuario);
        webServiceIntento.insertarIntentoSqLite("0", manager.getUsuario());
        idIntento = webServiceIntento.consultarUltimoIdIntentoByIdLogSqLite(idLog);
        loadImagenCategoria();
        llamarPreguntas();
        if (GameObject.FindObjectOfType<appManager>()) {
            GameObject.FindObjectOfType<appManager>().cargando.SetActive(false);
        }
    }

    void loadImagenCategoria() {
        var categoria = webServiceCategoria.getCategoriaByIdSqLite(webServicePaquetes.getPaquetesByDescripcionSqLite(preguntas[0].descripcionPaquete).idCategoria).descripcion;
        categoria = categoria.Replace(" ", string.Empty);
        Debug.Log(categoria);
        var textura = Resources.Load(categoria) as Texture2D;
        Debug.Log(textura);
        Rect rec = new Rect(0, 0, textura.width, textura.height);
        var sprite = Sprite.Create(textura, rec, new Vector2(0.5f, 0.5f), 100);
        PanelRespuestas.GetComponent<Image>().sprite = sprite;
    }

    private void Update() {
        if (comenzarPregunta == true) {
            if (tiempo > 0) {
                tiempo -= Time.deltaTime;
            }
            float t = Mathf.Round(tiempo);
            if (tiempo <= (time / 3)) {
                Tiempo.color = colores[1];
            } else if(tiempo < ((time / 3) * 2) && tiempo > (time / 3)){
                Tiempo.color = colores[2];
            } else if(tiempo >= ((time / 3) * 2)){
                Tiempo.color = colores[0];
            }

            Tiempo.text = "" + Mathf.Round(tiempo).ToString();
            if (Mathf.Round(tiempo)==timeDelete) {
                PanelTiempo.fillAmount = PanelTiempo.fillAmount - timeClockPerSecond;
                timeDelete--;
            }

            if (tiempo <= 0.1f) {
                idRespuesta = "0";
                correctas = -1;
                comenzarPregunta = false;
            }
        }

        if (correctas >= 0 && countPreguntas < preguntas.Length) {
            switch (descripcionTipoEjercicio) {
                case "Seleccion simple":
                    if (correctas >= correctasAContestar) {
                        webServiceRegistro.validarAccionSqlite("Respondió correctamente(Simple)", manager.getUsuario(), "Respondió pregunta");
                        respuestaCorrecta();
                    }
                    break;
                case "Completar palabra":
                    //panelCompletarPalabra.SetActive(true);
                    //Metodo original de ejercicio completar palabra
                    //textoCompletado.text = fraseCompletada;
                    //if (fraseCompletada == fraseACompletar) {
                    //    webServiceRegistro.validarAccionSqlite("Respondió correctamente(Completar palabra): " + fraseCompletada, manager.getUsuario(), "Respondió pregunta");
                    //    fraseCompletada = "";
                    //    webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idPregunta, idRespuesta, idIntento);
                    //    textoCompletado.text = fraseACompletar;
                    //    comenzarPregunta = false;
                    //    respuestaCorrecta();
                    //}

                    //textoCompletado.text = respuestaFraseCompletada;
                    if (respuestaFraseCompletada == fraseACompletar) {
                        webServiceRegistro.validarAccionSqlite("Respondió correctamente(Completar palabra): " + respuestaFraseCompletada, manager.getUsuario(), "Respondió pregunta");
                        respuestaFraseCompletada = "";
                        Array.Clear(letras, 0, letras.Length);
                        webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idPregunta, idRespuesta, idIntento);
                        textoCompletado.text = fraseACompletar;
                        comenzarPregunta = false;
                        respuestaCorrecta();
                    }
                    break;
                case "Seleccion Multiple":
                    textoCompletado.text = "Respuestas faltantes " + (correctasAContestar - correctas);
                    if (correctas >= correctasAContestar) {
                        comenzarPregunta = false;
                        webServiceRegistro.validarAccionSqlite("Respondió correctamente(Seleccion Multiple)", manager.getUsuario(), "Respondió pregunta");
                        respuestaCorrecta();
                    }
                    break;
                case "Relacionar":
                    if (contador > 1 && contador <= 60) {
                        contador++;
                        textoCompletado.text = "Par encontrado";
                    } else if (contador == 0 || contador > 60) {
                        contador = 0;
                        textoCompletado.text = "Selecciona par 1";
                    }

                    if (seleccion) {
                        contador = 1;
                        textoCompletado.text = "Selecciona par 2";
                        if (parDos != "") {
                            if (parUno == parDos) {
                                webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idPregunta, idRespuesta, idIntento);
                                correctas++;
                                parUno = "a";
                                parDos = "";
                                seleccion = false;
                                contador = 2;
                            } else {
                                seleccion = false;
                                parUno = "a";
                                parDos = "";
                                correctas = -1;
                            }
                        }
                        if (correctas >= correctasAContestar / 2) {
                            comenzarPregunta = false;
                            webServiceRegistro.validarAccionSqlite("Respondió correctamente(Relacionar)", manager.getUsuario(), "Respondió pregunta");
                            seleccion = false;
                            respuestaCorrecta();
                        }
                    }
                    break;
                case "Seleccion simple texto":
                    if (correctas >= correctasAContestar) {
                        webServiceRegistro.validarAccionSqlite("Respondió correctamente(Seleccion simple texto)", manager.getUsuario(), "Respondió pregunta");
                        respuestaCorrecta();
                    }
                    break;
                default:

                    break;
            }
        } else if (correctas < 0) {
            if (descripcionTipoEjercicio == "Relacionar") {
                foreach (var anim in canvasParentOfAnswers.GetComponentsInChildren<Animator>()) {
                    anim.enabled = true;
                }
            }
            textoCompletado.text = "Respuesta: " + fraseACompletar;
            textoPuntajeObtenido.text = "";
            webServiceRegistro.validarAccionSqlite("Respondió incorrectamente(" + descripcionTipoEjercicio + ")", manager.getUsuario(), "Respondió pregunta");
            countPreguntas++;
            correctas = 0;
            racha = 0;
            multiplicador = 1;
            textoRacha.text = "0";
            textoMultiplicador.text = "X1";
            textoMultiplicador.transform.parent.parent.gameObject.SetActive(false);
            Handheld.Vibrate();
            StartCoroutine(activaObjeto(incorrectoimg));
            descripcionTipoEjercicio = "";
            webServiceIntento.updateIntentoSqlite(idIntento, score.ToString());
            webServiceDetalleIntento.insertarDetalleIntentoSqLite("False", idPregunta, idRespuesta, idIntento);
            //panelCompletarPalabra.SetActive(false);
        }
    }

    public void respuestaCorrecta() {
        if (descripcionTipoEjercicio != "Relacionar") {
            foreach (var anim in canvasParentOfAnswers.GetComponentsInChildren<Animator>()) {
                anim.enabled = true;
            }
        }
        aciertos++;
        countPreguntas++;
        correctas = 0;
        verificarRacha();
        StartCoroutine(activaObjeto(correctoimg));
        descripcionTipoEjercicio = "";
        webServiceIntento.updateIntentoSqlite(idIntento, score.ToString());
        //panelCompletarPalabra.SetActive(false);
    }

    public void verificarRacha() {
        int puntajePregunta;
        racha++;
        if (racha > mayorRacha) {
            mayorRacha = racha;
        }
        textoRacha.text = racha + "";
        if (racha >= 2) {
            puntajePregunta = 100 * multiplicador;
            //PuntajeObtenido.SetActive(false);
            textoPuntajeObtenido.text = "+" + puntajePregunta + "";
            score = score + puntajePregunta;
            if (multiplicador < 4) {
                multiplicador++;
            }
            textoMultiplicador.text = "X" + multiplicador;
            textoMultiplicador.transform.parent.parent.gameObject.SetActive(false);
            textoMultiplicador.transform.parent.parent.gameObject.SetActive(true);
        } else {
            puntajePregunta = 100;
            //PuntajeObtenido.SetActive(false);
            textoPuntajeObtenido.text = "+" + puntajePregunta + "";
            score = score + puntajePregunta;
        }
    }

    public void llamarPreguntas() {
        //panelCompletarPalabra.SetActive(false);
        if (countPreguntas < preguntas.Length) {
            textoCompletado.text = "-";
            respuestaFraseCompletada = "";
            idPregunta = preguntas[countPreguntas].id;
            descripcionTipoEjercicio = preguntas[countPreguntas].descripcionEjercicio;
            webServiceRegistro.validarAccionSqlite("Pregunta: " + preguntas[countPreguntas].descripcion, manager.getUsuario(), "Entró a pregunta");
            switch (descripcionTipoEjercicio) {
                case "Seleccion simple":
                    imprimePregunta();
                    break;
                case "Completar palabra":
                    if (canvasParentOfAnswers.GetComponent<GridLayoutGroup>()) {
                        Destroy(canvasParentOfAnswers.GetComponent<GridLayoutGroup>());
                    }
                    fraseCompletada = "";
                    imprimePregunta();
                    textoCompletado.text = "Intentos 3";
                    break;
                case "Seleccion Multiple":
                    imprimePregunta();
                    break;
                case "Relacionar":
                    contador = 0;
                    imprimePregunta();
                    break;
                case "Seleccion simple texto":
                    imprimePregunta();
                    break;
                default:
                    break;
            }
            comenzarPregunta = true;
        } else {
            destroyChildrens();
            textoPuntaje.text = "";
            textoMultiplicador.text = "";
            textoPuntajeMarcador.text = score + "";
            textoRachaMax.text = mayorRacha + "";
            textoAciertos.text = aciertos + "";
            PanelTiempo.gameObject.SetActive(false);
            Tiempo.gameObject.SetActive(false);
            getNota();
            webServiceRegistro.validarAccionSqlite("Puntaje obtenido: " + score, manager.getUsuario(), "Puntaje obtenido");
            webServiceRegistro.validarAccionSqlite("Terminó ejercicio", manager.getUsuario(), "Terminó ejercicio");

            int dificultad = 1;
            int nPreguntas = preguntas.Length;
            if (nPreguntas <= 5) {
                dificultad = 1;
            }else if (nPreguntas > 5 && nPreguntas <= 10) {
                dificultad = 2;
            } else if(nPreguntas > 10 && nPreguntas <= 15) {
                dificultad = 3;
            } else if(nPreguntas > 15){
                dificultad = 4;
            }

            int exitEstadistica = webServiceEstadistica.existPuntajePaqueteSqlite(dificultad, preguntas[0].idPaquete, manager.getIdUsuario());
            Debug.Log("HAY PUNTAJES DE ESTE PAQUETE " + exitEstadistica);
            if (exitEstadistica == 0) {
                if (webServiceEstadistica.insertarEstadisticasSqLite(score.ToString(), dificultad, preguntas.Length, preguntas[0].idPaquete, manager.getIdUsuario()) != 1) {
                    Debug.Log("ERROR AL INSERTAR EN LA TABLA ESTADISTICAS");
                } else {
                    Debug.Log("SE INSERTO CORRECTAMENTE EN LA TABLA ESTADISTICA");
                }
            }else{
                if (webServiceEstadistica.valPuntajePaqueteSqlite(score.ToString(), dificultad, preguntas[0].idPaquete, manager.getIdUsuario()) != 1) {
                    Debug.Log("NO SE ENCONTRARON PUNTAJES MAYORES");
                    if (webServiceEstadistica.updatePuntajePaqueteSqlite(score.ToString(), preguntas.Length, dificultad, preguntas[0].idPaquete, manager.getIdUsuario()) ==1) {
                        Debug.Log("SE MODIFICO CORRRECTAMENTE EL PUNTAJE");
                    } else {
                        Debug.Log("ERROR AL MODIFICAR EL PUNTAJE");
                    }
                } else {
                    Debug.Log("SE ENCONTRARON PUNTAJES MAYORES");
                }
            }
        }
    }

    public void getNota() {
        PanelPregunta.SetActive(false);
        PanelScore.SetActive(false);
        PanelRespuestas.SetActive(false);
        scoreFinal.SetActive(true);
        float promedio = (aciertos * 10.0f) / numPreguntas;
        string nota = "";
        string modificador = null;
        if (promedio == 10.0f) {
            nota = "S";
            modificador = null;
        } else if (promedio >= 9.5f && promedio < 10.0f) {
            nota = "A";
            modificador = "+";
        } else if (promedio >= 9.0f && promedio < 9.5f) {
            nota = "A";
            modificador = null;
        } else if (promedio >= 8.5f && promedio < 9f) {
            nota = "B";
            modificador = "+";
        } else if (promedio >= 8.0f && promedio < 8.5f) {
            nota = "B";
            modificador = null;
        } else if (promedio >= 7.5f && promedio < 8f) {
            nota = "C";
            modificador = "+";
        } else if (promedio >= 7.0f && promedio < 7.5f) {
            nota = "C";
            modificador = null;
        } else if (promedio >= 6.5f && promedio < 7.0f) {
            nota = "D";
            modificador = "+";
        } else if (promedio >= 6.0f && promedio < 6.5f) {
            nota = "D";
            modificador = null;
        } else if (promedio < 6.0f) {
            nota = "F";
            modificador = null;
        }
        var imagenNota = GameObject.Find("letraNota").GetComponent<Image>();
        var spriteObjNota = Resources.Load("Letras/letra-" + nota);
        Texture2D texNota = spriteObjNota as Texture2D;
        Rect recNota = new Rect(0, 0, texNota.width, texNota.height);
        var spriteNota = Sprite.Create(texNota, recNota, new Vector2(0.5f, 0.5f), 100);
        imagenNota.sprite = spriteNota;

        if (modificador != null) {
            var imagenModificador = GameObject.Find("modificadorNota").GetComponent<Image>();
            var spriteObjModificador = Resources.Load("Letras/letra-" + modificador);
            Texture2D texModificador = spriteObjModificador as Texture2D;
            Rect recModificador = new Rect(0, 0, texModificador.width, texModificador.height);
            var spriteModificador = Sprite.Create(texModificador, recModificador, new Vector2(0.5f, 0.5f), 100);
            imagenModificador.sprite = spriteModificador;
        } else {
            GameObject.Find("modificadorNota").SetActive(false);
        }

    }

    public void imprimePregunta() {
        preguntaText.text = preguntas[countPreguntas].descripcion;
        if (webServiceRespuestas.getRespuestasByPreguntaSqLite(preguntas[countPreguntas].id) != null) {
            if (descripcionTipoEjercicio == "Completar palabra") {
                var respuestasOfQuestion = webServiceRespuestas.getRespuestasByPreguntaSqLite(preguntas[countPreguntas].id).respuestas[0];
                idRespuesta = respuestasOfQuestion.id;
                var palabra = respuestasOfQuestion.descripcion;
                llenarLetras(palabra);
                fraseACompletar = palabra.ToUpper();
                fraseACompletarPublica = palabra.ToUpper();
            } else {
                var respuestasOfQuestion = webServiceRespuestas.getRespuestasByPreguntaSqLite(preguntas[countPreguntas].id);
                llenarRespuestas(respuestasOfQuestion.respuestas);
            }
        } else {
            countPreguntas = preguntas.Length;
            manager.cambiarEscena("menuCategorias", "menuCategorias");
        }
    }

    public void llenarRespuestas(webServiceRespuestas.respuestaData[] respuestas) {
        var numberOfObjects = respuestas.Length;
        if (numberOfObjects == 6) {
            //gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 30, 0));
        }
        var radius = 4f;
        //int p = 1;
        int i = 0;
        destroyChildrens();
        if (!canvasParentOfAnswers.GetComponent<GridLayoutGroup>()) {
            var x = canvasParentOfAnswers.AddComponent<GridLayoutGroup>();
            x.padding.top = 25;
            x.cellSize = new Vector2(325, 325);
            x.spacing = new Vector2(50, 50);
            x.startAxis = GridLayoutGroup.Axis.Horizontal;
            x.childAlignment = TextAnchor.UpperCenter;
            x.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            x.constraintCount = 2;
        }
        foreach (var respuesta in respuestas) {
            if (respuesta.correcto == "True") {
                correctasAContestar++;
            }
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            crearBoton(respuesta, angle, radius);
            i++;
        }
        StartCoroutine(deleteGrid());
    }

    public void llenarLetras(string palabra) {
        palabra = palabra.ToUpper();
        crearPanelRespuesta(palabra);
        crearPanelLetras(palabra);
        StartCoroutine(deleteGrid());
    }

    //Metodo Original Completar Palabra
    //public void llenarLetras(string palabra) {
    //    palabra = palabra.ToUpper();
    //    var letras = shuffleArray(palabra);
    //    var numberOfObjects = palabra.Length;
    //    var radius = 4f;
    //    int p = 1;
    //    int i = 0;
    //    if (!canvasParentOfAnswers.GetComponent<GridLayoutGroup>()) {
    //        var x = canvasParentOfAnswers.AddComponent<GridLayoutGroup>();
    //        x.padding.top = 25;
    //        x.cellSize = new Vector2(325, 325);
    //        x.spacing = new Vector2(50, 50);
    //        x.startAxis = GridLayoutGroup.Axis.Horizontal;
    //        x.childAlignment = TextAnchor.UpperCenter;
    //        x.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    //        x.constraintCount = 2;
    //    }
    //    foreach (char caratcter in letras) {
    //        float angle = i * Mathf.PI * 2 / numberOfObjects;
    //        crearBotonLetra(caratcter, angle, radius);
    //        i++;
    //    }
    //    StartCoroutine(deleteGrid());
    //}

    //Metodo Teclado Completar palabra
    //public void llenarLetras(string palabra) {
    //    palabra = palabra.ToUpper();
    //    var letras = shuffleArray(palabra);
    //    var numberOfObjects = palabra.Length;
    //    var radius = 4f;
    //    int p = 1;
    //    int i = 0;
    //    if (!canvasParentOfAnswers.GetComponent<GridLayoutGroup>()) {
    //        var x = canvasParentOfAnswers.AddComponent<GridLayoutGroup>();
    //        x.padding.top = 50;
    //        x.cellSize = new Vector2(700, 100);
    //        x.spacing = new Vector2(20, 20);
    //        x.startAxis = GridLayoutGroup.Axis.Horizontal;
    //        x.childAlignment = TextAnchor.UpperCenter;
    //        x.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    //        x.constraintCount = 1;
    //    }
    //    crearBotonLetra();
    //    StartCoroutine(deleteGrid());
    //}

    IEnumerator deleteGrid() {
        yield return new WaitForSeconds(.3f);
        if (canvasParentOfAnswers.GetComponent<GridLayoutGroup>()) {
            Destroy(canvasParentOfAnswers.GetComponent<GridLayoutGroup>());
        }
    }

    //Metodo Teclado Completar palabra
    //public void crearBotonLetra() {
    //    var x = Instantiate(respuestaCompletarNueva, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
    //    x.transform.SetParent(canvasParentOfAnswers.transform, false);
    //    x.AddComponent<clickManager>();

    //    var y = Instantiate(btnValidarPalabra, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
    //    y.transform.SetParent(canvasParentOfAnswers.transform, false);
    //    y.AddComponent<clickManager>();
    //    addEvent(x, y);
    //}

    public void crearPanelRespuesta(string palabra) {
        var x = Instantiate(Respuesta);
        x.AddComponent<Slot2>();
        x.transform.SetParent(canvasParentOfAnswers.transform, false);
        x.name = "Respuesta";
        dividirPalabra(palabra, x, true);
    }

    public void crearPanelLetras(string palabra) {
        var x = Instantiate(PanelLetras, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        x.transform.SetParent(canvasParentOfAnswers.transform, false);
        x.name = "PanelLetras";
        dividirPalabra(palabra, x, false);
    }

    public void dividirPalabra(string palabra, GameObject panel, bool onlySlot) {
        var letras = shuffleArray(palabra);
        var numberOfObjects = palabra.Length;
        var radius = 4f;
        //int p = 1;
        int i = 0;
        foreach (char caratcter in letras) {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            crearBotonLetra(caratcter, angle, radius, panel, onlySlot);
            i++;
        }
    }

    public void crearBotonLetra(char respuesta, float angle, float radius, GameObject panel, bool onlySlot) {
        GameObject x;
        if (!onlySlot) {
            x = Instantiate(Slot, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        } else {
            x = Instantiate(NoSlot, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        }
        //var x = Instantiate(Slot, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        x.transform.SetParent(panel.transform, false);
        int index = x.transform.GetSiblingIndex();
        if (!onlySlot) {
            x.name = x.name +"L"+ index;
            var img = Instantiate(itemDrag, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            img.transform.SetParent(x.transform, false);
            var spriteObj = Resources.Load("Letras/letra-" + respuesta);
            var imagen = img.gameObject.GetComponent<Image>();
            Texture2D tex = spriteObj as Texture2D;
            Rect rec = new Rect(0, 0, tex.width, tex.height);
            var sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
            imagen.sprite = sprite;
            imagen.name = ""+respuesta;

            var hijos = panel.transform.childCount;
            panel.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
            panel.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            if (hijos >= 21) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(70f, 70f);
                panel.GetComponent<GridLayoutGroup>().constraintCount = 5;
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(70, 20);
                x.GetComponent<GridLayoutGroup>().spacing = new Vector2(60f, 60f);
            } else if (hijos >= 17 && hijos <= 20) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(100f, 100f);
                panel.GetComponent<GridLayoutGroup>().constraintCount = 4;
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(50, 20);
                x.GetComponent<GridLayoutGroup>().spacing = new Vector2(90f, 90f);
            } else if (hijos >= 13 && hijos <= 16) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(100f, 100f);
                panel.GetComponent<GridLayoutGroup>().constraintCount = 4;
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(70, 20);
                x.GetComponent<GridLayoutGroup>().spacing = new Vector2(90f, 90f);
            } else if (hijos >= 7 && hijos <= 12) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(150f, 150f);
                panel.GetComponent<GridLayoutGroup>().constraintCount = 3;
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(20, 20);
                x.GetComponent<GridLayoutGroup>().spacing = new Vector2(140f, 140f);
            } else if (hijos < 7) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(200f, 200f);
                panel.GetComponent<GridLayoutGroup>().constraintCount = 2;
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(20, 20);
                x.GetComponent<GridLayoutGroup>().spacing = new Vector2(190f, 190f);
            }
        } else {
            x.name = x.name + "R" + index;
            var hijos = panel.transform.childCount;
            panel.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.Flexible;
            panel.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
            if (hijos >= 14) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(40f, 40f);
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 10);
            } else if (hijos >= 12 && hijos <= 13) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(50f, 50f);
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 10);
            } else if (hijos >= 9 && hijos <= 11) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(60f, 60f);
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 10);
            } else if (hijos >= 7 && hijos <= 8) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(80f, 80f);
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 10);
            } else if (hijos < 7) {
                panel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(100f, 100f);
                panel.GetComponent<GridLayoutGroup>().spacing = new Vector2(10, 10);
            }

            //addEvent(x, respuesta);
        }
    }

    public void crearBoton(webServiceRespuestas.respuestaData respuesta, float angle, float radius) {
        try {
            GameObject respuestaToLoad = respuestaNormal;
            canvasParentOfAnswers.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            switch (descripcionTipoEjercicio) {
                case "Seleccion simple texto":
                    respuestaToLoad = respuestaTexto;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().cellSize = new Vector2(777f, 130f);
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().constraintCount = 1;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().spacing = new Vector2(50, 50);
                    break;
                case "Seleccion simple":
                    respuestaToLoad = respuestaNormal;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().cellSize = new Vector2(325.0f, 325.0f);
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().constraintCount = 2;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().spacing = new Vector2(50, 50);
                    break;
                case "Seleccion Multiple":
                    respuestaToLoad = respuestaNormal;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().cellSize = new Vector2(325.0f, 325.0f);
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().constraintCount = 2;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().spacing = new Vector2(50, 50);
                    break;
                case "Relacionar":
                    respuestaToLoad = respuestaRelacionar;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().cellSize = new Vector2(250f, 275f);
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().constraintCount = 3;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().spacing = new Vector2(20, 20);
                    break;
            }
            var x = Instantiate(respuestaToLoad, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            x.transform.SetParent(canvasParentOfAnswers.transform, false);
            if (descripcionTipoEjercicio != "Seleccion simple texto") {
                if (Int32.Parse(preguntas[countPreguntas].idPaquete) > 10) {
                    var splitUrk = respuesta.urlImagen.Split('/');
                    var path = splitUrk[splitUrk.Length - 1];
                    byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + path);
                    Texture2D texture = new Texture2D(8, 8);
                    texture.LoadImage(byteArray);
                    Rect rec = new Rect(0, 0, texture.width, texture.height);
                    var sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
                    x.GetComponent<Image>().sprite = sprite;
                } else {
                    var splitUrl = respuesta.urlImagen.Split('.');
                    var spriteObj = Resources.Load("preloadedPacks/" + splitUrl[0]);
                    var imagen = x.GetComponentInChildren<Image>().gameObject.GetComponent<Image>();
                    Texture2D tex = spriteObj as Texture2D;
                    Rect rec = new Rect(0, 0, tex.width, tex.height);
                    var sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
                    //imagen.sprite = sprite;
                    x.GetComponent<Image>().sprite = sprite;
                }
            } else {
                x.GetComponentInChildren<Text>().text = respuesta.descripcion;
                //llenar texto en base a la respuesta
            }

            if (descripcionTipoEjercicio == "Relacionar") {
                addEventPares(x, respuesta);
            } else {
                addEvent(x, respuesta);
            }
        } catch (Exception ex) {
            Debug.Log("No se encontro la imagen: " + ex);
        }
    }

    //Metodo Teclado Completar palabra
    //void addEvent(GameObject objInput, GameObject objBtn) {
    //    objBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate {
    //        comenzarPregunta = false;
    //        fraseCompletada = objInput.GetComponentInChildren<InputField>().text.ToUpper();
    //        if (fraseCompletada == fraseACompletar) {
    //            webServiceRegistro.validarAccionSqlite("Respondió correctamente(Completar palabra): " + fraseCompletada, manager.getUsuario(), "Respondió pregunta");
    //            fraseCompletada = "";
    //            webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idPregunta, idRespuesta, idIntento);
    //            textoCompletado.text = fraseACompletar;
    //            respuestaCorrecta();
    //        } else {
    //            textoCompletado.text = "Respuesta: " + fraseACompletar;
    //            webServiceRegistro.validarAccionSqlite("Respondió incorrectamente: " + fraseCompletada, manager.getUsuario(), "Respondió pregunta");
    //            correctas = -1;
    //            racha = 0;
    //            multiplicador = 1;
    //            textoRacha.text = "";
    //            textoMultiplicador.text = "X1";
    //        }
    //        objBtn.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
    //    });
    //}

    void addEvent(GameObject obj, char caracter) {
        obj.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            fraseCompletada += caracter;
            if (fraseCompletada[fraseCompletada.Length - 1] == fraseACompletar[fraseCompletada.Length - 1]) {

            } else {
                textoCompletado.text = "Respuesta: " + fraseACompletar;
                webServiceRegistro.validarAccionSqlite("Respondió incorrectamente: " + fraseCompletada, manager.getUsuario(), "Respondió pregunta");
                correctas = -1;
                racha = 0;
                multiplicador = 1;
                textoRacha.text = "0";
                textoMultiplicador.text = "";

            }
            Destroy(obj);
        });
    }

    void addEvent(GameObject obj, webServiceRespuestas.respuestaData respuesta) {
        if (descripcionTipoEjercicio == "Seleccion simple") {
            if (respuesta.correcto == "True") {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[7];
                obj.GetComponentInChildren<Animator>().enabled = false;
            } else {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[6];
                obj.GetComponentInChildren<Animator>().enabled = false;
            }
        } else if (descripcionTipoEjercicio == "Seleccion Multiple") {
            if (respuesta.correcto != "True") {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[6];
                obj.GetComponentInChildren<Animator>().enabled = false;
            }

        } else if (descripcionTipoEjercicio == "Seleccion simple texto") {
            if (respuesta.correcto != "True") {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[8];
                obj.GetComponentInChildren<Animator>().enabled = false;
            }
        }


        objsRespuestas.Add(obj);
        if (respuesta.correcto == "True") {
            posRespuestas.Add(1);
        } else {
            posRespuestas.Add(0);
        }
        obj.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            if (descripcionTipoEjercicio != "Seleccion Multiple") {
                comenzarPregunta = false;
            }

            idRespuesta = respuesta.id;
            if (respuesta.correcto == "True") {
                webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idPregunta, respuesta.id, idIntento);
                correctas++;
                Color color = colores[0];
                color.a = 0.5f;
                obj.GetComponentInChildren<Button>().GetComponentInChildren<Image>().color = color;
                obj.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            } else {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[9];
                obj.GetComponentInChildren<Animator>().enabled = true;
                correctas = -1;
            }
            //Destroy(obj);
        });
    }

    void addEventPares(GameObject obj, webServiceRespuestas.respuestaData respuesta) {
        if (respuesta.relacion == "1") {
            if (animPar1 == false) {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[0];
                animPar1 = true;
            } else {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[1];
                animPar1 = false;
            }
            obj.GetComponentInChildren<Animator>().enabled = false;
        } else if(respuesta.relacion == "2"){
            if (animPar2 == false) {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[2];
                animPar2 = true;
            } else {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[3];
                animPar2 = false;
            }
            obj.GetComponentInChildren<Animator>().enabled = false;
        } else if (respuesta.relacion == "3") {
            if (animPar3 == false) {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[4];
                animPar3 = true;
            } else {
                obj.GetComponentInChildren<Animator>().runtimeAnimatorController = animaciones[5];
                animPar3 = false;
            }
            obj.GetComponentInChildren<Animator>().enabled = false;
        }
        objsRespuestas.Add(obj);
        relacionesPares.Add(respuesta.relacion);
        obj.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            idRespuesta = respuesta.id;
            if (seleccion) {
                parDos = respuesta.relacion;
                if (respuesta.relacion == "1") {
                    colorRespuestas(obj, colores[3]);
                } else if (respuesta.relacion == "2") {
                    colorRespuestas(obj, colores[2]);
                } else if (respuesta.relacion == "3") {
                    colorRespuestas(obj, colores[0]);
                } else {
                    Debug.Log("CursoManager line 628");
                    Debug.Log("Error al momento de asignar la relacion de un par.");
                }
            } else { 
                if (respuesta.relacion == "1") {
                    colorRespuestas(obj, colores[3]);
                }else if (respuesta.relacion == "2") {
                    colorRespuestas(obj, colores[2]);
                } else if (respuesta.relacion == "3") {
                    colorRespuestas(obj, colores[0]);
                } else {
                    Debug.Log("CursoManager line 628");
                    Debug.Log("Error al momento de asignar la relacion de un par.");
                }
                parUno = respuesta.relacion;
                webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idPregunta, idRespuesta, idIntento);
                seleccion = true;
            }
            idRespuesta = respuesta.id;
            //Destroy(obj);
        });
    }

    void valAudio(GameObject obj) {
        switch (racha) {
            case 0:
                obj.GetComponentInChildren<AudioSource>().clip = ops;
                Texture2D tex = opsSp as Texture2D;
                Rect rec = new Rect(0, 0, tex.width, tex.height);
                var sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
                //obj.GetComponentInChildren<Image>().sprite = sprite;
                break;
            case 1:
            case 2:
                obj.GetComponentInChildren<AudioSource>().clip = bien;
                Texture2D tex2 = bienSp as Texture2D;
                Rect rec2 = new Rect(0, 0, tex2.width, tex2.height);
                var sprite2 = Sprite.Create(tex2, rec2, new Vector2(0.5f, 0.5f), 100);
                //obj.GetComponentInChildren<Image>().sprite = sprite2;
                break;
            case 3:
                obj.GetComponentInChildren<AudioSource>().clip = genial;
                Texture2D tex3 = genialSp as Texture2D;
                Rect rec3 = new Rect(0, 0, tex3.width, tex3.height);
                var sprite3 = Sprite.Create(tex3, rec3, new Vector2(0.5f, 0.5f), 100);
                //obj.GetComponentInChildren<Image>().sprite = sprite3;
                break;
            case 4:
                obj.GetComponentInChildren<AudioSource>().clip = asombroso;
                Texture2D tex4 = asombrosoSp as Texture2D;
                Rect rec4 = new Rect(0, 0, tex4.width, tex4.height);
                var sprite4 = Sprite.Create(tex4, rec4, new Vector2(0.5f, 0.5f), 100);
                //obj.GetComponentInChildren<Image>().sprite = sprite4;
                break;
            case 5:
                obj.GetComponentInChildren<AudioSource>().clip = excelente;
                Texture2D tex5 = excelenteSp as Texture2D;
                Rect rec5 = new Rect(0, 0, tex5.width, tex5.height);
                var sprite5 = Sprite.Create(tex5, rec5, new Vector2(0.5f, 0.5f), 100);
                //obj.GetComponentInChildren<Image>().sprite = sprite5;
                break;
            default:
                int random = UnityEngine.Random.Range(0, 4);
                UnityEngine.Object spriteACargar = null;
                switch (random) {
                    case 0:
                        obj.GetComponentInChildren<AudioSource>().clip = perfect;
                        spriteACargar = perfectSp;
                        break;
                    case 1:
                        obj.GetComponentInChildren<AudioSource>().clip = woow;
                        spriteACargar = woowSp;
                        break;
                    case 2:
                        obj.GetComponentInChildren<AudioSource>().clip = sigueAsi;
                        spriteACargar = sigueAsiSp;
                        break;
                    case 3:
                        obj.GetComponentInChildren<AudioSource>().clip = increible;
                        spriteACargar = increibleSp;
                        break;
                }
                Texture2D tex6 = spriteACargar as Texture2D;
                Rect rec6 = new Rect(0, 0, tex6.width, tex6.height);
                var sprite6 = Sprite.Create(tex6, rec6, new Vector2(0.5f, 0.5f), 100);
                //obj.GetComponentInChildren<Image>().sprite = sprite6;
                break;
        }
    }

    void colorRespuestas(GameObject obj, Color color) {
        color.a = 0.5f;
        obj.GetComponentInChildren<Button>().GetComponentInChildren<Image>().color = color;
        obj.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
    }

    IEnumerator activaObjeto(GameObject objeto) {
        if (descripcionTipoEjercicio != "Completar palabra" && descripcionTipoEjercicio != "Relacionar") {
            int pos = 0;
            foreach (GameObject obj in objsRespuestas) {
                if (posRespuestas[pos] == 1) {
                    colorRespuestas(obj, colores[0]);
                    pos++;
                } else {
                    colorRespuestas(obj, colores[1]);
                    pos++;
                }
            }
        } else if(descripcionTipoEjercicio == "Relacionar"){
            int pos2 = 0;
            foreach (GameObject obj in objsRespuestas) {
                if (relacionesPares[pos2] == "1") {
                    colorRespuestas(obj, colores[3]);
                    pos2++;
                }else if(relacionesPares[pos2] == "2") {
                    colorRespuestas(obj, colores[2]);
                    pos2++;
                }else if (relacionesPares[pos2] == "3") {
                    colorRespuestas(obj, colores[0]);
                    pos2++;
                } else {
                    Debug.Log("CursoManager line 749");
                    Debug.Log("Error al momento mostrar las respuestas de la pregunta de tipo relacion.");
                }
            }
        }
        textoCompletado.text = "Respuesta: " + fraseACompletar;
        valAudio(objeto);
        objeto.SetActive(true);
        objeto.GetComponentInChildren<AudioSource>().Play();
        yield return new WaitUntil(() => objeto.GetComponentInChildren<AudioSource>().isPlaying == false);
        //yield return new WaitForSeconds(0.8f);
        textoPuntaje.text = score + "";
        objeto.SetActive(false);
        correctasAContestar = 0;
        textoPuntajeObtenido.text = "";
        fraseACompletar = "";
        //Variables control de retroalimentacion
        objsRespuestas.Clear();
        relacionesPares.Clear();
        posRespuestas.Clear();
        ////////
        ///Variables para control del tiempo
        comenzarPregunta = false;
        tiempo = time;
        PanelTiempo.fillAmount = 1f;
        timeDelete = time - 1;
        //////
        destroyChildrens();
        llamarPreguntas();
    }

    void destroyChildrens() {
        if (canvasParentOfAnswers.transform.childCount > 0) {
            foreach (Transform child in canvasParentOfAnswers.transform) {
                Destroy(child.gameObject);
            }
            //Destroy childs Abraham
            //foreach (var obj in canvasParentOfAnswers.GetComponentsInChildren<Canvas>()) {
            //    if (obj) {
            //        DestroyImmediate(obj.gameObject);
            //    }
            //}
        }
    }

    public void salir() {
        sicroManager = GameObject.Find("SincroManager").GetComponent<SyncroManager>();
        if (manager.isOnline) {
            GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(true, "Sincronizando datos");
        }
        sicroManager.synchronizationInRealTime();
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(false, "");
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("menuCategorias", "menuCategorias"));
    }

    public void reiniciar() {
        sicroManager = GameObject.Find("SincroManager").GetComponent<SyncroManager>();
        if (manager.isOnline) {
            GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(true, "Sincronizando datos");
        }
        sicroManager.synchronizationInRealTime();
        GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(false, "");
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("salon", "menuCategorias"));
    }

    public webServicePreguntas.preguntaData[] shuffleArray(webServicePreguntas.preguntaData[] preguntas) {
        for (int t = 0; t < preguntas.Length; t++) {
            var tmp = preguntas[t];
            int r = UnityEngine.Random.Range(t, preguntas.Length);
            preguntas[t] = preguntas[r];
            preguntas[r] = tmp;
        }
        return preguntas;
    }

    public char[] shuffleArray(string palabra) {
        char[] palabraChar = palabra.ToCharArray();
        for (int t = 0; t < palabraChar.Length; t++) {
            var tmp = palabraChar[t];
            int r = UnityEngine.Random.Range(t, palabraChar.Length);
            palabraChar[t] = palabraChar[r];
            palabraChar[r] = tmp;
        }
        return palabraChar;
    }

    void OnApplicationQuit() {
        if (manager.getUsuario() != "") {
            webServiceRegistro.validarAccionSqlite("El usuario no termino la partida", manager.getUsuario(), "Ejercicio sin terminar");
            webServiceLog.cerrarLog(manager.getUsuario());
        }
    }

    public void intentosCompletarPalabra(int intentos) {
        if (intentos == 0) {
            //idRespuesta = "0";
            correctas = -1;
            comenzarPregunta = false;

        }
    }

}
