using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_InputField code;
    string joinCode;    
    private async void Awake()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        StartGame();
    }
    private async void StartGame()
    {
        QueryResponse queryResponse = await UpdateLobbyList();
        if (queryResponse.Results.Count >= 1) //if there is an active lobby, join it 
        {
            Debug.Log("client");

            Debug.Log(joinCode + "join code");
            Debug.Log(queryResponse.Results[0].Name + "lobby code");
            JoinRelay(queryResponse.Results[0].Name); //gets the lobby it found and uses its name to join the same relay the host did 
        }
        else //otherwise, make one 
        {
            Debug.Log("host");
            joinCode = await CreateRelay(); //creates relay and sets relay join code to this string 
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(joinCode, 8); //makes relay join code the name of the lobby it just created

            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            Debug.Log(joinCode + "join code");
        }
        //queryResponse = await UpdateLobbyList();
        //Debug.Log(queryResponse.Results.Count + "querycount");
        //if (queryResponse.Results.Count > 1) //sometimes two players create lobbies at the saame time, this hopefully fixes that by making the second join the first
        //{

        //    Debug.Log("fart");
        //    JoinRelay(queryResponse.Results[0].Name);
        //}
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds) //this is so the lobby doesnt go inactive (which is a cringe feature)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

        //private void Start()
        //{
        //    hostButton.onClick.AddListener(async () =>
        //    {
        //        Debug.Log("host");
        //        joinCode = await CreateRelay();
        //        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(joinCode, 8);

        //        Debug.Log(joinCode + "join code");
        //        //Hide();
        //    });
        //    clientButton.onClick.AddListener(async () =>
        //    {
        //        Debug.Log("client");
        //        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

        //        //await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
        //        Debug.Log(joinCode + "join code");
        //        Debug.Log(queryResponse.Results[0].Name + "lobby code");
        //        JoinRelay(queryResponse.Results[0].Name);
        //        //Hide();
        //    });
        //}

        //private void Hide()
        //{
        //    gameObject.SetActive(false);
        //}

        private async Task<QueryResponse> UpdateLobbyList()
    {
        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

        return queryResponse;

    }

    private async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(10);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }

    }


    private async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with" + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            StartGame();
        }

    }

    private IEnumerator LobbyLateJoin()
    {
        Debug.Log("balls");
        yield return new WaitForSeconds(2);
    }
}
