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
    private Button CriarBtn;
    private Button EntrarBtn;
    private TextField IP;
    private TextField Port;

    private VisualElement telaInicial;
    private VisualElement tela;
    private VisualElement telaServers;
    private VisualElement telaCriar;
    private VisualElement telaEntrar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        HostBtn = root.Q<Button>("HostBtn");
        ClientBtn = root.Q<Button>("ClientBtn");
        CriarBtn = root.Q<Button>("CriarBtn");
        EntrarBtn = root.Q<Button>("EntrarBtn");

        IP = root.Q<TextField>("IpAdress");
        Port = root.Q<TextField>("PortAdress");

        tela = root.Q<VisualElement>("Holder");
        telaInicial = root.Q<VisualElement>("TelaInicial");
        telaServers = root.Q<VisualElement>("ServerHolder");
        telaCriar = root.Q<VisualElement>("TelaHostServer");
        telaEntrar = root.Q<VisualElement>("TelaEnterServer");

        HostBtn.RegisterCallback<ClickEvent>(Host);
        ClientBtn.RegisterCallback<ClickEvent>(Client);
        CriarBtn.RegisterCallback<ClickEvent>(CriarSalaTela);
        EntrarBtn.RegisterCallback<ClickEvent>(EntrarSalaTela);

        telaInicial.visible = true;
        telaServers.visible = false;
        telaCriar.visible = false;
        telaEntrar.visible = false;

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
        telaCriar.visible = false;
        telaServers.visible = false;
        NetworkManager.Singleton.StartHost();
        GravarReferencia();
    }

    private void Client(ClickEvent clickEvent)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(IP.value, ushort.Parse(Port.value));
        // SceneManager.LoadScene("MainGameScene");
        tela.visible = false;
        telaServers.visible = false;
        telaEntrar.visible = false;
        NetworkManager.Singleton.StartClient();
        GravarReferencia();
    }

    private void GravarReferencia()
    {
        PlayerPrefs.SetString("ip", IP.value);
        PlayerPrefs.SetString("port", Port.value);
    }

    private void CriarSalaTela(ClickEvent clickEvent)
    {
        telaInicial.visible = false;
        telaServers.visible = true;
        telaCriar.visible = true;
        telaEntrar.visible = false;
    }

    private void EntrarSalaTela(ClickEvent clickEvent)
    {
        telaInicial.visible = false;
        telaServers.visible = true;
        telaCriar.visible = false;
        telaEntrar.visible = true;
    }
}
