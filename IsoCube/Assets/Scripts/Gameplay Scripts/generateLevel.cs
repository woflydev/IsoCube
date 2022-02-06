using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateLevel : MonoBehaviour
{
    public string spawnableEnvironmentTagName;
    public string spawnableObstacleTagName;
    private GameObject[] sections;

    public Transform player;
    public GameObject referenceObject;
    private GameObject runtimeReferenceObject;
    private float runtimeObjectLength;

    public float minViewDistance;
    public float unloadChunkDistance;
    //public float deleteObstaclesDistanceFromPlayer;

    private float difficultyMultiplierFromMovementScript;
    private float additiveDifficultyFromMovementScript;

    private bool creatingSection = false;

    private Dictionary<float, GameObject> groundSectionsVisible = new Dictionary<float, GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        // finds all objects with tags
        sections = GameObject.FindGameObjectsWithTag(spawnableEnvironmentTagName);

        // finds variables from stuff
        difficultyMultiplierFromMovementScript = FindObjectOfType<playerMovements>().difficultyMultiplier;
        additiveDifficultyFromMovementScript = FindObjectOfType<playerMovements>().difficultyAdditiveOverTime;

        // sets reference object
        runtimeReferenceObject = referenceObject;
    }

    // Update is called once per frame
    private void Update()
    {
        // assigns reference object length
        runtimeObjectLength = runtimeReferenceObject.transform.localScale.z;

        // every time this function is called, the runtimeReferenceObject gets updated to the most recently created one.
        Vector3 pos = runtimeReferenceObject.transform.position;
        // gets the amount of 'z' that the ground object has, then offsets that on  the instantiate position Vector3.
        pos.z += runtimeObjectLength;

        // if the player is further away than the minimum viewing distance, then a new section should be created
        if (player.position.z > pos.z - minViewDistance)
        {
            if (!creatingSection)
            {
                creatingSection = true;
                updateChunks(pos);
            }
        }
    }

    public void updateChunks(Vector3 pos)
    {
        GameObject spawnObject = sections[Random.Range(0, sections.Length)];

        // starts coroutine to place the ground, as well as passingin some parameters, then checking if the sections are still able to be seen by the player
        StartCoroutine(placeLevelGround(spawnObject, pos));
        difficultyMultiplierFromMovementScript += additiveDifficultyFromMovementScript;
    }

    private IEnumerator placeLevelGround(GameObject objectToSpawn, Vector3 pos)
    {
        // spawns the ground object
        GameObject newInstantiatedObject = Instantiate(objectToSpawn, pos, Quaternion.identity);
        // parents the instantiated object to the parent object of ObjectPooling, to clean up the editor hierarchy.
        newInstantiatedObject.transform.SetParent(objectPooling.sharedInstance.parentObj);

        // updates the runtimeReferenceObject for the next time this function is called, to keep up with where the objects are currently at.
        runtimeReferenceObject = newInstantiatedObject;

        if (!groundSectionsVisible.ContainsKey(pos.z))
        {
            // this adds our 'chunk' to the ground sections dictionary
            groundSectionsVisible.Add(pos.z, newInstantiatedObject);
        }

        checkIfShouldUnload();

        spawnObstacles(newInstantiatedObject);

        creatingSection = false;

        yield return null;
    }

    private void checkIfShouldUnload()
    {
        foreach (KeyValuePair<float, GameObject> dictionaryKeys in groundSectionsVisible)
        {
            if (dictionaryKeys.Key < player.position.z - unloadChunkDistance)
            {
                Destroy(groundSectionsVisible[dictionaryKeys.Key]);
            }
        }
    }

    private void spawnObstacles(GameObject parent)
    {
        // for the spherecast later
        bool hitSomething = true;
        Vector3 spawnPos;

        float iterationCount = Mathf.RoundToInt(Random.Range(3 * difficultyMultiplierFromMovementScript, 4 * difficultyMultiplierFromMovementScript));
        for (int i = 0; i < iterationCount; i++)
        {
            Bounds colliderBounds = runtimeReferenceObject.GetComponent<Collider>().bounds;
            Vector3 colliderCenter = colliderBounds.center;

            float spawnOffsetX = Random.Range(colliderCenter.x - colliderBounds.extents.x, colliderCenter.x + colliderBounds.extents.x);
            float spawnOffsetZ = Random.Range(colliderCenter.z - colliderBounds.extents.z, colliderCenter.z + colliderBounds.extents.z);

            spawnPos = new Vector3(spawnOffsetX - 1, 5, spawnOffsetZ - 1);

            hitSomething = Physics.SphereCast(spawnPos, 8, Vector3.down, out _);

            if (!hitSomething)
            {
                GameObject spawnedObject = objectPooling.sharedInstance.getPooledObject();

                if (spawnedObject != null)
                {
                    spawnedObject.transform.position = spawnPos;
                    spawnedObject.transform.rotation = Quaternion.identity;
                    spawnedObject.SetActive(true);

                    // sets the spawned object's parent to the corresponding ground object that it sits on
                    //spawnedObject.transform.SetParent(parent.transform);
                }

                hitSomething = true;
            }
        }
    }
}
