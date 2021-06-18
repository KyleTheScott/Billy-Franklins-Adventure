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
    //private GameObject settingsUIPrefab;
    //[SerializeField]
    //private GameObject pauseMenuUIPrefab;
    //[SerializeField]
    private GameObject cameraPrefab;
    private Player player;
    //private Canvas settingsUI;
    //private Canvas pauseMenuUI;
    private CameraMovement camera;

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
        //GlobalGameController.instance.GetComponent<CheckPointSystem>().SetCheckPoint(SceneManager.GetActiveScene().name);
    }

    private void LoadInPrefabs()
    {
        if (player == null)
        {
            player = Instantiate(playerPrefab, playerSpawnPoint.position, playerPrefab.transform.rotation).GetComponent<Player>();
            DontDestroyOnLoad(player);
            DontDestroyOnLoad(Instantiate(uiPrefab));
            DontDestroyOnLoad(Instantiate(controllerPrefab));
            DontDestroyOnLoad(Instantiate(globalLightPrefab));
            camera = Instantiate(cameraPrefab).GetComponent<CameraMovement>();
            camera.playerTransform = player.transform;
            DontDestroyOnLoad(camera.gameObject);
            //settingsUI = Instantiate(settingsUIPrefab).GetComponent<Canvas>();
            //pauseMenuUI = Instantiate(pauseMenuUIPrefab).GetComponent<Canvas>();
            //DontDestroyOnLoad(settingsUI);
            //DontDestroyOnLoad(pauseMenuUI);
            //SetUIReferences();
        }
    }

    //private void SetUIReferences()
    //{
    //    settingsUI.GetComponent<SettingsMenu>().SetPauseMenuUIReference(pauseMenuUI);
    //    pauseMenuUI.GetComponent<PauseMenu>().SetSettingsUIReference(settingsUI, FindObjectOfType<GlobalGameController>().GetComponent<CheckPointSystem>());
    //}
}
