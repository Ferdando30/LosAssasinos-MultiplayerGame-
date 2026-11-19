using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Core;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class ModoRelay : MonoBehaviour
{
    public GameObject InicioHolder;
    public GameObject HostScreen;
    public GameObject EntrarScreen;
    public GameObject LobbyScreen;
    public GameObject StartScreen;
    public GameObject JoyStick;
    public GameObject TitleScreen;

    public GameObject DiscoStart;
    public GameObject DiscoOptions;

    public TextMeshProUGUI CodeTxt;
    public TMP_InputField joinInput;

    public Button Hostbtn;
    public Button JoinBtn;


    async void Start()
    {
        InicioHolder.SetActive(true);
        HostScreen.SetActive(false);
        EntrarScreen.SetActive(false);
        LobbyScreen.SetActive(false);
        StartScreen.SetActive(true);
        DiscoStart.SetActive(true);
        DiscoOptions.SetActive(false);
        TitleScreen.SetActive(true);
        JoyStick.SetActive(false);

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Hostbtn.onClick.AddListener(CreatRelay);
        JoinBtn.onClick.AddListener(() => JoinRelay(joinInput.text));
    }

    async void CreatRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        CodeTxt.text = "Code: " + joinCode;

        LobbyScreen.SetActive(true);
        JoyStick.SetActive(true);
        StartScreen.SetActive(false);

        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();
    }

    async void JoinRelay(string joinCode)
    {
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

        StartScreen.SetActive(false);
        JoyStick.SetActive(true);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();
    }

    public void CriarSalaTela()
    {
        InicioHolder.SetActive(false);
        HostScreen.SetActive(true);
        EntrarScreen.SetActive(false);
        LobbyScreen.SetActive(false);
    }

    public void EntrarSalaTela()
    {
        InicioHolder.SetActive(false);
        HostScreen.SetActive(false);
        EntrarScreen.SetActive(true);
        LobbyScreen.SetActive(false);
    }

    public void TitleScreenFunction()
    {
        TitleScreen.SetActive(false);
    }

    public void Options()
    {
        DiscoStart.SetActive(false);
        DiscoOptions.SetActive(true);
    }

    public void Back()
    {
        DiscoStart.SetActive(true);
        DiscoOptions.SetActive(false);
        HostScreen.SetActive(false);
        EntrarScreen.SetActive(false);
        InicioHolder.SetActive(true);
    }
}
