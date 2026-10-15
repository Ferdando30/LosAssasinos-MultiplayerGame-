using UnityEngine;
using Unity.Netcode;

public class MinigameButton : NetworkBehaviour
{
    [SerializeField] private GameObject basic_ui;
    [SerializeField] private GameObject minigame;

    public GameObject LightsFather;
    public NetworkVariable<bool> LightChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Awake()
    {
        Debug.Log("Awake");
    }

    //--------------------------------------------- (Network Spawn)

    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawn");
        LightChecker.OnValueChanged += OnLightChange;
    }

    public override void OnNetworkDespawn()
    {
        LightChecker.OnValueChanged -= OnLightChange;
    }

    //--------------------------------------------- (funções de UI)

    public void StartMinigame()
    {
        minigame.SetActive(true);
        basic_ui.SetActive(false);
    }

    public void CloseMinigame()
    {
        basic_ui.SetActive(true); 
        minigame.SetActive(false);
    }

    public void TurnLightsOff()
    {
        TurnLightsOffRpc();
    }

    //--------------------------------------------- (funções de netcoding)

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
