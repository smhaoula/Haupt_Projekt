using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{  

   bool alreadyOpened = false;
   bool open = false;
     Animator _animator;
      void Awake() => _animator = GetComponent<Animator>();
   
   
   void OnTriggerEnter(Collider other){
      
      if(!alreadyOpened){
         if(other.tag == "Player"){
            alreadyOpened = true;
            open = true;
            _animator.SetBool("Open", open);
            StartCoroutine(ChestWait());
         }
      }
     
   }
   IEnumerator ChestWait(){
      yield return new WaitForSeconds(0.5f);
      open = false;
      _animator.SetBool("Open", open);
   }
}
