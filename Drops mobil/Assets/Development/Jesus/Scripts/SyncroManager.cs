using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SyncroManager : MonoBehaviour {
    Scene scene;
    appManager manager;
    public string jsonGeneral;
    public string jsonPerUser;
    webServicePaquetes.Data paquetes = null;
    webServiceUsuario.userDataSqLite dataUser = null;
    webServiceUsuario.userDataSqLite[] dataUsers = null;
    webServiceLog.logData[] logs = null;
    webServiceRegistro.registroData[] registros = null;
    webServiceIntento.intentoDataSqLite[] intentos = null;
    webServiceDetalleIntento.detalleIntentoDataSqLite[] detalleIntento = null;
    public static string respuestaWsSincro = "0";
    string usuarioActual;
    string idUsuarioActual;

    // Start is called before the first frame update
    void Awake() {
        scene = SceneManager.GetActiveScene();
        if (scene.name == "menuCategorias") {
            //if (scene.name == "prueba") {
            manager = GameObject.Find("AppManager").GetComponent<appManager>();
            if (manager.isFirstLogin == true && manager.isOnline == true) {
                string user = manager.getUsuario();
                if (getDataUser(user)) {
                    manager.lastIdLog = webServiceLog.getLastLogSqLite(dataUser.id);
                }
                sincronizacionUsuarios();
                validarJson(jsonGeneral, false);
                return;
            }
        } else if (scene.name == "mainMenu" || scene.name == "mainMenuSesion") {
            if (UnityEngine.Application.internetReachability != NetworkReachability.NotReachable) {
                sincronizacionUsuarios();
                validarJson(jsonGeneral, false);
                return;
            }
        }
    }

    void Update() {
        if (scene.name == "mainMenu" || scene.name == "mainMenuSesion" && SyncroManager.respuestaWsSincro == "1") {
            SyncroManager.respuestaWsSincro = "0";
            borrarPaquetes();
        }
    }

    public void synchronizationInRealTime() {
        jsonGeneral = "";
        jsonPerUser = "";
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        if (manager.isOnline == true) {
            sicronizacionUsuarioActual();
            validarJson(jsonGeneral, true);
        }
    }


    public void validarJson(string json, bool realTime) {
        if (json != null && json != "{\"Usuarios\":[]}") {
            StartCoroutine(webServiceSincronizacion.SincroData(json, realTime));
        } else {
            Debug.Log("No hay datos para sincronizar");
            SyncroManager.respuestaWsSincro = "1";
        }
    }

    public bool getDataUser(string user) {
        string data = webServiceUsuario.consultarUsuarioSqLite(user);
        if (data != "0") {
            dataUser = JsonUtility.FromJson<webServiceUsuario.userDataSqLite>(data);
            return true;
        } else {
            return false;
        }
    }

    public void sicronizacionUsuarioActual() {
        if (getDataUser(manager.getUsuario())) {
            manager.lastIdLog = webServiceLog.getLastLogSqLite(dataUser.id);
            if (manager.lastIdLog != "0") {
                jsonGeneral += "{\"Usuarios\":[";
                usuarioActual = dataUser.usuario;
                idUsuarioActual = dataUser.id;
                jsonPerUser += "{\"id\": \"" + validateData(dataUser.id) + "\",";
                jsonPerUser += "\"usuario\": \"" + validateData(dataUser.usuario) + "\",";
                jsonPerUser += "\"nombre\": \"" + validateData(dataUser.nombre) + "\",";
                jsonPerUser += "\"rol\": \"" + validateData(dataUser.rol) + "\",";
                jsonPerUser += "\"gradoEstudios\": \"" + validateData(dataUser.gradoEstudios) + "\",";
                jsonPerUser += "\"programa\": \"" + validateData(dataUser.programa) + "\",";
                jsonPerUser += "\"fechaRegistro\": \"" + validateData(dataUser.fechaRegistro) + "\",";
                jsonPerUser += "\"status\": \"" + validateData(dataUser.status) + "\",";
                //Obtenemos el ultimo log perteneciente al usuario en turno
                getLogsUser(true);
                jsonPerUser += "}";
                jsonGeneral += jsonPerUser + "]}";
            } else {
                jsonGeneral = null;
            }
        } else {
            jsonGeneral = null;
        }
        //Debug.Log(jsonGeneral);
    }

    public void sincronizacionUsuarios() {
        //string user = manager.getUsuario();
        dataUsers = webServiceUsuario.consultarUsuariosSqLite();

        if (dataUsers != null) {
            //Continuamos generando el json agregando los logs del usuario
            jsonGeneral += "{\"Usuarios\":[";
            for (var i = 0; i < dataUsers.Length; i++) {
                usuarioActual = dataUsers[i].usuario;
                idUsuarioActual = dataUsers[i].id;
                jsonPerUser += "{\"id\": \"" + validateData(dataUsers[i].id) + "\",";
                jsonPerUser += "\"usuario\": \"" + validateData(dataUsers[i].usuario) + "\",";
                jsonPerUser += "\"nombre\": \"" + validateData(dataUsers[i].nombre) + "\",";
                jsonPerUser += "\"rol\": \"" + validateData(dataUsers[i].rol) + "\",";
                jsonPerUser += "\"gradoEstudios\": \"" + validateData(dataUsers[i].gradoEstudios) + "\",";
                jsonPerUser += "\"programa\": \"" + validateData(dataUsers[i].programa) + "\",";
                jsonPerUser += "\"fechaRegistro\": \"" + validateData(dataUsers[i].fechaRegistro) + "\",";
                jsonPerUser += "\"status\": \"" + validateData(dataUsers[i].status) + "\",";
                //Obtenemos los logs pertenecientes al usuario en turno
                getLogsUser(false);
                if (jsonPerUser != null) {
                    if ((dataUsers.Length - i) != 1) {
                        jsonPerUser += "},";
                    } else {
                        jsonPerUser += "}";
                    }
                }
            }

            jsonGeneral += jsonPerUser + "]}";
        } else {
            jsonGeneral = null;
        }
    }

    public void getLogsUser(bool last) {
        //Obtenemos logs del usuario
        if (last == true) {
            logs = webServiceLog.getLastLogByUser(idUsuarioActual, manager.lastIdLog);
        } else {
            if (scene.name == "mainMenu" || scene.name == "mainMenuSesion") {
                logs = webServiceLog.getLogsByUser(idUsuarioActual, "0");
            } else {
                logs = webServiceLog.getLogsByUser(idUsuarioActual, manager.lastIdLog);
            }
        }
        if (logs != null) {
            //Continuamos generando el json agregando los logs del usuario
            jsonPerUser += "\"logs\":[";
            for (var i = 0; i < logs.Length; i++) {
                jsonPerUser += "{\"id\": \"" + validateData(logs[i].id) + "\",";
                jsonPerUser += "\"fechaInicio\": \"" + validateData(logs[i].fechaInicio) + "\",";
                jsonPerUser += "\"fechaTermino\": \"" + validateData(logs[i].fechaTermino) + "\",";
                jsonPerUser += "\"dispositivo\": \"" + validateData(logs[i].dispositivo) + "\",";
                jsonPerUser += "\"idServer\": \"" + validateData(logs[i].idServer) + "\",";
                jsonPerUser += "\"idCodigo\": \"" + validateData(logs[i].idCodigo) + "\",";
                jsonPerUser += "\"idUsuario\": \"" + validateData(idUsuarioActual) + "\",";
                //Obtenemos los registros pertenecientes al log en turno
                getRegistrosUser(logs[i].id);
                //Obtenemos los intentos pertenecientes al log en turno
                getIntentosUser(logs[i].id);
                if ((logs.Length - i) != 1) {
                    jsonPerUser += "},";
                } else {
                    jsonPerUser += "}";
                }
            }
            jsonPerUser += "]";
        } else {
            jsonPerUser = null;
            Debug.Log("No se encontraron logs");
        }
    }

    public void getRegistrosUser(string idLog) {
        //Obtenemos los registros del log
        registros = webServiceRegistro.getRegistrossByLog(idLog);
        if (registros != null) {
            jsonPerUser += "\"registros\":[";
            //Continuamos generando el json agregando los registros del log
            for (var i = 0; i < registros.Length; i++) {
                //Obtenemos la descripcion de la accion para ponerla en lugar del id
                string descripcionAccion = webServiceAcciones.consultarDescripcionAccionSqLite(registros[i].idAccion);
                jsonPerUser += "{\"id\": \"" + validateData(registros[i].id) + "\",";
                jsonPerUser += "\"detalle\": \"" + validateData(registros[i].detalle) + "\",";
                jsonPerUser += "\"fechaRegistro\": \"" + validateData(registros[i].fechaRegistro) + "\",";
                jsonPerUser += "\"idLog\": \"" + validateData(registros[i].idLog) + "\",";
                jsonPerUser += "\"idUsuario\": \"" + validateData(usuarioActual) + "\",";
                if (descripcionAccion != "0") {
                    jsonPerUser += "\"idAccion\": \"" + validateData(descripcionAccion) + "\"";
                } else {
                    jsonPerUser += "\"idAccion\": \"" + validateData(descripcionAccion) + "\"";
                }
                if ((registros.Length - i) != 1) {
                    jsonPerUser += "},";
                } else {
                    jsonPerUser += "}";
                }
            }
            jsonPerUser += "],";
        } else {
            jsonPerUser += "\"registros\":[],";
        }
    }

    public void getIntentosUser(string idLog) {
        //Obtenemos los intentos del log
        intentos = webServiceIntento.getIntentosByLog(idLog);
        if (intentos != null) {
            //Continuamos generando el json agregando los intentos del log
            jsonPerUser += "\"intentos\":[";
            for (var i = 0; i < intentos.Length; i++) {
                jsonPerUser += "{\"id\": \"" + validateData(intentos[i].id) + "\",";
                jsonPerUser += "\"puntaje\": \"" + validateData(intentos[i].puntaje) + "\",";
                jsonPerUser += "\"fechaRegistro\": \"" + validateData(intentos[i].fechaRegistro) + "\",";
                jsonPerUser += "\"fechaModificacion\": \"" + validateData(intentos[i].fechaModificacion) + "\",";
                jsonPerUser += "\"idLog\": \"" + validateData(intentos[i].idLog) + "\",";
                //Obtenemos el detalle del intento
                getDetalleIntentoUser(intentos[i].id);
                if ((intentos.Length - i) != 1) {
                    jsonPerUser += "},";
                } else {
                    jsonPerUser += "}";
                }
            }
            jsonPerUser += "]";
        } else {
            jsonPerUser += "\"intentos\":[]";
        }
    }

    public void getDetalleIntentoUser(string idIntento) {
        //Obtenemos el detalle del intento
        detalleIntento = webServiceDetalleIntento.getDetalleIntentosByIntento(idIntento);
        if (detalleIntento != null) {
            //Continuamos generando el json agregando el detalle del intento al intento correspondiente
            jsonPerUser += "\"detalleIntento\":[";
            for (var i = 0; i < detalleIntento.Length; i++) {
                jsonPerUser += "{\"id\": \"" + validateData(detalleIntento[i].id) + "\",";
                jsonPerUser += "\"correcto\": \"" + validateData(detalleIntento[i].correcto) + "\",";

                string idServerPregunta = webServicePreguntas.consultarIdServerPreguntaSqLiteById(detalleIntento[i].idPregunta);
                string idServerRespuesta = webServiceRespuestas.consultarIdServerRespuestaSqLiteById(detalleIntento[i].idRespuesta);
                if (idServerPregunta != "0") {
                    jsonPerUser += "\"idPregunta\": \"" + idServerPregunta + "\",";
                } else {
                    jsonPerUser += "\"idPregunta\": \"" + detalleIntento[i].idPregunta + "\",";
                }

                if (idServerRespuesta != "0") {
                    jsonPerUser += "\"idRespuesta\": \"" + idServerRespuesta + "\",";
                } else {
                    jsonPerUser += "\"idRespuesta\": \"" + detalleIntento[i].idRespuesta + "\",";
                }

                jsonPerUser += "\"idIntento\": \"" + validateData(detalleIntento[i].idIntento) + "\"";
                if ((detalleIntento.Length - i) != 1) {
                    jsonPerUser += "},";
                } else {
                    jsonPerUser += "}";
                }
            }
            jsonPerUser += "]";
        } else {
            jsonPerUser += "\"detalleIntento\":[]";
        }
    }

    public string validateData(string data) {
        if (data != null && data != "") {
            return data;
        } else {
            return "";
        }
    }

    public void borrarPaquetes() {
        if (scene.name == "mainMenu" || scene.name == "mainMenuSesion") {
            paquetes = webServicePaquetes.getPaquetesBorradosSqLite();
            if (paquetes != null) {
                foreach (var paquete in paquetes.paquete) {
                    Debug.Log(paquete.id);
                    Debug.Log(paquete.descripcion);
                    var res = webServiceRespuestas.deleteRespuestasByPaqueteSqLite(paquete.id);
                    if (res == 1) {
                        res = webServicePreguntas.deletePreguntasByPaqueteSqLite(paquete.id);
                        if (res == 1) {
                            //Debug.Log("Se borraron las respuestas y preguntas del paquete " + paquete.id);
                            //res = webServicePaquetes.deletePaqueteSqLite(paquete.id);
                            //if (res == 1) {
                            //    Debug.Log("Se borro por completo el paquete " + paquete.id);
                            //} else {
                            //    Debug.Log("Error al borrar el paquete");
                            //}
                        } else {
                            //Debug.Log("Se borraron las respuestas del paquete " + paquete.id + "(error al borrar las preguntas)");
                        }
                    } else {
                        //Debug.Log("Error al borrar las respuestas del paquete " + paquete.id);
                    }
                }
            } else {
                //Debug.Log("No encontro paquetes borrados");
            }
        }
    }
}
