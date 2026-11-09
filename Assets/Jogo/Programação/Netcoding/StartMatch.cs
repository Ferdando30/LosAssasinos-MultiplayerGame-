using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class StartMatch : MonoBehaviour
{
    public void BeginMatch()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("MainGameScene", LoadSceneMode.Single);
    }
}
