using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour
{
    public void StartChildCoroutine(IEnumerator coroutineMethod)
 {
     StartCoroutine(coroutineMethod);
 }
}
