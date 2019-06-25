using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class mainMenuManager : MonoBehaviour {

    public GameObject[] vistas;
    GameObject vistaActiva;
    GameObject mascota;
    public GameObject terminosCondiciones;
    public GameObject btnTerminos;

    public void Start() {
        vistaActiva = vistas[0];
        mascota = GameObject.Find("Mascota");
        if (GameObject.FindObjectOfType<appManager>()) {
            GameObject.FindObjectOfType<appManager>().cargando.SetActive(false);
        }
    }

    /**
     * Función que se ejecuta al pulzar el boton Jugar
     * 
     */
    public void cambiarVista(int vista) {
        vistaActiva.SetActive(false);
        vistas[vista].SetActive(true);
        vistaActiva = vistas[vista];
        if (GameObject.FindObjectOfType<keyboardManager>()) {
            GameObject.FindObjectOfType<keyboardManager>().setUsuario("");
            GameObject.FindObjectOfType<keyboardManager>().setNombre("");
            GameObject.FindObjectOfType<keyboardManager>().setPassword("");
            GameObject.FindObjectOfType<keyboardManager>().setPassword2("");
            foreach (var input in GameObject.FindObjectOfType<keyboardManager>().inputs) {
                input.GetComponentInChildren<InputField>().text = "";
            }
        }

        if (vistaActiva.name == "2" || vistaActiva.name == "3") {
            btnTerminos.SetActive(true);
        } else {
            btnTerminos.SetActive(false);
        }

        valMascota();
    }

    public void pairingCode() {
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("ParingCode", "mainMenu"));
    }

    /**
     * Coroutine que espera a que termine de reprocucir el audio de click de los botones
     * para poder ir a la nueva escena
     */
    IEnumerator loadScene(string scene) {
        yield return new WaitUntil(() => this.GetComponent<AudioSource>().isPlaying == false);
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena(scene,"mainMenu"));
    }

    public void terminosCondicionesFunction() {
        if (terminosCondiciones.active == true) {
            terminosCondiciones.SetActive(false);
            btnTerminos.SetActive(true);
            vistaActiva.SetActive(true);
            valMascota();
        } else {
            terminosCondiciones.SetActive(true);
            btnTerminos.SetActive(false);
            vistaActiva.SetActive(false);
            mascota.SetActive(false);
        }
    }

    void valMascota() {
        if (vistaActiva.name == "3") {
            mascota.SetActive(false);
        } else {
            mascota.SetActive(true);
        }
    }
}
