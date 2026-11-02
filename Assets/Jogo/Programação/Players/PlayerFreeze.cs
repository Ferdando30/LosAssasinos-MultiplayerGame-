
using UnityEngine;
using Unity.Netcode;

public class PlayerFreeze : NetworkBehaviour
{
    private Rigidbody2D rb;

    private RigidbodyConstraints2D originalConstraints;

    [Header("Alvo da câmera")]
    [SerializeField] private Transform cameraFollowTarget;

    private Transform CameraTarget =>
        cameraFollowTarget != null ? cameraFollowTarget : transform;

    private SpectatorCameraController cameraController;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalConstraints = rb.constraints;
    }

    public override void OnNetworkSpawn()
    {
        IsFrozen.OnValueChanged += FreezePlayerChecker;
        FrozenBy.OnValueChanged += OnFrozenByChanged;

        cameraController =
           FindFirstObjectByType<SpectatorCameraController>();

        ApplyFreeze(IsFrozen.Value);
        AtualizarCameraEspectador();
    }

    public override void OnNetworkDespawn()
    {
        IsFrozen.OnValueChanged -= FreezePlayerChecker;
        FrozenBy.OnValueChanged -= OnFrozenByChanged;
    }

    private void FreezePlayerChecker(bool previousValue, bool newValue)
    {
        ApplyFreeze(newValue);
        AtualizarCameraEspectador();
    }

    private void OnFrozenByChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
    {
        AtualizarCameraEspectador();
    }

    private void ApplyFreeze(bool frozen)
    {
        if (frozen)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                             RigidbodyConstraints2D.FreezePositionY |
                             RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = originalConstraints;
        }
    }

    // Chamado pelo servidor.
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
        {
            cameraController =
                FindFirstObjectByType<SpectatorCameraController>();
        }

        if (cameraController == null)
            return;

        if (IsFrozen.Value)
        {
            if (FrozenBy.Value.TryGet(out NetworkObject attacker))
            {
                PlayerFreeze attackerFreeze =
                    attacker.GetComponent<PlayerFreeze>();

                Transform target = attackerFreeze != null
                    ? attackerFreeze.CameraTarget
                    : attacker.transform;

                cameraController.SetTarget(target);
            }
        }
        else
        {
            cameraController.SetTarget(CameraTarget);
        }
    }
}


