using UnityEngine;

public class MinigameButton : MonoBehaviour
{
    [SerializeField] private GameObject basic_ui;
    [SerializeField] private GameObject minigame;

    public void StartMinigame()
    {
        minigame.SetActive(true);
        basic_ui.SetActive(false);
    }

    public void CloseMinigame()
    {
        basic_ui.SetActive(true); 
        minigame.SetActive(false);
    }
}
