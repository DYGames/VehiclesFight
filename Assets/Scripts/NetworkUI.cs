using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkUI : MonoBehaviour
{
    public NetworkManager manager;
    public Text addresstext;
    public Text ipText;
    public Dropdown dropdown;
    string address;

    void Start()
    {
        address = "localhost";
        ipText.text = "Your IP : " + Network.player.ipAddress;
    }

    public void StartHost()
    {
        GameData.Instance.PlayerType = (Player.PLAYERTYPE)dropdown.value;
        Cursor.lockState = CursorLockMode.Locked;
        manager.StartHost();
        gameObject.SetActive(false);
    }

    public void StartClient()
    {
        GameData.Instance.PlayerType = (Player.PLAYERTYPE)dropdown.value;
        manager.networkAddress = address;
        Cursor.lockState = CursorLockMode.Locked;
        manager.StartClient();
        gameObject.SetActive(false);
    }

    public void AddressChange()
    {
        address = addresstext.text;
    }
}
