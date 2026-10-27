using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MinigameButton : NetworkBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float requiredHoldTime = 2.0f;
    private float currentHoldTime = 0f;
    private bool isHolding = false;

    [Header("Colliders")]
    public string currentZone = "Nenhuma";
    public Collider2D lightZone;
    public Collider2D poisonZone;
    public Collider2D closetZone;
    private Transform playerLocal;
    

    [Header("Tasks")]
    public GameObject LightsFather;
    public SpriteRenderer Drink;

    [Header("UI")]
    public UnityEngine.UI.Slider timeSlider;
    public UnityEngine.UI.Image buttonImage;

    private Animator animator;

    
    public NetworkVariable<bool> LightChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> PoisonChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Sprites")]
    public Sprite NormalDrink;
    public Sprite PoisonDrink;

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
                animator = player.GetComponent<Animator>();
                break;
            }
        }
    }
    private void Zonas()
    {
        EncontrarPlayerLocal();
        if (playerLocal == null || lightZone == null || poisonZone == null || closetZone == null)
            return;

        bool DentroLuz = lightZone.bounds.Contains(playerLocal.position);
        bool DentroPoison = poisonZone.bounds.Contains(playerLocal.position);
        bool DentroArmario = closetZone.bounds.Contains(playerLocal.position);

        if (DentroLuz && !DentroPoison)
        {
            buttonImage.enabled = true;
            timeSlider.gameObject.SetActive(true);
            currentZone = "LuzMiniGame";
        }

        else if(!DentroLuz && DentroPoison)
        {
            buttonImage.enabled = true;
            timeSlider.gameObject.SetActive(true);
            currentZone = "PoisonMiniGame";
        }

        else if (DentroArmario && !DentroLuz && !DentroPoison)
        {
            buttonImage.enabled = true;
            timeSlider.gameObject.SetActive(true);
            currentZone = "ArmarioMiniGame";
        }

        else if (!DentroLuz && !DentroPoison)
        {
            buttonImage.enabled = false;
            timeSlider.gameObject.SetActive(false);
            currentZone = "Nenhuma";
        }
    }

    //--------------------------------------------- (Update)

    private void Update()
    {
        Zonas();
        if (!isHolding || currentZone == "Nenhuma")
            return;
        

        currentHoldTime += Time.deltaTime;
        timeSlider.value = Mathf.Lerp(1f, 0f, currentHoldTime / requiredHoldTime);
        animator.SetBool("Interacting", true);

        if (currentHoldTime >= requiredHoldTime)
        {
            isHolding = false;
            animator.SetBool("Interacting", false);
            currentHoldTime = 0f;
            timeSlider.value = 1f;

            if(currentZone == "LuzMiniGame")
            {
                TurnLightsOffRpc();
            }

            else if(currentZone == "PoisonMiniGame")
            {
                PoisonDrinkRpc();
            }
            else if (currentZone == "ArmarioMiniGame")
            {
                return;
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
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
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
        Drink.sprite = PoisonDrink;
    }

    [Rpc(SendTo.Server)]
    void TurnLightsOffRpc()
    {
        LightChecker.Value = true;
    }

    [Rpc(SendTo.Server)]
    void PoisonDrinkRpc()
    {
        PoisonChecker.Value = true;
    }
}
