using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private SpriteRenderer ghostSprite;
    [SerializeField] private float dissipationRate = 0.01f;
    private float currentTransparency;
    [SerializeField] private float maxTransparency = 1f;
    [SerializeField] private float minTransparency = 0f;
    [SerializeField] private bool dissipatedByLantern = false;

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
        ghostSprite.color = new Color(1.0f, 1.0f, 1.0f, maxTransparency);
        currentTransparency = maxTransparency;
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

    public void SetDissipationRate(float rate)
    {
        dissipationRate = rate;
    }

    public float GetDissipationRate()
    {
        return dissipationRate;
    }

    public void SetGhostDissipation()
    {
        ghostState = GhostState.DISSIPATING;
    }

    public void DissipateGhost()
    {
        currentTransparency -= dissipationRate;
        ghostSprite.color = new Color(1.0f, 1.0f, 1.0f, currentTransparency);
        if (currentTransparency <= minTransparency)
        {
            ghostState = GhostState.DISSIPATED;
        }
    }

    public void SetGhostAppearing()
    {
        ghostState = GhostState.APPEARING;
    }

    public void GhostAppearing()
    {
        currentTransparency += dissipationRate;
        ghostSprite.color = new Color(1.0f, 1.0f, 1.0f, currentTransparency);
        if (currentTransparency >= maxTransparency)
        {
            ghostState = GhostState.APPEARED;
        }
    }

}
