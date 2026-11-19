using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class OpenDoor : NetworkBehaviour
{
    //private Transform playerLocal;
    public Collider2D DoorCollider;

    public Sprite OpenDoorSprite;
    public Sprite ClosedDoorSprite;

    private SpriteRenderer myRenderer;

    public NetworkVariable<bool> DoorChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Zonas()
    {
        bool playerInside = false;

        foreach (NetworkObject player in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (!player.CompareTag("Player"))
                continue;

            if (DoorCollider.bounds.Contains(player.transform.position))
            {
                playerInside = true;
                break;
            }
        }

        if (DoorChecker.Value != playerInside)
        {
            DoorChecker.Value = playerInside;
        }
    }

    void Update()
    {
        if (!IsSpawned || !IsServer)
            return;

        Zonas();
    }

    public override void OnNetworkSpawn()
    {
        myRenderer = GetComponent<SpriteRenderer>();

        DoorChecker.OnValueChanged += OnDoorChange;

        AtualizarSprite(DoorChecker.Value);
    }

    public override void OnNetworkDespawn()
    {
        DoorChecker.OnValueChanged -= OnDoorChange;
    }

    private void OnDoorChange(bool previousValue, bool newValue)
    {
        AtualizarSprite(newValue);
    }

    private void AtualizarSprite(bool isOpen)
    {
        if (isOpen)
            myRenderer.sprite = OpenDoorSprite;
        else
            myRenderer.sprite = ClosedDoorSprite;
    }
}
