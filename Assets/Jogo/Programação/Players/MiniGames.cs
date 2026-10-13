using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class MiniGames : NetworkBehaviour
{
    private VisualElement TelaLights;

    private Button OpenMiniGame;
    private Button ExitMiniGame;
    private Button LightsBtn;

    public GameObject LightsFather;
    public NetworkVariable<bool> LightChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        TelaLights = root.Q<VisualElement>("LightsMiniGame");
        OpenMiniGame = root.Q<Button>("InteractBtn");
        ExitMiniGame = root.Q<Button>("ExitBtn");
        LightsBtn = root.Q<Button>("LightsBtn");

        TelaLights.visible = false;

        OpenMiniGame.RegisterCallback<ClickEvent>(OpenMiniGames);
        ExitMiniGame.RegisterCallback<ClickEvent>(CloseMiniGames);
        

        LightsFather.SetActive(true);
        LightsBtn.RegisterCallback<ClickEvent>(TurnLightsOff);
    }

    public override void OnNetworkSpawn()
    {
        
        LightChecker.OnValueChanged += OnLightChange;
    }

    public override void OnNetworkDespawn()
    {
        LightChecker.OnValueChanged -= OnLightChange;
    }

    void OpenMiniGames(ClickEvent clickEvent)
    {
        TelaLights.visible = true;
    }

    void CloseMiniGames(ClickEvent clickEvent)
    {
        TelaLights.visible = false;
    }
    void TurnLightsOff(ClickEvent clickEvent)
    {
        TurnLightsOffRpc();
    }

    private void OnLightChange(bool previousValue, bool newValue)
    {
        LightsFather.SetActive(false);
    }

    [Rpc(SendTo.Server)]
    void TurnLightsOffRpc()
    {
        LightChecker.Value = true;
    }
}
