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
        //LightsBtn.RegisterCallback<ClickEvent>(TurnLightsOff);
    }

    public override void OnNetworkSpawn()
    {
        if(LightsBtn != null)
        {
            LightsBtn.RegisterCallback<ClickEvent>(TurnLightsOff);
        }
       
    }

    public override void OnNetworkDespawn()
    {
        if (LightsBtn != null)
        {
            LightsBtn.UnregisterCallback<ClickEvent>(TurnLightsOff);
        }
            
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
        if (!IsSpawned) return;

        TurnLightsOffRpc();
        Debug.Log("Opa");
    }

    [Rpc(SendTo.Server)]
    void TurnLightsOffRpc()
    {
        LightsFather.SetActive(false);
        Debug.Log("RPC executado no servidor com sucesso!");
    }
}
