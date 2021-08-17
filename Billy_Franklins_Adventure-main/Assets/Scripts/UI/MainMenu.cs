using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private LoadingMenu loadingUI = null;
    [SerializeField]
    private Canvas settingsUI = null;
    [SerializeField]
    private CreditsUI creditsUI = null;
    public string level = "Puzzle1";
    private string save = "";

    private void Awake()
    {
        try
        {
            settingsUI.gameObject.SetActive(false);
            loadingUI.gameObject.SetActive(false);
            creditsUI.gameObject.SetActive(false);

        }
        catch 
        {
            Debug.Log("Couldn't get settings UI...");
        }
    }

    private void Start()
    {
        DestroyDDOLObjects();
        //try to get save (if it exists)
        save = PlayerPrefs.GetString("scene", "");

        //if save exists then show continue game
        if (save.CompareTo("") == 0)
        {
           Button button = GameObject.Find("ContinueGame Button").GetComponent<Button>();
           button.interactable = false;
           button.image.color = Color.grey;
        }

        if (PlayerPrefs.GetInt("LoadCredits", 1) == 1)
        {
            PlayerPrefs.SetInt("LoadCredits", 0);
            Credits();
        }
    }

    private void DestroyDDOLObjects()
    {
        GameObject.Find("Player(Clone)").GetComponent<Player>().DestroyUI();
        Destroy(GameObject.Find("Player(Clone)"));
        Destroy(GameObject.Find("Main Camera(Clone)"));
        Destroy(GameObject.Find("ControllerManager(Clone)"));
        //Destroy(GameObject.Find("Settings UI(Clone)"));
        //Destroy(GameObject.Find("Pause Menu UI(Clone)"));
        Destroy(GameObject.Find("Global Light 2D(Clone)"));
        Destroy(GameObject.Find("UI(Clone)"));

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

    public void Credits()
    {
        creditsUI?.gameObject.SetActive(true);
    }

    //closes game
    public void ExitGame()
    {
        Application.Quit();
    }

    public void MainMenuUICalled()
    {
        settingsUI.gameObject.SetActive(false);
        creditsUI.gameObject.SetActive(false);
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
            //float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //loadingUI.slider.value = progress;
            //loadingUI.progressText.text = progress * 100.0f + "%";
            yield return null;

        }
    }
}
