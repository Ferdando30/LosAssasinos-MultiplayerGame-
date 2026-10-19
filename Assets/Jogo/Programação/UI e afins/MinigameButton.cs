using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class MinigameButton : NetworkBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float requiredHoldTime = 2.0f;
    private float currentHoldTime = 0f;
    private bool isHolding = false;

    public string zonaAtual = "Nenhuma";
    public Collider2D zonaLuz;
    private Transform playerLocal;

    public GameObject LightsFather;
    public NetworkVariable<bool> LightChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //--------------------------------------------- (trigger)
    private void EncontrarPlayerLocal()
    {
        if (playerLocal != null)
            return;

        foreach (NetworkObject player in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (player.IsOwner && player.CompareTag("Player"))
            {
                playerLocal = player.transform; 
                break;
            }
        }
    }
    private void Zonas()
    {
        EncontrarPlayerLocal();
        if (playerLocal == null || zonaLuz == null)
            return;

        bool DentroLuz = zonaLuz.bounds.Contains(playerLocal.position);

        if (DentroLuz)
        {
            zonaAtual = "LuzMiniGame";
        }

        else if (!DentroLuz)
        {
            zonaAtual = "Nenhuma";
        }
    }

    //--------------------------------------------- (Update)

    private void Update()
    {
        Zonas();
        if (!isHolding || zonaAtual == "Nenhuma") return;

        currentHoldTime += Time.deltaTime;

        if (currentHoldTime >= requiredHoldTime)
        {
            isHolding = false;
            currentHoldTime = 0f;

            if(zonaAtual == "LuzMiniGame")
            {
                TurnLightsOffRpc();
            }
        }
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

    public void OnPointerDown(PointerEventData eventData)
    {
        //if (!IsOwner) return;
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //if (!IsOwner) return;
        isHolding = false;
        currentHoldTime = 0f;
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
