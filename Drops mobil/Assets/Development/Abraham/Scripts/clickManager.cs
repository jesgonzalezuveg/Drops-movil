using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class clickManager : MonoBehaviour {

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

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => {
            source.clip = click;
            source.Play();
        });
        trigger.triggers.Add(entry);


        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.PointerExit;
        entry3.callback.AddListener((data) => {
            
        });
        trigger.triggers.Add(entry3);

        source.playOnAwake = false;
        source.clip = hover;
    }
}
