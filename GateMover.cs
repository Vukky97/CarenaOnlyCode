using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMover : MonoBehaviour
{
    float speed = 3f;
    float delta = 20f;  //delta is the difference between min y to max y.
    private float initialZ;

    private void Start()
    {
        initialZ = transform.position.z - delta / 2;
    }

    void FixedUpdate()
    {
        float z = Mathf.PingPong(speed * Time.time, delta);
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, z + initialZ);
        transform.position = pos;
    }
}
