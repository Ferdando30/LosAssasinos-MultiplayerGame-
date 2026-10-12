using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MiniGames : MonoBehaviour
{
    private VisualElement TelaLights;

    private Button OpenMiniGame;
    private Button ExitMiniGame;
    private Button LightsBtn;

    public GameObject LightsFather;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        TelaLights = root.Q<VisualElement>("LightsMiniGame");
        OpenMiniGame = root.Q<Button>("InteractBtn");
        ExitMiniGame = root.Q<Button>("ExitBtn");
        LightsBtn = root.Q<Button>("LightsBtn");

        TelaLights.visible = false;

        OpenMiniGame.RegisterCallback<ClickEvent>(OpenMiniGames);
        ExitMiniGame.RegisterCallback<ClickEvent>(CloseMiniGames);
        LightsBtn.RegisterCallback<ClickEvent>(TurnLightsOff);

        LightsFather.SetActive(true);
    }

    void OpenMiniGames(ClickEvent clickEvent)
    {
        TelaLights.visible = true;
    }

    void CloseMiniGames(ClickEvent clickEvent)
    {
        TelaLights.visible = false;
    }

    void TurnLightsOff(ClickEvent clickEvent)
    {
        LightsFather.SetActive(false);
    }
}
