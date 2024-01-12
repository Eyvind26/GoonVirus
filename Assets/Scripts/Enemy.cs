using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Player player;
    private bool catched = false;
    private Vector3 velocity = Vector3.zero;
    private float acceleration = 0;

    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        if (!catched) transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position, Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            catched = true;

            // Player ded
            player.GetComponent<Rigidbody>().isKinematic = true;

            // Camera locked on enemy
            Camera.main.GetComponent<FPSView>().enabled = false;
            Vector3 lookDir = transform.position - Camera.main.transform.position;
            Quaternion q = Quaternion.LookRotation(lookDir);
            Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, q, Time.deltaTime * 300);

            // Enemy do thingies
            StartCoroutine(EnemyKill());
        }
    }

    IEnumerator EnemyKill()
    {
        acceleration += Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, Camera.main.transform.position, ref velocity, 1f-acceleration);
        yield return new WaitForSeconds(1f);
        player.goon += 10;
    }
}
