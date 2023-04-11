using JetBrains.Annotations;
using System.Numerics;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Gun : NetworkBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletLocal;
    [SerializeField] GameObject head;
    [SerializeField] GameObject player;

    GameObject target;
    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Instantiate(bullet, head.transform.position + (1.2f * head.transform.forward), head.transform.rotation);
            player.GetComponent<Rigidbody>().AddForce((transform.forward * -1) * 500);
            ShootServerRpc((head.transform.position + (1.2f * head.transform.forward)) , head.transform.rotation);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(UnityEngine.Vector3 spawnPos, UnityEngine.Quaternion rot) //i had to do this unity engine bullshit to clarify which one i was talking about. lil unity got confused uwu
    {
        GameObject shot = Instantiate(bullet, spawnPos + (transform.forward * 2), rot);

        ServerRpcParams serverRpcParams = default;
        var clientId = serverRpcParams.Receive.SenderClientId;

        shot.GetComponent<NetworkObject>().Spawn();
        
        
    }

    [ClientRpc] 

    private void ShootClientRPC()
    {
        if(!IsOwner)
        {
            Debug.Log("jizz");
        }

        Debug.Log("cum");
    }
}
