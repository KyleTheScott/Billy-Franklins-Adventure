using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class PuzzleController : MonoBehaviour
{
    [SerializeField]
    private int playersNewMaxCharge = 1;
    [SerializeField]
    private int lanternNum = 1;
    [SerializeField]
    private int lanternLitNum = 0;
    [SerializeField]
    private Transform playerSpawnPoint;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject controllerPrefab;
    [SerializeField]
    private GameObject globalLightPrefab;
    [SerializeField]
    private GameObject uiPrefab;
    [SerializeField]
    private GameObject cameraPrefab;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        LoadInPrefabs();
        LoadPuzzle();  
    }


    private void LoadPuzzle()
    {
        if (player != null)
        {
            player.maxLightCharges = playersNewMaxCharge;
            player.lightCharges = playersNewMaxCharge;
            player.onLightChargesChanged.Invoke(player.lightCharges, player.maxLightCharges);
        }
        else
        {
            Debug.Log("Player was not found");
        }

        GlobalGameController.instance.SetLanternAmount(lanternNum, lanternLitNum);
        GlobalGameController.instance.GetComponent<CheckPointSystem>().SetCheckPoint(SceneManager.GetActiveScene().name);
    }

    private void LoadInPrefabs()
    {
        if (player == null)
        {
            player = Instantiate(playerPrefab, playerSpawnPoint.position, playerPrefab.transform.rotation).GetComponent<Player>();
            Instantiate(uiPrefab);
            Instantiate(controllerPrefab);
            Instantiate(globalLightPrefab);
            Instantiate(cameraPrefab).GetComponent<CameraMovement>().playerTransform = player.transform;
        }
    }
}
