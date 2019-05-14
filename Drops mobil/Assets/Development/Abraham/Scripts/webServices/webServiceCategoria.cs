using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;

public class webServiceCategoria : MonoBehaviour {

    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServiceCategorias.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    /**
     * Estructura que almacena los datos de categoria
     */
    [Serializable]
    public class categoriaData {
        public string id = "";
        public string descripcion = "";
        public string status = "";
        public string fechaRegistro = "";
        public string fechaModificacion = "";
    }

    [Serializable]
    public class Data {
        public categoriaData[] categoria;
    }

    /**
     * Función que regresa el id de la categoria que se pide
     * @param descripcion nombre de la categoria que se esta solicitando el ID
     */
    public static string getIdCategoriaByNameSqLite(string descripcionP) {
        string query = "SELECT id FROM catalogoCatgoriaPaquete WHERE descripcion = '" + descripcionP + "' ;";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            categoriaData data = JsonUtility.FromJson<categoriaData>(result);
            return data.id;
        } else {
            return "0";
        }
    }

    public static categoriaData getCategoriaByDescripcionSqLite(string descripcionP) {
        string query = "SELECT * FROM catalogoCatgoriaPaquete WHERE descripcion = '" + descripcionP + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            categoriaData categoria = JsonUtility.FromJson<categoriaData>(result);
            return categoria;
        } else {
            return null;
        }
    }

    public static categoriaData getCategoriaByIdSqLite(string id) {
        string query = "SELECT * FROM catalogoCatgoriaPaquete WHERE id = '" + id + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            categoriaData categoria = JsonUtility.FromJson<categoriaData>(result);
            return categoria;
        } else {
            return null;
        }
    }

    public static categoriaData[] getCategoriasSql() {
        string query = "SELECT * FROM catalogoCatgoriaPaquete;";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            result = "{\"categoria\":[" + result + "]}";
            Data categoria = JsonUtility.FromJson<Data>(result);
            return categoria.categoria;
        } else {
            return null;
        }
    }

    public static int insertarCategoriaSqLite(categoriaData categoria) {
        string query = "INSERT INTO catalogoCatgoriaPaquete (descripcion, status,fechaRegistro, fechaModificacion) VALUES ('" + categoria.descripcion + "', '" + categoria.status + "', '" + categoria.fechaRegistro + "','" + categoria.fechaModificacion + "');";
        var result = conexionDB.alterGeneral(query);
        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }


    public static IEnumerator getCategorias() {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "consultarCategorias");
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
                    text = "{\"categoria\":" + text + "}";
                    Data myObject = JsonUtility.FromJson<Data>(text);
                    GameObject.Find("AppManager").GetComponent<appManager>().setCategorias(myObject.categoria);
                }
            }
        }
    }

}
