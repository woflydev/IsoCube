using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class playerMovements : MonoBehaviour
{
    public Rigidbody rb;

    public float forwardForce = 6000f;
    public float sidewaysForce = 200f;
    public float dashForce = 20000f;

    public float speedMultiplier = 1f;
    public float speedAdditiveOverTime = 0.00001f;

    public float difficultyMultiplier = 1f;
    public float difficultyAdditiveOverTime = 0.5f;

    public bool checkJump;
    public Vector3 jumpVector;
    public float jumpForce = 0.2f;

    public bool isGrounded;

    // for the dash thingies
    public bool isDashing;
    public bool isGroundPounding;
    public bool isSuperJumping;

    public int dashCounter;
    private int currentPresses;
    private int targetPresses;
    public int minPressAddDash;
    public int maxPressAddDash;

    public float dashCoolDownTime;
    private float nextDashTime;

    public float groundPoundCoolDownTime;
    private float nextGPTime;

    public float superJumpCoolDownTime;
    private float nextSJTime;

    // the ground layer's name, for friction and gravity and weird stuff like that
    public string nameOfGroundLayer;

    // to prevent people from resetting the highscore without knowing it and pressing weird UI buttons without prompts lol
    public bool playerInputEnabled;

    private void Start()
    {
        jumpVector = new Vector3(0f, 2f, 0f);
        checkJump = false;

        isGrounded = false;

        isDashing = false;
        isGroundPounding = false;
        isSuperJumping = false;

        playerInputEnabled = true;

        // resets current presses
        currentPresses = 0;
        targetPresses = Random.Range(minPressAddDash, maxPressAddDash);
        nextDashTime = 0;
    }

    // using fixedupdate for reasons below
    private void FixedUpdate()
    {
        if (playerInputEnabled)
        {
            // moving the player is in fixedupdate, since physics is weird in Update()
            movePlayer();
        }
    }

    // update is once a frame, putting stuff here that isn' physics
    private void Update()
    {
        if (playerInputEnabled)
        {
            // continually checks if the player should jump/should be able to jump
            if (Input.GetKey(KeyCode.Space) && isGrounded)
            {
                // resets the isGrounded variable
                isGrounded = false;
                checkJump = true;
            }

            if (Input.GetKey(KeyCode.W) && !isGrounded && dashCounter > 0 && !isDashing && Time.time >= nextDashTime)
            {
                isDashing = true;
                dashCounter -= 1;

                nextDashTime = Time.time + dashCoolDownTime;
            }

            if (Input.GetKey(KeyCode.S) && !isGrounded && !isGroundPounding && Time.time >= nextGPTime)
            {
                isGroundPounding = true;

                nextGPTime = Time.time + groundPoundCoolDownTime;
            }

            if (Input.GetKey(KeyCode.LeftShift) && !isGroundPounding && !isDashing && Time.time >= nextSJTime)
            {
                isSuperJumping = true;

                nextSJTime = Time.time + superJumpCoolDownTime;
            }

            if (currentPresses >= targetPresses)
            {
                currentPresses = 0;
                targetPresses = Random.Range(minPressAddDash, maxPressAddDash);
                if (dashCounter < 5)
                {
                    dashCounter += 1;
                }
            }
        }
    }
    
    private void movePlayer()
    {
        // detects player inputs
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
            currentPresses += 1;
        }
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
            currentPresses += 1;
        }

        // superjump rigidbody code
        if (isSuperJumping)
        {
            rb.AddForce(0, sidewaysForce * 40f * Time.deltaTime, 0, ForceMode.Impulse);

            isSuperJumping = false;
        }

        // the groundpound rigidbody code
        if (isGroundPounding)
        {
            rb.AddForce(0, -sidewaysForce * 100f * Time.deltaTime, 0, ForceMode.Impulse);

            isGroundPounding = false;
        }

        // the dashing rigidbody code
        if (isDashing)
        {
            // applies dash force
            rb.AddForce(0, 0, dashForce * Time.deltaTime, ForceMode.VelocityChange);

            isDashing = false;
        }

        // if it's not dashing then just keep pushing the damn cube forward lol
        if (!isDashing)
        {
            // continuously adding the forward and 'fake gravity' force the the cube
            rb.AddForce(0, 0, forwardForce * speedMultiplier * Time.deltaTime);
            speedMultiplier += speedAdditiveOverTime;
        }

        if (checkJump)
        {
            // then applies the force lol
            rb.AddForce(jumpVector * jumpForce * Time.deltaTime, ForceMode.Impulse);
            checkJump = false;
        }
    }

    public void applyDeathForce()
    {
        // adds a kinda 'explosion force' to the player when it hits something, don't touch valuessss pls
        rb.AddExplosionForce(50, transform.position, 50f, 0.5f, ForceMode.Impulse);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(nameOfGroundLayer))
        {
            isGrounded = false;
        }
    }

    // uses oncollision stay since it detects for collisions every single frame. however, due to its sensitivity, had to tone down the jump force.
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(nameOfGroundLayer))
        {
            isGrounded = true;
            // have to set this to false here to turn off the groundPound the moment it hits the ground.
            // this is because the downwards force will stop the forwards force.
            // ik i can adjust forward force proportional to the downwards force, but i really can't be bothered.
            isGroundPounding = false;
        }
    }
}
