using Photon.Pun;
using UnityEngine;

public class SpinnerSynchronization : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;
    Vector3 networkedPosition;
    Quaternion networkedRotation;
    public bool isTeleportEnabled = true;
    public float teleporDistance = 1.0f;
    private float distance;
    private float angle;
    private GameObject arena;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        networkedPosition = new Vector3();
        networkedRotation = new Quaternion();
        arena = GameObject.Find("Arena");
    }
    
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance * (1.0f / PhotonNetwork.SerializationRate));
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkedRotation, angle * (1.0f / PhotonNetwork.SerializationRate));
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position - arena.transform.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else
        {
            networkedPosition = (Vector3)stream.ReceiveNext() + arena.transform.position;
            networkedRotation = (Quaternion)stream.ReceiveNext();
            if (isTeleportEnabled)
            {
                if (Vector3.Distance(rb.position, networkedPosition) > teleporDistance)
                {
                    rb.position = networkedPosition;
                }
            }
           
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            rb.velocity = (Vector3)stream.ReceiveNext();
            networkedPosition += rb.velocity * lag;
            distance = Vector3.Distance(rb.position, networkedPosition);
            rb.angularVelocity = (Vector3)stream.ReceiveNext();
            networkedRotation = Quaternion.Euler(rb.angularVelocity*lag) * networkedRotation;
            angle = Quaternion.Angle(rb.rotation, networkedRotation);
        }
    }
}