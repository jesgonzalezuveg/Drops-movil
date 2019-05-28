using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class usuariosCards : MonoBehaviour {

    public GameObject cardToInstantiate;

    // Start is called before the first frame update
    void Start() {
        var usuarios = webServiceUsuario.consultarUsuariosSqLite();
        foreach (var usuario in usuarios) {
            var card = Instantiate(cardToInstantiate) as GameObject;
            card.name = usuario.nombre + "card";
            card.transform.SetParent(this.transform);
            card.transform.localPosition = new Vector3(0, 0, 0);
            card.GetComponentsInChildren<Text>()[0].text = usuario.nombre;
            card.GetComponentsInChildren<Text>()[1].text = usuario.usuario;
            Debug.Log(usuario.imagen);
            StartCoroutine(getUserImg(usuario.imagen, card));
            if (usuario.nombre == "Invitado") {
                card.GetComponentsInChildren<Text>()[0].text = "Nueva partida";
                card.GetComponentsInChildren<Text>()[1].text = "(No se guardan tus datos)";
            }
            //byte[] b = new byte[4];
            //for (int j = 0; j < 4; j++) {
            //    var i = UnityEngine.Random.Range(0, 255);
            //    b[j] = Convert.ToByte(i);
            //}
            //card.GetComponentsInChildren<Image>()[1].color = new Color32(b[0], b[1], b[2], 255);
            card.transform.localScale = new Vector3(1, 1, 1);
            card.transform.localRotation = Quaternion.Euler(0, 0, 0);
            addcardEvent(card, usuario);
        }
    }

    void addcardEvent(GameObject card, webServiceUsuario.userDataSqLite usuario) {
        card.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            if (usuario.nombre == "Invitado") {
                GameObject.FindObjectOfType<appManager>().setNombre("Invitado");
                GameObject.FindObjectOfType<appManager>().setUsuario("Invitado");
                GameObject.FindObjectOfType<appManager>().setGradoEstudios("");
                GameObject.FindObjectOfType<appManager>().setImagen("http://sii.uveg.edu.mx/unity/dropsV2/img/invitado.png");
                webServiceLog.insertarLogSqLite("Invitado");
                StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("menuCategorias", "mainMenu"));
            } else {
                GetComponentInParent<mainMenuManager>().cambiarVista(2);
                GameObject.Find("inputMatricula").GetComponentInChildren<InputField>().text = usuario.usuario;
                GameObject.Find("Teclado").GetComponent<keyboardManager>().setUsuario(usuario.usuario);
            }
        });
    }

    IEnumerator getUserImg(string url, GameObject card) {
        string path = url.Split('/')[url.Split('/').Length - 1];
        if (File.Exists(Application.persistentDataPath + path)) {
            byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + path);
            Texture2D texture = new Texture2D(8, 8);
            texture.LoadImage(byteArray);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            card.GetComponentsInChildren<Image>()[1].sprite = sprite;
        } else {
            WWW www = new WWW(url);
            yield return www;
            Texture2D texture = www.texture;
            byte[] bytes;
            if (path.Split('.')[path.Split('.').Length - 1] == "jpg" || path.Split('.')[path.Split('.').Length - 1] == "jpeg") {
                bytes = texture.EncodeToJPG();
            } else {
                bytes = texture.EncodeToPNG();
            }
            File.WriteAllBytes(Application.persistentDataPath + path, bytes);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            card.GetComponentsInChildren<Image>()[1].sprite = sprite;
        }
    }

}
