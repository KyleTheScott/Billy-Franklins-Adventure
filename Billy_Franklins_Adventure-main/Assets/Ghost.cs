using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private SpriteRenderer ghostSprite;
    [SerializeField] private float dissipationRate = 0.01f;
    [SerializeField] private float currentTransparency = 1f;
    private bool dissipatedByLantern = false;

    public enum GhostState
    {
        APPEARED,
        APPEARING,
        DISSIPATED,
        DISSIPATING
    }
    [SerializeField] private GhostState ghostState = GhostState.APPEARED;


    // Start is called before the first frame update
    void Start()
    {
        ghostSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ghostState == GhostState.DISSIPATING)
        {
            DissipateGhost();
        }
        else if (ghostState == GhostState.APPEARING)
        {
            GhostAppearing();
        }
    }

    public void SetDissipatedByLantern(bool state)
    {
        dissipatedByLantern = state;
    }

    public bool GetDissipatedByLantern()
    {
        return dissipatedByLantern;
    }

    public void SetGhostDissipation()
    {
        Debug.Log("Dissipate ghost");
        ghostState = GhostState.DISSIPATING;
    }

    public void DissipateGhost()
    {
        Debug.Log("Dissipating ghost");
        currentTransparency -= dissipationRate;
        ghostSprite.color = new Color(1.0f, 1.0f, 1.0f, currentTransparency);
        if (currentTransparency <= 0)
        {
            Debug.Log("Dissipated ghost");
            ghostState = GhostState.DISSIPATED;
        }
    }

    public void SetGhostAppearing()
    {
        Debug.Log("ghost appear");
        ghostState = GhostState.APPEARING;
    }

    public void GhostAppearing()
    {
        Debug.Log("ghost appearing");
        currentTransparency += dissipationRate;
        ghostSprite.color = new Color(1.0f, 1.0f, 1.0f, currentTransparency);
        if (currentTransparency >= 1)
        {
            Debug.Log("ghost appeared");
            ghostState = GhostState.APPEARED;
        }
    }

}
