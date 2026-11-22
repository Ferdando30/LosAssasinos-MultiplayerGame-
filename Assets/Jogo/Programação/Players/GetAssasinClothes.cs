using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class GetAssasinClothes : NetworkBehaviour
{
    private Transform playerLocal;
    public Collider2D AssasinColider;
    private Animator PlayerAnim;
    public Image BtnImage;

    public RuntimeAnimatorController assasinControler;
    public List<RuntimeAnimatorController> NormalControler;

    private bool hasClothes = false;

    private void EncontrarPlayerLocal()
    {
        if (playerLocal != null)
            return;

        foreach (NetworkObject player in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (player.IsOwner && player.CompareTag("Player"))
            {
                playerLocal = player.transform;
                PlayerAnim = player.GetComponent<Animator>();
                break;
            }
        }
    }

    private void Zonas()
    {
        EncontrarPlayerLocal();

        if (playerLocal == null)
            return;

        bool InMask = AssasinColider.bounds.Contains(playerLocal.position);

        if (InMask)
        {
           
            BtnImage.enabled = true;
        }
        else
        {
            BtnImage.enabled = false;
        }
    }

    void Update()
    {
        Zonas();
        
    }

    public void BotaoTrocarSkin()
    {
        ChangeSkinRpc();
    }

    [Rpc(SendTo.Server)]
    public void ChangeSkinRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        ChangeSkinOnAllClientsRpc(clientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeSkinOnAllClientsRpc(ulong clientId)
    {
        foreach (NetworkObject player in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (player.OwnerClientId == clientId && player.CompareTag("Player"))
            {
                Debug.Log("Jogador encontrado: " + player.name);

                PlayerMask mask = player.GetComponent<PlayerMask>();

                if (mask != null)
                {
                    mask.GetMask();
                }


                Animator anim = player.GetComponent<Animator>();

                if (anim != null)
                {
                        anim.runtimeAnimatorController = assasinControler;
                        Debug.Log("SKIN ALTERADA!");
                   
                }

                break;
            }
        }
    }
}