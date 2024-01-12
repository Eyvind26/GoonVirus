using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goo : MonoBehaviour
{
    public AudioSource startSource;
    public AudioSource gooSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            startSource.Play();
            gooSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            gooSource.Stop();
        }
    }
}
