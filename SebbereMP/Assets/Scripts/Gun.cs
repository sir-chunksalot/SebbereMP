using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;

public class Gun : NetworkBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject head;
    [SerializeField] GameObject player;
    [SerializeField] GameObject magazine;
    [SerializeField] int maxAmmo;
    [SerializeField] int reloadTime;

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
            player.GetComponent<Rigidbody>().AddForce((transform.right) * 500); //pushes player back
            float spawnTime = Time.fixedTime;
            ShootServerRpc(head.transform.position + (1.2f * head.transform.forward), head.transform.rotation);
            Debug.Log("gun owner client id is: " + OwnerClientId);

            ammo--;
            SetUIAmmo(ammo);
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        StartCoroutine(ReloadWait());
    }

    private void SetUIAmmo(int num)
    {
        text.text = num.ToString();
    }

    private IEnumerator ReloadWait()
    {
        yield return new WaitForSeconds(reloadTime);
        ammo = maxAmmo;
        SetUIAmmo(ammo);
    }

    [ServerRpc]
    private void ShootServerRpc(UnityEngine.Vector3 spawnPos, Quaternion rot) //i had to do this unity engine bullshit to clarify which one i was talking about. lil unity got confused uwu
    {
        GameObject shot = Instantiate(bullet, spawnPos, rot);

        ServerRpcParams serverRpcParams = default;
        var clientId = serverRpcParams.Receive.SenderClientId;

        shot.GetComponent<NetworkObject>().Spawn();
        shot.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
    }

}
