using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
   [SerializeField] private GameObject placementPanel;
   [SerializeField] private GameObject joinRoomPanel;
   [SerializeField] private GameObject roomPanel;
   [SerializeField] private TextMeshProUGUI roomNameText;

   private void Start()
   {
      placementPanel.SetActive(true);
      joinRoomPanel.SetActive(false);
   }
   
   public void OnPlayButton()
   {
      PhotonNetwork.JoinRandomRoom();
   }

   public override void OnPlayerEnteredRoom(Player newPlayer)
   {
      base.OnPlayerEnteredRoom(newPlayer);
      placementPanel.SetActive(false);
      joinRoomPanel.SetActive(false);
      roomPanel.SetActive(false);
   }

   public override void OnJoinedRoom()
   {
      if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
      {
         placementPanel.SetActive(false);
         joinRoomPanel.SetActive(false);
         roomPanel.SetActive(true);
         roomNameText.text = PhotonNetwork.CurrentRoom.Name;
      }
      else
      {
         placementPanel.SetActive(false);
         joinRoomPanel.SetActive(false);
         roomPanel.SetActive(false);
      }
   }
   
   public void OnReturnMenuButton()
   {
      SceneManager.LoadScene(0);
   }
   
   public override void OnJoinRandomFailed(short returnCode, string message)
   {
      CreateRoom();
   }

   public void OnLeaveRoomButton()
   {
      PhotonNetwork.LeaveRoom();
      SceneManager.LoadScene(0);

   }
   
   public override void OnLeftRoom()
   {
      SceneManager.LoadScene(0);
   }
   
   private void CreateRoom()
   {
      string roomName = PhotonNetwork.LocalPlayer.NickName + "'s Room";
      RoomOptions roomOptions = new RoomOptions();
      roomOptions.MaxPlayers = 2;
      PhotonNetwork.CreateRoom(roomName, roomOptions);
   }
}