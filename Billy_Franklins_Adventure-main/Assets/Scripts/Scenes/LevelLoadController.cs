using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoadController : MonoBehaviour
{
    [SerializeField] private string next_scene_to_load_ = "";
    [SerializeField] private string prev_scene_to_destroy_ = "";
    [SerializeField] private GameObject next_scene_position_ = null;
    [SerializeField] private int players_new_max_charge = 2;
    [Tooltip("Gizmo helper color, has no effect on code logic")]
    [SerializeField] private Vector4 gizmo_color_ = new Vector4(0, 1, 1, 0.75f);
    [Tooltip("Gizmo helper radius, has no effect on code logic")]
    [SerializeField] private float gizmo_radius_ = 0.4f;
    [SerializeField] private GhostWallController ghostWall;
    [SerializeField] private bool LowerWallOnTriggerEnter = true;
    [SerializeField] private PuzzleController puzzleController;
    [SerializeField] bool InstantLoadLevel = false;
    private Player player;


    private bool has_level_loaded_ = false;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    IEnumerator LoadNextLevel()
    {
        Debug.LogError("Loading");
        
        FindObjectOfType<ObjectsCollision>().EmptyObjects();
        
        AsyncOperation load_scene_op = SceneManager.LoadSceneAsync(next_scene_to_load_, LoadSceneMode.Additive);
        


        while (!load_scene_op.isDone)
        {
            Debug.Log(">>> Loading next scene: " + next_scene_to_load_);
            yield return null;
        }

        if (next_scene_position_ != null)
        {
            player.SetPlayerInLevel(false);
            Debug.Log("The next scene position is set");
            GameObject[] root_level_objs = GameObject.FindGameObjectsWithTag("LevelRoot");

            foreach (GameObject root_level in root_level_objs)
            {
                Debug.Log(root_level.name);
                if (root_level.name == ("root_" + next_scene_to_load_))
                {
                    Debug.Log(">>> root_level is:");
                    Debug.Log(root_level);
                    root_level.transform.position = next_scene_position_.transform.position;
                    break;
                }
            }
                
        }

        //if (prev_scene_to_destroy_ == null)
        //{
        //    player.SetPlayerState(Player.PlayerState.LIGHTNING_CHARGES_START);
        //    player.SetAnimationMovement(true);
        //}

        GlobalGameController.instance.GetComponent<CheckPointSystem>().SetCheckPoint(next_scene_to_load_);
        FindObjectOfType<Charges>().GetComponent<Charges>().SetLampOn(false);
        has_level_loaded_ = true;
       
    }

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (next_scene_to_load_ != "" && !has_level_loaded_)
            {
                if (InstantLoadLevel)
                {
                    Debug.LogError("New Level Load");
                    DestroyDDOLObjects();
                    SceneManager.LoadScene(next_scene_to_load_, LoadSceneMode.Single);
                    GameplayUI.instanceGameplayUI.FadeIn();
                    has_level_loaded_ = true;
                }
                else
                {
                    StartCoroutine("LoadNextLevel");
                }
            }
            else
            {
                //Debug.Log("loadMainMenu= " + loadMainMenu);
                //if (loadMainMenu == true)
                //{
                //    Debug.Log("MainMenu Instant Load");
                //    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                //}
                //else
                //{
                //    Debug.LogWarning(">>> Cannot load next scene!");
                //    loadMainMenu = true;
                //    Debug.Log("loadMainMenu = true automatically");
                //}
            }
               
            if (prev_scene_to_destroy_ != "" && SceneManager.GetSceneByName(prev_scene_to_destroy_).isLoaded)
            {
                
                SceneManager.UnloadSceneAsync(prev_scene_to_destroy_);

                //player.SetPlayerKiteLightning();
                GameplayUI.instanceGameplayUI.FadeIn();
                ElectricityController.instanceElectrical.ConnectConnectedObjects();
                //Debug.LogError("connect connected 0");
            }
            else
            {
                GameplayUI.instanceGameplayUI.FadeIn();
            }

            if (LowerWallOnTriggerEnter)
            {
                if (puzzleController != null)
                {
                    if (!player.GetPlayerInLevel())
                    {
                        player.SetPlayerInLevel(true);
                       
                        //player.SetPlayerKiteLightning();
                    }
                    puzzleController.LoadPuzzle();
                }
                
                if (ghostWall != null)
                {
                    ghostWall.LowerGhostWall();
                    gameObject.SetActive(false);
                }
                
            }
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

    void OnDrawGizmos()
    {
        // Draw a green sphere at the transform's position
        Gizmos.color = gizmo_color_;
        Gizmos.DrawWireSphere(transform.position, gizmo_radius_);
    }
}
