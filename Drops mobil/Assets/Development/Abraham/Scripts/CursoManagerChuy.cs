using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.EventSystems;

public class CursoManagerChuy: MonoBehaviour {

    public Text textoPuntaje;
    public Text textoPuntajeMarcador;
    public GameObject correctoimg;
    public GameObject incorrectoimg;
    public GameObject scoreFinal;
    public GameObject[] preguntasTipos; 

    // Variables para la poscision aleatoria
    int num;
    Vector3 temp1, temp2, temp3, temp4, temp5, temp6;
    Vector3 rotation1, rotation2, rotation3, rotation4, rotation5, rotation6;
    Transform transform1, transform2, transform3, transform4, transform5, transform6;
    GameObject Imagen1, Imagen2, Imagen3, Imagen4, Imagen5, Imagen6;
    // Variables para la poscision aleatoria

    //Variables que almacenan la preguntas y sus respuestas
    webServicePreguntas.preguntaData[] preguntas = null;
    List<webServiceRespuestas.Data> respuestasTodas = new List<webServiceRespuestas.Data>();
    appManager manager;

    //Tipo de pregunta
    string descripcionTipoEjercicio;

    //Variables que almacena el numero de preguntas descargadas y el puntaje actual
    int countPreguntas = 0;
    int score;

    //Variables para ejercicio de seleccion multiple
    int countSelectMultiple; //Contador que indica el numero de respuestas correctas en el ejercicio
    public Sprite opcionCorrecta; //Imagen que aparece cuando se selecciona una respuesta correcta.

    //Variables para ejercicio de relacionar pares.
    int numPares;//Numero de pares a relacionar
    string par1;//primer par seleccionado
    string par2;//segundo par seleccionado
    string par1Name;//Nombre del gameobject del primer par seleccionado
    string par2Name;//Nombre del gameobject del ssegundo par seleccionado

    //Variables para ejercicio de completar la frase
    int numLetras;//Numero total de letras
    int letraPos;//Valor para validar el orden de las letras de la palabra
    string palabraCompleta; //Almacena la palabra a completar
    public Text palabraCom;//Variable que almacena el avance al completar la palabra

    //Variables para registrar el intento y su detalle
    string idLog;
    string idUsuario;
    string idIntento;

    void Start() {
        par1 = "";
        par2 = "";
        par1Name = "";
        par2Name = "";
        numPares = 0;
        numLetras = 0;
        letraPos = 0;
        palabraCom.text = "";
        countSelectMultiple = 0;
        score = 0;
        textoPuntaje.text = score + "";
        correctoimg.SetActive(false);
        incorrectoimg.SetActive(false);
        scoreFinal.SetActive(false);
        desactivarPreguntas();
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        preguntas = manager.preguntasCategoria;
        var categoria = webServiceCategoria.getCategoriaByIdSqLite(webServicePaquetes.getPaquetesByDescripcionSqLite(preguntas[0].descripcionPaquete).idCategoria).descripcion;
        categoria = categoria.Replace(" ", string.Empty);
        //Debug.Log("salonTemplate" + categoria);
        var salon = Instantiate(Resources.Load("Salones/salonTemplate" + categoria));
        setSalonPosition(salon as GameObject);
        for (var i = 0; i < preguntas.Length; i++) {
            var respuestas = webServiceRespuestas.getRespuestasByPreguntaSqLite(manager.preguntasCategoria[i].id);
            respuestasTodas.Add(respuestas);
        }

        idUsuario = webServiceUsuario.consultarIdUsuarioSqLite(manager.getUsuario());
        idLog = webServiceLog.getLastLogSqLite(idUsuario);
        webServiceIntento.insertarIntentoSqLite("0", manager.getUsuario());
        idIntento = webServiceIntento.consultarUltimoIdIntentoByIdLogSqLite(idLog);
        llamarPreguntas(countPreguntas);
        
    }

