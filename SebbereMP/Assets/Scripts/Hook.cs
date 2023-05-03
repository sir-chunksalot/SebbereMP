using Unity.Netcode;
using UnityEngine;

public class Hook : NetworkBehaviour
{
    private bool collisionStatus = false;
    private void OnCollisionEnter(Collision collision)
    {
        collisionStatus = true;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public bool GetCollisionStatus()
    {
        return collisionStatus;
    }
}
