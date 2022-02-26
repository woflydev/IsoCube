using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapBack : MonoBehaviour
{
    public Transform playerPosition;
    private GameObject environmentController;
    private GameObject gameManager;
    public float boundaryLimit = 200f;

    private void Start()
    {
        environmentController = GameObject.FindGameObjectWithTag("Environment");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector3.Distance(Vector3.zero, playerPosition.position) > boundaryLimit)
        {
            environmentController.transform.SetParent(playerPosition.transform);
            playerPosition.position = new Vector3(0, 1, 0);
            environmentController.transform.SetParent(gameManager.transform);
        }
    }
}