    public void llamarPreguntas(int position) {
        //Debug.Log("Pregunta:"+manager.preguntasCategoria[position].descripcion);
        if (position<preguntas.Length) {
            //Debug.Log("Entro al if:");
        //if (position<6) {
            //Debug.Log("Insertar accion Entro a pregunta");
            webServiceRegistro.validarAccionSqlite("Pregunta: " + manager.preguntasCategoria[position].descripcion, manager.getUsuario(), "Entró a pregunta");
            //Debug.Log("Se inserto correctamente");
            //Validamos de que tipo de ejercicio se trata
            descripcionTipoEjercicio = webServiceEjercicio.consultarDescripcionEjercicioByIdSqLite(manager.preguntasCategoria[position].idTipoEjercicio);
            //Debug.Log(descripcionTipoEjercicio);
            if (descripcionTipoEjercicio == "Seleccion simple") {
                //Debug.Log("Entro al if tipo:");
                preguntasTipos[0].SetActive(true);
                var canvasObjS = GameObject.Find("CanvasObjS");
                for (int i = 0; i < canvasObjS.transform.childCount; i++) {
                    canvasObjS.transform.GetChild(i).gameObject.SetActive(true);
                    canvasObjS.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                }
                GameObject.Find("txtQuestionS").GetComponent<Text>().text = manager.preguntasCategoria[position].descripcion;
                int count = 1;
                for (int i = 0; i < respuestasTodas[position].respuestas.Length; i++) {
                    string url = respuestasTodas[position].respuestas[i].urlImagen;
                    var splitUrk = url.Split('/');
                    var urlReal = splitUrk[splitUrk.Length - 1];
                    //Debug.Log("Respuesta"+ urlReal);
                    ponerImagen(count + "S", urlReal, descripcionTipoEjercicio, respuestasTodas[position].respuestas[i].correcto, manager.preguntasCategoria[position].id, respuestasTodas[position].respuestas[i].id);
                    count++;
                }
            }else if (descripcionTipoEjercicio == "Completar palabra") {
                //Debug.Log("Entro al if tipo:");
                preguntasTipos[3].SetActive(true);
                var canvasObjC = GameObject.Find("CanvasObjC");
                for (int i = 0; i < canvasObjC.transform.childCount; i++) {
                    canvasObjC.transform.GetChild(i).gameObject.SetActive(true);
                    canvasObjC.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                    canvasObjC.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                GameObject.Find("txtQuestionC").GetComponent<Text>().text = manager.preguntasCategoria[position].descripcion;
                int count = 1;
                for (int i = 0; i < respuestasTodas[position].respuestas.Length; i++) {
                    palabraCompleta = respuestasTodas[position].respuestas[i].descripcion.ToUpper();
                    foreach (char letra in palabraCompleta) {
                        ponerImagenLetra(letra, count+"C", respuestasTodas[position].respuestas[i].id, manager.preguntasCategoria[position].id);
                        count++;
                    }
                    numLetras = count-1;
                }

                for (int j = count; j<= 15; j++) {
                    var objeto = "objRespuesta" + j + "C";
                    GameObject.Find(objeto).GetComponent<Image>().color = new Color(0,0,0,0);
                    GameObject.Find(objeto).SetActive(false); 
                }
            }else if (descripcionTipoEjercicio == "Seleccion Multiple") {
                //Debug.Log("Entro al if tipo:");
                preguntasTipos[1].SetActive(true);
                var canvasObjM = GameObject.Find("CanvasObjM");
                for (int i = 0; i < canvasObjM.transform.childCount; i++) {
                    canvasObjM.transform.GetChild(i).gameObject.SetActive(true);
                    canvasObjM.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                }
                GameObject.Find("txtQuestionM").GetComponent<Text>().text = manager.preguntasCategoria[position].descripcion;
                int count = 1;
                for (int i = 0; i < respuestasTodas[position].respuestas.Length; i++) {
                    string url = respuestasTodas[position].respuestas[i].urlImagen;
                    var splitUrk = url.Split('/');
                    var urlReal = splitUrk[splitUrk.Length - 1];
                    //Debug.Log("Respuesta" + urlReal);
                    //Debug.Log("Respuesta NPI: " + respuestasTodas[position].respuestas[i].correcto);
                    ponerImagen(count + "M", urlReal, descripcionTipoEjercicio, respuestasTodas[position].respuestas[i].correcto, manager.preguntasCategoria[position].id, respuestasTodas[position].respuestas[i].id);
                    count++;
                }
            }else if (descripcionTipoEjercicio == "Relacionar") {
                //Debug.Log("Entro al if tipo:");
                preguntasTipos[2].SetActive(true);
                var canvasObjP = GameObject.Find("CanvasObjP");
                for (int i = 0; i < canvasObjP.transform.childCount; i++) {
                    canvasObjP.transform.GetChild(i).gameObject.SetActive(true);
                    canvasObjP.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                    canvasObjP.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                GameObject.Find("txtQuestionP").GetComponent<Text>().text = manager.preguntasCategoria[position].descripcion;
                int count = 1;
                for (int i = 0; i < respuestasTodas[position].respuestas.Length; i++) {
                    string url = respuestasTodas[position].respuestas[i].urlImagen;
                    var splitUrk = url.Split('/');
                    var urlReal = splitUrk[splitUrk.Length - 1];
                    //Debug.Log("Respuesta" + urlReal);
                    ponerImagen(count + "P", urlReal, descripcionTipoEjercicio, respuestasTodas[position].respuestas[i].relacion, manager.preguntasCategoria[position].id, respuestasTodas[position].respuestas[i].id);
                    count++;
                    numPares++;
                }
                numPares = numPares / 2;
            } else {
                //Debug.Log("No se encontro el tipo de ejercicio");
            }
        }
    }

    public void ponerImagenLetra(char letra, string i, string idRespuesta, string idPregunta) {
        //Debug.Log("letra es: " + letra);
        //Debug.Log("i es: " + i);
        //Debug.Log("id pregunta es: " + idPregunta);
        //Debug.Log("id respuesta es: " + idRespuesta);
        var objeto = "objRespuesta" + i;
        var spriteObj= Resources.Load("Letras/letra-" + letra);
        var imagen = GameObject.Find(objeto);
        Texture2D tex = spriteObj as Texture2D;
        Rect rec = new Rect(0, 0, tex.width, tex.height);
        var sprite = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
        /*EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => {
            string obj = objeto;
            char objLetra = letra;
            validarOrden(obj, letra);

        });
        imagen.GetComponent<EventTrigger>().triggers.Add(entry);*/
        imagen.GetComponent<Button>().onClick.AddListener(delegate {
            string idI = idIntento;
            string idR = idRespuesta;
            string idP = idPregunta;
            string obj = objeto;
            char objletra = letra;
            this.validarOrden(obj, letra, idI, idR, idP);
        });

        imagen.GetComponent<Image>().sprite = sprite;
        imagen.GetComponent<RectTransform>().LookAt(GameObject.Find("Main Camera").transform);
        imagen.GetComponent<RectTransform>().Rotate(new Vector3(0,180, 0));
    }

    public void ponerImagen(string i, string path, string tipo, string respuesta, string idPregunta, string idRespuesta) {
        if (File.Exists(Application.persistentDataPath + path)) {
            byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + path);
            Texture2D texture = new Texture2D(8, 8);
            texture.LoadImage(byteArray);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            var objeto = "objRespuesta" + i;
            var imagen = GameObject.Find(objeto);
            if (tipo == "Seleccion simple") {
                if (respuesta == "True") {
                    imagen.GetComponent<Button>().onClick.AddListener(delegate {
                        string idI = idIntento;
                        string idR = idRespuesta;
                        string idP = idPregunta;
                        string correcto = respuesta;
                        string objName = objeto;
                        webServiceDetalleIntento.insertarDetalleIntentoSqLite(correcto, idP, idR, idI);
                        correctoSimple(objName, idI);
                    });
                } else {
                    imagen.GetComponent<Button>().onClick.AddListener(delegate {
                        string idI = idIntento;
                        string idR = idRespuesta;
                        string idP = idPregunta;
                        string correcto = respuesta;
                        string objName = objeto;
                        webServiceDetalleIntento.insertarDetalleIntentoSqLite(correcto, idP, idR, idI);
                        incorrecto(objeto, idI);
                    });
                }
            }else if (tipo == "Seleccion Multiple") {
                if (respuesta == "True") {
                    countSelectMultiple++;
                    imagen.GetComponent<Button>().onClick.AddListener(delegate {
                        //Debug.Log("correcto");
                        string objName = objeto;
                        string idI = idIntento;
                        string idR = idRespuesta;
                        string idP = idPregunta;
                        string correcto = respuesta;
                        webServiceDetalleIntento.insertarDetalleIntentoSqLite(correcto, idP, idR, idI);
                        correctoMultiple(objName, idI);
                    });
                } else {
                    imagen.GetComponent<Button>().onClick.AddListener(delegate {
                        //Debug.Log("incorrecto");
                        string objName = objeto;
                        string idI = idIntento;
                        string idR = idRespuesta;
                        string idP = idPregunta;
                        string correcto = respuesta;
                        webServiceDetalleIntento.insertarDetalleIntentoSqLite(correcto, idP, idR, idI);
                        countSelectMultiple = 0;
                        incorrecto(objeto, idI);
                    });
                }
            }else if (tipo == "Relacionar") {
                imagen.GetComponent<Button>().onClick.AddListener(delegate {
                    string objName = objeto;
                    string relacion = respuesta;
                    string idI = idIntento;
                    string idR = idRespuesta;
                    string idP = idPregunta;
                    validarPares(objName, relacion, idP, idR, idI);
                });
            }
            imagen.GetComponent<Image>().sprite = sprite;
            imagen.GetComponent<RectTransform>().LookAt(GameObject.Find("Main Camera").transform);
            imagen.GetComponent<RectTransform>().Rotate(new Vector3(0, 180, 0));
        }
    }

    public void correctoSimple(string obj, string idI) {
        if (descripcionTipoEjercicio != "Completar palabra") {
            webServiceRegistro.validarAccionSqlite("Respondió correctamente(Simple)", manager.getUsuario(), "Respondió pregunta");
        }
        countPreguntas++;
        //Debug.Log("correcto  --- " + preguntas.Length + "CountPreguntas: " + countPreguntas);
        StartCoroutine(esperaSegundos(correctoimg));
        score++;
        textoPuntaje.text = score + "";
        if (preguntas.Length > countPreguntas) {
            desactivarPreguntas();
            llamarPreguntas(countPreguntas);
        } else {
            desactivarPreguntas(); ;
            textoPuntajeMarcador.text = score + "";
            webServiceIntento.updateIntentoSqlite(idI, score.ToString());
            webServiceRegistro.validarAccionSqlite("Puntaje obtenido: " + score, manager.getUsuario(), "Puntaje obtenido");
            webServiceRegistro.validarAccionSqlite("Terminó ejercicio", manager.getUsuario(), "Terminó ejercicio");
            scoreFinal.SetActive(true);
        }
    }

    public void correctoMultiple(string obj, string idI) {
        var myGameObject = GameObject.Find(obj);
        countSelectMultiple--;
        if (countSelectMultiple != 0) {
            myGameObject.GetComponent<Image>().sprite = opcionCorrecta;
            //Debug.Log("Faltan respuestas");
        } else {
            countPreguntas++;
            //Debug.Log("Respuestas completas");
            StartCoroutine(esperaSegundos(correctoimg));
            score++;
            textoPuntaje.text = score + "";
            //Debug.Log(preguntas.Length);
            //Debug.Log(countPreguntas);
            if (preguntas.Length > countPreguntas) {
                desactivarPreguntas();
                webServiceRegistro.validarAccionSqlite("Respondió correctamente(Opción Multiple)", manager.getUsuario(), "Respondió pregunta");
                llamarPreguntas(countPreguntas);
            } else {
                desactivarPreguntas();
                textoPuntajeMarcador.text = score + "";
                webServiceIntento.updateIntentoSqlite(idI, score.ToString());
                webServiceRegistro.validarAccionSqlite("Puntaje obtenido: " + score, manager.getUsuario(), "Puntaje obtenido");
                webServiceRegistro.validarAccionSqlite("Terminó ejercicio", manager.getUsuario(), "Terminó ejercicio");
                scoreFinal.SetActive(true);
            }
        }
    }

    public void validarPares(string obj, string relacion, string idP, string idR, string idI) {
        var myGameObject = GameObject.Find(obj);
        if (par1 == "" && par2 == "") {
            //Debug.Log("entro a la validacion de pares");
            par1Name = obj;
            par1 = relacion;
            //webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idP, idR, idI);
            myGameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            myGameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        } else if (par1 != "" && par2 == "") {
            //Debug.Log("entro a la validacion 2 de pares");
            par2 = relacion;
            par2Name = obj;

            if (par1 == par2) {
                var par1Seleccionado = GameObject.Find(par1Name);
                var par2Seleccionado = GameObject.Find(par2Name);
                //Debug.Log("Par correcto");
                webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idP, idR, idI);
                numPares--;
                if (numPares > 0) {
                    par1Seleccionado.SetActive(false);
                    par2Seleccionado.SetActive(false);
                    par1 = "";
                    par2 = "";
                    par1Name = "";
                    par2Name = "";
                } else {
                    par1Seleccionado.SetActive(false);
                    par2Seleccionado.SetActive(false);
                    par1 = "";
                    par2 = "";
                    par1Name = "";
                    par2Name = "";
                    numPares = 0;
                    correctoPar(true, idI);
                }
            } else {
                //Debug.Log("Par incorrecto");
                par1 = "";
                par2 = "";
                par1Name = "";
                par2Name = "";
                numPares = 0;
                webServiceDetalleIntento.insertarDetalleIntentoSqLite("False", idP, idR, idI);
                correctoPar(false, idI);
            }
        }
    }

