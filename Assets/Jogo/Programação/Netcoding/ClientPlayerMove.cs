using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
namespace NetcodeDemo
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField]
        PlayerInput m_PlayerInput;
        [SerializeField]
        TopDownController m_TopDownController;
        private void Awake()
        {
            m_PlayerInput.enabled = false;
            m_TopDownController.enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            enabled = IsClient;
            if(!IsOwner)
            {
               enabled = false;
                m_PlayerInput.enabled = false;
                m_TopDownController.enabled = false;
                return;
            }

            m_PlayerInput.enabled = true;
            m_TopDownController.enabled = true;
        }
    }
}
