using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Canvas settingsUI = null;
    public CheckPointSystem checkPointSystem = null;

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
        SceneManager.LoadScene(checkPointSystem.GetCheckPoint(), LoadSceneMode.Single);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void PauseMenuCalled()
    {
        Time.timeScale = 0;
    }

    public void SetSettingsUIReference(Canvas reference, CheckPointSystem check)
    {
        settingsUI = reference;
        checkPointSystem = check;
    }
}
