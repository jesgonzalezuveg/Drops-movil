using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainChara : MonoBehaviour
{
    Animator mainAnim;

    bool touchHead;
    bool toScreen;

    // Start is called before the first frame update
    void Start()
    {
        mainAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch (0);
			Vector2 touchPos = Camera.main.ScreenToWorldPoint (touch.position);

            Debug.Log(touchPos);

			// processing touch phases
			switch (touch.phase) {

			// when you touch the screen for the first time and if you touches a body collider then animation is allowed
			// so if you touch another screen point then animation will not start
			// in another words you have to touch a body to start an animation
            //Function of toHead
			case TouchPhase.Began:
				if (GetComponent<Collider2D> () == Physics2D.OverlapPoint (touchPos)) {
					touchHead = true;
				}
				break;

				// if you move your finger touching a body then animation is playing by setting animators isBodyStroked variable to true
			case TouchPhase.Moved:
				if (GetComponent<Collider2D> () == Physics2D.OverlapPoint (touchPos) && touchHead) {
					mainAnim.SetBool ("toHead", true);
				}

				// if your finger is off the body then animation stops by setting animators isBodyStroked variable to false
				if (GetComponent<Collider2D> () != Physics2D.OverlapPoint (touchPos)) {
					mainAnim.SetBool ("toHead", false);
				}
				break;

				// if you stop moving your finger then animation stops by setting animators isBodyStroked variable to false
			case TouchPhase.Stationary:
				mainAnim.SetBool ("toHead", false);
				break;

				// if you release your finger then animation stops by setting animators isBodyStroked variable to true
				// and animation is not allowed anymore until you touch a body again
			case TouchPhase.Ended:
				mainAnim.SetBool ("toHead", false);
				touchHead = false;
				break;
			}
		}
    }
}