    public void validarOrden(string objName, char letraVal, string idI, string idR, string idP) {
        //Debug.Log(letraVal);
        //Debug.Log(palabraCompleta[letraPos]);
        if (palabraCompleta[letraPos]==letraVal) {
            var myGameObject = GameObject.Find(objName);
            myGameObject.SetActive(false);
            letraPos++;
            palabraCom.text = palabraCom.text+letraVal;
        } else {
            for (int i = 1; i <= 15; i++) {
                var objeto = "objRespuesta" + i + "C";
                var myGameObject = GameObject.Find(objeto);
                if (myGameObject != null) {
                    myGameObject.SetActive(false);
                }
            }
            palabraCom.text = palabraCom.text + letraVal;
            webServiceRegistro.validarAccionSqlite("Respondió incorrectamente: " + palabraCom.text, manager.getUsuario(), "Respondió pregunta");
            palabraCom.text = "";
            numLetras = 0;
            letraPos = 0;
            webServiceDetalleIntento.insertarDetalleIntentoSqLite("False", idP, idR, idI);
            incorrecto(objName, idI);
        }

        if (numLetras != 0) {
            if (numLetras == letraPos) {
                webServiceRegistro.validarAccionSqlite("Respondió correctamente: " + palabraCom.text, manager.getUsuario(), "Respondió pregunta");
                palabraCom.text = "";
                numLetras = 0;
                letraPos = 0;
                webServiceDetalleIntento.insertarDetalleIntentoSqLite("True", idP, idR, idI);
                correctoSimple(objName, idI);
            }
        }
    }

