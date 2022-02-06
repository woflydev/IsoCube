using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadNextLevel : MonoBehaviour
{
    private gameManager gameManagerScript;

    private void Start()
    {
        gameManagerScript = FindObjectOfType<gameManager>();
    }

    public void loadLevel()
    {
        gameManagerScript.loadNext();
    }
}
