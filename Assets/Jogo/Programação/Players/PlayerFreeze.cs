using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem.OnScreen;

public class PlayerFreeze : NetworkBehaviour
{
    [Header("Alvo da câmera")]
    [SerializeField] private Transform cameraFollowTarget;

    private Transform CameraTarget =>
        cameraFollowTarget != null ? cameraFollowTarget : transform;

    private SpectatorCameraController cameraController;
    private OnScreenStick joystick;

    private NetworkVariable<bool> IsFrozen = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private NetworkVariable<NetworkObjectReference> FrozenBy =
        new NetworkVariable<NetworkObjectReference>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        IsFrozen.OnValueChanged += FreezePlayerChecker;
        FrozenBy.OnValueChanged += OnFrozenByChanged;

        if (IsOwner)
        {
            cameraController =
                FindFirstObjectByType<SpectatorCameraController>();

            joystick = FindFirstObjectByType<OnScreenStick>();

            AtualizarCameraEspectador();
        }
    }

    public override void OnNetworkDespawn()
    {
        IsFrozen.OnValueChanged -= FreezePlayerChecker;
        FrozenBy.OnValueChanged -= OnFrozenByChanged;
    }

    private void FreezePlayerChecker(bool previousValue, bool newValue)
    {
        AtualizarCameraEspectador();
    }

    private void OnFrozenByChanged(
        NetworkObjectReference previousValue,
        NetworkObjectReference newValue)
    {
        AtualizarCameraEspectador();
    }

    public void SetFrozenServer(bool frozen, NetworkObject attacker)
    {
        if (!IsServer)
            return;

        IsFrozen.Value = frozen;

        if (frozen && attacker != null)
        {
            FrozenBy.Value = new NetworkObjectReference(attacker);
        }
        else
        {
            FrozenBy.Value = default;
        }
    }

    private void AtualizarCameraEspectador()
    {
        if (!IsOwner)
            return;

        if (cameraController == null)
            cameraController =
                FindFirstObjectByType<SpectatorCameraController>();

        if (joystick == null)
            joystick = FindFirstObjectByType<OnScreenStick>();

        if (IsFrozen.Value)
        {
            // Esconde o joystick apenas neste cliente
            if (joystick != null)
                joystick.gameObject.SetActive(false);

            if (FrozenBy.Value.TryGet(out NetworkObject attacker))
            {
                PlayerFreeze attackerFreeze =
                    attacker.GetComponent<PlayerFreeze>();

                Transform target = attackerFreeze != null
                    ? attackerFreeze.CameraTarget
                    : attacker.transform;

                if (cameraController != null)
                    cameraController.SetTarget(target);
            }
        }
       
    }
}