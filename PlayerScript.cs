using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public AudioClip musicClipOne;

    public AudioClip musicClipTwo;

    public AudioSource musicSource;

    Animator anim;

    private Rigidbody2D rd2d;

    public float speed;

    public Text score;

    private int scoreValue = 0;

    public Transform groundcheck;

    public float checkRadius;

    public LayerMask allGround;

    private bool isOnGround;

    public Text winText;			//Store a reference to the UI Text component which will display the 'You win' message.

    public Text lives;

    private int livesValue = 3;

    private bool facingRight = true;

    public float prevHorizontalPos = 0;

    private bool prevHorizontalMove = false;

    // Start is called before the first frame update
    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        score.text = scoreValue.ToString();
        lives.text = livesValue.ToString();

        //Set the text property of our scoreText object to "Score: " followed by the number stored in our score variable.
        score.text = "Score: " + scoreValue.ToString();

        //Initialze winText to a blank string since we haven't won yet at beginning.
        winText.text = "";

        //Set the text property of our livesText object to "Lives: " followed by the number stored in our lives variable.
        lives.text = "Lives: " + livesValue.ToString();

        musicSource.clip = musicClipOne;
        musicSource.Play();
        musicSource.loop = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isOnGround = Physics2D.OverlapCircle(groundcheck.position, checkRadius, allGround);
        float hozMovement = Input.GetAxis("Horizontal");
        float vertMovement = Input.GetAxis("Vertical");
        rd2d.AddForce(new Vector2(hozMovement * speed, vertMovement * speed));
        // Debug.Log(groundcheck.position.x - prevHorizontalPos);
        prevHorizontalMove = (Mathf.Abs(groundcheck.position.x - prevHorizontalPos) > 0.01);
        prevHorizontalPos = groundcheck.position.x;
        if (isOnGround && (prevHorizontalMove || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            Debug.Log("On Ground And Moving");
            anim.SetInteger("State", 1);
        }
        else if (isOnGround && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Not Moving");    
            anim.SetInteger("State", 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            anim.SetInteger("State", 2);
        }
         
        else if (isOnGround && !Input.GetKey(KeyCode.W))
        {
            anim.SetInteger("State", 0);
        }
        
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (facingRight == false && hozMovement > 0)
        {
            Flip();
        }
        else if (facingRight == true && hozMovement < 0)
        {
            Flip();
        }
    }

    //OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);

            livesValue -= 1;
            lives.text = livesValue.ToString();
        }

        if (livesValue == 0)
        {
            winText.text = "You lose! Game created by John Keith!";
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Coin")
        {
            scoreValue += 1;
            score.text = scoreValue.ToString();
            Destroy(collision.collider.gameObject);

            if (scoreValue == 4)
            {
                transform.position = new Vector2(28.2f, 0f);
                livesValue = 3;
                lives.text = livesValue.ToString();

            }

            //Check if we've collected all 8 pickups. If we have...
            if (scoreValue >= 8)
                //... then set the text property of our winText object to "You win!"
                winText.text = "You win! Game created by John Keith!";
        }

        if (scoreValue >= 8)
        {
            musicSource.clip = musicClipTwo;
            musicSource.Play();
            musicSource.loop = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" && isOnGround)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rd2d.AddForce(new Vector2(0, 3), ForceMode2D.Impulse);
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x = Scaler.x * -1;
        transform.localScale = Scaler;
    }
}
