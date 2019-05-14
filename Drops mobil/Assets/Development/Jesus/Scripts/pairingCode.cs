using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class pairingCode : MonoBehaviour {
    //Variables que guardan el codigo generado, la segunda con guión en medio de los 6 caracteres
    string code;
    string code2;

    public static string status;
    public static int valCodigoSii;
    public static string idCodigoServer;
    private int salir;
    private int countFrames;
    private int cargaCodigo;
    UnityEvent listenerCode = new UnityEvent();

    private webServiceUsuario.userDataSqLite usuario = null;
    private webServiceLog.logData log = null;
    private WebServiceCodigo.Data codigo = null;

    public GameObject internetNecesario;

    public void setLog(webServiceLog.logData datos) {
        log = datos;
    }

    public void setUsuario(webServiceUsuario.userDataSqLite datos) {
        usuario = datos;
    }

    public void setCodigo(WebServiceCodigo.Data datos) {
        codigo = datos;
    }

    // Start is called before the first frame update
    void Awake() {
        code = generateCode();
        code2 = "";
        status = "5";
        valCodigoSii = 3;
        countFrames = 0;
        cargaCodigo = 0;
        salir = 0;
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            StartCoroutine(internetNecesarioActive());
        }
    }

    public void regenerarCodigo() {

        Debug.Log("Regenerando codigo");
        code = generateCode();
        code2 = "";
        status = "5";
        valCodigoSii = 3;
        countFrames = 0;
        cargaCodigo = 0;
        salir = 0;
    }

    // Update is called once per frame
    void Update() {
        if (countFrames >= 30) {
            if (code != "" || code != null) {
                code2 = "" + code[0] + code[1] + code[2] + "-" + code[3]+ code[4] + code[5];
                //Paso 1 de pairing code: generara el codigo y guardarlo en el servidor
                if (salir == 0) {
                    StartCoroutine(WebServiceCodigo.obtenerCodigo(code, 1));
                    if (valCodigoSii == 1) {
                        pairingCode.valCodigoSii = 3;
                        Debug.Log("El código ya exixte");
                        code = generateCode();
                    } else if (valCodigoSii == 0) {
                        pairingCode.valCodigoSii = 3;
                        Debug.Log("El código no exixte");
                        WebServiceCodigo.guardarCodigoSqlite(code);
                        StartCoroutine(WebServiceCodigo.insertarCodigo(code));
                        foreach (var objeto in GameObject.FindGameObjectsWithTag("codigo")) {
                            objeto.GetComponent<Text>().text = code2;
                        }
                        listenerCode.AddListener(emparejarCodigo);
                        salir = 1;
                    } else {
                        Debug.Log("Esperando Respuesta del Web Service 1");
                    }
                }

                //Paso 2 de pairing code: verificar si el codio es tomado por algun usuario para emparejarlo y generar la sesión
                // Iniciar Listener
                if (salir == 1) {
                    if (status == "2") {
                        Debug.Log("Quitting");
                        int res = WebServiceCodigo.editarCodigoSqlite(code, 2);
                        if (res == 1) {
                            Debug.Log("Se modifico el status");
                            listenerCode.RemoveListener(emparejarCodigo);
                            salir = 2;
                            foreach (var objeto in GameObject.FindGameObjectsWithTag("codigo")) {
                                objeto.GetComponent<Text>().text = "Codigo emparejado";
                            }
                            Debug.Log("Emparejando datos de sesion generados");
                            salir = 3;
                            pairingCode.valCodigoSii = 3;
                        } else {
                            Debug.Log("No se modifico el status");
                        }
                    } else {
                        listenerCode.Invoke();
                        if (valCodigoSii == 1) {
                            pairingCode.valCodigoSii = 3;
                        } else if (valCodigoSii == 0) {
                            pairingCode.valCodigoSii = 3;
                        } else {
                            Debug.Log("Esperando Respuesta del Web Service 2");
                        }
                    }
                }

                //Paso 3 de pairing code: sincronizar los datos de la sesion generada en el servidor con la db local
                if (salir == 3) {
                    StartCoroutine(wsParingCode.getDataSesionByCode(idCodigoServer));
                    if (valCodigoSii == 1) {
                        pairingCode.valCodigoSii = 3;
                        Debug.Log("Se obtuvieron los datos de sesion para emparejarlos");
                        //Validar si el usuario ya existe en la db local
                        var res = webServiceUsuario.existUserSqlite(usuario.usuario);
                        if (res != 1) {
                            //Guardar el registro del usuario en la db local
                            int resSaveUser;
                            if (usuario.rol!="UsuarioExterno") {
                                 resSaveUser = webServiceUsuario.insertarUsuarioSqLite(usuario.usuario, usuario.nombre, usuario.rol, usuario.gradoEstudios, usuario.programa, usuario.fechaRegistro, Int32.Parse(usuario.status), usuario.password, "");
                            } else {
                                 resSaveUser = webServiceUsuario.insertarUsuarioSqLite(usuario.usuario, usuario.nombre, usuario.rol, usuario.gradoEstudios, usuario.programa, usuario.fechaRegistro, Int32.Parse(usuario.status), usuario.password, "http://sii.uveg.edu.mx/unity/dropsV2/img/invitado.png");
                            }
                            if (resSaveUser == 1) {
                                Debug.Log("El usuario se guardo correctamente");
                                //Obtener datos del usuario que se acaba de registrar de la db local
                                var resultado = webServiceUsuario.consultarUsuarioSqLite(usuario.usuario);
                                if (resultado != "0") {
                                    webServiceUsuario.userDataSqLite data = JsonUtility.FromJson<webServiceUsuario.userDataSqLite>(resultado);
                                    if (data.rol != "UsuarioExterno") {
                                        StartCoroutine(webServiceUsuario.getUserData(data.usuario));
                                    } else {
                                        appManager manager = GameObject.Find("AppManager").GetComponent<appManager>();
                                        manager.setUsuario(data.usuario);
                                        manager.setNombre(data.nombre);
                                        manager.setGradoEstudios(data.gradoEstudios);
                                        manager.setGradoEstudios(data.rol);
                                        manager.setImagen("http://sii.uveg.edu.mx/unity/dropsV2/img/invitado.png");
                                    }
                                    //Guardar el log del usuario en la db local
                                    Debug.Log("El usuario se guardo correctamente");
                                    var resSaveLog = webServiceLog.insertarLogSqLite(log.fechaInicio, log.fechaTermino, log.dispositivo, 1, log.idServer, log.idCodigo, data.id);
                                    if (resSaveLog == 1) {
                                        Debug.Log("El log se inserto correctamente");
                                        //Cambiar estado del codigo a 3 tanto en local
                                        var resEditCode = WebServiceCodigo.editarCodigoSqlite(codigo.descripcion, 3);
                                        if (resEditCode == 1) {
                                            Debug.Log("El estado del codigo local se cambio correctammente");
                                            salir = 4;
                                        } else {
                                            Debug.Log("No se pudo realizar el combio del estado");
                                            salir = 5;
                                        }
                                    } else {
                                        Debug.Log("El log no se inserto correctamente");
                                    }
                                } else {
                                    Debug.Log("El log no se inserto correctamente");
                                }
                            } else {
                                Debug.Log("No se encontro el usuario que se acaba de registrar");
                            }
                        } else {
                            Debug.Log("El usuario ya existe");
                            //Obtener datos del usuario ya registrado en la db local
                            var resultado = webServiceUsuario.consultarUsuarioSqLite(usuario.usuario);
                            if (resultado != "0") {
                                webServiceUsuario.userDataSqLite data = JsonUtility.FromJson<webServiceUsuario.userDataSqLite>(resultado);
                                if (data.rol != "UsuarioExterno") {
                                    StartCoroutine(webServiceUsuario.getUserData(data.usuario));
                                } else {
                                    appManager manager = GameObject.Find("AppManager").GetComponent<appManager>();
                                    manager.setUsuario(data.usuario);
                                    manager.setNombre(data.nombre);
                                    manager.setGradoEstudios(data.gradoEstudios);
                                    manager.setGradoEstudios(data.rol);
                                    manager.setImagen("http://sii.uveg.edu.mx/unity/dropsV2/img/invitado.png");
                                }
                                //Guardar el log del usuario en la db local
                                var resSaveLogSqlite = webServiceLog.insertarLogSqLite(log.fechaInicio, log.fechaTermino, log.dispositivo, 1, log.idServer, log.idCodigo, data.id);
                                if (resSaveLogSqlite == 1) {
                                    Debug.Log("El log se inserto correctamente");
                                    //Cambiar estado del codigo a 3 tanto en local
                                    var resEditSQLite = WebServiceCodigo.editarCodigoSqlite(codigo.descripcion, 3);
                                    if (resEditSQLite == 1) {
                                        Debug.Log("El estado del codigo local se cambio correctammente");
                                        salir = 4;
                                    } else {
                                        Debug.Log("No se pudo realizar el combio del estado");
                                        salir = 5;
                                    }
                                } else {
                                    Debug.Log("El log no se inserto correctamente");
                                }
                            } else {
                                Debug.Log("No se encontro el usuario ya registrado");
                            }
                        }
                    } else if (valCodigoSii == 0) {
                        //pairingCode.valCodigoSii = 3;
                        Debug.Log(valCodigoSii);
                        Debug.Log("No obtuvieron los datos de sesion para emparejarlos");
                    } else {
                        Debug.Log("Esperando Respuesta del Web Service 3");
                    }
                }

                //Paso 4 de pairing code: cambiar el estado del codigo del servidor a 3
                if (salir == 4) {
                    StartCoroutine(WebServiceCodigo.updateCode(codigo.id, 3));
                    if (valCodigoSii == 1) {
                        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("menuCategorias", "menuCategorias"));
                    } else if (valCodigoSii == 0) {
                        Debug.Log("No se pudo actualizar codigo en servidor");
                    } else {
                        Debug.Log("Esperando Respuesta del Web Service 4");
                    }
                }
            } else {
                code = generateCode();
            }

            countFrames = 0;
        } else {
            countFrames++;
            cargaCodigo++; if (salir == 0) {
                if (cargaCodigo == 1) {
                    foreach (var objeto in GameObject.FindGameObjectsWithTag("codigo")) {
                        objeto.GetComponent<Text>().text = ".";
                    }
                } else if (cargaCodigo == 2) {
                    foreach (var objeto in GameObject.FindGameObjectsWithTag("codigo")) {
                        objeto.GetComponent<Text>().text = "..";
                    }
                } else if (cargaCodigo == 3) {
                    foreach (var objeto in GameObject.FindGameObjectsWithTag("codigo")) {
                        objeto.GetComponent<Text>().text = "...";
                    }
                } else if (cargaCodigo == 4) {
                    foreach (var objeto in GameObject.FindGameObjectsWithTag("codigo")) {
                        objeto.GetComponent<Text>().text = "....";
                    }
                } else if (cargaCodigo == 5) {
                    foreach (var objeto in GameObject.FindGameObjectsWithTag("codigo")) {
                        objeto.GetComponent<Text>().text = ".....";
                    }
                } else if (cargaCodigo == 6) {
                    foreach (var objeto in GameObject.FindGameObjectsWithTag("codigo")) {
                        objeto.GetComponent<Text>().text = "......";
                    }
                    cargaCodigo = 0;
                }
            }
        }
    }

    /** Funcion que sirve para verifica si el estado del codigo en el SII cambia a 2(en uso por algun usuario)
   **/
    void emparejarCodigo() {
        //Web service que verifica si el estado del codigo en el SII
        StartCoroutine(WebServiceCodigo.obtenerCodigo(code, 2));
    }

    /** Funcion que sirve para generar un código de 8 caracteres de manera aleatoria
   *
   *@param  chars Lista de caracteres 
   *@param  stringChars Contenedor de los 6 caracteres que contendra el codigo
   *@param  random Funcion para elección aleatoria
   *@param  finalString Código obtentido
   *@method conexionDB.alterGeneral metodo que recibe una query ya sea para realizar un insert, update o delete
   **/
    public string generateCode() {
        code2 = "";
        var chars = "abcdefghijklmnopqrstuvwxyz01234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var stringChars = new char[6];
      
        for (int i = 0; i < stringChars.Length; i++) {
            stringChars[i] = chars[UnityEngine.Random.Range(0, 62)];
            if (i == 2) {
                code2 = code2 + stringChars[i] + "-";
            } else {
                code2 = code2 + stringChars[i];
            }
        }
        Debug.Log("MI CODIGO2 ES ANTES DEL IF: " + code2);

        var finalString = new string(stringChars);
        return finalString;
    }

    IEnumerator internetNecesarioActive() {
        internetNecesario.SetActive(true);
        yield return new WaitForSeconds(20);
        internetNecesario.SetActive(false);
    }
}
