using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] float bulletPower;
    [SerializeField] float bulletLifeTime;
    [SerializeField] float chargeUpTime;
    private float spawnTime;
    void Start() //though the client might have instantiated the bullet, the server always owns it. this code is run on the hosts machine 
    {
        Debug.Log(IsOwner);
        Debug.Log("bullet owner is " + OwnerClientId);
        if (!IsOwner)
        {
            return;
        }
        StartCoroutine(ChargeUpTime());
        StartCoroutine(TimeToExplode());

    }

    public void SetSpawnTime(float spawnTime)
    {
        this.spawnTime = spawnTime;
    }


    private IEnumerator ChargeUpTime()
    {
        yield return new WaitForSeconds(chargeUpTime); //this is to make people with bad pings experience more consistent 

        gameObject.GetComponent<MeshRenderer>().enabled = true; //enables the stuff to make it work
        gameObject.GetComponent<SphereCollider>().enabled = true;
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 1800); //shoots bullet forward

    }
    private IEnumerator TimeToExplode()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        DestroyGameObjectServerRPC();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if(!(other.gameObject.tag == "Player" && other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer))
        {
            Debug.Log("fartsyas");
            ExplosionSpawnServerRPC(gameObject.transform.position);
        }

    }

    [ServerRpc]
    private void ExplosionSpawnServerRPC(Vector3 spawnPos) 
    {
        GameObject boom = Instantiate(explosion, spawnPos, Quaternion.identity);

        boom.GetComponent<NetworkObject>().Spawn();

        //destroys bullet after explosion is spawned
        DestroyGameObjectServerRPC();
    }

    [ServerRpc]
    private void DestroyGameObjectServerRPC()
    {
        Destroy(gameObject);
    }
}