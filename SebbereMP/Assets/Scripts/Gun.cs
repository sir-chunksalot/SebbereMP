using JetBrains.Annotations;
using System.Numerics;
using System.Security.Cryptography;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;

public class Gun : NetworkBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject head;
    [SerializeField] GameObject player;
    [SerializeField] GameObject magazine;
    [SerializeField] int maxAmmo;

    private GameObject target;
    private TextMeshProUGUI text;
    private int ammo;

    private void Start()
    {
        ammo = maxAmmo;
        text = magazine.GetComponent<TextMeshProUGUI>();
        Debug.Log("this is sparts" + text.gameObject);
        SetUIAmmo(ammo);
    }
    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed && ammo > 0)
        {
            player.GetComponent<Rigidbody>().AddForce((transform.forward * -1) * 500); //pushes player back
            float spawnTime = Time.fixedTime;
            ShootServerRpc(head.transform.position + (1.2f * head.transform.forward) , head.transform.rotation, spawnTime);
            Debug.Log("gun owner client id is: " + OwnerClientId);

            ammo--;
            SetUIAmmo(ammo);
        }
    }

    private void SetUIAmmo(int num)
    {
        text.text = num.ToString();
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
