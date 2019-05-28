using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;

public class webServiceUsuario : MonoBehaviour {

    private const string USUARIO_DATA = "http://sii.uveg.edu.mx/core/api/apiUsuarios.php";     ///< URL del API que se utilizará
    private const string API_KEY = "AJFFF-ASFFF-GWEGG-WEGERG-ERGEG-EGERG-ERGEG";                ///< API_KEY KEY que se necesitará para la conexión

    /** Estructura que almacena los datos del usuario desde SII
     */
    [Serializable]
    public class Data {
        public string Usuario = "";
        public string Nombre = "";
        public string PrimerApellido = "";
        public string SegundoApellido = "";
        public string Correo = "";
        public string Imagen = "";
        public string ProgramaAcademico = "";
        public string ProgramaEstudio = "";
    }

    /**
     * Estructura que almacena los datos del usuario desde SqLite
     */
    [Serializable]
    public class userDataSqLite {
        public string id = "";
        public string usuario = "";
        public string nombre = "";
        public string rol = "";
        public string gradoEstudios = "";
        public string programa = "";
        public string fechaRegistro = "";
        public string status = "";
        public string syncroStatus = "";
        public string password = "";
        public string imagen = "";
    }

    /**
     * Estructura que almacena los datos del usuario desde SqLite
     */
    [Serializable]
    public class userDataSqLite2 {
        public string id = "";
        public string usuario = "";
        public string nombre = "";
        public string rol = "";
        public string gradoEstudios = "";
        public string programa = "";
        public string fechaRegistro = "";
        public string status = "";
        public string password = "";
    }

    [Serializable]
    public class usersAllDataSqLite {
        public userDataSqLite[] usuarios;
    }

    /** Estructura que almacena los datos del usuario y en caso de ser necesario los datos de inicio de sesión 
     */
    [Serializable]
    public class JsonResponse {
        public Data data = new Data();
        public string mensaje = "";
        public string estatus = "";
        public string estatusCode = "";

    }

    /** Función que inseta los datos del usuario en la base de datos local
     * @param usuario matricula o correo del usuario
     * @param nombre nombre del usuario
     * @param rol tipo de usuario puede ser usuarioUveg, invitado o invitadoFacebook
     * @param gradoEstudios puede ser nulo, en caso de ser alumno uveg insertará el nivel de estudios que tiene
     * @param programa puede ser nulo, en caso de ser alumno uveg insertará el programa al cual esta inscrito
     */
    public static int insertarUsuarioSqLite(string usuario, string nombre, string rol, string gradoEstudios, string programa, string contraseña, string imagenUrl) {
        string query = "INSERT INTO usuario (usuario, nombre, rol, gradoEstudios, programa, fechaRegistro, status, SyncroStatus, password, imagen) VALUES ('" + usuario + "','" + nombre + "','" + rol + "','" + gradoEstudios + "','" + programa + "', dateTime('now','localtime'), 1, 0, '"+ contraseña +"','" + imagenUrl + "');";
        var result = conexionDB.alterGeneral(query);
        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que inserta los datos del usuario en la base de datos local
     * @param usuario matricula o correo del usuario
     * @param nombre nombre del usuario
     * @param rol tipo de usuario puede ser usuarioUveg, invitado o invitadoFacebook
     * @param gradoEstudios puede ser nulo, en caso de ser alumno uveg insertará el nivel de estudios que tiene
     * @param programa puede ser nulo, en caso de ser alumno uveg insertará el programa al cual esta inscrito
     */
    public static int insertarUsuarioSqLite(string usuario, string nombre, string rol, string gradoEstudios, string programa, string fechaRegistro, int status, string contraseña, string imagenUrl) {
        string query = "INSERT INTO usuario (usuario, nombre, rol, gradoEstudios, programa, fechaRegistro, status, syncroStatus, password, imagen) VALUES ('" + usuario + "', '" + nombre + "', '" + rol + "', '" + gradoEstudios + "', '" + programa + "',  dateTime('now','localtime'), " + status + ", 2, '" + contraseña + "','" + imagenUrl + "')";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que consulta si es que el usuario que esta ingresado ya esta dado de alta
     * @param usuario matricula o correo electronico del usuario
     */
    public static string consultarUsuarioSqLite(string usuario) {
        string query = "SELECT * FROM usuario WHERE usuario = '" + usuario + "';";
        var result = conexionDB.selectGeneral(query);
        return result;
    }

    public static userDataSqLite[] consultarUsuariosSqLite() {
        string query = "SELECT * FROM usuario;";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            byte[] bytes = Encoding.Default.GetBytes(result);
            result = Encoding.UTF8.GetString(bytes);
            result = "{\"usuarios\":[" + result + "]}";
            usersAllDataSqLite data = JsonUtility.FromJson<usersAllDataSqLite>(result);
            return data.usuarios;
        } else {
            return null;
        }
    }

    /** Función que consulta el id del usuario
     * @param usuario matricula o correo electronico del usuario
     */
    public static string consultarIdUsuarioSqLite(string usuario) {
        string query = "SELECT id FROM usuario WHERE usuario = '" + usuario + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            userDataSqLite data = JsonUtility.FromJson<userDataSqLite>(result);
            return data.id;
        } else {
            return "0";
        }
    }

