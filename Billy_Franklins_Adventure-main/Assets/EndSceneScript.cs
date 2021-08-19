using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    LevelLoadController controller;
    [SerializeField]
    private float endSceneWaitTime = 5.0f;
    private bool isTriggered = false;

    private void Start()
    {
        controller.gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            Player player = FindObjectOfType<Player>();
            GameplayUI.instanceGameplayUI.FadeOut();
            player.SetAnimationMovement(false);
            player.PlayerControlsStatus(false);
            StartCoroutine(DisplayEndScene());
        }
        
    }

    private IEnumerator DisplayEndScene()
    {
        yield return new WaitForSeconds(1.0f);
        GameplayUI.instanceGameplayUI.DisplayEndText(true);
        GameplayUI.instanceGameplayUI.FadeInText();
        yield return new WaitForSeconds(endSceneWaitTime);
        controller.gameObject.SetActive(true);
    }
}
