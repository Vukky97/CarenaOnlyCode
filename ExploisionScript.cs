using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploisionScript : MonoBehaviourPun
{
    [Tooltip("After this amount of time the explosion destroyed and removed from the scene/netwrok")]
    public float destroyTime = 1.5f;

    private void Start()
    {
        StartCoroutine(DestroyExploisionCoroutine(destroyTime));
    }

    IEnumerator DestroyExploisionCoroutine(float afterTime)
    {
        yield return new WaitForSeconds(afterTime);
        Destroy(this.gameObject);
    }

}
