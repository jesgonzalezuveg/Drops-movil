using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class pruebaWebService : MonoBehaviour
{
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
        public string numPreguntas = "";
    }

    [Serializable]
    public class Data {
        public preguntaData[] preguntas;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getPreguntasOfPack("Paises"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static IEnumerator getPreguntasOfPack(string paquete) {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "consultarPreguntasOfPack");
        form.AddField("descripcion", paquete);

        byte[] rawFormData = form.data;
        WWW request = new WWW(URL, rawFormData/*, headers*/);
        yield return request;
        Debug.Log(request.error);
        string text;
        text = request.text;
        if (text == "0") {
            Debug.Log("No se encontraron preguntas");
        } else {
            text = "{\"preguntas\":" + text + "}";
            //Data myObject = JsonUtility.FromJson<Data>(text);
            Debug.Log(text);
        }


        /*
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "consultarPreguntasOfPack");
        form.AddField("descripcion", paquete);
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form)) {
            AsyncOperation asyncLoad = www.SendWebRequest();

            Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone) {
                Debug.Log("Descargando...");
                yield return null;
            }


            if (www.isNetworkError || www.isHttpError) {
                Debug.Log("Es un error www.error");
                Debug.Log(www.error);
                Debug.Log("Es un error isHttpError");
                Debug.Log(www.isHttpError);
                Debug.Log("Es un error isNetworkError");
                Debug.Log(www.isNetworkError);
                Debug.Log("Es un error www.error");
                Debug.Log(www.error);
                Debug.Log("response code");
                Debug.Log(www.responseCode);
            } else {
                string text;
                text = www.downloadHandler.text;
                if (text == "0") {
                    Debug.Log("No se encontraron preguntas");
                } else {
                    text = "{\"preguntas\":" + text + "}";
                    Data myObject = JsonUtility.FromJson<Data>(text);
                    Debug.Log(text);
                }
            }
        }*/
    }
}
