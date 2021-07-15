using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour , IDamageable
{
    public CharacterController controller;
    [SerializeField]
    GameObject Image;

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
    float _timeColliding;
    public float timeThreshold = 2f;

    public GameObject QuestObject;
    public Sprite QuestImage;
    public bool pickedUpQuestObject;
    int playerDamage;
    public TriggerTextManager textManager;

    void Awake() => _animator = GetComponent<Animator>();

    void Start()
    {
        textManager = FindObjectOfType<TriggerTextManager>();
        playerDamage = 10;
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
                    parentObject.GetComponent<Enemyki>().TakeDamage(playerDamage);
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

    public void IncreasePlayerDamage()
    {
        playerDamage += 5;
    }

    public void StartQuest(GameObject quest, Sprite image)
    {
        QuestObject = quest;
        QuestImage = image;
    }

    public void FinishQuest()
    {
        QuestObject = null;
        QuestImage = null;
        pickedUpQuestObject = false;
    }


    void OnTriggerEnter(Collider other)
    {
        if(QuestObject!=null)
        {
            if(other.gameObject.tag.Equals(QuestObject.tag))
            {
                pickedUpQuestObject = true;
                textManager.SetQuestImage(QuestImage);

            }
        }
        
    }

    void OnCollisionEnter(Collision collision) {
         if (collision.gameObject.tag == "EnemyCollider") {
             _timeColliding = 0f;
             TakeDamage(10);
         }
    }

    void OnCollisionStay(Collision collision){
        if(collision.gameObject.CompareTag("EnemyCollider")){
            if (_timeColliding < timeThreshold) {
                 _timeColliding += Time.deltaTime;
             } else {
                 // Time is over theshold, player takes damage
                 TakeDamage(10);
                 // Reset timer
                 _timeColliding = 0f;
             }

        }
    }

    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            GameOver();
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void GameOver()
    {
        Image.SetActive(true);
        StartCoroutine(LoadMenu());
    }
    IEnumerator LoadMenu()
    {

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("1");
    }
}
