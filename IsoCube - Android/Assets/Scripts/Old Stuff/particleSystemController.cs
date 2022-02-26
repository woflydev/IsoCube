using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleSystemController : MonoBehaviour
{
    public ParticleSystem playerDust;

    public void createDust()
    {
        playerDust.Play();
    }

    public void stopDust()
    {
        playerDust.Stop();
    }
}
