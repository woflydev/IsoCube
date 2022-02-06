using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 mainCamOffset;
    public Vector3 deathCamOffset;
    private Vector3 savedDeathCamOffset;

    // camera transition variables
    public float startZoomRate;
    public float gradualZoomRateDecrease;
    public float startZoomPosY;
    public float stopZoomPosY;
    
    private bool moveDeathCam;

    private Camera mainCamera;
    private Camera deathCamera;

    public GameObject deathCam;
    public GameObject mainCam;

    // smoothcamera variables
    public float smoothingValue = 5f;
    public float staticCamTimeLimit;
    private bool switchedToDynamicCam;
    private bool staticCam;

    private void Awake()
    {
        // sets the references for the two cameras for further use later
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        deathCamera = GameObject.FindGameObjectWithTag("Death Camera").GetComponent<Camera>();

        // saves the deathcam offset so we can use it later
        savedDeathCamOffset = deathCamOffset;
    }

    public void resetCamera()
    {
        mainCamera.enabled = true;
        deathCamera.enabled = false;
        moveDeathCam = false;
        staticCam = true;
        switchedToDynamicCam = false;

        // sets the death camera to the Y that is specified in the inspector at the time
        deathCam.transform.position = new Vector3(player.position.x, deathCamOffset.y, player.position.z);
    }

    public void Start()
    {
        resetCamera();
    }

    // update is continuously called
    private void Update()
    {
        moveMainPlayerCamera();

        if (moveDeathCam)
        {
            engageDeathCam();
        }
    }

    // continuously moves the player towards an offset pre-specified by the inspector
    private void moveMainPlayerCamera()
    {
        if (!switchedToDynamicCam)
        {
            StartCoroutine(changeToSmoothCam());
        }

        if (!staticCam)
        {
            // handles the smoothing of the main camera
            Vector3 desiredPosition = player.position + mainCamOffset;
            Vector3 smoothedPosition = Vector3.Lerp(mainCam.transform.position, desiredPosition, smoothingValue * Time.deltaTime);

            mainCam.transform.position = smoothedPosition;

            deathCam.transform.position = new Vector3(player.position.x, deathCamOffset.y, player.position.z);
        }
        else
        {
            // moves the main camera staticly with the player's position and the inputted offset
            mainCam.transform.position = player.position + mainCamOffset;
        }
    }

    public void switchToDeathCam(bool doSwitch)
    {
        if (doSwitch)
        {
            mainCamera.enabled = false;
            deathCamera.enabled = true;
            moveDeathCam = true;
        }
    }

    private void engageDeathCam()
    {
        if (deathCam.transform.position.y <= stopZoomPosY)
        {
            // this handles the gradual slowing down of the zoom
            if (startZoomRate > 0)
            {
                startZoomRate -= gradualZoomRateDecrease;
            }

            // this is for future reference, idk what to do about it rn
            //Vector3 desiredDeathCamPos = new Vector3(player.position.x, deathCamOffset.y, player.position.z);
            //Vector3 smoothedDeathCamPos = Vector3.Lerp(deathCam.transform.position, desiredDeathCamPos, 100f * Time.deltaTime);
            // this gradually changes the starting deathcam height, based on the zoomrate
            //deathCamOffset.y += startZoomRate * Time.deltaTime;

            Vector3 tempPos = deathCam.transform.position;
            deathCamOffset.y += startZoomRate * Time.deltaTime;

            // moves the deathcam to the smoothed position
            deathCam.transform.position = tempPos;
        }
    }

    private IEnumerator changeToSmoothCam()
    {
        yield return new WaitForSeconds(staticCamTimeLimit);
        staticCam = false;
        switchedToDynamicCam = true;
    }
}
