using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class GetAssasinClothes : MonoBehaviour
{
    private Transform playerLocal;
    public Collider2D AssasinColider;
    private Animator PlayerAnim;
    public Image BtnImage;

    public RuntimeAnimatorController assasinControler;
    public List <RuntimeAnimatorController> NormalControler;



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

        else if (!InMask)
        {
BtnImage.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Zonas();
    }

    public void ChangeSkin()
    {
PlayerAnim.runtimeAnimatorController = assasinControler;
    }
}
