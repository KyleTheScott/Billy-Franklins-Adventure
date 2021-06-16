using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private Canvas mainMenuUI = null;
    private Canvas pauseMenuUI = null;

    // Start is called before the first frame update
    private void Awake()
    {
        try
        {
            mainMenuUI = GameObject.Find("Main Menu UI").GetComponent<Canvas>();
            pauseMenuUI = GameObject.Find("Pause Menu UI").GetComponent<Canvas>();
        }
        catch
        {
            Debug.Log("Couldn't get main menu/pause menu UI...");
        }
    }

    public void Back()
    {
        if (mainMenuUI != null) 
        {
            mainMenuUI.gameObject.SetActive(true);
            mainMenuUI.GetComponent<MainMenu>().MainMenuUICalled();
        }
    }

    public void SettingsUICalled()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.gameObject.SetActive(false);
        }
        else if (pauseMenuUI != null)
        {
            pauseMenuUI.gameObject.SetActive(false);
        }
    }
}
