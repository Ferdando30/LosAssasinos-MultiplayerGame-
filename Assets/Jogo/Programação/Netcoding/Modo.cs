using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Modo : MonoBehaviour
{
    private Button HostBtn;
    private Button ClientBtn;
    private TextField IP;
    private TextField Port;

    private VisualElement tela;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        HostBtn = root.Q<Button>("HostBtn");
        ClientBtn = root.Q<Button>("ClientBtn");
        IP = root.Q<TextField>("IpAdress");
        Port = root.Q<TextField>("PortAdress");
        tela = root.Q<VisualElement>("Holder");

        HostBtn.RegisterCallback<ClickEvent>(Host);
        ClientBtn.RegisterCallback<ClickEvent>(Client);

        tela.visible = true;

        if (PlayerPrefs.HasKey("ip"))
            IP.value = PlayerPrefs.GetString("ip");
        if (PlayerPrefs.HasKey("port"))
            Port.value = PlayerPrefs.GetString("port");
    }

    private void Host(ClickEvent clickEvent)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("127.0.0.1", ushort.Parse(Port.value), "0.0.0.0");
        //SceneManager.LoadScene("MainGameScene");
        tela.visible = false;
        NetworkManager.Singleton.StartHost();
        GravarReferencia();
    }

    private void Client(ClickEvent clickEvent)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(IP.value, ushort.Parse(Port.value));
        // SceneManager.LoadScene("MainGameScene");
        tela.visible = false;
        NetworkManager.Singleton.StartClient();
        GravarReferencia();
    }

    private void GravarReferencia()
    {
        PlayerPrefs.SetString("ip", IP.value);
        PlayerPrefs.SetString("port", Port.value);
    }
}
