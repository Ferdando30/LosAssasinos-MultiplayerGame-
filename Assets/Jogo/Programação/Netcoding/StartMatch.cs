using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class StartMatch : MonoBehaviour
{
    public void BeginMatch()
    {
        GameObject[] gobjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in gobjects)
        {
            TopDownController objScript;
            objScript = obj.GetComponent<TopDownController>();
            objScript.timer = 0.1f;
        }
        NetworkManager.Singleton.SceneManager.LoadScene("MainGameScene", LoadSceneMode.Single);
    }
}
