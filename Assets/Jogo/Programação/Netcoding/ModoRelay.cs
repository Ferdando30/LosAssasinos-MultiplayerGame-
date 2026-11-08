using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Core;
using UnityEngine.UIElements;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class ModoRelay : MonoBehaviour
{
    private Button HostBtn;
    private Button ClientBtn;
    private Button CriarBtn;
    private Button EntrarBtn;

    private VisualElement telaServers;
    private VisualElement telaCriar;
    private VisualElement telaEntrar;
    private VisualElement LobbyScreen;
    private VisualElement StartScreen;

    private Label CodeTxt;

    async void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        HostBtn = root.Q<Button>("HostBtn");
        ClientBtn = root.Q<Button>("ClientBtn");
        CriarBtn = root.Q<Button>("CriarBtn");
        EntrarBtn = root.Q<Button>("EntrarBtn");

        telaServers = root.Q<VisualElement>("TelaInicial");
        telaCriar = root.Q<VisualElement>("TelaHostServer");
        telaEntrar = root.Q<VisualElement>("TelaEnterServer");
        LobbyScreen = root.Q<VisualElement>("LobbyScreen");
        StartScreen = root.Q<VisualElement>("StartScreen");
        CodeTxt = root.Q<Label>("CodeTxt");

        telaServers.visible = true;
        telaCriar.visible = false;
        telaEntrar.visible = false;

        CriarBtn.RegisterCallback<ClickEvent>(CriarSalaTela);
        EntrarBtn.RegisterCallback<ClickEvent>(EntrarSalaTela);

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    async void CreatRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        CodeTxt.text = "Code: " + joinCode;

        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();
    }

    async void JoinRelay(string joinCode)
    {
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();
    }

    private void CriarSalaTela(ClickEvent clickEvent)
    {
        telaServers.visible = false;
        telaCriar.visible = true;
        telaEntrar.visible = false;
    }

    private void EntrarSalaTela(ClickEvent clickEvent)
    {
        telaServers.visible = false;
        telaCriar.visible = false;
        telaEntrar.visible = true;
    }
}
