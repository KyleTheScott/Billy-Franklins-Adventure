using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private LoadingMenu loadingUI = null;
    private Canvas settingsUI = null;
    public string level = "Puzzle1";

    // Start is called before the first frame update
    private void Awake()
    {
        try
        {
            settingsUI = GameObject.Find("Settings UI").GetComponent<Canvas>();
            settingsUI.gameObject.SetActive(false);
            loadingUI = GameObject.Find("Loading UI").GetComponent<LoadingMenu>();
            loadingUI.gameObject.SetActive(false);
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
        LoadLevel(level);
    }

    //start/load puzzle1
    public void NewGame()
    {
        //might change later...
        LoadLevel(level);
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

    public void LoadLevel (string sceneName)
    {
        if (loadingUI != null)
        {
            loadingUI.gameObject.SetActive(true);
            StartCoroutine(LoadAsync(sceneName));
        }
        else
        {
            SceneManager.LoadScene(level, LoadSceneMode.Single);
        }
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingUI.slider.value = progress;
            loadingUI.progressText.text = progress * 100.0f + "%";
            yield return null;

        }
    }
}
