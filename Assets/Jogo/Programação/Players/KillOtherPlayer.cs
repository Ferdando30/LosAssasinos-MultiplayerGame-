using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections.Generic;

public class KillOtherPlayer : NetworkBehaviour
{
    [Header("Imagem do botão")]
    public Image KillBtnImage;

    private Animator animator;
    private Transform playerLocal;

    private List<Collider2D> PlayerKillCollider = new List<Collider2D>();

    private Rigidbody2D rbTarget;
    private NetworkObject playerTarget;

    //public NetworkVariable<bool> RbChecker = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

    private void EncontrarHitboxOutroPlayer()
    {
        PlayerKillCollider.Clear();

        foreach (NetworkObject player in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (player.IsOwner)
            {
                continue;
            }

            foreach (Collider2D hitbox in player.GetComponentsInChildren<Collider2D>(true))
            {
                if (hitbox.CompareTag("PlayerKillBox"))
                {
                    PlayerKillCollider.Add(hitbox);
                    break;
                }
            }
        }
    }

    private void EnterKillZone()
    {
        EncontrarPlayerLocal();
        EncontrarHitboxOutroPlayer();
        if (playerLocal == null || PlayerKillCollider == null)
            return;

        bool InsidePlayerKillBox = false;

        if (playerLocal != null)
        {
            foreach (Collider2D hitbox in PlayerKillCollider)
            {
                if (hitbox == null || !hitbox.enabled || !hitbox.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (hitbox.OverlapPoint(playerLocal.position))
                {
                    InsidePlayerKillBox = true;
                    playerTarget = hitbox.GetComponentInParent<NetworkObject>();
                    if (playerTarget != null)
                        rbTarget = playerTarget.GetComponent<Rigidbody2D>();
                    break;
                }
            }
        }

        KillBtnImage.enabled = InsidePlayerKillBox;
    }

    public void killOtherPlayer()
    {
        Debug.Log("Matou:" + playerTarget.NetworkObjectId);
        FreezeTargetRpc(new NetworkObjectReference(playerTarget));
        
    }

    private void Update()
    {
        EnterKillZone();
    }


    [Rpc(SendTo.Server)]
    private void FreezeTargetRpc(NetworkObjectReference targetReference)
    {
        if (!targetReference.TryGet(out NetworkObject target))
            return;

        PlayerFreeze freeze = target.GetComponent<PlayerFreeze>();

        if (freeze == null)
            return;

        Debug.Log("Servidor congelando: " + target.NetworkObjectId);

        freeze.SetFrozenServer(true);
    }
}