using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;

public class webServiceEjercicio : MonoBehaviour {

    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServiceEjercicios.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    /**
     * Estructura que almacena los datos de categoria
     */
    [Serializable]
    public class ejercicioData {
        public string id = "";
        public string descripcion = "";
        public string status = "";
        public string fechaRegistro = "";
        public string fechaModificacion = "";
    }

    [Serializable]
    public class Data {
        public ejercicioData[] ejercicios;
    }

    public static ejercicioData getEjercicioByDescripcionSqLite(string descripcion) {
        string query = "SELECT * FROM catalogoEjercicio WHERE descripcion = '" + descripcion + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            ejercicioData categoria = JsonUtility.FromJson<ejercicioData>(result);
            return categoria;
        } else {
            return null;
        }
    }

    public static int insertarEjercicioSqLite(string descripcion, string status, string fechaRegistro, string fechaModificacion) {
        string query = "INSERT INTO catalogoEjercicio (descripcion, status,fechaRegistro, fechaModificacion) VALUES ('" + descripcion + "', '" + status + "', '" + fechaRegistro + "','" + fechaModificacion + "');";
        var result = conexionDB.alterGeneral(query);
        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static string consultarDescripcionEjercicioByIdSqLite(string id) {
        string query = "SELECT * FROM catalogoEjercicio WHERE id = '" + id + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            ejercicioData categoria = JsonUtility.FromJson<ejercicioData>(result);
            return categoria.descripcion;
        } else {
            return "0";
        }
    }


    public static IEnumerator getEjercicios() {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "consultarEjercicios");
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
                    Debug.Log("No se encontraron tipos de ejercicios");
                } else {
                    text = "{\"ejercicios\":" + text + "}";
                    Data myObject = JsonUtility.FromJson<Data>(text);
                    GameObject.Find("AppManager").GetComponent<appManager>().setEjerciocio(myObject.ejercicios);
                }
            }
        }
    }

}