    /** Función que consulta el id del usuario
     * @param usuario matricula o correo electronico del usuario
     */
    public static userDataSqLite consultarLoginUsuarioSqLite(string usuario, string password) {
        string query = "SELECT * FROM usuario WHERE usuario = '" + usuario + "' AND password = '" + password + "';";
        var result = conexionDB.selectGeneral(query);
        if (result != "0") {
            userDataSqLite data = JsonUtility.FromJson<userDataSqLite>(result);
            return data;
        } else {
            Debug.Log("161 WSUsu No hay datos en primer consulta");
            query = "SELECT usuario FROM usuario WHERE usuario = '" + usuario + "';";
            var result2 = conexionDB.selectGeneral(query);
            if (result2 != "0") {
                userDataSqLite data = JsonUtility.FromJson<userDataSqLite>(result2);
                return data;
            } else {
                Debug.Log("168 WSUsu No hay datos en segunda consulta");
                return null;
            }
        }
    }

    /** Función para actualizar los datos del usuario
     * @param usuario matricula o correo electronico del usuario
     * @param nombre nombre completo del usuario
     * @param rol rol del usuario
     * @param gradoEstudios el grado de estudios del usuario
     * @param programa la carrera del usuario
     * @param status estado del usuario
     */
    public static int updateUserSqlite(string usuario, string nombre, string rol, string gradoEstudios, string programa, int status) {
        string query = "UPDATE usuario SET usuario = '" + usuario + "', nombre = '" + nombre + "', rol = '" + rol + "', gradoEstudios = '" + gradoEstudios + "', programa = '" + programa + "', status = " + status + " WHERE usuario = '" + usuario + "'";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static int updateUserSqlite(string usuario, string imagen) {
        string query = "UPDATE usuario SET imagen = '" + imagen + "' WHERE usuario = '" + usuario + "'";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    public static int updateSyncroStatusSqlite(string id, int sincroStatus) {
        string query = "UPDATE usuario SET syncroStatus = '" + sincroStatus + "' WHERE id = '" + id + "'";
        var result = conexionDB.alterGeneral(query);

        if (result == 1) {
            return 1;
        } else {
            return 0;
        }
    }

    /** Función que verifica di el usuario existe
     * @param usuario matricula o correo electronico del usuario
     */
    public static int existUserSqlite(string usuario) {
        string query = "SELECT * FROM usuario WHERE usuario = '" + usuario + "'";
        var result = conexionDB.selectGeneral(query);

        if (result != "0") {
            return 1;
        } else {
            return 0;
        }
    }


    /** Coroutine que consulta base de datos de SII para obtener los datos del usuario
     * @param usuario matricula, correo institucional o correo personal del alumno que ingresa
     * @param contraseña, contraseña del usuario del cual quieres consultar datos, sirve para verificar el login
     */
    public static IEnumerator getUserData(string usuario, string contraseña) {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;
        form.AddField("data", "{\"usuario\": \"" + usuario + "\", \"contrasena\": \"" + contraseña + "\"}");
        using (UnityWebRequest www = UnityWebRequest.Post(USUARIO_DATA, form)) {
            AsyncOperation asyncLoad = www.SendWebRequest();
            while (!asyncLoad.isDone) {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError) {
                if (GameObject.Find("Mensaje")) {
                    GameObject.Find("Mensaje").GetComponent<Text>().text = "Se requiere conexión a Internet";
                }
                Debug.Log(www.error + " 224 WSUsu");
                GameObject.Find("Player").GetComponent<PlayerManager>().setMensaje(false, "");
            } else {
                string text;
                text = www.downloadHandler.text;
                text = text.Replace("[", "");
                text = text.Replace("]", "");
                JsonResponse data = JsonUtility.FromJson<JsonResponse>(text);
                if (data.data.Nombre != "") {
                    if (data.estatusCode == "001") {
                        string nombreCompleto = data.data.Nombre + " " + data.data.PrimerApellido + " " + data.data.SegundoApellido;
                        appManager manager = GameObject.Find("AppManager").GetComponent<appManager>();
                        manager.setUsuario(data.data.Usuario);
                        manager.setNombre(nombreCompleto);
                        manager.setCorreo(data.data.Correo);
                        manager.setImagen(data.data.Imagen);
                        manager.setGradoEstudios(data.data.ProgramaEstudio);
                        var idLocal = consultarIdUsuarioSqLite(data.data.Usuario);
                        if (idLocal == "0") {
                            insertarUsuarioSqLite(data.data.Usuario, nombreCompleto, "usuarioUveg", data.data.ProgramaAcademico, data.data.ProgramaEstudio, contraseña, data.data.Imagen);
                        }
                        webServiceLog.insertarLogSqLite(data.data.Usuario);
                        webServiceRegistro.validarAccionSqlite("Login teclado", data.data.Usuario, "Login");
                        SceneManager.LoadScene("menuCategorias");
                    } else {
                        //Aqui va mensaje de contraseña incorrecta
                        //GameObject.FindObjectOfType<keyboardManager>().mensaje.text = "Contraseña incorrecta";
                        GameObject.FindObjectOfType<PlayerManager>().setMensaje(false, "");
                        Debug.Log("Contraseña incorrecta 252 WSUsu");
                        if (GameObject.Find("Mensaje")) {
                            GameObject.Find("Mensaje").GetComponent<Text>().text = "Contraseña incorrecta";
                        }
                    }
                } else {
                    //Preguntas si existe en la BD del SII.unity
                    WWWForm form2 = new WWWForm();
                    Dictionary<string, string> headers2 = form2.headers;
                    form2.AddField("metodo", "consultarUsuario");
                    form2.AddField("usuario", usuario);
                    form2.AddField("password", contraseña);
                    using (UnityWebRequest www2 = UnityWebRequest.Post("http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServiceUsuarios.php", form2)) {
                        AsyncOperation asyncLoad2 = www2.SendWebRequest();
                        // Wait until the asynchronous scene fully loads
                        while (!asyncLoad2.isDone) {
                            yield return null;
                        }

                        if (www2.isNetworkError || www2.isHttpError) {
                            Debug.Log(www2.error + " 269 WSUsu");
                        } else {
                            string text2;
                            text2 = www2.downloadHandler.text;
                            if (text2 == "0") {
                                Debug.Log("El usuario no existe" + " 274 WSUsu");
                            } else {
                                //text2 = "{\"userDataSqLite2\":" + text2 + "}";
                                Debug.Log(text2 + " 277 WSUsu");
                                text2 = text2.Replace("[", "");
                                text2 = text2.Replace("]", "");
                                userDataSqLite2 myObject = JsonUtility.FromJson<userDataSqLite2>(text2);
                                appManager manager = GameObject.Find("AppManager").GetComponent<appManager>();
                                manager.setUsuario(myObject.usuario);
                                manager.setNombre(myObject.nombre);
                                manager.setGradoEstudios(myObject.programa);
                                var idLocal = consultarIdUsuarioSqLite(myObject.usuario);
                                if (idLocal == "0") {
                                    insertarUsuarioSqLite(myObject.usuario, myObject.nombre, "usuarioApp", myObject.programa, myObject.programa, myObject.password, "http://sii.uveg.edu.mx/unity/dropsV2/img/invitado.png");
                                }
                                webServiceLog.insertarLogSqLite(myObject.usuario);
                                webServiceRegistro.validarAccionSqlite("Login teclado", myObject.usuario, "Login");
                                SceneManager.LoadScene("menuCategorias");
                            }
                        }
                    }
                    GameObject.FindObjectOfType<PlayerManager>().setMensaje(false, "");
                    Debug.Log("El usuario no existe" + " 296 WSUsu");
                    if (GameObject.Find("Mensaje")) {
                        GameObject.Find("Mensaje").GetComponent<Text>().text = "El usuario no existe";
                    }
                }
            }
        }
    }

    public static IEnumerator getUserData(string usuario) {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;
        form.AddField("data", "{\"usuario\": \"" + usuario + "\", \"contrasena\": \"\"}");
        using (UnityWebRequest www = UnityWebRequest.Post(USUARIO_DATA, form)) {
            AsyncOperation asyncLoad = www.SendWebRequest();
            // Wait until fully loads
            while (!asyncLoad.isDone) {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError) {
                GameObject.Find("Mascota").GetComponentInChildren<Text>().text = "Se requiere conexión a internet";
                Debug.Log(www.error + " 314 WSUsu");
            } else {
                string text;
                text = www.downloadHandler.text;
                text = text.Replace("[", "");
                text = text.Replace("]", "");
                JsonResponse data = JsonUtility.FromJson<JsonResponse>(text);
                if (data.data.Usuario != "") {
                    string nombreCompleto = data.data.Nombre + " " + data.data.PrimerApellido + " " + data.data.SegundoApellido;
                    appManager manager = GameObject.Find("AppManager").GetComponent<appManager>();
                    var res = updateUserSqlite(data.data.Usuario, data.data.Imagen);
                    manager.setUsuario(data.data.Usuario);
                    manager.setNombre(nombreCompleto);
                    manager.setCorreo(data.data.Correo);
                    manager.setImagen(data.data.Imagen);
                    if (data.data.ProgramaEstudio != null && data.data.ProgramaEstudio != "") {
                        manager.setGradoEstudios(data.data.ProgramaEstudio);
                    } else {
                        manager.setGradoEstudios("Usuario UVEG");
                    }

                    webServiceRegistro.validarAccionSqlite("Login Pairing Code", data.data.Usuario, "Login");
                    //webServiceRegistro.insertarRegistroSqLite("Login Pairing Code", data.data.Usuario, 1);
                } else {
                    //Aqui va mensaje de usuario incorrecto
                    Debug.Log("El usuario no existe");
                }
            }
        }
    }

    /** Coroutine que consulta base de datos de SII para obtener los datos del usuario
     * @param usuario matricula, correo institucional o correo personal del alumno que ingresa
     * @param name nombre del usuario de facebook
     * @facebook bool que detecta si se inicio sesión con facebook
     */
    public static IEnumerator getUserData(string usuario, string name, string imagen) {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;
        form.AddField("data", "{\"usuario\":\"" + usuario + "\"}");
        using (UnityWebRequest www = UnityWebRequest.Post(USUARIO_DATA, form)) {
            AsyncOperation asyncLoad = www.SendWebRequest();
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone) {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError) {
                GameObject.Find("Mascota").GetComponentInChildren<Text>().text = "Se requiere conexión a internet";
                Debug.Log(www.error + " 361 WSUsu");
            } else {
                string text;
                text = www.downloadHandler.text;
                text = text.Replace("[", "");
                text = text.Replace("]", "");
                JsonResponse data = JsonUtility.FromJson<JsonResponse>(text);
                appManager manager = GameObject.Find("AppManager").GetComponent<appManager>();
                if (data.data.Usuario != "") {
                    string nombreCompleto = data.data.Nombre + " " + data.data.PrimerApellido + " " + data.data.SegundoApellido;
                    manager.setUsuario(data.data.Usuario);
                    manager.setNombre(nombreCompleto);
                    manager.setCorreo(data.data.Correo);
                    manager.setImagen(data.data.Imagen);
                    manager.setGradoEstudios(data.data.ProgramaEstudio);
                    if (consultarUsuarioSqLite(data.data.Usuario) != "0") {
                        webServiceLog.insertarLogSqLite(data.data.Usuario);
                        webServiceRegistro.insertarRegistroSqLite("Login Facebook", data.data.Usuario, 1);
                        SceneManager.LoadScene("menuCategorias");

                    } else {
                        if (insertarUsuarioSqLite(data.data.Usuario, nombreCompleto, "usuarioUveg", data.data.ProgramaAcademico, data.data.ProgramaEstudio, "", data.data.Imagen) == 1) {
                            webServiceLog.insertarLogSqLite(data.data.Usuario);
                            //webServiceRegistro.insertarRegistroSqLite("Login Facebook", data.data.Usuario, 2);
                            webServiceRegistro.validarAccionSqlite("Login Facebook", data.data.Usuario, "Login");
                            SceneManager.LoadScene("menuCategorias");
                        } else {
                            Debug.Log("Fallo el insert");
                        }
                    }
                } else {
                    manager.setUsuario(usuario);
                    manager.setNombre(name);
                    manager.setCorreo(usuario);
                    manager.setImagen(imagen);
                    manager.setGradoEstudios(data.data.ProgramaEstudio);
                    if (consultarUsuarioSqLite(usuario) != "0") {
                        webServiceLog.insertarLogSqLite(usuario);
                        webServiceRegistro.insertarRegistroSqLite("Login Facebook", usuario, 2);
                        SceneManager.LoadScene("menuCategorias");
                    } else {
                        if (insertarUsuarioSqLite(usuario, name, "usuarioFacebook", "", "","", imagen) == 1) {
                            webServiceLog.insertarLogSqLite(data.data.Usuario);
                            webServiceRegistro.insertarRegistroSqLite("Login Facebook", data.data.Usuario, 2);
                            SceneManager.LoadScene("menuCategorias");
                        } else {
                            Debug.Log("Fallo el insert");
                        }
                    }
                }
            }
        }
    }

    public static IEnumerator insertUsuario(string usuario, string nombre, string contraseña) {
        ////////////////////////////
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = API_KEY;
        Debug.Log("{\"usuario\": \"" + usuario + "\", \"contrasena\": \"\"}");
        form.AddField("data", "{\"usuario\": \"" + usuario + "\", \"contrasena\": \"\"}");
        using (UnityWebRequest www = UnityWebRequest.Post(USUARIO_DATA, form)) {
            AsyncOperation asyncLoad = www.SendWebRequest();
            // Wait until fully loads
            while (!asyncLoad.isDone) {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError) {
                GameObject.Find("Mascota").GetComponentInChildren<Text>().text = "Se requiere conexión a internet";
                Debug.Log(www.error + " 429 WSUsu");
                GameObject.Find("Mascota").GetComponentInChildren<Text>().text = "Se requiere conexión a internet";
                GameObject.FindObjectOfType<keyboardManager>().setUsuario("");
                GameObject.FindObjectOfType<keyboardManager>().setNombre("");
                GameObject.FindObjectOfType<keyboardManager>().setPassword("");
                GameObject.FindObjectOfType<keyboardManager>().setPassword2("");

            } else {
                string text;
                text = www.downloadHandler.text;
                text = text.Replace("[", "");
                text = text.Replace("]", "");
                JsonResponse data = JsonUtility.FromJson<JsonResponse>(text);
                if (data.data.Nombre != "") {
                    Debug.Log("El usuario ya existe");
                } else {
                    GameObject.FindObjectOfType<PlayerManager>().setMensaje(true, "");
                    WWWForm form2 = new WWWForm();
                    Dictionary<string, string> headers2 = form2.headers;
                    form2.AddField("metodo", "insertarUsuario");
                    form2.AddField("usuario", usuario);
                    form2.AddField("nombre", nombre);
                    form2.AddField("password", contraseña);
                    using (UnityWebRequest www2 = UnityWebRequest.Post("http://sii.uveg.edu.mx/unity/dropsV2/controllers/webServiceUsuarios.php", form2)) {
                        AsyncOperation asyncLoad2 = www2.SendWebRequest();
                        // Wait until the asynchronous scene fully loads
                        while (!asyncLoad2.isDone) {
                            yield return null;
                        }

                        if (www2.isNetworkError || www2.isHttpError) {
                            Debug.Log(www2.error + " 460 WSUsu");
                        } else {
                            string text2;
                            text2 = www2.downloadHandler.text;
                            if (text2 == "0") {
                                Debug.Log("Fallo el insert");
                            } else {
                                GameObject.FindObjectOfType<keyboardManager>().setUsuario("");
                                GameObject.FindObjectOfType<keyboardManager>().setNombre("");
                                GameObject.FindObjectOfType<keyboardManager>().setPassword("");
                                GameObject.FindObjectOfType<keyboardManager>().setPassword2("");
                                GameObject.FindObjectOfType<PlayerManager>().setMensaje(false, "");
                                GameObject.FindObjectOfType<mainMenuManager>().cambiarVista(2);
                                //GameObject.Find("Mascota").GetComponentInChildren<Text>().text = "Tu usuario se registro con éxito";
                            }
                        }
                    }
                }
            }
        }
    }


}
