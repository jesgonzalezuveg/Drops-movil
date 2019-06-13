using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class webServiceSincronizacion : MonoBehaviour
{
    //URL de webservice del SII para los procesos de la sincronizacion
    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServiceSincronizacion.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    [Serializable]
    public class Registro {
        public string id = "";
        public string detalle = "";
        public string fechaRegistro = "";
        public string idLog = "";
        public string idUsuario = "";
        public string idAccion = "";
    }
    

    [Serializable]
    public class DetalleIntento {
        public string id = "";
        public string correcto = "";
        public string idPregunta = "";
        public string idRespuesta = "";
        public string idIntento = "";
    }

    [Serializable]
    public class Intento {
        public string id = "";
        public string puntaje = "";
        public string fechaRegistro = "";
        public string fechaModificacion = "";
        public string idLog = "";
        public DetalleIntento[] detalleIntento;
    }

    [Serializable]
    public class Log {
        public string id = "";
        public string fechaInicio = "";
        public string fechaTermino = "";
        public string dispositivo = "";
        public string idCodigo = "";
        public string idUsuario = "";
        public Registro[] registros;
        public Intento[] intentos;
    }

    [Serializable]
    public class Usuario {
        public string id = "";
        public string usuario = "";
        public string nombre = "";
        public string rol = "";
        public string gradoEstudios = "";
        public string programa = "";
        public string fechaRegistro = "";
        public string status = "";
        public Log[] logs;
    }

    [Serializable]
    public class RootObject {
        public Usuario[] Usuarios; 
    }

    public static int changeSyncroStatus(string json, string idServer) {
        //Debug.Log(json);
        RootObject myObject = JsonUtility.FromJson<RootObject>(json);
        for (int i =0; i < myObject.Usuarios.Length; i++) {
           if(webServiceUsuario.updateSyncroStatusSqlite(myObject.Usuarios[i].id, 2)==1) {
                for (int j = 0; j < myObject.Usuarios[i].logs.Length; j++) {
                    if (webServiceLog.updateSyncroStatusSqlite(myObject.Usuarios[i].logs[j].id, 2, idServer) == 1) {
                        if (myObject.Usuarios[i].logs[j].registros.Length>0) {
                            for (int k = 0; k < myObject.Usuarios[i].logs[j].registros.Length; k++) {
                                webServiceRegistro.updateSyncroStatusSqlite(myObject.Usuarios[i].logs[j].registros[k].id, 2);
                                webServiceRegistro.deleteRegistroSqlite(myObject.Usuarios[i].logs[j].registros[k].id);
                            }
                        }

                        if (myObject.Usuarios[i].logs[j].intentos.Length > 0) {
                            for (int l = 0; l < myObject.Usuarios[i].logs[j].intentos.Length; l++) {
                                if (webServiceIntento.updateSyncroStatusSqlite(myObject.Usuarios[i].logs[j].intentos[l].id, 2) == 1) {
                                    if (myObject.Usuarios[i].logs[j].intentos[l].detalleIntento.Length > 0) {
                                        for (int m = 0; m < myObject.Usuarios[i].logs[j].intentos[l].detalleIntento.Length; m++) {
                                            webServiceDetalleIntento.updateSyncroStatusSqlite(myObject.Usuarios[i].logs[j].intentos[l].detalleIntento[m].id, 2);
                                            webServiceDetalleIntento.deleteDetalleIntentoSqlite(myObject.Usuarios[i].logs[j].intentos[l].detalleIntento[m].id);
                                        }
                                        webServiceIntento.deleteIntentoSqlite(myObject.Usuarios[i].logs[j].intentos[l].id);
                                    }
                                }
                            }
                        }
                        if (idServer == "0") {
                            webServiceLog.deleteLogSqlite(myObject.Usuarios[i].logs[j].id);
                        }
                    }
                }
            }
        }
        //myObject.Usuarios
        return 1;
    }

    public static IEnumerator SincroData(string data, bool realTime) {
        //Start the fading process
        WWWForm form = new WWWForm();
        //Debug.Log(idCodigo);
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;
        if (realTime) {
            form.AddField("metodo", "sincronizarTiempoReal");
        } else {
            form.AddField("metodo", "sincronizar");
        }
        form.AddField("dataSincronizacion", data);
        //byte[] rawFormData = form.data;
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form)) {
            //www.chunkedTransfer = false;
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {

            } else {
                string text;
                text = www.downloadHandler.text;
                text = text.Replace("[", "");
                text = text.Replace("]", "");
                if (text != "0") {
                    //appManager manager = GameObject.Find("AppManager").GetComponent<appManager>();
                    //manager.lastIdLog = text;
                    //Debug.Log("ESTE ES EL ULTIMO LOG: " + manager.lastIdLog);
                    SyncroManager.respuestaWsSincro = "1";
                    //Debug.Log("Sincronizacion realizada");
                    int resultado;
                    if (realTime) {
                        resultado = changeSyncroStatus(data, text);
                    } else {
                        resultado = changeSyncroStatus(data, "0");
                    }
                    if (resultado == 1) {
                        Debug.Log("respuesta local: termino sincronizacion");
                        SyncroManager.respuestaWsSincro = "1";
                    } else {
                        Debug.Log("respuesta local: error al sincronizar");
                        SyncroManager.respuestaWsSincro = "0";
                    }
                } else {
                    SyncroManager.respuestaWsSincro = "0";
                    Debug.Log("Fallo la sincronizacion");
                }
            }
        }
    }
}
