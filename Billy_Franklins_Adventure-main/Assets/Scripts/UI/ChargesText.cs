using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChargesText : MonoBehaviour
{
    Player player = null;
    private Charges charges = null;
    TextMeshProUGUI text = null;

    // Start is called before the first frame update
    void Start()
    {
        //Get player
        player = FindObjectOfType<Player>();
        charges = FindObjectOfType<Charges>();
        if (player != null)
        {
            charges.onLightChargesChanged.AddListener(OnLightChargesCallback);
        }
        else
        {
            Debug.LogWarning("Couldn't find player, Check DarkBorder");
        }
        


        //Get text
        text = GetComponent<TextMeshProUGUI>();
        OnLightChargesCallback(charges.GetLightCharges(), charges.GetMaxLightCharges());
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
