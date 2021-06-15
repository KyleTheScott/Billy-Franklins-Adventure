using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private Canvas mainMenuUI = null;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            mainMenuUI = GameObject.Find("Main Menu UI").GetComponent<Canvas>();
        }
        catch
        {
            Debug.Log("Couldn't get settings UI...");
        }
    }

    public void Back()
    {
        if (mainMenuUI != null) 
        {
            mainMenuUI.gameObject.SetActive(true);
            mainMenuUI.GetComponent<StartMenu>().SettingsUICalled();
        }
    }

    public void StartMenuUICalled()
    {
        mainMenuUI.gameObject.SetActive(false);
    }
}
