using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;


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
    public Collider2D phoneZone;
    public Collider2D alarmZone;
    public Collider2D stealKnifeZone;
    private Transform playerLocal;
    

    [Header("Tasks")]
    public GameObject LightsFather;
    public GameObject AlarmLight;
    public SpriteRenderer Lights;
    public SpriteRenderer Drink;
    public SpriteRenderer Phone;
    public SpriteRenderer Alarm;
    public SpriteRenderer stealKnife;

    [Header("UI")]
    public UnityEngine.UI.Slider timeSlider;
    public UnityEngine.UI.Image buttonImage;

    private Animator animator;
    private TopDownController playerController;

    
    public NetworkVariable<bool> LightChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> PoisonChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> PhoneChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> AlarmChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> StealKnifeChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Sprites")]
    public Sprite NormalDrink;
    public Sprite PoisonDrink;
    public Sprite NormalPhone;
    public Sprite BrokenPhone;
    public Sprite AlarmOn;
    public Sprite AlarmOff;
    public Sprite KnifemOn;
    public Sprite KnifeOff;
    public Sprite LightsOn;
    public Sprite LightsOff;

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
                playerController = player.GetComponent<TopDownController>();
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
        bool DentroPhone = phoneZone.bounds.Contains(playerLocal.position);
        bool DentroAlarm = alarmZone.bounds.Contains(playerLocal.position);
        bool DentroKnifeSteal = stealKnifeZone.bounds.Contains(playerLocal.position);

        if (DentroLuz && !DentroPoison && !DentroArmario)
        {
            buttonImage.enabled = true;
            timeSlider.gameObject.SetActive(true);
            currentZone = "LuzMiniGame";
        }

        else if(!DentroLuz && DentroPoison && !DentroArmario && !DentroPhone)
        {
            buttonImage.enabled = true;
            timeSlider.gameObject.SetActive(true);
            currentZone = "PoisonMiniGame";
        }

        else if (DentroArmario && !DentroLuz && !DentroPoison && !DentroPhone)
        {
            buttonImage.enabled = true;
            timeSlider.gameObject.SetActive(true);
            currentZone = "ArmarioMiniGame";
        }

        else if (DentroPhone && !DentroArmario && !DentroLuz && !DentroPoison)
        {
            buttonImage.enabled = true;
            timeSlider.gameObject.SetActive(true);
            currentZone = "PhoneMiniGame";
        }

        else if (DentroAlarm && !DentroPhone && !DentroArmario && !DentroLuz && !DentroPoison)
        {
            buttonImage.enabled = true;
            timeSlider.gameObject.SetActive(true);
            currentZone = "AlarmMiniGame";
        }

        else if (DentroKnifeSteal && !DentroAlarm && !DentroPhone && !DentroArmario && !DentroLuz && !DentroPoison)
        {
            buttonImage.enabled = true;
            timeSlider.gameObject.SetActive(true);
            currentZone = "StealKnifeMiniGame";
        }

        else if (!DentroLuz && !DentroPoison && !DentroArmario && !DentroPhone && !DentroAlarm && !DentroKnifeSteal)
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
                playerController.RandomSkin();
            }
            else if (currentZone == "PhoneMiniGame")
            {
                PhoneBreakRpc();
            }
            else if (currentZone == "AlarmMiniGame")
            {
                AlarmOffRpc();
            }
            else if (currentZone == "StealKnifeMiniGame")
            {
                KnifeOffRpc();
            }
        }
    }


    //--------------------------------------------- (Network Spawn)

    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawn");
        LightChecker.OnValueChanged += OnLightChange;
        PoisonChecker.OnValueChanged += OnPoisonChange;
        PhoneChecker.OnValueChanged += OnPhoneChange;
        AlarmChecker.OnValueChanged += OnAlarmChange;
        StealKnifeChecker.OnValueChanged += OnStealKnifeChange;
    }

    public override void OnNetworkDespawn()
    {
        LightChecker.OnValueChanged -= OnLightChange;
        PoisonChecker.OnValueChanged -= OnPoisonChange;
        PhoneChecker.OnValueChanged -= OnPhoneChange;
        AlarmChecker.OnValueChanged-= OnAlarmChange;
        StealKnifeChecker.OnValueChanged -= OnStealKnifeChange;

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
        Lights.sprite = LightsOff;
    }

    private void OnPoisonChange(bool previousValue, bool newValue)
    {
        Drink.sprite = PoisonDrink;
    }

    private void OnPhoneChange(bool previousValue, bool newValue)
    {
        Phone.sprite = BrokenPhone;
    }

    private void OnAlarmChange(bool previousValue, bool newValue)
    {
        Alarm.sprite = AlarmOff;
        AlarmLight.SetActive(false);
    }
    private void OnStealKnifeChange(bool previousValue, bool newValue)
    {
        stealKnife.sprite = KnifeOff;
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

    [Rpc(SendTo.Server)]
    void PhoneBreakRpc()
    {
        PhoneChecker.Value = true;
    }

    [Rpc(SendTo.Server)]
    void AlarmOffRpc()
    {
        AlarmChecker.Value = true;
    }

    [Rpc(SendTo.Server)]
    void KnifeOffRpc()
    {
        StealKnifeChecker.Value = true;
    }
}