    public void correctoPar(bool res, string idI) {
        if (res==true) {
            webServiceRegistro.validarAccionSqlite("Respondió correctamente(Par)", manager.getUsuario(), "Respondió pregunta");
            countPreguntas++;
            //Debug.Log("correcto  --- " + preguntas.Length + "CountPreguntas: " + countPreguntas);
            StartCoroutine(esperaSegundos(correctoimg));
            score++;
            textoPuntaje.text = score + "";
            if (preguntas.Length > countPreguntas) {
                desactivarPreguntas();
                llamarPreguntas(countPreguntas);
            } else {
                desactivarPreguntas(); ;
                textoPuntajeMarcador.text = score + "";
                webServiceIntento.updateIntentoSqlite(idI, score.ToString());
                webServiceRegistro.validarAccionSqlite("Puntaje obtenido: " + score, manager.getUsuario(), "Puntaje obtenido");
                webServiceRegistro.validarAccionSqlite("Terminó ejercicio", manager.getUsuario(), "Terminó ejercicio");
                scoreFinal.SetActive(true);
            }
        } else {
            webServiceRegistro.validarAccionSqlite("Respondió incorrectamente(Par)", manager.getUsuario(), "Respondió pregunta");
            countPreguntas++;
            //Debug.Log("incorrecto  --- " + preguntas.Length + "CountPreguntas: " + countPreguntas);
            StartCoroutine(esperaSegundos(incorrectoimg));
            if (preguntas.Length > countPreguntas) {
                desactivarPreguntas();
                llamarPreguntas(countPreguntas);
            } else {
                desactivarPreguntas();
                textoPuntajeMarcador.text = score + "";
                webServiceIntento.updateIntentoSqlite(idI, score.ToString());
                webServiceRegistro.validarAccionSqlite("Puntaje obtenido: " + score, manager.getUsuario(), "Puntaje obtenido");
                webServiceRegistro.validarAccionSqlite("Terminó ejercicio", manager.getUsuario(), "Terminó ejercicio");
                scoreFinal.SetActive(true);
            }
        }
    }

