using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunPlayer : MonoBehaviour
{
    //public int moveSpeed; // how fast to move
    public float velIncr;
    private bool useVelocity = true;
    private float xVelocity;
    public float maxVelocity;         // how fast the player can move
    public float moveForce = 25;      // the force the player moves at

    public ComboDisplay head;         // to display the combo

    private float touchDeadZone = 10; // swiping doesn't happen if change is less than the dead zone
    public float jumpForce;           // added to player when player jumps on enemy
    public float fallForce;           // for slamming

    public int deathTreshold = -10;   // player cannot go below this height without dying

    public LayerMask groundLayer;     // the ground
    public LayerMask enemyLayer;
    public ContactFilter2D enemyFilter;
    public Transform feet;
    public Boots boots;

    public StatsDisplay stats;

    private float snot = 0;
    public float maxSnot = 400;

    public bool upright = true;
    private bool grounded = false;  // touching ground?
    private bool slamming = false; // indicates whether slamming
    private bool canSlam = true;
    private bool canJump = false;

    private bool jumping = false; // indicates whether jumping

    private int hp;

    Rigidbody2D _rigidbody;

    public int partsPassed;

    private AudioSource audio;

    public AudioClip sniff;

    

    void Start()
    {
        audio = GetComponent<AudioSource>();
        xVelocity = maxVelocity;
        partsPassed = 0;
        hp = PublicVars.max_hp;
        head = GetComponent<ComboDisplay>();
        _rigidbody = GetComponent<Rigidbody2D>();
        transform.position = PublicVars.spawnPos;
    }

    /**
     * determines how to handle a touch on the right side of the screen
     */
    void HandleTouch(int i)
    {
        Touch touch = Input.GetTouch(i);
        //if (touch.position.x > Screen.width / 2)
        //{
            switch (touch.phase)
            {
                case TouchPhase.Began: // jump if grounded
                    jumping = true;
                    Flip();
                    break;
                case TouchPhase.Moved: // slam if swiping down enough
                    if (!grounded && canSlam && !slamming && touch.deltaPosition.y < -touchDeadZone)
                    {
                        // flip if not upright
                        if (!upright)
                        {
                            Flip();
                        }
                        jumping = false;
                        StartCoroutine(StopVelocity(.5f));
                        _rigidbody.velocity = new Vector2(0, -fallForce);
                        StartCoroutine(SlamTime(.5f));
                        //kicking = false;
                        //transform.eulerAngles = Vector3.zero;
                        //transform.position = new Vector2(transform.position.x, transform.position.y + .5f);
                    }
                    break;
                case TouchPhase.Ended: // done with jumping
                    //jumping = false;
                    break;
            }
        //}
    }

    void Update()
    {

        if (Time.timeScale == 0) { return; }

        if (transform.position.y < deathTreshold)
        {
            Die();
        }

        /* set part if needed */
        if (PublicVars.nextPartSet && transform.position.x > PublicVars.part.end.position.x)
        {
            PublicVars.part = PublicVars.nextPart;
            PublicVars.nextPart = null;
            PublicVars.nextPartSet = false;

            if (++partsPassed % PublicVars.difficulty == 0)
            {
                maxVelocity += velIncr++;
                xVelocity = maxVelocity;
            }
        }

        /* move */
        if (useVelocity)
        {
            _rigidbody.velocity = new Vector2(xVelocity, _rigidbody.velocity.y);
        }
        //_rigidbody.AddForce(Vector2.right * moveForce);
        if (_rigidbody.velocity.x > maxVelocity)
        {
            _rigidbody.velocity = new Vector2(maxVelocity, _rigidbody.velocity.y);
        }

        bool lastGrounded = grounded;
        // get ground collision
        grounded = Physics2D.OverlapCircle(feet.position, .7f, groundLayer);

        if (grounded == lastGrounded)
        {
            canJump = canJump || grounded;
        }
        else
        {
            // if there was a change in grounded
            // if landed, end combo
            if (grounded) {
                PublicVars.comboCount = 0;
                head.UpdateCombo();
                useVelocity = true;
            }
            canJump = grounded;
        }

        if (grounded) 
        {
            slamming = false;
        }

        // TODO possible pivot
        if (!PublicVars.useGravity)
        {
            if (canJump && !grounded)
            {
                jumping = true;
            }
        }

        // handle touches
        int touchCount = Input.touchCount;
        if (touchCount > 0)
        {
            for (int i = 0; i < touchCount; ++i)
            {
                HandleTouch(i);
            } // for each touch
        }
    } // update

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("collectible"))
        {
            audio.PlayOneShot(sniff);
            Destroy(collision.gameObject);
            ++PublicVars.collectibles;
            snot += 1f + PublicVars.comboCount;
            stats.slider.value = snot / maxSnot;
            if (!PublicVars.useGravity)
            {
                jumping = false;
                canJump = false;
                _rigidbody.gravityScale = Mathf.Abs(_rigidbody.gravityScale);
                if (transform.localScale.y < 0)
                {
                    transform.localScale *= new Vector2(1, -1);
                }
            }
        }
    }

    // jump on enemy or hit pain
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            float enemyThreshold = 0.1f;
            // if player is above or below enemy (depending on gravity)
            bool canBop = upright 
                ? transform.position.y >= collision.gameObject.transform.position.y + enemyThreshold 
                : transform.position.y <= collision.gameObject.transform.position.y - enemyThreshold;
            if (canBop)
            {
                ++PublicVars.comboCount;
                // display combo count
                head.UpdateCombo();
                // add bounce to player
                int scale = upright ? 1 : -1;
                if (scale == 1 && slamming)
                {
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, fallForce);
                    slamming = false;
                    canSlam = true;
                }
                else
                {
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce * 2f * scale);
                }
                Destroy(collision.collider.gameObject);
                useVelocity = true;
            }
            else
            {
                print("hit a pain point there");
                Vector2 vel = (transform.position - collision.transform.position).normalized * 15f;
                _rigidbody.velocity = vel;
                TakeDamage();
                StartCoroutine(StopVelocity(.4f));
            }
        }
        if (collision.gameObject.CompareTag("pain"))
        {
            TakeDamage();
        }
        if (collision.gameObject.CompareTag("bouncer"))
        {
            StartCoroutine(StopVelocity(0.5f));
            //Destroy(collision.gameObject);
        }
    }

    private void Flip()
    {
        _rigidbody.gravityScale = -_rigidbody.gravityScale;
        transform.localScale *= new Vector2(1, -1);
        upright = !upright;
    }

    // resets fall grace period
    public IEnumerator SlamTime(float wait)
    {
        slamming = true;
        yield return new WaitForSeconds(wait);
        slamming = false;
    }

    public IEnumerator StopVelocity(float wait)
    {
        useVelocity = false;
        yield return new WaitForSeconds(wait);
        useVelocity = true;
    }

    private void TakeDamage(int damage = 1)
    {
        if ((hp -= damage) <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        PublicVars.comboCount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
