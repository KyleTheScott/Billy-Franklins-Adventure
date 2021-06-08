using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    [SerializeField]
    private int players_new_max_charge = 1;

    // Start is called before the first frame update
    void Start()
    {
        LoadPlayersLightCharged();  
    }


    private void LoadPlayersLightCharged()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.maxLightCharges = players_new_max_charge;
            player.lightCharges = players_new_max_charge;
            player.onLightChargesChanged.Invoke(player.lightCharges, player.maxLightCharges);
        }
        else
        {
            Debug.Log("Player was not found");
        }
    }
}
