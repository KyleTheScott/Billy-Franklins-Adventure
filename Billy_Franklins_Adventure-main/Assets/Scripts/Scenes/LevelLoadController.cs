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
    [SerializeField] private bool RaiseWallOnTriggerEnter = true;
                                

    private bool has_level_loaded_ = false;
    IEnumerator LoadNextLevel()
    {
        AsyncOperation load_scene_op = SceneManager.LoadSceneAsync(next_scene_to_load_, LoadSceneMode.Additive);
        while (!load_scene_op.isDone)
        {
            Debug.Log(">>> Loading next scene: " + next_scene_to_load_);
            yield return null;
        }

        if (next_scene_position_ != null)
        {
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

        GlobalGameController.instance.GetComponent<CheckPointSystem>().SetCheckPoint(next_scene_to_load_);
        GameObject.Find("Player(Clone)").GetComponent<Player>().SetLampOn(false);
        has_level_loaded_ = true;
    }

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (next_scene_to_load_ != "" && !has_level_loaded_)
            {
                StartCoroutine("LoadNextLevel");
            }
            else
            {
                Debug.LogWarning(">>> Cannot load next scene!");
            }

            if (prev_scene_to_destroy_ != "" && SceneManager.GetSceneByName(prev_scene_to_destroy_).isLoaded)
            {
                SceneManager.UnloadSceneAsync(prev_scene_to_destroy_);
               
            }

            if (RaiseWallOnTriggerEnter)
            {
                if (ghostWall != null)
                {
                    ghostWall.RaiseGhostWall();
                }

            }
        }
        else
        {
            Debug.LogWarning(">>> Not collided with Player!");
        }
    }


    void OnDrawGizmos()
    {
        // Draw a green sphere at the transform's position
        Gizmos.color = gizmo_color_;
        Gizmos.DrawWireSphere(transform.position, gizmo_radius_);
    }
}
