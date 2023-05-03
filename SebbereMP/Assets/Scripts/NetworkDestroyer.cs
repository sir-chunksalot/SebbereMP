using Unity.Netcode;

public class NetworkDestroyer : NetworkBehaviour
{
    //// Called by the Player
    //[ClientRpc]
    //public void ServerDestroyClientRpc(GameObject obj)
    //{
    //    DestroyObjectServerRpc(obj);
    //}

    //// Executed only on the server
    //[ServerRpc]
    //private void DestroyObjectServerRpc(GameObject obj)
    //{
    //    // It is very unlikely but due to the network delay
    //    // possisble that the other player also tries to
    //    // destroy exactly the same object beofre the server
    //    // can tell him that this object was already destroyed.
    //    // So in that case just do nothing.
    //    if (!obj) return;
    //    Debug.Log("shit");
    //    obj.GetComponent<NetworkObject>().Despawn();
    //}
}