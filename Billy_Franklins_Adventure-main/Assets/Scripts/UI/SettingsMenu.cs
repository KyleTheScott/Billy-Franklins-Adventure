using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public Canvas mainMenuUI = null;
    public Canvas pauseMenuUI = null;

    // Start is called before the first frame update
    private void Awake()
    {
        try
        {
            mainMenuUI = FindObjectOfType<MainMenu>().GetComponent<Canvas>();
        }
        catch
        {
            Debug.Log("Couldn't get main menu UI...");
        }
    }
    private void Start()
    {
        Player player = FindObjectOfType<Player>()?.GetComponent<Player>();
        if (player != null)
        {
            player.ReferenceSetingsMenuUI(GetComponent<Canvas>());
        }
    }

    public void Back()
    {
        if (mainMenuUI != null) 
        {
            mainMenuUI.gameObject.SetActive(true);
            mainMenuUI.GetComponent<MainMenu>().MainMenuUICalled();
            FindObjectOfType<MusicController>().PlayMenuSelect();
        }
        else if (pauseMenuUI != null)
        {
            pauseMenuUI.gameObject.SetActive(true);
            pauseMenuUI.GetComponent<PauseMenu>().PauseMenuCalled();
            FindObjectOfType<MusicController>().PlayMenuSelect();
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

    public void SetPauseMenuUIReference(Canvas reference)
    {
        pauseMenuUI = reference;

    }
}
