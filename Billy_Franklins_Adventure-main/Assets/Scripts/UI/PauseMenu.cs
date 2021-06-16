using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private Canvas settingsUI = null;

    // Start is called before the first frame update
    private void Awake()
    {
        try
        {
            settingsUI = GameObject.Find("Settings UI").GetComponent<Canvas>();
        }
        catch
        {
            Debug.Log("Couldn't get settings UI...");
        }
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
    }

    public void Settings()
    {
        //if there is settings UI then show it
        if (settingsUI != null)
        {
            settingsUI.gameObject.SetActive(true);
            settingsUI.GetComponent<SettingsMenu>().SettingsUICalled();
        }
    }

    public void ReloadCheckpoint()
    {
        //where is checkpoint system located in scene (what object)?
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void PauseMenuCalled()
    {
        Time.timeScale = 0;
    }
}
