using UnityEngine;

public class SaveGame : MonoBehaviour
{
    #region Singleton
    public static SaveGame instance;

    private void Awake()
    {
        //Make sure there is only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    public void SaveCheckpoint(string scene)
    {
        //don't save on puzzle 1
        if (scene.CompareTo("Puzzle1") != 0)
        {
            PlayerPrefs.SetString("scene", scene);
            PlayerPrefs.Save();
        }
    }
    public void DeleteCheckpoint()
    {
        if (PlayerPrefs.GetString("scene", "") != "")
        {
            PlayerPrefs.DeleteKey("scene");
        }
    }
}
