using System;
using Core.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private TextMeshProUGUI playerInGame;

    private void Awake()
    {
        serverBtn.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartServer();
        }));
        hostBtn.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartHost();
        }));
        clientBtn.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartClient();
        }));
    }

    private void Update()
    {
        playerInGame.text = $"Player in game: {PlayersManager.Instance.PlayersInGame}";
    }
}
