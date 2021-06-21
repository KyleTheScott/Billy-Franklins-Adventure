using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointSystem : MonoBehaviour
{
    public string checkPointScene = "";
    private bool playerDied;

    private void Awake()
    {
        //set checkpoint once next level is loaded
        checkPointScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Start()
    {
    }

    public void PlayerDeath()
    {
        Debug.Log("Died");
        //if player dies then reload scene
        Player player = GameObject.FindObjectOfType<Player>().GetComponent<Player>();
        player.SetLampOn(false);
        playerDied = true;
        SceneManager.LoadScene(checkPointScene, LoadSceneMode.Single);
    }

    public void SetCheckPoint(string sceneName)
    {
        checkPointScene = sceneName;
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
            FindObjectOfType<Player>().transform.position = FindObjectOfType<PuzzleController>().PlayerSpawnPoint.position;
            playerDied = false;

        }
    }
}
