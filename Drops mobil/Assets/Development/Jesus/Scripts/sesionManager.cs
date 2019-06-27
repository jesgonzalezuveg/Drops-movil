using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sesionManager : MonoBehaviour
{
    private appManager manager;

    private void Awake() {
        manager = GameObject.Find("AppManager").GetComponent<appManager>();
        if (GameObject.FindObjectOfType<appManager>()) {
            GameObject.FindObjectOfType<appManager>().cargando.SetActive(false);
        }
    }
    public void changeScene() {
        StartCoroutine(GameObject.FindObjectOfType<appManager>().cambiarEscena("menuCategorias", "menuCategorias"));
    }

    public void logOut() {
        manager.isFirstLogin = true;
        manager.setUsuario(null);
        manager.setNombre(null);
        manager.setCorreo(null);
        manager.setImagen(null);
        manager.setGradoEstudios(null);
        int res = webServiceUsuario.updateAllSesionStatusSqlite(0);
        if (res == 0) {
            Debug.Log("Error en sesionManager en linea 23");
            Debug.Log("Error al modificar los status de sesion de los usuarios");
        }
        webServiceRegistro.validarAccionSqlite("Logout", manager.getUsuario(), "Cerrar sesión");
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("mainMenu", "mainMenu"));
    }
}
