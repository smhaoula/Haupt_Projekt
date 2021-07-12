using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour , IDamageable
{
    public CharacterController controller;

    public Transform cam;
     Vector3 direction;

    public float speed = 6f;
    public float turnSmoothTime = 1f;
    float turnSmoothVelocitiy;
    Animator _animator;
    bool fight;
    bool roll;
    public int Health = 100;
    public int currentHealth;

    public HealthBar healthBar;

    void Awake() => _animator = GetComponent<Animator>();
    
    void Start()
    {
        gameObject.tag = "Player";
        currentHealth=Health;
        healthBar.SetMaxHealth(Health);
    }

    void Update(){
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        //Animation
        float velocityZ = Vector3.Dot(direction, transform.forward);
        float velocityX = Vector3.Dot(direction, transform.right);
        
        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);


        if(direction.magnitude >= 0.1f){

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocitiy, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f)* Vector3.forward;
            controller.Move(moveDir.normalized*speed*Time.deltaTime);
        }
        

        if(Input.GetMouseButtonDown(0)){
            fight = true;
             _animator.SetBool("Fight", fight);
            StartCoroutine(FightAnimation());
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
            foreach (Collider hitCollider in hitColliders)
            {
                //Debug.Log(hitCollider.tag);
                
                if(hitCollider.gameObject.tag.Equals("EnemyCollider")){
                    GameObject parentObject = hitCollider.transform.parent.gameObject;
                    parentObject.GetComponent<Enemyki>().TakeDamage();
                }
            }
        }

        if(Input.GetMouseButtonDown(1)){
            roll = true;
            _animator.SetBool("Roll", roll);
            StartCoroutine(RollAnimation());

        }
        
    }

    IEnumerator FightAnimation(){
        yield return new WaitForSeconds(0.5f);
        fight = false;
         _animator.SetBool("Fight", fight);
    }

    IEnumerator RollAnimation(){
        yield return new WaitForSeconds(0.5f);
        roll = false;
         _animator.SetBool("Roll", roll);
    }

    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("EnemyCollider")){
            currentHealth = currentHealth-10;
            healthBar.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(int Damage)
    {
        Health -= Damage;

        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
