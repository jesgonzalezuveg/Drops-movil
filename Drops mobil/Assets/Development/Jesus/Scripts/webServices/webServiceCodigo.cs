using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;

public class WebServiceCodigo : MonoBehaviour {
    //URL de webservice del SII para las acciones con los codigos
    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServiceController.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    /** Estructura que almacena los datos del codigo desde SII
     */
    [Serializable]
    public class Data {
        public string id = "";
        public string descripcion = "";
        public string status = "";
        public string fechaRegistro = "";
        public string fechaModificacion = "";
    }

    /** Estructura que almacena los datos de un arreglo de codigos desde SII
     */
    [Serializable]
    public class DataCodigos {
        public Data[] codigos;
    }

    /** Coroutine que consulta base de datos de SII para insertar los datos correspondientes al codigo generado
     * @param codigo codigo que genero la aplicacion para el emparejamiento
     */
    public static IEnumerator insertarCodigo(string codigo) {
        //Start the fading process
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "insertarCodigo");
        form.AddField("codigo", codigo);
        //byte[] rawFormData = form.data;
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form)) {
            //www.chunkedTransfer = false;
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string text;
                text = www.downloadHandler.text;
                Debug.Log("Form upload complete!");
                Debug.Log(text);
            }
        }
    }

    /** Coroutine que consulta base de datos de SII para verificar si el codigo introducido cuenta con el status solicitado 
     * @param codigo codigo que se quiere verficar
     * @param status estado en el cual se debe buscar el codigo
     */
    public static IEnumerator obtenerCodigo(string codigo, int status) {
        //Start the fading process
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "obtenerCodigo");
        form.AddField("codigo", codigo);
        form.AddField("status", status);
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
                if (text=="0") {
                    //Debug.Log("No se encontro el código");
                    pairingCode.status = text;
                    pairingCode.valCodigoSii = 0;
                } else {
                    //Debug.Log("Se encontro el código");
                    Data data = JsonUtility.FromJson<Data>(text);
                    pairingCode.status = data.status;
                    pairingCode.valCodigoSii = 1;
                    pairingCode.idCodigoServer = data.id;
                }
            }
        }
    }

    /** Coroutine que consulta base de datos de SII para actualizar el estado del codigo en el SII
     * @param idCodigo id del codigo en el SII
     * @param status estado al cual se quiere cambiar el codigo
     */
    public static IEnumerator updateCode(string idCodigo, int status) {
        //Start the fading process
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "updateStatusCode");
        form.AddField("idCodigo", idCodigo);
        form.AddField("status", status);
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
                if (text == "0") {
                    //Debug.Log("No se encontro el código");
                    pairingCode.status = text;
                    pairingCode.valCodigoSii = 0;
                } else {
                    pairingCode.status = text;
                    pairingCode.valCodigoSii = 1;
                }
            }
        }
    }

    /** Función que inserta los datos del codigo en la base de datos local
     * @param codigo codigo generado por la aplicacion
     */
    public static int guardarCodigoSqlite(string codigo) {
        string query = "INSERT INTO codigo (descripcion, status, fechaRegistro, fechaModificacion) VALUES ('" + codigo + "', 1, dateTime('now','localtime'), dateTime('now','localtime'))";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que modifica estado del codigo en la base de datos local
     * @param codigo codigo que se quiere modificar
     * @param status estado en el que se encuentra el codigo antes de la modificacion
     */
    public static int editarCodigoSqlite(string codigo, int status) {
        string query = "UPDATE codigo SET status = " + status + ", fechaModificacion = dateTime('now','localtime') WHERE descripcion = '" + codigo + "' AND status = " + (status - 1) + "";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que verifica si el codigo existe en la base de datos local
     * @param codigo codigo que se quiere verificar su existencia
     */
    public static int existeCodigoSqlite(string codigo) {
        string query = "SELECT * FROM codigo WHERE descripcion = '" + codigo + "' AND status = 1";
        var result = conexionDB.selectGeneral(query);

        if (result != "0") {
            return 1;
        } else {
            return 0;
        }
    }
}
