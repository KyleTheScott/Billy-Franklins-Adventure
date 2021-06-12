using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointSystem : MonoBehaviour
{
    public string checkPoint = "";

    private void Awake()
    {
        //set checkpoint once next level is loaded
        checkPoint = SceneManager.GetActiveScene().name;
    }

    public void PlayerDeath()
    {
        //if player dies then reload scene
        SceneManager.LoadScene(checkPoint, LoadSceneMode.Single);
    }

    public void SetCheckPoint(string sceneName)
    {
        checkPoint = sceneName;
    }
}
