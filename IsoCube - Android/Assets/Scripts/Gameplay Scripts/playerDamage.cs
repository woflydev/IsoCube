using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDamage : MonoBehaviour
{
    // for player collisions and death and fun stuff like that :)
    private gameManager gameManagerScript;
    private cameraFollow cameraFollowScript;

    public float outOfBoundsY = 10;

    // for the restarting of the level
    public float restartDelay;

    public bool debugUnkillable;

    private void Start()
    {
        // just getting some preliminaries, setting up for variable use
        gameManagerScript = FindObjectOfType<gameManager>();
        cameraFollowScript = FindObjectOfType<cameraFollow>();
    }

    private void Update()
    {
        //checkIfStartDeathOverlay();
        checkFallDamage();
    }

    // checks to see if the player should die bcs they fell off the platform
    private void checkFallDamage()
    {
        // continually checks if the player is out of bounds, resets if they are
        if (transform.position.y <= -outOfBoundsY)
        {
            // see camerFollow, switchToDeathCam function
            cameraFollowScript.switchToDeathCam(true);
            gameManagerScript.gameOverSequence();
        }
    }

    // for detecting collisions with other solid objects
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Obstacle" && !debugUnkillable)
        {
            gameManagerScript.gameOverSequence();

            cameraFollowScript.switchToDeathCam(true);

            // because we're colliding with the actual 'object' in the scene, we need to get the Parent ParticleSystem component.
            ParticleSystem collisionParticleSystem = other.gameObject.GetComponentInChildren<ParticleSystem>();
            // plays it
            collisionParticleSystem.Play();

            // finds the player's death PS and plays it (in children)
            ParticleSystem selfPS = GetComponentInChildren<ParticleSystem>();
            selfPS.Play();

            // destroys the mesh renderer that sits on the object the player collided on, making it invisible
            //Destroy(other.gameObject.GetComponentInChildren<MeshRenderer>());

            // destroys all the children
            int childrenNum = other.transform.childCount;
            for (int i = 0; i < childrenNum; i++)
            {
                Destroy(other.transform.GetChild(i).GetComponent<MeshRenderer>());
            }

            // destroys all the children of self
            int selfChildNum = transform.childCount;
            for (int i = 0; i < selfChildNum; i++)
            {
                Destroy(transform.GetChild(i).GetComponent<MeshRenderer>());
            }

            // destroys the player box's mesh renderer
            Destroy(gameObject.GetComponent<MeshRenderer>());

            // player goes boom (silently) lol
            playerMovements playerMoveScript = FindObjectOfType<playerMovements>();
            //playerMoveScript.applyDeathForce();
        }
    }
}