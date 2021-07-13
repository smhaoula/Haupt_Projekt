using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
   public Transform cam;
    void LateUpdate()
    {
        //transform.LookAt(transform.position + cam.forward);
        transform.LookAt(transform.position + Camera.main.transform.forward);

    }
}
