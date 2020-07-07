using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public PlayerCar playerCar;

    // prevent to stuck on top of obstacles, walls, other cars
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Obstacle")
        {
            playerCar.Boost(0.5f, true);
        }
        if (collider.gameObject.tag == "Car")
        {
            playerCar.Boost(0.5f, true);
        }
        if (collider.gameObject.tag == "Wall")
        {
            playerCar.Boost(0.5f, true);
        }
    }
}
