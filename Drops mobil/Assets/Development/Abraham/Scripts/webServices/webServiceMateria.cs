using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;

public class webServiceMateria : MonoBehaviour {

    //private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServiceMaterias.php";
    //private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    ///**
    // * Estructura que almacena los datos de una materia
    // */
    //[Serializable]
    //public class materiaData {
    //    public string id = "";
    //    public string claveMateria = "";
    //    public string descripcion = "";
    //    public string status = "";
    //    public string fechaRegistro = "";
    //    public string fechaModificacion = "";
    //    public string idCategoria = "";
    //    public string descripcionCategoria = "";
    //}

    //[Serializable]
    //public class Data {
    //    public materiaData[] materias;
    //}

    ///**
    // * Funcion que regresa un string con los ids de las materias que pertenecen a determinada categoria
    // * cada id de materia esta separado por una coma ejemplo 1,4,5,16
    // * @param categoria, id de la categoria de la cual se desean consultar sus materias
    // */
    //public static string getIdMateriasByCategoriaSqLite(string categoria) {
    //    string query = "SELECT id FROM catalogoMateria WHERE idCategoria = " + categoria + ";";
    //    var result = conexionDB.selectGeneral(query);
    //    if (result != "0") {
    //        result = result.Replace("{\"id\": \"", "");
    //        result = result.Replace("\"}","");
    //        return result;
    //    } else {
    //        return "0";
    //    }
    //}

    //public static materiaData getMateriaByClaveSqLite(string clave) {
    //    string query = "SELECT * FROM catalogoMateria WHERE claveMateria = '" + clave + "';";
    //    var result = conexionDB.selectGeneral(query);
    //    if (result != "0") {
    //        materiaData materia = JsonUtility.FromJson<materiaData>(result);
    //        return materia;
    //    } else {
    //        return null;
    //    }
    //}

    //public static int insertarMateriaSqLite(string clave, string descripcion, string status, string fechaRegistro, string fechaModificacion, string idCategoria) {
    //    string query = "INSERT INTO catalogoMateria (claveMateria, descripcion, status, fechaRegistro, fechaModificacion, idCategoria) VALUES ('" + clave + "', '" + descripcion + "', '" + status + "', '" + fechaRegistro + "','" + fechaModificacion + "', '" + idCategoria + "' );";
    //    var result = conexionDB.alterGeneral(query);
    //    if (result == 1) {
    //        return 1;
    //    } else {
    //        return 0;
    //    }
    //}

    //public static IEnumerator getMaterias() {
    //    WWWForm form = new WWWForm();
    //    Dictionary<string, string> headers = form.headers;
    //    headers["Authorization"] = API_KEY;

    //    form.AddField("metodo", "consultarMaterias");
    //    using (UnityWebRequest www = UnityWebRequest.Post(URL, form)) {
    //        AsyncOperation asyncLoad = www.SendWebRequest();
    //        // Wait until the asynchronous scene fully loads
    //        while (!asyncLoad.isDone) {
    //            yield return null;
    //        }

    //        if (www.isNetworkError || www.isHttpError) {
    //            Debug.Log(www.error);
    //        } else {
    //            string text;
    //            text = www.downloadHandler.text;
    //            if (text == "") {
    //                Debug.Log("No se encontraron Materias");
    //            } else {
    //                text = "{\"materias\":" + text + "}";
    //                Data myObject = JsonUtility.FromJson<Data>(text);
    //                GameObject.Find("AppManager").GetComponent<appManager>().setMaterias(myObject.materias);
    //            }
    //        }
    //    }
    //}

}
