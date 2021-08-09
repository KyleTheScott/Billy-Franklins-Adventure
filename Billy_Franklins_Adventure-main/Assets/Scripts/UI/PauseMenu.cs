using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Canvas settingsUI = null;
    public CheckPointSystem checkPointSystem = null;

    private void Start()
    {
        Debug.Log("StartPauseMenu");
        FindObjectOfType<Player>().GetComponent<Player>().ReferencePauseMenuUI(GetComponent<Canvas>());
    }

    public void ContinueGame()
    {
        FindObjectOfType<MusicController>().PlayMenuSelect();
        Time.timeScale = 1;
        FindObjectOfType<Player>().GetComponent<Player>().ClosePauseMenu();
    }

    public void Settings()
    {
        //if there is settings UI then show it
        if (settingsUI != null)
        {
            FindObjectOfType<MusicController>().PlayMenuSelect();
            settingsUI.gameObject.SetActive(true);
            settingsUI.GetComponent<SettingsMenu>().SettingsUICalled();
        }
    }

    public void ReloadCheckpoint()
    {
        //reload level and move player
        //SceneManager.LoadScene(checkPointSystem.GetCheckPoint(), LoadSceneMode.Single);
        //FindObjectOfType<Player>().gameObject.transform.position = GameObject.Find("PlayerSpawnPoint").GetComponent<Transform>().transform.position;
        FindObjectOfType<ObjectsCollision>().EmptyObjects();
        FindObjectOfType<MusicController>().PlayMenuSelect();
        checkPointSystem.PlayerDeath();
    }

    public void MainMenu()
    {
        FindObjectOfType<MusicController>().PlayMenuSelect();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

        //removes menu's when returning to main menu
        DestroyDDOLObjects();
        Destroy(settingsUI.gameObject);
        Destroy(gameObject);
    }

    public void PauseMenuCalled()
    {
        Time.timeScale = 0;
        if(settingsUI != null)
        {
            settingsUI.gameObject.SetActive(false);
        }
    }

    public void SetSettingsUIReference(Canvas reference, CheckPointSystem check)
    {
        settingsUI = reference;
        checkPointSystem = check;
    }

    private void Update()
    {
        //close pause menu if key is hit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ContinueGame();
        }
    }

    private void DestroyDDOLObjects()
    {
        Destroy(GameObject.Find("Player(Clone)"));
        Destroy(GameObject.Find("Main Camera(Clone)"));
        Destroy(GameObject.Find("ControllerManager(Clone)"));
        Destroy(GameObject.Find("Global Light 2D(Clone)"));
        Destroy(GameObject.Find("UI(Clone)"));

    }
}
