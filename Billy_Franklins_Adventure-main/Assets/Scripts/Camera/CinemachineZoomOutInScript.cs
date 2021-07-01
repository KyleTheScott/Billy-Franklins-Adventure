using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineZoomOutInScript : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera puzzleRoomCam; //Puzzle room cam
    [SerializeField]
    private string MainCameraTag = "MainCamera";
    [SerializeField]
    private string PlayerTag = "Player";


    private CinemachineVirtualCamera playerCam; //Player cam

    private void Start()
    {
        foreach(CinemachineVirtualCamera cinemachineVirtual in FindObjectsOfType<CinemachineVirtualCamera>())
        {
            if (cinemachineVirtual.CompareTag(MainCameraTag))
            {
                playerCam = cinemachineVirtual;
                return;
            }
        }
        if (playerCam == null)
        {
            Debug.LogError("Unable to find player camera");
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PlayerTag))
        {
            playerCam.Priority = 0;
            puzzleRoomCam.Priority = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(PlayerTag))
        {
            playerCam.Priority = 1;
            puzzleRoomCam.Priority = 0;
        }
    }
}
