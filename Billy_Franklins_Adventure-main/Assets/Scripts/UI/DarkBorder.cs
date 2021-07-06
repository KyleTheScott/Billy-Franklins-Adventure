using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkBorder : MonoBehaviour
{
    Image darkBorderImage = null; 

    // Start is called before the first frame update
    void Start()
    {
        //Get player
        Charges charges = FindObjectOfType<Charges>();

        if(charges != null)
        {
            charges.onLightChargesChanged.AddListener(OnLightChargesCallback);
        }
        else
        {
            Debug.LogWarning("Couldn't find player, Check DarkBorder");
        }

        //Get lamp
        Lamp lamp = FindObjectOfType<Lamp>();
        if(lamp != null)
        {
            lamp.onLampOn.AddListener(OnLampOnCallback);
        }
        else
        {
            Debug.LogWarning("Couldn't find lamp, Check DarkBorder");
        }


        //Get image
        darkBorderImage = GetComponent<Image>();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void OnLightChargesCallback(int lightCharges, int maxLightCharges)
    {
        if (darkBorderImage != null)
        {
            //Color newColor = darkBorderImage.color;
            
            //Set alpha based on charges left
            //newColor.a = 1 - ((float)lightCharges / (float)maxLightCharges);

           // darkBorderImage.color = newColor;
        }
    }

    void OnLampOnCallback()
    {
        if (darkBorderImage != null)
        {
            Debug.Log("Lamp on call back");
            Color newColor = darkBorderImage.color;

            //Set alpha based on charges left
            newColor.a = 0;

            darkBorderImage.color = newColor;
        }
    }
}
