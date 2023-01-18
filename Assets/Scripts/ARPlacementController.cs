using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementController : MonoBehaviour
{
    [SerializeField] private GameObject arenaPrefab;
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject placementPanel;
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private SpawnManager spawnManager;
    
    private ARRaycastManager arOriginRaycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool isArenaPlaced = false;
    
    void Start()
    {
        arOriginRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    private void Update()
    {
        if (!isArenaPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }
    }
    
    public void PlaceArena()
    {
        isArenaPlaced = true;
        planeManager.enabled = false;
        placementIndicator.SetActive(false);
        GameObject arena = Instantiate(arenaPrefab, placementPose.position, Quaternion.identity);
        arena.name = "Arena";
        placementPanel.SetActive(false);
        joinRoomPanel.SetActive(true);
        spawnManager.areneTransform = arena.transform;
        spawnManager.spawnPoint = arena.transform.GetChild(0).GetChild(1).position;
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOriginRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}