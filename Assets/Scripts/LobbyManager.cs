using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TextMeshProUGUI userNameInputPlaceHolder;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TextMeshProUGUI spinnerTypeText;
    
    private bool connectedToLobby = false;
    private SpinnerType spinnerType;

    private void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {   
            infoText.text = "You are logged in as " + PhotonNetwork.LocalPlayer.NickName;
            lobbyPanel.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            loginPanel.SetActive(true);
        }
    }
    
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        connectedToLobby = true;
        spinnerType = SpinnerType.Attacker;
        spinnerTypeText.text = "Spinner Type:\nAttacker";
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { "SpinnerType", 0 } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp); 
    }
    
    public void OnClickStartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void onClickConnect()
    {
        if (userNameInput.text.Length >= 1)
        {
            PhotonNetwork.LocalPlayer.NickName = userNameInput.text;
            infoText.text = "You are logged in as " + PhotonNetwork.LocalPlayer.NickName;
            lobbyPanel.SetActive(true);
            loginPanel.SetActive(false);
            userNameInput.text = "";
            userNameInputPlaceHolder.gameObject.SetActive(true);
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public void DisconnetFromLobby()
    {
        PhotonNetwork.Disconnect();
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    public void OnChangeSpinnerTypeButton()
    {
        int index = (int)spinnerType;
        index++;
        index = index % 2;
        if (index == (int) SpinnerType.Attacker)
        {
            spinnerType = SpinnerType.Attacker;
            spinnerTypeText.text = "Spinner Type:\nAttacker";
        }
        else if(index == (int) SpinnerType.Defender)
        {
            spinnerType = SpinnerType.Defender;
            spinnerTypeText.text = "Spinner Type:\nDefender";
        }
        
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { "SpinnerType", (int)spinnerType } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }
}
