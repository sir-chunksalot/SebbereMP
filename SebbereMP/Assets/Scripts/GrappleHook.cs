using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleHook : NetworkBehaviour
{
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject shootPos;
    private LineRenderer rope;
    private Rigidbody playerRB;
    private LayerMask layerMask;
    private bool pulling;
    private Vector3 targetPos;

    void Start()
    {
        rope = gameObject.GetComponent<LineRenderer>();
        layerMask = LayerMask.GetMask("Ground");
        playerRB = player.GetComponent<Rigidbody>();
    }

    public void ShootHook(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (pulling) {
            DisconnectGrapple();
        }
        else
        {
            RaycastHit hit;

            if (Physics.Raycast(shootPos.transform.position, shootPos.transform.forward, out hit, maxGrappleDistance/*, layerMask*/))
            {
                player.GetComponent<Player>().SetStopMovement(true);
                rope.SetPosition(0, shootPos.transform.position);
                rope.SetPosition(1, hit.point);
                Pull(hit.point);
            }
        }

    }

    private void Pull(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        playerRB.useGravity = false;
        playerRB.drag = 0;
        pulling = true;

    }

    private void Update()
    {
        if(pulling)
        {
            player.GetComponent<Rigidbody>().velocity = (targetPos - player.transform.position).normalized * 30; //constantly sets the velocity to go towards the target
            rope.SetPosition(0, shootPos.transform.position); //updates the grapples origin point as the player moves forward
            if ((rope.GetPosition(0) - rope.GetPosition(1)).magnitude < new Vector3(1, 1, 1).magnitude) //if the grapples origin point gets close enough to its end point, disconnect grapple
            {
                DisconnectGrapple();
            }
        }

    }

    private void DisconnectGrapple()
    {
        pulling = false;
        player.GetComponent<Player>().SetStopMovement(false);
        playerRB.useGravity = true;
        rope.SetPosition(0, rope.GetPosition(1));
    }
}
