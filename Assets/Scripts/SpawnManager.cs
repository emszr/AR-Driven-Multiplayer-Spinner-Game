using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform areneTransform;
    public Vector3 spawnPoint;
    private SpinnerType spinnerType;

    [SerializeField] private GameObject attackerSpinnerPrefab;
    [SerializeField] private GameObject defenderSpinnerPrefab;
    
    private void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    
    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    
    private void SpawnPlayer()
    {
        object playerSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SpinnerType", out playerSelectionNumber))
        {
            GameObject playerGameobject;
            if ((int)playerSelectionNumber == 0)
            {
                spinnerType = SpinnerType.Attacker;
                playerGameobject = Instantiate(attackerSpinnerPrefab, spawnPoint, Quaternion.identity);
            }
            else
            {
                spinnerType = SpinnerType.Defender;
                playerGameobject = Instantiate(defenderSpinnerPrefab, spawnPoint, Quaternion.identity);
            }
            
            PhotonView _photonView = playerGameobject.GetComponent<PhotonView>();
            if (PhotonNetwork.AllocateViewID(_photonView))
            {
                object[] data = new object[]
                {
                    playerGameobject.transform.position - areneTransform.position, playerGameobject.transform.rotation, _photonView.ViewID, (int)spinnerType
                };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption =EventCaching.AddToRoomCache

                };
                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode,data, raiseEventOptions,sendOptions);
            }
            else
            {
                Debug.Log("Failed to allocate a viewID");
                Destroy(playerGameobject);
            }
        }
    }
    
    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector3 receivedPosition = (Vector3)data[0];
            Quaternion receivedRotation = (Quaternion)data[1];
            int receivedPlayerSelectionData = (int)data[3];
            GameObject player = null;

            if (receivedPlayerSelectionData == (int)SpinnerType.Attacker)
            {
                player = Instantiate(attackerSpinnerPrefab, receivedPosition+ areneTransform.position, receivedRotation);
            }
            else
            {
                player = Instantiate(defenderSpinnerPrefab, receivedPosition+ areneTransform.position, receivedRotation);
            }
            
            PhotonView _photonView = player.GetComponent<PhotonView>();
            _photonView.ViewID = (int)data[2];
        }
    }
    
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }
    }
}