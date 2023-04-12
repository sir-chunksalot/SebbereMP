using JetBrains.Annotations;
using System.Numerics;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking.Types;
using UnityEngine.UIElements;

public class Gun : NetworkBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject head;
    [SerializeField] GameObject player;

    GameObject target;
    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.GetComponent<Rigidbody>().AddForce((transform.forward * -1) * 500); //pushes player back
            float spawnTime = Time.fixedTime;
            ShootServerRpc(head.transform.position + (1.2f * head.transform.forward) , head.transform.rotation, spawnTime);
            Debug.Log("gun owner client id is: " + OwnerClientId);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(UnityEngine.Vector3 spawnPos, UnityEngine.Quaternion rot, float spawnTime) //i had to do this unity engine bullshit to clarify which one i was talking about. lil unity got confused uwu
    {
        GameObject shot = Instantiate(bullet, spawnPos, rot);

        ServerRpcParams serverRpcParams = default;
        var clientId = serverRpcParams.Receive.SenderClientId;

 

        

        shot.GetComponent<NetworkObject>().Spawn();
        shot.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
    }

}
