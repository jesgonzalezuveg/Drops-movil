using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
public class webServiceAcciones : MonoBehaviour {

    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServiceAcciones.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    /**
     * Estructura que almacena los datos de las acciones desde SqLite
     */
    [Serializable]
    public class accionData {
        public string id = "";
        public string descripcion = "";
        public string status = "";
        public string fechaRegistro = "";
        public string fechaModificacion = "";
    }

    /** Estructura que almacena los datos de las acciones desde SII
     */
    [Serializable]
    public class Data {
        public accionData[] acciones;
    }

    /** Función que inseta los datos de la accion en la base de datos local
     * @param descripcion descripcion de la accion 
     * @param status estado de la accion (activa o inanctiva)
     */
    public static int insertarAccionSqLite(string descripcion, string status) {
        string query = "INSERT INTO catalogoAcciones (descripcion, status, fechaRegistro, fechaModificacion) VALUES ('" + descripcion + "'," + status + ", dateTime('now','localtime'), dateTime('now','localtime'));";
        var result = conexionDB.alterGeneral(query);
        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que consulta y trae los datos de la accion solicitada
     * @param accion descripcion de la accion a consultar
     */
    public static string consultarAccionSqLite(string accion) {
        string query = "SELECT * FROM catalogoAcciones WHERE descripcion = '" + accion + "';";
        var result = conexionDB.selectGeneral(query);
        return result;
    }

    /** Función que consulta el id de la accion
     * @param accion descripcion de la accion a consultar
     */
    public static string consultarIdAccionSqLite(string accion) {
        string query = "SELECT id FROM catalogoAcciones WHERE descripcion = '" + accion + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            accionData data = JsonUtility.FromJson<accionData>(result);
            return data.id;
        } else {
            return "0";
        }
    }

    public static string consultarDescripcionAccionSqLite(string idAccion) {
        string query = "SELECT descripcion FROM catalogoAcciones WHERE id = '" + idAccion + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            accionData data = JsonUtility.FromJson<accionData>(result);
            return data.descripcion;
        } else {
            return "0";
        }
    }

    /** Función para actualizar los datos de una accion
     * @param accion descripcion de la accion 
     * @param status estado de la accion (activa o inanctiva)
     * @param idServer id de la accion en el servidor
     */
    public static int updateAccionSqlite(string accion, string status, string idServer) {
        string query = "UPDATE catalogoAcciones SET descripcion = '" + accion + "', status = " + status + ", fechaModificacion =  dateTime('now','localtime') WHERE idServer = " + idServer + "";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que verifica si la accion existe
     * @param usuario matricula o correo electronico del usuario
     */
    public static int existAccionSqlite(string accion) {
        string query = "SELECT * FROM catalogoAcciones WHERE descripcion = '" + accion + "'";
        var result = conexionDB.selectGeneral(query);

        if (result != "0") {
            return 1;
        } else {
            return 0;
        }
    }

    public static IEnumerator getAcciones() {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "consultarAcciones");
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form)) {
            AsyncOperation asyncLoad = www.SendWebRequest();
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone) {
                yield return null;
            }

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string text;
                text = www.downloadHandler.text;
                if (text == "") {
                    Debug.Log("No se encontraron categorias");
                } else {
                    text = "{\"acciones\":" + text + "}";
                    Data myObject = JsonUtility.FromJson<Data>(text);
                    GameObject.Find("AppManager").GetComponent<appManager>().setAcciones(myObject.acciones);
                }
            }
        }
    }

}
