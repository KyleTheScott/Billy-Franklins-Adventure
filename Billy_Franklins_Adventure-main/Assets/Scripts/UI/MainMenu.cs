using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private LoadingMenu loadingUI = null;
    private Canvas settingsUI = null;
    public string level = "Puzzle1";
    private string save = "";

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

    private void Start()
    {
        //try to get save (if it exists)
        save = PlayerPrefs.GetString("scene", "");

        //if save exists then show continue game
        if (save.CompareTo("") == 0)
        {
           Button button = GameObject.Find("ContinueGame Button").GetComponent<Button>();
           button.interactable = false;
           button.image.color = Color.grey;
        }
    }

    //load last saved puzzle
    public void ContinueGame()
    {
        LoadLevel(save);     
    }

    //start/load puzzle1
    public void NewGame()
    {
        //Delete old save
        PlayerPrefs.DeleteKey("scene");

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
