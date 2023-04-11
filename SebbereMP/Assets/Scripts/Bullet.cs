using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] float bulletPower;
    [SerializeField] float bulletLifeTime;
    [SerializeField] float chargeUpTime;
    Rigidbody rb;
    void Start()
    {
        if (!IsOwner) return;
        StartCoroutine(ChargeUpTime());
        StartCoroutine(TimeToExplode());

    }


    private IEnumerator ChargeUpTime()
    {
        yield return new WaitForSeconds(chargeUpTime);
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 1800);
    }
    private IEnumerator TimeToExplode()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(gameObject);
    }
}