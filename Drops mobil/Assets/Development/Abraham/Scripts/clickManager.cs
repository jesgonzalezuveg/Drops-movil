using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class clickManager : MonoBehaviour {

    public bool cambiarDialogoMascota;
    public bool isOnlyMesagge = false;
    public string mensaje;
    public AudioClip clipDialog;
    public bool wasPlayed;

    private AudioClip click;        ///< click audioClip que almacena el audio de click 
    private AudioClip hover;        ///< hover audioClip que almacena el audio de hover
    private AudioSource source;     ///< source audioSource que reproducira los audioClips
    private EventTrigger trigger;   ///< trigger EventTrigger que manejara los eventos de hover y click

    /**
     * Funcion que se manda llamar al inicio de la aplicacion(frame 1)
     * Añade los componentes de tipo eventTrigger con sus respectivos eventos
     * obtiene de la carpeta resources los clips de audio de click y hover
     */
    void Start() {
        if (!this.GetComponent<GraphicRaycaster>()) {
            gameObject.AddComponent<GraphicRaycaster>();
        } else {
            gameObject.GetComponent<GraphicRaycaster>();
        }

        wasPlayed = false;
        click = Resources.Load("Sounds/click_2") as AudioClip;
        hover = Resources.Load("Sounds/hover") as AudioClip;

        if (!this.GetComponent<AudioSource>()) {
            source = gameObject.AddComponent<AudioSource>();
        } else {
            source = gameObject.GetComponent<AudioSource>();
        }
        if (!this.GetComponent<EventTrigger>()) {
            trigger = gameObject.AddComponent<EventTrigger>();
        } else {
            trigger = gameObject.GetComponent<EventTrigger>();
        }

        if (!isOnlyMesagge) {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => {
                source.clip = click;
                source.Play();
            });
            trigger.triggers.Add(entry);
        }

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerEnter;
        entry2.callback.AddListener((data) => {
            if (cambiarDialogoMascota) {
                if (GameObject.FindObjectOfType<appManager>().mascotaActive) {
                    if (clipDialog) {
                        if (GameObject.Find("Mascota").GetComponent<AudioSource>().clip != clipDialog) {
                            GameObject.Find("Mascota").GetComponent<AudioSource>().clip = clipDialog;
                            if (!wasPlayed) {
                                GameObject.Find("Mascota").GetComponent<AudioSource>().Play();
                                wasPlayed = true;
                            }
                            GameObject.Find("Mascota").GetComponentInChildren<Canvas>().gameObject.SetActive(false);
                            GameObject.Find("Mascota").GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
                            GameObject.Find("Mascota").GetComponentInChildren<Text>().text = mensaje;
                        }
                    }
                }
            }
            if (!isOnlyMesagge) {
                gameObject.transform.localScale += gameObject.GetComponent<RectTransform>().localScale * .05f;
                source.clip = hover;
                source.Play();
            }
        });
        trigger.triggers.Add(entry2);

        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.PointerExit;
        entry3.callback.AddListener((data) => {
            if (!isOnlyMesagge) {
                gameObject.GetComponent<RectTransform>().localScale -= gameObject.GetComponent<RectTransform>().localScale * .05f;
            }
        });
        trigger.triggers.Add(entry3);

        source.playOnAwake = false;
        source.clip = hover;
    }
}
