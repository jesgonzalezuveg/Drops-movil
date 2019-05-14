using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using UnityEngine.Networking;

public class webServicePreguntas : MonoBehaviour {

    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServicePreguntas.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    /**
     * Estructura que almacena los datos de una pregunta
     */
    [Serializable]
    public class preguntaData {
        public string id = "";
        public string descripcion = "";
        public string status = "";
        public string fechaRegistro = "";
        public string fechaModificacion = "";
        public string idTipoEjercicio = "";
        public string idPaquete = "";
        public string idServer = "";
        public string descripcionEjercicio = "";
        public string descripcionPaquete = "";
    }

    [Serializable]
    public class Data {
        public preguntaData[] preguntas;
    }

    public static string consultarIdServerPreguntaSqLiteById(string id) {
        string query = "SELECT idServer FROM pregunta WHERE id = '" + id + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            preguntaData data = JsonUtility.FromJson<preguntaData>(result);
            return data.idServer;
        } else {
            return "0";
        }
    }

    public static int insertarPreguntaSqLite(string descripcion, string status, string fechaRegistro, string fechaModificacion, string idTipoEjercicio, string idPaquete, string idServer) {
        string query = "INSERT INTO pregunta (descripcion, status, fechaRegistro, fechaModificacion, idTipoEjercicio, idPaquete, idServer) VALUES ('" + descripcion + "', '" + status + "', '" + fechaRegistro + "','" + fechaModificacion + "', '" + idTipoEjercicio + "', '" + idPaquete + "', '" + idServer + "');";
        var result = conexionDB.alterGeneral(query);
        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static int updatePreguntaSqLite(preguntaData pregunta, string idServer) {
        string idTipoEjercicio = webServiceEjercicio.getEjercicioByDescripcionSqLite(pregunta.descripcionEjercicio).id;
        string idPaquete = webServicePaquetes.getPaquetesByDescripcionSqLite(pregunta.descripcionPaquete).id;
        string query = "UPDATE  pregunta SET  `descripcion` =  '" + pregunta.descripcion + "',`status` = '" + pregunta.status + "', `fechaRegistro` = '" + pregunta.fechaRegistro + "', `fechaModificacion` = dateTime('now','localtime'), `idTipoEjercicio` = '" + idTipoEjercicio + "', `idPaquete` = '" + idPaquete + "' WHERE  idServer = " + idServer + "; ";
        var result = conexionDB.alterGeneral(query);
        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static preguntaData getPreguntaByDescripcionSqLite(string descripcion) {
        string query = "SELECT * FROM pregunta WHERE descripcion = '" + descripcion + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            preguntaData data = JsonUtility.FromJson<preguntaData>(result);
            return data;
        } else {
            return null;
        }
    }

    public static preguntaData getPreguntaByIdServerSqLite(string idServer) {
        string query = "SELECT * FROM pregunta WHERE idServer = '" + idServer + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            preguntaData data = JsonUtility.FromJson<preguntaData>(result);
            return data;
        } else {
            return null;
        }
    }

    public static preguntaData[] getPreguntasByPackSqLite(string ipPaquete) {
        string query = "SELECT a.*, c.descripcion AS descripcionEjercicio, d.descripcion AS descripcionPaquete FROM pregunta AS a INNER JOIN catalogoEjercicio AS c INNER JOIN paquete AS d ON a.idTipoEjercicio = c.id AND a.idPaquete = d.id WHERE d.id = '" + ipPaquete + "'";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            result = "{\"preguntas\":[" + result + "]}";
            Debug.Log(result);
            Data data = JsonUtility.FromJson<Data>(result);
            return data.preguntas;
        } else {
            return null;
        }
    }


    public static preguntaData[] getPreguntasByPackSqLiteCurso(string ipPaquete, float limite) {
        string query = "SELECT a.*, c.descripcion AS descripcionEjercicio, d.descripcion AS descripcionPaquete FROM pregunta AS a INNER JOIN catalogoEjercicio AS c INNER JOIN paquete AS d ON a.idTipoEjercicio = c.id AND a.idPaquete = d.id WHERE d.id = '" + ipPaquete + "' ORDER BY random() LIMIT " + limite + ";";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            byte[] bytes = Encoding.Default.GetBytes(result);
            result = Encoding.UTF8.GetString(bytes);
            result = "{\"preguntas\":[" + result + "]}";
            Data data = JsonUtility.FromJson<Data>(result);
            return data.preguntas;
        } else {
            return null;
        }
    }


    public static IEnumerator getPreguntas() {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "consultarPreguntas");
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
                    Debug.Log("No se encontraron preguntas");
                } else {
                    text = "{\"preguntas\":" + text + "}";
                    Data myObject = JsonUtility.FromJson<Data>(text);
                    GameObject.Find("AppManager").GetComponent<appManager>().setPreguntas(myObject.preguntas);
                }
            }
        }
    }


    public static IEnumerator getPreguntasOfPack(string paquete) {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "consultarPreguntasOfPack");
        form.AddField("descripcion", paquete);
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
                if (text == "0") {
                    Debug.Log("No se encontraron preguntas");
                } else {
                    text = "{\"preguntas\":" + text + "}";
                    Data myObject = JsonUtility.FromJson<Data>(text);
                    GameObject.Find("AppManager").GetComponent<appManager>().setPreguntas(myObject.preguntas);
                }
            }
        }
    }

}
