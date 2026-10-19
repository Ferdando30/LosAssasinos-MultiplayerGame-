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
    public Collider2D zonaPoison;
    private Transform playerLocal;

    public GameObject LightsFather;
    public SpriteRenderer Drink;

    public NetworkVariable<bool> LightChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> PoisonChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    

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
        if (playerLocal == null || zonaLuz == null || zonaPoison == null)
            return;

        bool DentroLuz = zonaLuz.bounds.Contains(playerLocal.position);
        bool DentroPoison = zonaPoison.bounds.Contains(playerLocal.position);

        if (DentroLuz && !DentroPoison)
        {
            zonaAtual = "LuzMiniGame";
        }

        else if(!DentroLuz && DentroPoison)
        {
            zonaAtual = "PoisonMiniGame";
        }

        else if (!DentroLuz && !DentroPoison)
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

            else if(zonaAtual == "PoisonMiniGame")
            {
                PoisonDrinkRpc();
            }
        }
    }


    //--------------------------------------------- (Network Spawn)

    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawn");
        LightChecker.OnValueChanged += OnLightChange;
        PoisonChecker.OnValueChanged += OnPoisonChange;
    }

    public override void OnNetworkDespawn()
    {
        LightChecker.OnValueChanged -= OnLightChange;
        PoisonChecker.OnValueChanged -= OnPoisonChange;
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

    private void OnPoisonChange(bool previousValue, bool newValue)
    {
        Drink.color = Color.green;
    }

    [Rpc(SendTo.Server)]
    void TurnLightsOffRpc()
    {
        LightChecker.Value = true;
    }

    void PoisonDrinkRpc()
    {
        PoisonChecker.Value = true;
    }
}
