
using UnityEngine;
using Unity.Netcode;

public class PlayerFreeze : NetworkBehaviour
{
    private Rigidbody2D rb;

    private RigidbodyConstraints2D originalConstraints;

    private NetworkVariable<bool> IsFrozen = new NetworkVariable<bool>(
        false,
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

        // Aplica o estado atual ao entrar na rede.
        ApplyFreeze(IsFrozen.Value);
    }

    public override void OnNetworkDespawn()
    {
        IsFrozen.OnValueChanged -= FreezePlayerChecker;
    }

    private void FreezePlayerChecker(bool previousValue, bool newValue)
    {
        ApplyFreeze(newValue);
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
    public void SetFrozenServer(bool frozen)
    {
        if (!IsServer)
            return;

        IsFrozen.Value = frozen;
    }
}

