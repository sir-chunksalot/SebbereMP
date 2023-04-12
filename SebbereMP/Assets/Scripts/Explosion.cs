using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Explosion : NetworkBehaviour
{
    [SerializeField] float aliveTime;
    private void Start()
    {
        if (!IsOwner) return;
        StartCoroutine(Boom());
    }

    private void Update()
    {
        
    }

    private IEnumerator Boom()
    {
        yield return new WaitForSeconds(aliveTime);

        Destroy(gameObject);
    }
}
