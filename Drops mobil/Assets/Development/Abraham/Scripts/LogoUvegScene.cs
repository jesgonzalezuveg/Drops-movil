using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class LogoUvegScene : MonoBehaviour {

    public RawImage rawImage;
    public VideoPlayer video;
    bool bandera = false;

    public void Update() {
        if (video.isPlaying) {
            rawImage.texture = video.texture;
            bandera = true;
        }
        if (bandera) {
            if (!video.isPlaying) {
                StartCoroutine(changeScene());
            }
        }

    }

    public IEnumerator changeScene() {
        yield return new WaitForSeconds(2);
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("mainMenu", "mainMenu"));
    }

}
