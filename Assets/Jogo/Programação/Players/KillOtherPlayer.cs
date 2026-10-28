using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class KillOtherPlayer : MonoBehaviour
{
    [Header("Imagem do botão")]
    public Image KillBtnImage;

    private Animator animator;
    private Transform playerLocal;
    private Collider2D PlayerKillCollider;

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
        if (PlayerKillCollider != null)
            return;

        foreach (NetworkObject player in
            FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (player.IsOwner)
                continue;

            foreach (Collider2D hitbox in
                player.GetComponentsInChildren<Collider2D>(true))
            {
                if (hitbox.CompareTag("PlayerKillBox"))
                {
                    PlayerKillCollider = hitbox;
                    return;
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

        bool InsideOtherPlayer = PlayerKillCollider.bounds.Contains(playerLocal.position);

        if (InsideOtherPlayer)
        {
            KillBtnImage.enabled = true;
        }

        else if (!InsideOtherPlayer)
        {
            KillBtnImage.enabled = false;
        }
    }

    public void killOtherPlayer()
    {
        Debug.Log("Matou");
    }

    private void Update()
    {
        EnterKillZone();
    }
}
