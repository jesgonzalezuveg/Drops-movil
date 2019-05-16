using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class CursoManager : MonoBehaviour {

    SyncroManager sicroManager;
    public GameObject Multi;
    public GameObject PuntajeObtenido;
    public Text textoMultiplicador;
    public Text textoPuntajeObtenido;
    public Text textoPuntaje;
    public Text textoPuntajeMarcador;
    public Text textoCompletado;
    public GameObject correctoimg;
    public GameObject incorrectoimg;
    public GameObject scoreFinal;
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
    public GameObject respuestaTexto;
    public GameObject canvasParentOfAnswers;

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

    void Start() {
        bien = Resources.Load("audios/Great") as AudioClip;
        bienSp = Resources.Load("UserInterface/bien");

        genial = Resources.Load("audios/genial") as AudioClip;
        genialSp = Resources.Load("UserInterface/genial");

        asombroso = Resources.Load("audios/Awesome") as AudioClip;
        asombrosoSp = Resources.Load("UserInterface/asombroso");

        excelente = Resources.Load("audios/Excellent") as AudioClip;
        excelenteSp = Resources.Load("UserInterface/excelente");

        perfect = Resources.Load("audios/Perfect") as AudioClip;
        perfectSp = Resources.Load("UserInterface/perfecto");

        woow = Resources.Load("audios/Woow") as AudioClip;
        woowSp = Resources.Load("UserInterface/wooow");

        increible = Resources.Load("audios/Increible") as AudioClip;
        increibleSp = Resources.Load("UserInterface/increible");

        sigueAsi = Resources.Load("audios/sigueAsi") as AudioClip;
        sigueAsiSp = Resources.Load("UserInterface/sigueasi");

        ops = Resources.Load("audios/reiniciando") as AudioClip;
        opsSp = Resources.Load("UserInterface/ops");

        mayorRacha = 0;
        racha = 0;
        aciertos = 0;
        maxPuntosPorPartida = 0;
        multiplicador = 1;
        textoPuntajeObtenido.text = "";
        scoreFinal.SetActive(false);
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        preguntas = manager.preguntasCategoria;
        numPreguntas = preguntas.Length;
        maxPuntosPorPartida = 700 + ((numPreguntas - 4) * 400);
        loadSalonCategoria();
        var idUsuario = webServiceUsuario.consultarIdUsuarioSqLite(manager.getUsuario());
        var idLog = webServiceLog.getLastLogSqLite(idUsuario);
        webServiceIntento.insertarIntentoSqLite("0", manager.getUsuario());
        idIntento = webServiceIntento.consultarUltimoIdIntentoByIdLogSqLite(idLog);
        llamarPreguntas();
    }

    private void Update() {
        if (correctas >= 0 && countPreguntas < preguntas.Length) {
            switch (descripcionTipoEjercicio) {
                case "Seleccion simple":
                    if (correctas >= correctasAContestar) {
                        webServiceRegistro.validarAccionSqlite("Respondió correctamente(Simple)", manager.getUsuario(), "Respondió pregunta");
                        respuestaCorrecta();
                    }
                    break;
                case "Completar palabra":
                    textoCompletado.text = fraseCompletada;
                    if (fraseCompletada == fraseACompletar) {
                        webServiceRegistro.validarAccionSqlite("Respondió correctamente(Completar palabra): " + fraseCompletada, manager.getUsuario(), "Respondió pregunta");
                        fraseCompletada = "";
                        fraseACompletar = "l";
                        webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idPregunta, idRespuesta, idIntento);
                        textoCompletado.text = "";
                        respuestaCorrecta();
                    }
                    break;
                case "Seleccion Multiple":
                    textoCompletado.text = "Preguntas faltantes " + (correctasAContestar - correctas);
                    if (correctas >= correctasAContestar) {
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
            textoCompletado.text = "";
            webServiceRegistro.validarAccionSqlite("Respondió incorrectamente(" + descripcionTipoEjercicio + ")", manager.getUsuario(), "Respondió pregunta");
            countPreguntas++;
            correctas = 0;
            racha = 0;
            multiplicador = 1;
            //textoRacha.text = "";
            textoMultiplicador.text = "";
            StartCoroutine(activaObjeto(incorrectoimg));
            webServiceIntento.updateIntentoSqlite(idIntento, score.ToString());
            webServiceDetalleIntento.insertarDetalleIntentoSqLite("False", idPregunta, idRespuesta, idIntento);
        }
    }

    public void respuestaCorrecta() {
        aciertos++;
        countPreguntas++;
        correctas = 0;
        verificarRacha();
        StartCoroutine(activaObjeto(correctoimg));
        webServiceIntento.updateIntentoSqlite(idIntento, score.ToString());
        textoCompletado.text = "";
    }

    public void verificarRacha() {
        int puntajePregunta;
        racha++;
        if (racha > mayorRacha) {
            mayorRacha = racha;
        }
        //textoRacha.text = racha + "";
        if (racha >= 2) {
            puntajePregunta = 100 * multiplicador;
            textoPuntajeObtenido.text = "+" + puntajePregunta + "";
            score = score + puntajePregunta;
            if (multiplicador < 4) {
                multiplicador++;
            }
            Multi.SetActive(false);
            textoMultiplicador.text = "X" + multiplicador;
            Multi.SetActive(true);
        } else {
            puntajePregunta = 100;
            textoPuntajeObtenido.text = "+" + puntajePregunta + "";
            score = score + puntajePregunta;
        }
    }

    public void llamarPreguntas() {
        if (countPreguntas < preguntas.Length) {
            textoCompletado.text = "-";
            idPregunta = preguntas[countPreguntas].id;
            descripcionTipoEjercicio = preguntas[countPreguntas].descripcionEjercicio;
            webServiceRegistro.validarAccionSqlite("Pregunta: " + preguntas[countPreguntas].descripcion, manager.getUsuario(), "Entró a pregunta");
            switch (descripcionTipoEjercicio) {
                case "Seleccion simple":
                    imprimePregunta();
                    break;
                case "Completar palabra":
                    fraseCompletada = "";
                    imprimePregunta();
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
        } else {
            destroyChildrens();
            textoPuntaje.text = "";
            textoMultiplicador.text = "";
            textoPuntajeMarcador.text = score + "";
            //textoRachaMax.text = mayorRacha + "";
            //textoAciertos.text = aciertos + "";
            getNota();
            webServiceRegistro.validarAccionSqlite("Puntaje obtenido: " + score, manager.getUsuario(), "Puntaje obtenido");
            webServiceRegistro.validarAccionSqlite("Terminó ejercicio", manager.getUsuario(), "Terminó ejercicio");
            scoreFinal.SetActive(true);
        }
    }

    public void getNota() {
        float promedio = (aciertos * 10.0f) / numPreguntas;
        Debug.Log(promedio);
        string nota = "";
        string modificador = null;
        if (promedio == 10.0f) {
            //textoNotaLetra.text = "S";
            nota = "S";
        } else if (promedio >= 9.5f && promedio < 10.0f) {
            //textoNotaLetra.text = "A+";
            nota = "A";
            modificador = "+";
        } else if (promedio >= 9.0f && promedio < 9.5f) {
            //textoNotaLetra.text = "A";
            nota = "A";
        } else if (promedio >= 8.5f && promedio < 9f) {
            //textoNotaLetra.text = "B+";
            nota = "B";
            modificador = "+";
        } else if (promedio >= 8.0f && promedio < 8.5f) {
            //textoNotaLetra.text = "B";
            nota = "B";
        } else if (promedio >= 7.5f && promedio < 8f) {
            //textoNotaLetra.text = "C+";
            nota = "C";
            modificador = "+";
        } else if (promedio >= 7.0f && promedio < 7.5f) {
            //textoNotaLetra.text = "C";
            nota = "C";
        } else if (promedio >= 6.5f && promedio < 7.0f) {
            //textoNotaLetra.text = "D+";
            nota = "D";
            modificador = "+";
        } else if (promedio >= 6.0f && promedio < 6.5f) {
            //textoNotaLetra.text = "D";
            nota = "D";
        } else if (promedio < 6.0f) {
            //textoNotaLetra.text = "F";
            nota = "F";
        }
        scoreFinal.SetActive(true);
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
            } else {
                var respuestasOfQuestion = webServiceRespuestas.getRespuestasByPreguntaSqLite(preguntas[countPreguntas].id);
                llenarRespuestas(respuestasOfQuestion.respuestas);
            }
        } else {
            Debug.Log("No hay respuestas");
            countPreguntas = preguntas.Length;
            manager.cambiarEscena("menuCategorias", "menuCategorias");
        }
    }

    public void llenarRespuestas(webServiceRespuestas.respuestaData[] respuestas) {
        var numberOfObjects = respuestas.Length;
        if (numberOfObjects == 6) {
            gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 30, 0));
        }
        var radius = 4f;
        int p = 1;
        int i = 0;
        destroyChildrens();
        foreach (var respuesta in respuestas) {
            if (respuesta.correcto == "True") {
                correctasAContestar++;
            }
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            crearBoton(respuesta, angle, radius);
            i++;
        }
    }

    public void llenarLetras(string palabra) {
        palabra = palabra.ToUpper();
        var letras = shuffleArray(palabra);
        var numberOfObjects = palabra.Length;
        var radius = 4f;
        int p = 1;
        int i = 0;
        foreach (char caratcter in letras) {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            crearBotonLetra(caratcter, angle, radius);
            i++;
        }
    }

    public void crearBotonLetra(char respuesta, float angle, float radius) {
        Vector3 pos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
        var x = Instantiate(respuestaCompletar, pos, Quaternion.Euler(new Vector3(0, 0, 0)));
        x.transform.SetParent(canvasParentOfAnswers.transform, false);
        x.transform.LookAt(GameObject.Find("CenterEyeAnchor").transform);
        var rotation = x.transform.localRotation.eulerAngles;
        rotation += new Vector3(-21, 180, 0);
        x.transform.localRotation = Quaternion.Euler(rotation);
        x.AddComponent<clickManager>();
        Debug.Log("Letras/letra-" + respuesta);
        var spriteObj = Resources.Load("Letras/letra-" + respuesta);
        var imagen = x.GetComponentInChildren<Button>().gameObject.GetComponent<Image>();
        Texture2D tex = spriteObj as Texture2D;
        Rect rec = new Rect(0, 0, tex.width, tex.height);
        var sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
        imagen.sprite = sprite;
        addEvent(x, respuesta);
    }

    public void crearBoton(webServiceRespuestas.respuestaData respuesta, float angle, float radius) {
        try {
            GameObject respuestaToLoad;
            switch (descripcionTipoEjercicio) {
                case "Seleccion simple texto":
                    respuestaToLoad = respuestaTexto;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().cellSize = new Vector2(325.0f,325.0f);
                    break;
                case "Seleccion simple":
                    respuestaToLoad = respuestaNormal;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().cellSize = new Vector2(325.0f, 325.0f);
                    break;
                case "Seleccion Multiple":
                    respuestaToLoad = respuestaNormal;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().cellSize = new Vector2(325.0f, 325.0f);
                    break;
                case "Relacionar":
                    respuestaToLoad = respuestaRelacionar;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().cellSize = new Vector2(257.5f, 194.3f);
                    break;
                case "Completar palabra":
                    respuestaToLoad = respuestaCompletar;
                    canvasParentOfAnswers.GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 100);
                    break;
            }

            var x = Instantiate(respuestaCompletar, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            x.transform.SetParent(canvasParentOfAnswers.transform, false);

            if (descripcionTipoEjercicio == "Relacionar") {
                addEventPares(x, respuesta);
            } else {
                addEvent(x, respuesta);
            }
        } catch (Exception ex) {
            Debug.Log("No se encontro la imagen: " + ex);
        }
    }

    void addEvent(GameObject obj, char caracter) {
        obj.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            fraseCompletada += caracter;
            if (fraseCompletada[fraseCompletada.Length - 1] == fraseACompletar[fraseCompletada.Length - 1]) {

            } else {
                textoCompletado.text = "";
                webServiceRegistro.validarAccionSqlite("Respondió incorrectamente: " + fraseCompletada, manager.getUsuario(), "Respondió pregunta");
                correctas = -1;
                racha = 0;
                multiplicador = 1;
                //textoRacha.text = "";
                textoMultiplicador.text = "";
            }
            Destroy(obj);
        });
    }

    void addEvent(GameObject obj, webServiceRespuestas.respuestaData respuesta) {
        obj.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            idRespuesta = respuesta.id;
            if (respuesta.correcto == "True") {
                webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idPregunta, respuesta.id, idIntento);
                correctas++;
            } else {
                correctas = -1;
            }
            Destroy(obj);
        });
    }

    void addEventPares(GameObject obj, webServiceRespuestas.respuestaData respuesta) {
        obj.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            idRespuesta = respuesta.id;
            if (seleccion) {
                parDos = respuesta.relacion;
            } else {
                parUno = respuesta.relacion;
                webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idPregunta, idRespuesta, idIntento);
                seleccion = true;
            }
            idRespuesta = respuesta.id;
            Destroy(obj);
        });
    }

    void valAudio(GameObject obj) {
        switch (racha) {
            case 0:
                obj.GetComponentInChildren<AudioSource>().clip = ops;
                Texture2D tex = opsSp as Texture2D;
                Rect rec = new Rect(0, 0, tex.width, tex.height);
                var sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
                obj.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
                break;
            case 1:
            case 2:
                obj.GetComponentInChildren<AudioSource>().clip = bien;
                Texture2D tex2 = bienSp as Texture2D;
                Rect rec2 = new Rect(0, 0, tex2.width, tex2.height);
                var sprite2 = Sprite.Create(tex2, rec2, new Vector2(0.5f, 0.5f), 100);
                obj.GetComponentInChildren<SpriteRenderer>().sprite = sprite2;
                break;
            case 3:
                obj.GetComponentInChildren<AudioSource>().clip = genial;
                Texture2D tex3 = genialSp as Texture2D;
                Rect rec3 = new Rect(0, 0, tex3.width, tex3.height);
                var sprite3 = Sprite.Create(tex3, rec3, new Vector2(0.5f, 0.5f), 100);
                obj.GetComponentInChildren<SpriteRenderer>().sprite = sprite3;
                break;
            case 4:
                obj.GetComponentInChildren<AudioSource>().clip = asombroso;
                Texture2D tex4 = asombrosoSp as Texture2D;
                Rect rec4 = new Rect(0, 0, tex4.width, tex4.height);
                var sprite4 = Sprite.Create(tex4, rec4, new Vector2(0.5f, 0.5f), 100);
                obj.GetComponentInChildren<SpriteRenderer>().sprite = sprite4;
                break;
            case 5:
                obj.GetComponentInChildren<AudioSource>().clip = excelente;
                Texture2D tex5 = excelenteSp as Texture2D;
                Rect rec5 = new Rect(0, 0, tex5.width, tex5.height);
                var sprite5 = Sprite.Create(tex5, rec5, new Vector2(0.5f, 0.5f), 100);
                obj.GetComponentInChildren<SpriteRenderer>().sprite = sprite5;
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
                obj.GetComponentInChildren<SpriteRenderer>().sprite = sprite6;
                break;
        }
    }

    IEnumerator activaObjeto(GameObject objeto) {
        textoCompletado.text = "";
        destroyChildrens();
        valAudio(objeto);
        objeto.SetActive(true);
        objeto.GetComponentInChildren<AudioSource>().Play();
        PuntajeObtenido.SetActive(false);
        PuntajeObtenido.SetActive(true);
        yield return new WaitUntil(() => objeto.GetComponentInChildren<AudioSource>().isPlaying == false);
        textoPuntaje.text = score + "";
        objeto.SetActive(false);
        correctasAContestar = 0;
        textoPuntajeObtenido.text = "";
        PuntajeObtenido.SetActive(false);
        llamarPreguntas();
    }

    void loadSalonCategoria() {
        var categoria = webServiceCategoria.getCategoriaByIdSqLite(webServicePaquetes.getPaquetesByDescripcionSqLite(preguntas[0].descripcionPaquete).idCategoria).descripcion;
        categoria = categoria.Replace(" ", string.Empty);
        var salon = Instantiate(Resources.Load("Salones/salonTemplate" + categoria) as GameObject);
        salon.SetActive(true);
        salon.transform.localScale = new Vector3(5.216759f, 4, 4);
        salon.transform.localPosition = new Vector3(10.68f, 1.11f, 6.93f);
        salon.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
    }

    void destroyChildrens() {
        if (canvasParentOfAnswers.transform.childCount > 0) {
            foreach (var obj in canvasParentOfAnswers.GetComponentsInChildren<Canvas>()) {
                DestroyImmediate(obj.gameObject);
            }
        }
    }

    public void salir() {
        sicroManager = GameObject.Find("SincroManager").GetComponent<SyncroManager>();
        sicroManager.synchronizationInRealTime();
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("menuCategorias", "menuCategorias"));
    }

    public void reiniciar() {
        sicroManager = GameObject.Find("SincroManager").GetComponent<SyncroManager>();
        sicroManager.synchronizationInRealTime();
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

}
