using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    [FMODUnity.EventRef]
    private string gameEventRef;
    [SerializeField]
    [FMODUnity.EventRef]
    private string menuSelectEventRef;
    [SerializeField]
    [FMODUnity.EventRef]
    private string menuTapEventRef;
    [SerializeField]
    private float playMusic;
    [SerializeField]
    private float menuMusic;
    [SerializeField]
    private float musicVolume = 0.8f;
    [SerializeField]
    private float menuSelectVolume = 0.8f;
    [SerializeField]
    private float menuTapVolume = 0.8f;
    private FMOD.Studio.EventInstance gameMusicSoundEvent;
    public void Start()
    {
        gameMusicSoundEvent = FMODUnity.RuntimeManager.CreateInstance(gameEventRef);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(gameMusicSoundEvent, transform, GetComponent<Rigidbody>());
        gameMusicSoundEvent.setVolume(musicVolume);
        gameMusicSoundEvent.setParameterByName("Menu Music", menuMusic);
        gameMusicSoundEvent.setParameterByName("Play Music", playMusic);
        gameMusicSoundEvent.start();
    }

    public void PlayMenuTap()
    {
        FMODUnity.RuntimeManager.PlayOneShot(menuTapEventRef, menuTapVolume);
    }

    public void PlayMenuSelect()
    {
        FMODUnity.RuntimeManager.PlayOneShot(menuSelectEventRef, menuSelectVolume);
    }

    public void OnDestroy()
    {
        gameMusicSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
