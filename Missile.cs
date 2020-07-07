using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviourPun
{
    [Tooltip("The speed of the Missile")]
    public float speed = 30f;
    [Tooltip("The amount of damage that this missile causes if hits/touches a car.")]
    public float damage = 10f;
    [Tooltip("After this amount of time the missile destroyed and removed from the scene")]
    public float destroyTime = 1.5f;

    public GameObject TrailFireFX;
    public GameObject ExplosionFX;

    private GameObject TrailFire;
    private GameObject createdExploison;

    // Player's photonviewID who spawned this missile.
    [HideInInspector]
    public int ownerPhotonViewID;


    private void Start()
    {
        TrailFire = Instantiate(TrailFireFX, transform.position, transform.localRotation);
        Destroy(TrailFire, destroyTime);
        StartCoroutine("DestroyMissileCoroutine");
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        if (TrailFire)
        {
            TrailFire.transform.position = this.transform.localPosition - (transform.forward * 1f);
            TrailFire.transform.localRotation = this.transform.localRotation * Quaternion.Euler(new Vector3(0f, 90f, 0f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        createdExploison = Instantiate(ExplosionFX, transform.position, transform.rotation);
        Destroy(createdExploison, 1f);
        StartCoroutine("DelayedDestroy");

        // MasterClient validation, only cause damage if masterclient sees it
        if (!PhotonNetwork.IsMasterClient)
            return;

        // here we damage the car wih the missiles damage, trough MasterClient validation
        if (other.gameObject.tag == "Car")
        {
            GameObject collidedGO = other.gameObject;
            PlayerCar collidedCar = collidedGO.GetComponent<PlayerCar>();

            if (collidedCar != null)
            {
                Debug.Log("MissileDMG RPC called with this view: " + ownerPhotonViewID);
                other.GetComponent<PhotonView>().RPC("TakeDamageMissile", RpcTarget.AllViaServer, damage, ownerPhotonViewID);
            }
        }
    }

    [PunRPC]
    public void SetOwnerRPC(int ownerPhotonView)
    {
        ownerPhotonViewID = ownerPhotonView;
    }

    IEnumerator DestroyMissileCoroutine()
    {
        yield return new WaitForSeconds(destroyTime);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // This method need to avoid not apearing rockets on other clients
    IEnumerator DelayedDestroy()
    {
        // to other players see it too
        yield return new WaitForSeconds(0.05f);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