    public void incorrecto(string objName, string idI) {
        if (descripcionTipoEjercicio != "Completar palabra") {
            webServiceRegistro.validarAccionSqlite("Respondió incorrectamente", manager.getUsuario(), "Respondió pregunta");
        }
        countPreguntas++;
        StartCoroutine(esperaSegundos(incorrectoimg));
        if (preguntas.Length > countPreguntas) {
            desactivarPreguntas();
            llamarPreguntas(countPreguntas);
        } else {
            desactivarPreguntas();
            textoPuntajeMarcador.text = score + "";
            webServiceIntento.updateIntentoSqlite(idI, score.ToString());
            webServiceRegistro.validarAccionSqlite("Puntaje obtenido: " + score, manager.getUsuario(), "Puntaje obtenido");
            webServiceRegistro.validarAccionSqlite("Terminó ejercicio", manager.getUsuario(), "Terminó ejercicio");
            scoreFinal.SetActive(true);
        }
    }

    public void desactivarPreguntas() {
        preguntasTipos[0].SetActive(false);
        preguntasTipos[1].SetActive(false);
        preguntasTipos[2].SetActive(false);
        preguntasTipos[3].SetActive(false);
    }

    IEnumerator esperaSegundos(GameObject objeto) {
        objeto.SetActive(true);
        objeto.GetComponentInChildren<AudioSource>().Play();
        yield return new WaitUntil(() => objeto.GetComponentInChildren<AudioSource>().isPlaying == false);
        //yield return new WaitForSeconds(segundos);
        objeto.SetActive(false);
    }

