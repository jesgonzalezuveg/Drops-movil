using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Text;

public class webServiceLog : MonoBehaviour {

    //URL de webservice del SII para los procesos de log
    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServiceLog.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica
    /** 
     * Estructura que almacena los datos de un log
     */
    [Serializable]
    public class logData {
        public string id = "";
        public string fechaInicio = "";
        public string fechaTermino = "";
        public string dispositivo = "";
        public string syncroStatus = "";
        public string idServer = "";
        public string idCodigo = "";
        public string idUsuario = "";
    }

    [Serializable]
    public class dataLog {
        public logData[] logs;
    }

    public static int updateSyncroStatusSqlite(string id, int sincroStatus, string idServer) {
        //Debug.Log("EL ID SERVER ES: " +idServer);
        if (idServer != "0") { 
            sincroStatus = 1;
        }
        string query = "UPDATE log SET syncroStatus = '" + sincroStatus + "', idServer = " + idServer + " WHERE id = '" + id + "'";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static int deleteLogSqlite(string id) {
        string query = "DELETE FROM log WHERE id = " + id + "";
        var result = conexionDB.alterGeneral(query);

        if (result > 0) {
            return 1;
        } else {
            return 0;
        }
    }

    public static int updateIdServerSqlite(string id, string idServer) {
        string query = "UPDATE log SET idServer = '" + idServer + "' WHERE id = '" + id + "'";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static logData[] getLogsByUser(string idUsuario, string lastIdLog) {
        string query = "SELECT * FROM log WHERE syncroStatus <> 2 AND id <> "+lastIdLog+" AND idUsuario = " + idUsuario + ";";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            byte[] bytes = Encoding.Default.GetBytes(result);
            result = Encoding.UTF8.GetString(bytes);
            result = "{\"logs\":[" + result + "]}";
            dataLog data = JsonUtility.FromJson<dataLog>(result);
            return data.logs;
        } else {
            return null;
        }
    }

    public static logData[] getLastLogByUser(string idUsuario, string lastIdLog) {
        string query = "SELECT * FROM log WHERE syncroStatus <> 2 AND id = " + lastIdLog + " AND idUsuario = " + idUsuario + ";";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            byte[] bytes = Encoding.Default.GetBytes(result);
            result = Encoding.UTF8.GetString(bytes);
            result = "{\"logs\":[" + result + "]}";
            dataLog data = JsonUtility.FromJson<dataLog>(result);
            return data.logs;
        } else {
            return null;
        }
    }

    /** Función que inserta los datos del log
     * @param usuario matricula o correo del usuario
     */
    public static int insertarLogSqLite(string usuario) {
        string id = webServiceUsuario.consultarIdUsuarioSqLite(usuario);
        string query = "INSERT INTO log (fechaInicio, fechaTermino, dispositivo, syncroStatus, idServer, idCodigo, idUsuario) VALUES (dateTime('now','localtime'), dateTime('now','localtime'), '" + SystemInfo.deviceModel + "',0,0,0,'" + id + "');";
        var result = conexionDB.alterGeneral(query);
        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que inserta los datos del log en db local cuando se genera el log primero en la db del servidor debido al loging por pairing code
     * @param fechaInicio fecha cuando inicio la sesion
     * @param fechaTermino fecha cuando termino la sesion
     * @param dispositivo nombre del dispositivo donde se uso la aplicacion
     * @param syncroStatus campo que indica si el registro de la tabla ya esta sincronizado
     * @param idCodigo id del codigo guardado en la db del servidor
     * @param idUsuario id del usuario generado en la db local
     */
    public static int insertarLogSqLite(string fechaInicio, string fechaTermino, string dispositivo, int syncroStatus, string idServer, string idCodigo, string idUsuario) {
        string query = "INSERT INTO log (fechaInicio, fechaTermino, dispositivo, syncroStatus, idServer, idCodigo, idUsuario) VALUES (dateTime('now','localtime'), dateTime('now','localtime'), '" + SystemInfo.deviceModel + "', " + syncroStatus + ", " + idServer + ", " + idCodigo + ", " + idUsuario + ")";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que consulta el ultimo log de un usuario
     * @param usuario matricula o correo del usuario
     */
    public static string getLastLogSqLite(string idUsuario) {
        string query = "SELECT id FROM log WHERE idUsuario = " + idUsuario + " ORDER by fechaInicio DESC LIMIT 1;";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            logData data = JsonUtility.FromJson<logData>(result);
            return data.id;
        } else {
            return "0";
        }
    }

    /** Función que inseta los datos del log
     * @param usuario matricula o correo del usuario
     */
    public static int cerrarLog(string usuario) {
        string id = webServiceUsuario.consultarIdUsuarioSqLite(usuario);
        var lastLog = getLastLogSqLite(id);
        string query = "UPDATE log SET fechaTermino = dateTime('now','localtime') WHERE id = " + lastLog + ";";
        var result = conexionDB.alterGeneral(query);
        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static IEnumerator getIdLastLog(string idUsuario) {
        //Start the fading process
        WWWForm form = new WWWForm();
        //Debug.Log(idCodigo);
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "getLastLogByUser");
        form.AddField("idUsuario", idUsuario);
        //byte[] rawFormData = form.data;
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form)) {
            //www.chunkedTransfer = false;
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string text;
                text = www.downloadHandler.text;
                text = text.Replace("[", "");
                text = text.Replace("]", "");
                Debug.Log(text);
                Debug.Log("Respuesta json");
                if (text != "0") {
                    appManager manager = GameObject.Find("AppManager").GetComponent<appManager>();
                    manager.lastIdLog = text;
                    Debug.Log("ESTE ES EL ULTIMO LOG: " + manager.lastIdLog);
                } else {
                    SyncroManager.respuestaWsSincro = "0";
                    Debug.Log("Fallo la sincronizacion");
                }
            }
        }
    }

}
