using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectPooling : MonoBehaviour
{
    public static objectPooling sharedInstance;

    private generateLevel genLevelScript;

    public List<GameObject> obstaclePool = new List<GameObject>();
    public GameObject obstacleToPool;
    public int amountOfObstaclesToPool;

    public Transform parentObj;

    private void Awake()
    {
        genLevelScript = FindObjectOfType<generateLevel>();

        sharedInstance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        GameObject tmp;

        for (int i = 0; i < amountOfObstaclesToPool; i++)
        {
            tmp = Instantiate(obstacleToPool);
            tmp.SetActive(false);

            obstaclePool.Add(tmp);

            tmp.transform.SetParent(parentObj);
        }
    }

    // simple instructions to get the obstacles to deactivate themselves when player passes them
    private void FixedUpdate()
    {
        foreach (GameObject obj in obstaclePool)
        {
            Transform childObj = obj.transform.GetChild(0);

            // since only the child of the obstacle container is falling, we have to check THAT instead.
            if (childObj.transform.position.z < genLevelScript.player.position.z - 20f)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    public GameObject getPooledObject()
    {
        for (int i = 0; i < amountOfObstaclesToPool; i++)
        {
            // this checks if any of the obsacles in the heirarchy are available
            if (!obstaclePool[i].activeInHierarchy)
            {
                return obstaclePool[i];
            }
        }

        return null;
    }
}
