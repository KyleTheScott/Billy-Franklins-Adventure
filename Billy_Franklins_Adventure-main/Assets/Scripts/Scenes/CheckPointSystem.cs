using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointSystem : MonoBehaviour
{
    public string checkPointScene = "";
    private bool playerDied;
    private Shooting shooting;

    private void Awake()
    {
        //set checkpoint once next level is loaded
        checkPointScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
        shooting = FindObjectOfType<Shooting>();
    }

    public void Start()
    {
    }

    public void PlayerDeath()
    {
        Debug.Log("Died");
        //if player dies then reload scene
        //Player player = GameObject.FindObjectOfType<Player>().GetComponent<Player>();
        Charges charges = FindObjectOfType<Charges>();
        charges.SetLampOn(false);
        playerDied = true;
        GameplayUI.instanceGameplayUI.ResetFadeOut();
        SceneManager.LoadScene(checkPointScene, LoadSceneMode.Single);
        FindObjectOfType<Player>().SetMovingRight(true);
       

    }

    public void SetCheckPoint(string sceneName)
    {
        checkPointScene = sceneName;
        Debug.Log("SetCheckpoint: " + sceneName);

        //save checkpoint
        SaveGame.instance.SaveCheckpoint(sceneName);
    }

    public string GetCheckPoint()
    {
        return checkPointScene;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SceneLoaded");
        if (playerDied)
        {
            Debug.Log("DEATH");
            FindObjectOfType<Player>().transform.position = FindObjectOfType<PuzzleController>().PlayerSpawnPoint.position;
            FindObjectOfType<PuzzleController>().LoadPuzzle();
            FindObjectOfType<Player>().SetPlayerKiteLightning();
            playerDied = false;
        }
    }
}