    public void salir() {
        SceneManager.LoadScene("menuCategorias");
    }

    void OnApplicationQuit() {
        if (manager.getUsuario() != "") {
            webServiceRegistro.validarAccionSqlite("El usuario no termino la partida", manager.getUsuario(), "Ejercicio sin terminar");
            webServiceLog.cerrarLog(manager.getUsuario());
        }
    }

    public void PosAleatoria(int tipo) {
        //el valor de la variable debe ser acorde al numero de respuestas en la pregunta
        int maxvalues = 0;
        //if para identificar a que pregunta entrará
        if (tipo == 1) {
            print("Entro a Q1");
            Imagen1 = GameObject.FindGameObjectWithTag("objRespuesta1");
            Imagen2 = GameObject.FindGameObjectWithTag("objRespuesta2");
            Imagen3 = GameObject.FindGameObjectWithTag("objRespuesta3");
            Imagen4 = GameObject.FindGameObjectWithTag("objRespuesta4");

            transform1 = Imagen1.GetComponent<Transform>();
            transform2 = Imagen2.GetComponent<Transform>();
            transform3 = Imagen3.GetComponent<Transform>();
            transform4 = Imagen4.GetComponent<Transform>();

            temp1 = transform1.position;
            temp2 = transform2.position;
            temp3 = transform3.position;
            temp4 = transform4.position;

            rotation1 = transform1.rotation.eulerAngles;
            rotation2 = transform2.rotation.eulerAngles;
            rotation3 = transform3.rotation.eulerAngles;
            rotation4 = transform4.rotation.eulerAngles;
            maxvalues = 5;
        }

        if (tipo == 2) {
            print("Entro a Q3");
            Imagen1 = GameObject.FindGameObjectWithTag("Q3.1");
            Imagen2 = GameObject.FindGameObjectWithTag("Q3.2");
            Imagen3 = GameObject.FindGameObjectWithTag("Q3.3");
            Imagen4 = GameObject.FindGameObjectWithTag("Q3.4");

            transform1 = Imagen1.GetComponent<Transform>();
            transform2 = Imagen2.GetComponent<Transform>();
            transform3 = Imagen3.GetComponent<Transform>();
            transform4 = Imagen4.GetComponent<Transform>();

            temp1 = transform1.position;
            temp2 = transform2.position;
            temp3 = transform3.position;
            temp4 = transform4.position;

            rotation1 = transform1.rotation.eulerAngles;
            rotation2 = transform2.rotation.eulerAngles;
            rotation3 = transform3.rotation.eulerAngles;
            rotation4 = transform4.rotation.eulerAngles;
            maxvalues = 5;
        }

        if (tipo == 3) {
            print("Entro a multi");
            Imagen1 = GameObject.FindGameObjectWithTag("MultiR1");
            Imagen2 = GameObject.FindGameObjectWithTag("MultiR2");
            Imagen3 = GameObject.FindGameObjectWithTag("MultiR3");
            Imagen4 = GameObject.FindGameObjectWithTag("MultiR4");
            Imagen5 = GameObject.FindGameObjectWithTag("MultiR5");

            transform1 = Imagen1.GetComponent<Transform>();
            transform2 = Imagen2.GetComponent<Transform>();
            transform3 = Imagen3.GetComponent<Transform>();
            transform4 = Imagen4.GetComponent<Transform>();
            transform5 = Imagen5.GetComponent<Transform>();

            temp1 = transform1.position;
            temp2 = transform2.position;
            temp3 = transform3.position;
            temp4 = transform4.position;
            temp5 = transform5.position;

            rotation1 = transform1.rotation.eulerAngles;
            rotation2 = transform2.rotation.eulerAngles;
            rotation3 = transform3.rotation.eulerAngles;
            rotation4 = transform4.rotation.eulerAngles;
            rotation5 = transform5.rotation.eulerAngles;
            maxvalues = 6;
        }

        if (tipo == 4) {
            print("Entro a par");
            Imagen1 = GameObject.FindGameObjectWithTag("Par1.1");
            Imagen2 = GameObject.FindGameObjectWithTag("Par1.2");
            Imagen3 = GameObject.FindGameObjectWithTag("Par2.1");
            Imagen4 = GameObject.FindGameObjectWithTag("Par2.2");
            Imagen5 = GameObject.FindGameObjectWithTag("Par3.1");
            Imagen6 = GameObject.FindGameObjectWithTag("Par3.2");

            transform1 = Imagen1.GetComponent<Transform>();
            transform2 = Imagen2.GetComponent<Transform>();
            transform3 = Imagen3.GetComponent<Transform>();
            transform4 = Imagen4.GetComponent<Transform>();
            transform5 = Imagen5.GetComponent<Transform>();
            transform6 = Imagen6.GetComponent<Transform>();

            temp1 = transform1.position;
            temp2 = transform2.position;
            temp3 = transform3.position;
            temp4 = transform4.position;
            temp5 = transform5.position;
            temp6 = transform6.position;

            rotation1 = transform1.rotation.eulerAngles;
            rotation2 = transform2.rotation.eulerAngles;
            rotation3 = transform3.rotation.eulerAngles;
            rotation4 = transform4.rotation.eulerAngles;
            rotation5 = transform5.rotation.eulerAngles;
            rotation6 = transform6.rotation.eulerAngles;
            maxvalues = 7;
        }

        //Generar en orden aleatoria los numeros de 1 al 5
        int i = 1;
        HashSet<Int32> numeros = new HashSet<Int32>();
        System.Random ran = new System.Random();

        while (numeros.Count < maxvalues) {
            numeros.Add(ran.Next(maxvalues));
        }

        foreach (int n in numeros) {
            if (n != 0) {
                num = n;
                if (i == 1) {
                    asignarPos(transform1);
                    i++;
                } else if (i == 2) {
                    asignarPos(transform2);
                    i++;
                } else if (i == 3) {
                    asignarPos(transform3);
                    i++;
                } else if (i == 4) {
                    asignarPos(transform4);
                    i++;
                } else if (i == 5) {
                    asignarPos(transform5);
                    i++;

                } else if (i == 6) {
                    asignarPos(transform6);
                    i++;

                }

            }
        }
    }

    void asignarPos(Transform imagen) {
        switch (num) {
            case 1:
                imagen.position = temp1;
                imagen.rotation = Quaternion.Euler(rotation1);

                break;
            case 2:
                imagen.position = temp2;
                imagen.rotation = Quaternion.Euler(rotation2);

                break;
            case 3:
                imagen.position = temp3;
                imagen.rotation = Quaternion.Euler(rotation3);

                break;
            case 4:
                imagen.position = temp4;
                imagen.rotation = Quaternion.Euler(rotation4);

                break;

            case 5:
                imagen.position = temp5;
                imagen.rotation = Quaternion.Euler(rotation5);

                break;
            case 6:
                imagen.position = temp6;
                imagen.rotation = Quaternion.Euler(rotation6);

                break;
        }
    }

    void setSalonPosition(GameObject salon) {
        salon.SetActive(true);
        salon.transform.localScale = new Vector3(5.216759f, 4, 4);
        salon.transform.localPosition = new Vector3(10.68f, 1.11f, 6.93f);
        salon.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
    }

}
