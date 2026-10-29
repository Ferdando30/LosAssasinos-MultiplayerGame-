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
    private NetworkObject playerTarget;
  

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
                    break;
                }
            }
        }

        KillBtnImage.enabled = InsidePlayerKillBox;
    }

    public void killOtherPlayer()
    {
        Debug.Log("Matou:" + playerTarget.NetworkObjectId);
        freezePlayerServerRpc(playerTarget.NetworkObjectId);
    }

    private void Update()
    {
        EnterKillZone();
    }

    [ServerRpc(RequireOwnership = false)]
    private void freezePlayerServerRpc(ulong targetId)
    {
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(targetId, out NetworkObject target))
        {
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

                rb.freezeRotation = true;
            }
        }
    }
}