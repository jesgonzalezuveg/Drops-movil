using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainChara : MonoBehaviour{
    Animator mainAnim;

    bool touchHead;
    bool toScreen;

    // Start is called before the first frame update
    void Start(){
        mainAnim = GetComponent<Animator>();
    }

    public void touchHeadFunc() {
        mainAnim.SetBool("toScreen", true);
        StartCoroutine(endAnim("toScreen"));
    }

    public void touchBodyFunc() {
        mainAnim.SetBool("toHead", true);
        StartCoroutine(endAnim("toHead"));
    }

    IEnumerator endAnim(string nombre) {
        yield return new WaitForSeconds(1f);
        mainAnim.SetBool(nombre, false);
    }

}
