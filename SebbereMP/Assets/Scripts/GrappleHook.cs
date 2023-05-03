using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleHook : NetworkBehaviour
{
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float maxGrappleSpeed;
    [SerializeField] private float hookShotSpeed;
    [SerializeField] private float aceleration;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject shootPos;
    [SerializeField] private GameObject grappler;
    [SerializeField] private GameObject hook;
    private GameObject hooky;
    private LineRenderer rope;
    private Rigidbody playerRB;
    private LayerMask layerMask;
    private bool pulling;
    private bool hookyState;
    private Vector3 targetPos;
    private Hook hookScript;

    void Start()
    {
        rope = gameObject.GetComponent<LineRenderer>();
        layerMask = LayerMask.GetMask("Ground");
        playerRB = player.GetComponent<Rigidbody>();
    }

    public void ShootHook(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (pulling)
        {
            Debug.Log("disconnect call (manual)");
            DisconnectGrapple();
        }
        else
        {
            RaycastHit hit;
            if(Physics.Raycast(shootPos.transform.position, shootPos.transform.forward, out hit, maxGrappleDistance/*, layerMask*/)) {
                if (!hookyState)
                {
                    GrappleShotServerRPC(); //spawns hook
                    hookyState = true;
                }
            }
            


        }

    }

    private void Pull()
    {

        player.GetComponent<Player>().SetStopMovement(true);
        rope.SetPosition(0, grappler.transform.position);
        rope.SetPosition(1, hooky.transform.position);

        //playerRB.useGravity = false;
        playerRB.drag = 0;
        pulling = true;
        Debug.Log("pull");

    }

    private void FixedUpdate() //does the pulling 
    {
        if (pulling)
        {
            if(player.GetComponent<Rigidbody>().velocity.magnitude < maxGrappleSpeed)
            {
               playerRB.velocity += (hooky.transform.position - player.transform.position).normalized * aceleration; //constantly sets the velocity to go towards the target
            }
             playerRB.velocity -= playerRB.velocity.normalized / (aceleration/2);

            rope.SetPosition(0, grappler.transform.position); //updates the grapples origin point as the player moves forward
            if ((rope.GetPosition(0) - rope.GetPosition(1)).magnitude < new Vector3(1.5f, 1.5f, 1.5f).magnitude) //if the grapples origin point gets close enough to its end point, disconnect grapple
            {
                Debug.Log("disconnect call");
                DisconnectGrapple();
            }
        }

    }

    private void Update() //updates script function based on current hook status
    {
        if (hookyState && !pulling)
        {
            rope.SetPosition(0, grappler.transform.position);
            rope.SetPosition(1, hooky.transform.position);
            if ((hooky.transform.position - grappler.transform.position).magnitude > maxGrappleDistance) //test case in case wacky shit happens and hook doesnt hit anything 
            {
                DisconnectGrapple(); //without it hook potentially could go forever (that is bad news bears)
            }
            if (hookScript.GetCollisionStatus())
            {
                Debug.Log("pull call");
                Pull();
            }
        }
    }

    private void DisconnectGrapple()
    {
        DestroyHookServerRPC();
        hookyState = false;
        player.GetComponent<Player>().SetStopMovement(false);
        playerRB.useGravity = true;
        rope.SetPosition(0, rope.GetPosition(1));
        pulling = false;
        Debug.Log("disconnected");
    }

    [ServerRpc]
    private void GrappleShotServerRPC()
    {
        hooky = Instantiate(hook, grappler.transform.position, gameObject.transform.rotation);

        hooky.GetComponent<NetworkObject>().Spawn();

        hooky.GetComponent<Rigidbody>().velocity = shootPos.transform.forward * hookShotSpeed;

        hookScript = hooky.GetComponent<Hook>();
    }

    [ServerRpc]
    private void DestroyHookServerRPC()
    {
        Destroy(hooky);
    }
}
