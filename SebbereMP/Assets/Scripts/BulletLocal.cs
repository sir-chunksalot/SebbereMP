using Unity.Netcode;
using UnityEngine;

public class BulletLocal : MonoBehaviour
{
    [SerializeField] float bulletPower;
    [SerializeField] float bulletLifeTime;
    [SerializeField] GameObject bullet;
    Rigidbody rb;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * bulletPower);

        //ShootServerRpc();
    }

    [ServerRpc]
    private void ShootServerRpc() //i had to do this unity engine bullshit to clarify which one i was talking about. lil unity got confused uwu
    {
        GameObject shot = Instantiate(bullet, gameObject.transform.position, gameObject.transform.rotation);

        ServerRpcParams serverRpcParams = default;
        var clientId = serverRpcParams.Receive.SenderClientId;

        shot.GetComponent<NetworkObject>().Spawn();

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
