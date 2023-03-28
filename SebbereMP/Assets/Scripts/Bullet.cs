using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] float bulletPower;
    [SerializeField] float bulletLifeTime;
    Rigidbody rb;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * bulletPower);
        StartCoroutine(BulletLifeTime());

    }

    private IEnumerator BulletLifeTime()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(gameObject);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player" || other.gameObject.tag == "Wall")
    //    {
    //        if(gameObject != null)
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //}
}
