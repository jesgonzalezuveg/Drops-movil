using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class webServiceAvisos : MonoBehaviour
{
    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServicePaquetes.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    /**
     * Estructura que almacena los datos de un paquete
     */
    [Serializable]
    public class paqueteDataNuevos {
        public string id = "";
        public string descripcion = "";
        public string fechaRegistro = "";
        public string urlImagen = "";
        public string descripcionCategoria = "";
        public string fechaServidor = "";
        public string fechaIntervalo = "";
    }

    [Serializable]
    public class DataNuevos {
        public paqueteDataNuevos[] paquete;
    }

    [Serializable]
    public class logAvisos {
        public string id = "";
        public string fechaVisualizacion = "";
        public string idPaquete = "";
        public string idUsuario = "";
    }

    [Serializable]
    public class DataAvisos {
        public logAvisos[] logAviso;
    }

    public static DataAvisos getLogAvisosLast3DaysSqLite(string idPaquete, string idUsuario, string fecha) {
        string query = "SELECT * FROM aviso WHERE idPaquete = "+idPaquete+" AND idUsuario = "+idUsuario+ " AND Datetime(fechaVisualizacion) >= Datetime('"+fecha+"');";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            result = "{\"avisoLog\": [" + result + "]}";
            DataAvisos log = JsonUtility.FromJson<DataAvisos>(result);
            return log;
        } else {
            return null;
        }
    }

    public static int insertarLogAvisoSqLite(string idPaquete, string idUsuario, string fecha) {
        try {
            string query = "INSERT INTO aviso (fechaVisualizacion, idPaquete, idUsuario) VALUES ('" + fecha + "', "+idPaquete+", "+idUsuario+");";
            var result = conexionDB.alterGeneral(query);
            if (result == 1) {
                return 1;
            } else {
                return 0;
            }
        } catch (Exception ex) {
            Debug.LogError("Error en insertarIntentoSqLite: " + ex);
            return 0;
        }
    }

    public static int isPackDonwload(string idPaquete) {
        string query = "SELECT * FROM descarga WHERE idPaquete = " + idPaquete + ";";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            return 1;
        } else {
            return 0;
        }
    }

    public static IEnumerator getPaquetesMasNuevos(string intervalo) {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "getPaquetesNuevos");
        form.AddField("intervalo", intervalo);
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
                if (text == "" || text =="0") {
                    if (GameObject.Find("AppManager").GetComponent<appManager>().getOld == 0) {
                        GameObject.Find("AppManager").GetComponent<appManager>().getOld = 1;
                    }
                } else {
                    text = "{\"paquete\":" + text + "}";
                    DataNuevos myObject = JsonUtility.FromJson<DataNuevos>(text);
                    GameObject.Find("AvisosManager").GetComponent<avisosManager>().setPaquetesMasNuevos(myObject.paquete);
                }
            }
        }
    }
}
