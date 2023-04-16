using Unity.Netcode;
using UnityEngine;

public class Viewpoint : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            gameObject.GetComponent<Camera>().enabled = false;
        }

    }
}
