using Unity.Netcode;
using UnityEngine;

public class Viewpoint : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        Debug.Log("fat");
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            Debug.Log("poopy");
            gameObject.GetComponent<Camera>().enabled = false;
        }

    }
}
