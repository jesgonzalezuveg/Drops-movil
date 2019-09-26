using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchChara : MonoBehaviour
{
    public Animator animController;
    
    void Start(){
        animController = GetComponent<Animator>();
        
    }

    IEnumerator volverIdle()
    {
        yield return new WaitForSeconds(.2f);
        animController.SetBool("toHead", false);
        animController.SetBool("toScreen", false);
    }


    public void toHead()
    {
        if (animController){
            animController.SetBool("toHead", true);
            StartCoroutine(volverIdle());
        }
       
    }


    public void toScreen()
    {
        if (animController){
            animController.SetBool("toScreen", true);
            StartCoroutine(volverIdle());
        }
    }

}
