using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectEventDetection : MonoBehaviour
{
    public float selfDestroyY;

    // Update is called once per frame
    private void Update()
    {
        if (gameObject.transform.position.y < selfDestroyY)
        {
            // destroys itself if it falls below a certain y value
            Destroy(gameObject);
        }
    }
}
