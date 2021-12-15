using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Combo_Player : MonoBehaviour
{
    public int moveSpeed; // how fast to move

    public ComboDisplay head;

    private float touchDeadZone = 10;
    public float jumpForce;
    public float fallForce;

    public int deathTreshold = -10;

    public LayerMask groundLayer;
    public Transform feet;
    public Boots boots;

    public VariableJoystick joystick;

    private bool grounded = false;  // touching ground?
    private bool slamming = false; // indicates whether slamming
    private bool canSlam = true;
    private bool canJump = false;

    private bool jumping = false; // indicates whether jumping

    private int hp;

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
        if (touch.position.x > Screen.width / 2)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began: // jump if grounded
                    if (canJump)
                    {
                        StartCoroutine(JumpTime(.2f));
                        canJump = false;
                    }
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
                        jumping = false;
                        StartCoroutine(SlamTime(2f));
                    }
                    break;
                case TouchPhase.Ended: // done with jumping
                    jumping = false;
                    break;
            }
        }
    }

    void Update()
    {
        if (Time.timeScale == 0) { return; }

        if (transform.position.y < deathTreshold)
        {
            Die();
        }
        // move
        float xSpeed = joystick.Horizontal * moveSpeed;
        //if (Mathf.Abs(xSpeed) > 0.001f)
        //{
        _rigidbody.velocity = new Vector2(xSpeed, _rigidbody.velocity.y);
        //}

        bool lastGrounded = grounded;
        // get ground collision
        grounded = Physics2D.OverlapCircle(feet.position, .5f, groundLayer);

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
        }

        // TODO possible pivot
        if (!PublicVars.useGravity)
        {
            if (canJump)
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

        if (jumping)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);
        }

        if (slamming)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -fallForce);
        }

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
        if (collision.collider.gameObject.CompareTag("enemy"))
        {
            if (boots.byEnemy)
            {
                ++PublicVars.comboCount;
                Destroy(collision.collider.gameObject);
                // display combo count
                head.updateCombo();
                StartCoroutine(JumpTime(.2f));
                if (slamming)
                {
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, fallForce);
                    StartCoroutine(SlamGrace(.1f));
                }
                else
                {
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce * .5f);
                }
                StartCoroutine(JumpGrace(.1f));
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
        else if (collision.collider.gameObject.CompareTag("pain"))
        {
            if (--hp <= 0)
            {
                Die();
            }
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
