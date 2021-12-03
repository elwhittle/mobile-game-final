using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo_Player : MonoBehaviour
{
    public int moveSpeed; // how fast to move

    public ComboDisplay head;

    private float touchDeadZone = 10;
    public float jumpForce;
    public float fallForce;

    public LayerMask groundLayer;
    public Transform feet;

    public VariableJoystick joystick;

    private bool grounded = false;  // touching ground?
    private bool slamming = false; // indicates whether slamming
    private bool canSlam = true;
    private bool canJump = false;

    private bool jumping = false; // indicates whether jumping

    Rigidbody2D _rigidbody;

    void Start()
    {
        head = GetComponent<ComboDisplay>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    /**
     * determines how to handle a touch on the right side of the screen
     */
    void handleTouch(int i)
    {
        Touch touch = Input.GetTouch(i);
        if (touch.position.x > Screen.width / 2)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began: // jump if grounded
                    if (canJump)
                    {
                        StartCoroutine(jumpTime(.2f));
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
                        StartCoroutine(slamTime(2f));
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
        // move
        float xSpeed = joystick.Horizontal * moveSpeed;
        //if (Mathf.Abs(xSpeed) > 0.001f)
        //{
        _rigidbody.velocity = new Vector2(xSpeed, _rigidbody.velocity.y);
        //}

        bool lastGrounded = grounded;
        // get ground collision
        grounded = Physics2D.OverlapCircle(feet.position, .3f, groundLayer);

        if (grounded == lastGrounded)
        {
            canJump = canJump || grounded;
        }
        else
        {
            // if there was a change in grounded
            // if landed, end combo
            if (grounded) {
                print(PublicVars.comboCount.ToString());
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
                handleTouch(i);
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

    // jump on enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            // display combo count
            ++PublicVars.comboCount;
            head.updateCombo();
            Destroy(other.gameObject);
            StartCoroutine(jumpTime(.2f));
            if (slamming)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, fallForce);
                StartCoroutine(slamGrace(.1f));
            }
            else
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce*.3f);
            }
            StartCoroutine(jumpGrace(.1f));
        } // if landing on an enemy
    }

    // resets jump grace period
    public IEnumerator jumpTime(float wait)
    {
        jumping = true;
        yield return new WaitForSeconds(wait);
        jumping = false;
    }

    // allows for jumping
    public IEnumerator jumpGrace(float wait)
    {
        jumping = false;
        canJump = true;
        yield return new WaitForSeconds(wait);
        canJump = false;
    }

    // resets fall grace period
    public IEnumerator slamTime(float wait)
    {
        slamming = true;
        yield return new WaitForSeconds(wait);
        slamming = false;
    }

    public IEnumerator slamGrace(float wait)
    {
        slamming = false;
        canSlam = false;
        yield return new WaitForSeconds(wait);
        canSlam = true;
    }
}
