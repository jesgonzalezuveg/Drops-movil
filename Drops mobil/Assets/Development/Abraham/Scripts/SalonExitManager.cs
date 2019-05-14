using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SalonExitManager : MonoBehaviour {

    public void clickSalir() {
        StartCoroutine(wait());
    }

    IEnumerator wait() {
        yield return new WaitUntil(() => gameObject.GetComponentInChildren<AudioSource>().isPlaying == false);
        StartCoroutine(GameObject.Find("AppManager").GetComponent<appManager>().cambiarEscena("menuCategorias","mainMenu"));
    }
}
