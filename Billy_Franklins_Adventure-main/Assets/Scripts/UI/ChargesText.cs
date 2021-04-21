using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChargesText : MonoBehaviour
{
    Player player = null;
    TextMeshProUGUI text = null;

    // Start is called before the first frame update
    void Start()
    {
        //Get player
        player = FindObjectOfType<Player>();

        if (player != null)
        {
            player.onLightChargesChanged.AddListener(OnLightChargesCallback);
        }
        else
        {
            Debug.LogWarning("Couldn't find player, Check DarkBorder");
        }



        //Get text
        text = GetComponent<TextMeshProUGUI>();

        OnLightChargesCallback(player.lightCharges, player.maxLightCharges);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void OnLightChargesCallback(int lightCharges, int maxLightCharges)
    {
        if (text != null)
        {
            text.text = "Charges: " + lightCharges.ToString() + " / " + maxLightCharges.ToString();
        }
    }
}
