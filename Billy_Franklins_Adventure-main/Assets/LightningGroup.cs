using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LightningGroup : MonoBehaviour
{
    [SerializeField] private List<BackgroundLightning> lightningGroup = new List<BackgroundLightning>();
    // Start is called before the first frame update
    void Start()
    { 
        foreach (Transform l in transform)
        {
            lightningGroup.Add(l.GetComponent<BackgroundLightning>());
        }
    }

    public void StrikeLightning()
    {
        int lightningNum = Random.Range(0, lightningGroup.Count);
        lightningGroup[lightningNum].StartStriking();
    }
}
