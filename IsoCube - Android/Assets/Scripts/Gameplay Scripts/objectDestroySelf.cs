using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectDestroySelf : MonoBehaviour
{
    public float selfDestroyY;

    // Update is called once per frame
    public void Update()
    {
        if (gameObject.transform.position.y < selfDestroyY)
        {
            // sets itself to inactive (since we're using object pooling) if it falls below a certain y value
            gameObject.SetActive(false);
        }
    }
}
