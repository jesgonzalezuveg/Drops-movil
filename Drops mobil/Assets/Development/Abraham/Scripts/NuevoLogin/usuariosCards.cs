using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class usuariosCards : MonoBehaviour {

    public GameObject cardToInstantiate;

    // Start is called before the first frame update
    void Start() {
        var usuarios = webServiceUsuario.consultarUsuariosSqLite();
        Debug.Log(usuarios.Length);
        foreach (var usuario in usuarios) {
            var card = Instantiate(cardToInstantiate) as GameObject;
            card.name = usuario.nombre + "card";
            card.transform.SetParent(this.transform);
            card.transform.localPosition = new Vector3(0, 0, 0);
            card.GetComponentsInChildren<Text>()[0].text = usuario.nombre;
            card.GetComponentsInChildren<Text>()[1].text = usuario.usuario;
            if (usuario.nombre == "Invitado") {
                card.GetComponentsInChildren<Text>()[0].text = "Nueva partida";
                card.GetComponentsInChildren<Text>()[1].text = "(No se guardan tus datos)";
            }
            byte[] b = new byte[4];
            for (int j = 0; j < 4; j++) {
                var i = UnityEngine.Random.Range(0, 255);
                b[j] = Convert.ToByte(i);
            }
            card.GetComponentsInChildren<Image>()[1].color = new Color32(b[0], b[1], b[2], 255);
            card.transform.localScale = new Vector3(1, 1, 1);
            card.transform.localRotation = Quaternion.Euler(0, 0, 0);
            addcardEvent(card, usuario);
        }
    }

    void addcardEvent(GameObject card, webServiceUsuario.userDataSqLite usuario) {
        card.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            if (usuario.nombre == "Invitado") {
                Debug.Log("Click en invitado");
                GameObject.FindObjectOfType<appManager>().setNombre("Invitado");
                GameObject.FindObjectOfType<appManager>().setUsuario("Invitado");
                GameObject.FindObjectOfType<appManager>().setGradoEstudios("");
                GameObject.FindObjectOfType<appManager>().setImagen("http://sii.uveg.edu.mx/unity/dropsV2/img/invitado.png");
                StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("menuCategorias", "mainMenu"));
            } else {
                GetComponentInParent<mainMenuManager>().cambiarVista(2);
                GameObject.Find("inputMatricula").GetComponentInChildren<Text>().text = usuario.usuario;
                GameObject.Find("Teclado").GetComponent<keyboardManager>().setUsuario(usuario.usuario);
            }
        });
    }
}
