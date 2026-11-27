using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class GetAssasinClothes : NetworkBehaviour
{
    private Transform playerLocal;
    private List<Collider2D> AssasinColiders = new List<Collider2D>();
    private Animator PlayerAnim;
    public Image BtnImage;

    public RuntimeAnimatorController assasinControler;
    public List<RuntimeAnimatorController> NormalControler;

    private bool hasClothes = false;

    private HashSet<ulong> maskConfirmed = new HashSet<ulong>();

    

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

        bool InMask = false;

        foreach (Collider2D col in AssasinColiders)
        {
            if (col.bounds.Contains(playerLocal.position))
            {
                InMask = true;
                break;
            }
        }

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
        GameObject[] clothesObj = GameObject.FindGameObjectsWithTag("Costume");

        foreach (GameObject obj in clothesObj)
        {
            Collider2D col = obj.GetComponent<Collider2D>();

            if (col != null && !AssasinColiders.Contains(col))
            {
                AssasinColiders.Add(col);
            }
        }

        Zonas();
        VerificarMascara();
    }

    private void VerificarMascara()
    {
        foreach (NetworkObject player in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (!player.CompareTag("Player"))
                continue;

            PlayerMask mask = player.GetComponent<PlayerMask>();

            if (mask == null)
                continue;

            if (mask.HasMask.Value)
            {
                maskConfirmed.Add(player.OwnerClientId);
            }
            else if (maskConfirmed.Contains(player.OwnerClientId) && IsServer)
            {
                int randomSkin = Random.Range(0, NormalControler.Count);

                ChangeToNormalSkinRpc(player.OwnerClientId, randomSkin);

                maskConfirmed.Remove(player.OwnerClientId);

                Debug.Log("VOLTANDO PARA SKIN NORMAL!");
            }
        }
    }

    public void BotaoTrocarSkin()
    {
        ChangeSkinRpc();
    }

    [Rpc(SendTo.Server)]
    public void ChangeSkinRpc(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        foreach (NetworkObject player in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (player.OwnerClientId == clientId && player.CompareTag("Player"))
            {
                PlayerMask mask = player.GetComponent<PlayerMask>();

                if (mask != null)
                {
                    mask.GetMask();
                }

                break;
            }
        }

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
    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeToNormalSkinRpc(ulong clientId, int skinIndex)
    {
        foreach (NetworkObject player in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (player.OwnerClientId == clientId && player.CompareTag("Player"))
            {
                Animator anim = player.GetComponent<Animator>();

                if (anim != null)
                {
                    anim.runtimeAnimatorController = NormalControler[skinIndex];
                    Debug.Log("SKIN NORMAL SINCRONIZADA!");
                }

                break;
            }
        }
    }
}

   