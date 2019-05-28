using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class LogoUvegScene : MonoBehaviour {

    public RawImage rawImage;
    public VideoPlayer video;
    bool bandera = false;
    bool cambiandoScene = false;

    public void Update() {
        if (video.isPlaying) {
            rawImage.texture = video.texture;
            bandera = true;
        }
        if (bandera) {
            if (!video.isPlaying) {
                if (!cambiandoScene) {
                    cambiandoScene = true;
                    StartCoroutine(changeScene());
                }
            }
        }

    }

    public IEnumerator changeScene() {
        var data = webServiceUsuario.consultarUsuarioSqLiteLogueado();
        Debug.Log(data);
        yield return new WaitForSeconds(2);
        if (data == null) {
            StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("mainMenu", "mainMenu"));
        } else {
            Debug.Log("usuario no es null");
            GameObject.FindObjectOfType<appManager>().setNombre(data.nombre);
            GameObject.FindObjectOfType<appManager>().setUsuario(data.usuario);
            GameObject.FindObjectOfType<appManager>().setGradoEstudios(data.programa);
            GameObject.FindObjectOfType<appManager>().setImagen(data.imagen);
            int res = webServiceUsuario.updateSesionStatusSqlite(data.usuario, 1);
            if (res == 0) {
                Debug.Log("Error en LogoUvegScene en linea 44");
                Debug.Log("Error al modificar el status de sesion del usuario");
            }
            StartCoroutine(GameObject.FindObjectOfType<appManager>().cambiarEscena("mainMenuSesion", "mainMenuSesion"));
        }
    }

}
