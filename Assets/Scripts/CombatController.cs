using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CombatController : MonoBehaviourPun
{
    private Rigidbody rb;
    public Spinner spinner;
    public GameObject joystick;
    public GameObject healthCanvas;
    private float startSpinSpeed;
    private float currentSpinSpeed;
    public TextMeshProUGUI spinSpeedRatio_Text;
    public SpinnerType spinnerType;

    public float common_Damage_Coefficient = 0.04f;
    private bool isDead = false;

    public float doDamage_Coefficient_Attacker = 10f;
    public float getDamaged_Coefficient_Attacker = 1.2f;

    public float doDamage_Coefficient_Defender = 0.75f;
    public float getDamaged_Coefficient_Defender = 0.2f;

    [SerializeField] private GameObject roundOverPanel;

    private void Awake()
    {
        startSpinSpeed = spinner.spinSpeed;
        currentSpinSpeed = spinner.spinSpeed;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetSpinnerType();
    }

    public void SetSpinnerType()
    {
        object tempObject;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SpinnerType", out tempObject))
        {
            int temp = (int)tempObject;
            if (temp == 0)
            {
                spinnerType = SpinnerType.Attacker;
            }
            else
            {
                spinnerType = SpinnerType.Defender;
            }
        }
    }
    
    [PunRPC]
    public void DoDamage(float damageAmount)
    {
        if (!isDead)
        {
            spinner.spinSpeed -= damageAmount;
            currentSpinSpeed = spinner.spinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;
            if (currentSpinSpeed < 100)
            {
                Die();
            }
        }
    }
    
    private void Die()
    {
        healthCanvas.SetActive(false);
        roundOverPanel.SetActive(true);
        isDead = true;
        GetComponent<MovementController>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        spinner.spinSpeed = 0f;
        joystick.SetActive(false);
        if (photonView.IsMine)
        {
            StartCoroutine(ReSpawn());
        }
    }
    
    private IEnumerator ReSpawn()
    {
        float respawnTime = 5.0f;
        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            GetComponent<MovementController>().enabled = false;
        }
        
        GetComponent<MovementController>().enabled = true;
        photonView.RPC("ReBorn", RpcTarget.All);
    }
    
    [PunRPC]
    public void ReBorn()
    {
        if (photonView.IsMine)
        {
            joystick.SetActive(true);
        }
        
        healthCanvas.SetActive(true);
        spinner.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinner.spinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;
        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        roundOverPanel.SetActive(false);
        isDead = false;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Spinner"))
        {
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            if (mySpeed > otherPlayerSpeed)
            {
                float default_Damage_Amount = mySpeed * 2000f * common_Damage_Coefficient;
                if (spinnerType == SpinnerType.Attacker)
                {
                    default_Damage_Amount *= doDamage_Coefficient_Attacker;
                }
                else
                {
                    default_Damage_Amount *= doDamage_Coefficient_Defender;
                }
                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    float dmg;
                    if (collision.other.gameObject.GetComponent<CombatController>().spinnerType == SpinnerType.Attacker)
                    {
                        dmg = default_Damage_Amount * getDamaged_Coefficient_Attacker;
                    }
                    else
                    {
                        dmg = default_Damage_Amount * getDamaged_Coefficient_Defender;
                    }
                    if (dmg > 750)
                    {
                        dmg = 400f;
                    }
                    
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, dmg);
                }
            }         
        }
    }
}