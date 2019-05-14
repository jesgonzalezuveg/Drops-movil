using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FBholder : MonoBehaviour {

    bool bandera = false;       ///< bandera Valida si se tiene o no una sesion de facebook activa para buscar el correo desde facebook y verifica si existe en UVEG


    /**
     * Función que se manda llamar antes que inicie la escena
     * inicializa el servicio de Facebook
     */
    public void iniciar() {
        if (!FB.IsInitialized) {
            FB.Init(() => {
                if (FB.IsInitialized) {
                    FB.ActivateApp();
                } else {

                }
            }, isGameShown => {
                if (!isGameShown) {
                    Time.timeScale = 0;
                } else {
                    Time.timeScale = 1;
                }
            });
        } else {
            FB.ActivateApp();
        }
        facebookLogin();
    }

    /**
     * Función que se manda llamar cada frame de la aplicación
     * Verifica si existe una sesión en facebook
     * en caso que exista una sesion ejecuta el metodo getCorreo();
     */ 
    private void Update() {
        if (FB.IsLoggedIn) {
            if (!bandera) {
                getCorreo();
            }
            bandera = true;
        } else {

        }
    }

    /**
     * Función que se manda llamar al momento de precionar el boton login con Facebook
     * solicita los permisos que el usuario debe aceptar en el popUp de facebook 
     */
    public void facebookLogin() {
        bandera = false;
        var permisos = new List<string>() { "public_profile","email","user_friends"};
        FB.LogInWithReadPermissions(permisos);
    }

    /**
     * Función que obtiene el correo electronico de Facebook y ejecuta una busqueda en la Base de datos de UVEG
     * 
     */ 
    public void getCorreo() {
        string query = "/me?fields=email,name,picture.type(large)";
        FB.API(query, HttpMethod.GET, result => {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var email = (string)dictionary["email"];
            var name = (string)dictionary["name"];
            var imagen = (Dictionary<string,object>)dictionary["picture"];
            var data = (Dictionary<string, object>)imagen["data"];
            var img = (string)data["url"];
            appManager manager = GameObject.Find("AppManager").GetComponent<appManager>();
            StartCoroutine(webServiceUsuario.getUserData(email,name,img));
        });
    }

}