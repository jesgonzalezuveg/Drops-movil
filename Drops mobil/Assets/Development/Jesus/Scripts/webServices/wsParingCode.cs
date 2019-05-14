using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;

public class wsParingCode : MonoBehaviour
{
    //URL de webservice del SII para los procesos del pairing code
    private const string URL = "http://sii.uveg.edu.mx/unity/dropsV2/controllers/wsPairingCodeController.php";
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";//KEY falsa, remplazar por autentica

    /** Estructura que almacena los datos generados del pairing code
     */
    [Serializable]
    public class Data {
        public webServiceLog.logData log = new webServiceLog.logData();
        public webServiceUsuario.userDataSqLite usuario = new webServiceUsuario.userDataSqLite();
        public WebServiceCodigo.Data codigo = new WebServiceCodigo.Data();
    }


    /** Coroutine que consulta base de datos de SII para obtener los datos de sesion generados del pairing code(usuario, log y codigo usado)
     * @param idCodigo id del codigo usado en el SII para generar la sesion 
     */
    public static IEnumerator getDataSesionByCode(string idCodigo) {
        //Start the fading process
        WWWForm form = new WWWForm();
        //Debug.Log(idCodigo);
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;

        form.AddField("metodo", "getSesionByCode");
        form.AddField("idCodigo", idCodigo);
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
                    pairingCode.status = text;
                    pairingCode.valCodigoSii = 0;
                } else {
                    Debug.Log(text);
                    Data data = JsonUtility.FromJson<Data>(text);
                    pairingCode pairingObj = GameObject.Find("bd").GetComponent<pairingCode>();
                    pairingObj.setLog(data.log); 
                    pairingObj.setUsuario(data.usuario); 
                    pairingObj.setCodigo(data.codigo);
                    pairingCode.valCodigoSii = 1;
                }
            }
        }
    }
}
