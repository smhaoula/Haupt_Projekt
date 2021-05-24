using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float speed = 6f;
    public float turnSmoothTime = 1f;
    float turnSmoothVelocitiy;
    Animator _animator;
    public int maxHealth = 100;
    public int currentHealth;

    //public HealthBar healthBar;

    //void Awake() => _animator = GetComponent<Animator>();
    /*void Start()
    {
        gameObject.tag = "Player";
        currentHealth=maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }*/

    void Update(){
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        //float playerYPos = transform.position.y;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f){

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocitiy, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f)* Vector3.forward;
            controller.Move(moveDir.normalized*speed*Time.deltaTime);
        }

        /*
        //Animation
        float velocityZ = Vector3.Dot(direction, transform.forward);
        float velocityX = Vector3.Dot(direction, transform.right);

        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        */
    }
}
