using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Rain : MonoBehaviour
{
    private ParticleSystem rainParticles;

    // Start is called before the first frame update
    void Start()
    {
        WindController.instance.BlowWindEvent.AddListener(BlowWindRain);
        WindController.instance.CalmWindEvent.AddListener(CalmWindRain);
        RainController.instance.RainEvent.AddListener(MakeItRain);
        RainController.instance.CalmRainEvent.AddListener(CalmRain);

        rainParticles = gameObject.GetComponent<ParticleSystem>();
        BlowWindRain();
    }

    public void BlowWindRain()
    {
        var vel = rainParticles.forceOverLifetime;
        vel.enabled = true;
        vel.xMultiplier = WindController.instance.GetWindSpeed();
        SetRainForce_Angle();
    }
    public void CalmWindRain()
    {
        var vel = rainParticles.forceOverLifetime;
        vel.enabled = true;
        vel.xMultiplier = WindController.instance.GetWindSpeed();
        SetRainForce_Angle();
    }

    private void MakeItRain()
    {
       
        var vel = rainParticles.emission;
        vel.enabled = true;
        vel.rateOverTime = RainController.instance.GetRainAmount();
    }
    private void CalmRain()
    {
        var vel = rainParticles.emission;
        vel.enabled = true;
        vel.rateOverTime = RainController.instance.GetRainAmount();
    }

    private void SetRainForce_Angle()
    {
        var forceOverLifetime = rainParticles.forceOverLifetime;
        forceOverLifetime.enabled = true;
        var rotationBySpeed = rainParticles.rotationBySpeed;
        rotationBySpeed.enabled = true;
        float windSpeed = WindController.instance.GetWindSpeed();

        rotationBySpeed.z = windSpeed * 0.1f;
        forceOverLifetime.x = windSpeed * 1.5f;
    }

}
