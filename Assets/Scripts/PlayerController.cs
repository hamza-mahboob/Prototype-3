using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRB;
    private Animator playerAnim;
    private AudioSource playerAudio;
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public AudioClip jumpSound;
    public AudioClip crashSound;
    Vector3 startPos;
    Vector3 endPos;
    public float jumpForce = 15;
    public float gravityModifier;
    public bool isOnGround = true;
    public bool hasJumped = false;
    public bool doubleScore = false;
    public bool hasStartAnimPlayed = false;
    public bool gameOver;
    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3(-4, 0, 0);
        endPos = new Vector3(0, 0, 0);
        //gets the rigid body component on player
        playerRB = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        Physics.gravity *= gravityModifier;
        gameOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        //start animation
        if (!hasStartAnimPlayed)
            StartAnim();

        //reset timescale and double bonus
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Time.timeScale = 1;
            doubleScore = false;
        }

        //makes the player jump or double jump by adding force
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && !gameOver)
        {
            Jump();
            Debug.Log("Jump one");
            hasJumped = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && hasJumped)
        {
            Jump();
            hasJumped = false;
            Debug.Log("Jump two");
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Time.timeScale = 1.5f;
            //double the score
            doubleScore = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        //checks if player is on ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            hasJumped = false;
            dirtParticle.Play();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))  //checks if player hits an obstacle
        {
            Debug.Log("Game Over!");
            gameOver = true;
            playerAnim.SetBool("Death_b", true);
            playerAnim.SetInteger("DeathType_int", 1);
            explosionParticle.Play();
            dirtParticle.Stop();
            playerAudio.PlayOneShot(crashSound, 1.0f);
        }
    }

    void Jump()
    {
        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isOnGround = false;
        playerAnim.SetTrigger("Jump_trig");
        dirtParticle.Stop();
        playerAudio.PlayOneShot(jumpSound, 1.0f);
    }

    void StartAnim()
    {
        transform.position = Vector3.Lerp(transform.position, Vector3.zero, Time.deltaTime * 2);
        playerAnim.SetFloat("Speed_f", 0.4f);

        if (transform.position.x > -0.5f && transform.position.x < -0.3f)
        {
            gameOver = false;
            playerAnim.SetFloat("Speed_f", 1);
            hasStartAnimPlayed = true;
            Debug.Log("Game not over");
        }
    }
}
