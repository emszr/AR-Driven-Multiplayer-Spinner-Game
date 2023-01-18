using System;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class Spinner : MonoBehaviourPun
{
    public float spinSpeed = 3000;
    public bool isSpinning = true;
    public TextMeshProUGUI playerNameText;
    public Joystick joystick;
    public GameObject spinnerModel;

    
    void Start()
    {
        if (photonView.IsMine)
        {
            transform.GetComponent<MovementController>().enabled = true;
            joystick.gameObject.SetActive(true);
        }
        else
        {
            transform.GetComponent<MovementController>().enabled = false;
            joystick.gameObject.SetActive(false);
        }

        SetPlayerName();
    }

    private void FixedUpdate()
    {
        if (isSpinning)
        {
            spinnerModel.transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime) ;
        }
    }

    void SetPlayerName()
    {
        if (playerNameText != null)
        {
            if (photonView.IsMine)
            {
                playerNameText.text = photonView.Owner.NickName;
                playerNameText.color = Color.red;
            }
            else
            {
                playerNameText.text = photonView.Owner.NickName;
            }
        }
    }
}

public enum SpinnerType
{
    Attacker = 0,
    Defender = 1
}

public enum RaiseEventCodes
{
    PlayerSpawnEventCode = 0
}