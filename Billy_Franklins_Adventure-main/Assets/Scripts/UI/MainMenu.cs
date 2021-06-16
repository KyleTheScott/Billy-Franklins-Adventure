using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Canvas settingsUI = null;
    public string level = "Puzzle1";

    // Start is called before the first frame update
    private void Awake()
    {
        try
        {
            settingsUI = GameObject.Find("Settings UI").GetComponent<Canvas>();
            settingsUI.gameObject.SetActive(false);
        }
        catch 
        {
            Debug.Log("Couldn't get settings UI...");
        }
    }

    //start/load last saverd puzzle (need saving system)
    public void ContinueGame()
    {
        //temp until saving is implemented...
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    //start/load puzzle1
    public void NewGame()
    {
        //might change later...
        SceneManager.LoadScene(level, LoadSceneMode.Single);
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

    //closes game
    public void ExitGame()
    {
        Application.Quit();
    }

    public void MainMenuUICalled()
    {
        settingsUI.gameObject.SetActive(false);
    }
}
