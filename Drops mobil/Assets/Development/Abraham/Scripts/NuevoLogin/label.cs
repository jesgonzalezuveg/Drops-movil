using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class label : MonoBehaviour {

    public string placeholder;

    // Update is called once per frame
    void Update() {
        if (gameObject.GetComponentInChildren<Text>().text.Length <= 0) {
            gameObject.GetComponentInChildren<Text>().text = placeholder;
        }
    }
}
