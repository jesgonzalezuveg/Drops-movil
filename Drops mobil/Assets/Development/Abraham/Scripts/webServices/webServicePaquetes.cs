using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;

public class webServicePaquetes : MonoBehaviour{

    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServicePaquetes.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    /**
     * Estructura que almacena los datos de un paquete
     */
    [Serializable]
    public class paqueteData {
        public string id = "";
        public string clave = "";
        public string descripcion = "";
        public string fechaRegistro = "";
        public string fechaModificacion = "";
        public string urlImagen = "";
        public string idCategoria = "";
        public string idServer = "";
        public string descripcionCategoria = "";
    }

    [Serializable]
    public class Data {
        public paqueteData[] paquete;
    }


    /**
     * Funcion que regresa los paquetes que existen en la base de datos local
     * 
     */
    public static Data getPaquetesSqLite() {
        string query = "SELECT * FROM paquete;";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            result = "{\"paquete\": [" + result + "]}";
            Data paquete = JsonUtility.FromJson<Data>(result);
            return paquete;
        } else {
            return null;
        }
    }

    public static paqueteData getPaquetesByDescripcionSqLite(string descripcion) {
        string query = "SELECT * FROM paquete WHERE descripcion = '" + descripcion + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            paqueteData paquete = JsonUtility.FromJson<paqueteData>(result);
            return paquete;
        } else {
            return null;
        }
    }

    public static paqueteData getPaquetesByIdServerSqLite(string id) {
        string query = "SELECT * FROM paquete WHERE idServer = '" + id + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            paqueteData paquete = JsonUtility.FromJson<paqueteData>(result);
            return paquete;
        } else {
            return null;
        }
    }

    public static paqueteData[] getPaquetesByCategoriaSqLite(string categoriaId) {
        string query = "SELECT * FROM paquete WHERE idCategoria = '" + categoriaId + "';";
        Debug.Log(query);
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            result = "{\"paquete\":[" + result + "]}";
            Data paquete = JsonUtility.FromJson<Data>(result);
            return paquete.paquete;
        } else {
            return null;
        }
    }

    public static int insertarPaqueteSqLite(paqueteData paquete) {
        //clave, descripcion, fechaRegistro, fechaModificacion, urlImagen, idCategoria, idServer
        var categoriaData = webServiceCategoria.getCategoriaByDescripcionSqLite(paquete.descripcionCategoria);
        if (categoriaData != null) {
            var idCategoria = categoriaData.id;
            if (idCategoria == "0") {
                return 0;
            }
            string query = "INSERT INTO paquete (clave, descripcion, fechaRegistro, fechaModificacion, urlImagen, idCategoria, idServer) VALUES ('" + paquete.clave + "','" + paquete.descripcion + "', dateTime('now','localtime'), '" + paquete.fechaModificacion + "','" + paquete.urlImagen + "','" + idCategoria + "','" + paquete.id + "');";
            var result = conexionDB.alterGeneral(query);
            if (result == 1) {
                return 1;
            } else {
                return 0;
            }
        }
        return 0;
    }


    public static IEnumerator getPaquetes() {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "consultarPaquetes");
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
                    Debug.Log("No se encontraron paquetes");
                } else {
                    text = "{\"paquete\":" + text + "}";
                    Data myObject = JsonUtility.FromJson<Data>(text);
                    GameObject.Find("AppManager").GetComponent<appManager>().setPaquetes(myObject.paquete);
                }
            }
        }
    }

}
