using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunPlayer : MonoBehaviour
{
    //public int moveSpeed; // how fast to move
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

    public VariableJoystick joystick;

    private bool bootsOnEnemy = false;
    private bool grounded = false;  // touching ground?
    private bool slamming = false; // indicates whether slamming
    private bool canSlam = true;
    private bool canJump = false;

    private bool jumping = false; // indicates whether jumping

    private List<Collider2D> results = new List<Collider2D>();

    //private bool byEnemy = false;

    private int hp;
    //private bool kicking = false;

    Rigidbody2D _rigidbody;

    void Start()
    {
        hp = PublicVars.max_hp;
        head = GetComponent<ComboDisplay>();
        _rigidbody = GetComponent<Rigidbody2D>();
        transform.position = PublicVars.spawnPos;
        if (PublicVars.currentGoal != null)
        {
            PublicVars.currentGoal.gameObject.SetActive(true);
        }
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
                    _rigidbody.gravityScale = -_rigidbody.gravityScale;
                    transform.localScale *= new Vector2(1, -1);
                //if (canJump)
                //{
                //    jumping = true;
                //    //StartCoroutine(JumpTime(.2f));
                //    //canJump = false;
                //}
                //else if (!grounded && !kicking) // kick if not
                //{
                //    kicking = true;
                //    transform.eulerAngles = Vector3.forward * 90;
                //}
                break;
                // TODO pivot?
                //case TouchPhase.Stationary:
                //    if (canJump)
                //    {
                //        jumping = true;
                //    }
                //    break;
                case TouchPhase.Moved: // slam if swiping down enough
                    if (!grounded && canSlam && !slamming && touch.deltaPosition.y < -touchDeadZone)
                    {
                        _rigidbody.gravityScale = Mathf.Abs(_rigidbody.gravityScale);
                        if (transform.localScale.y < 0f)
                        {
                            transform.localScale *= new Vector2(1, -1);
                        }
                        jumping = false;
                        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -fallForce);
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
        /* move */
        //float xSpeed = joystick.Horizontal * moveSpeed;
        //_rigidbody.velocity = new Vector2(xSpeed, _rigidbody.velocity.y);
        _rigidbody.AddForce(Vector2.right * moveForce);
        if (_rigidbody.velocity.x > maxVelocity)
        {
            _rigidbody.velocity = new Vector2(maxVelocity, _rigidbody.velocity.y);
        }

        bool lastGrounded = grounded;
        // get ground collision
        grounded = Physics2D.OverlapCircle(feet.position, .7f, groundLayer);

        //byEnemy = boots.ByEnemy();

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
                head.updateCombo();
            }
            canJump = grounded;
        }

        if (grounded) 
        {
            slamming = false;
            //if (kicking)
            //{
            //    kicking = false;
            //    transform.eulerAngles = Vector3.zero;
            //    transform.position = new Vector2(transform.position.x, transform.position.y + .5f);
            //}
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

        //if (jumping)
        //{
        //    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);
        //}

        //if (slamming)
        //{
        //    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -fallForce);
        //}

    } // update

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("collectible"))
        {
            Destroy(collision.gameObject);
            ++PublicVars.collectibles;
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
        else if (collision.gameObject.CompareTag("goal"))
        {
            PublicVars.spawnPos = collision.gameObject.GetComponent<Goal>().NextGoal();
        }
    }

    // jump on enemy or hit pain
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            int enemies = Physics2D.OverlapCollider(boots.gameObject.GetComponent<CapsuleCollider2D>(), enemyFilter, results);
            results.Clear();
            //bool byEnemy = Physics2D.OverlapCircle(feet.position, .65f, enemyLayer);
            if (enemies > 0)
            {
                ++PublicVars.comboCount;
                // display combo count
                head.updateCombo();
                // add bounce to player
                int scale = transform.position.y > collision.gameObject.transform.position.y ? 1 : -1;
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

                // x velocity should bounce?
                //if (slamming)
                //{
                //    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, fallForce);
                //    StartCoroutine(SlamGrace(.1f));
                //}
                //else //if (kicking)
                //{
                //    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce * 2f);
                //}
                //StartCoroutine(JumpGrace(.1f));
            }
            else
            {
                print("hit a pain point there");
                Vector2 vel = (transform.position - collision.transform.position).normalized * 15f;
                _rigidbody.velocity = vel;
                if (--hp <= 0)
                {
                    Die();
                }
            }
        }
        if (collision.gameObject.CompareTag("pain"))
        {
            if (--hp <= 0)
            {
                Die();
            }
        }
        if (collision.gameObject.CompareTag("bouncer"))
        {
            Destroy(collision.gameObject);
        }
    }

    // resets jump grace period
    public IEnumerator JumpTime(float wait)
    {
        jumping = true;
        yield return new WaitForSeconds(wait);
        jumping = false;
    }

    // allows for jumping
    public IEnumerator JumpGrace(float wait)
    {
        jumping = false;
        canJump = true;
        yield return new WaitForSeconds(wait);
        canJump = false;
    }

    // resets fall grace period
    public IEnumerator SlamTime(float wait)
    {
        slamming = true;
        yield return new WaitForSeconds(wait);
        slamming = false;
    }

    public IEnumerator SlamGrace(float wait)
    {
        slamming = false;
        canSlam = false;
        yield return new WaitForSeconds(wait);
        canSlam = true;
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
