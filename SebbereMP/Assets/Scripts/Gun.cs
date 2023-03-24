using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : NetworkBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject head;
    void Start()
    {

    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TestServerRpc();
        }

    }

    [ServerRpc]
    private void TestServerRpc()
    {
        GameObject spawnedBullet = Instantiate(bullet, head.transform.position, head.transform.rotation);
        spawnedBullet.GetComponent<NetworkObject>().Spawn();
    }
}
