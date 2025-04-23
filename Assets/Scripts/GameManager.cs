using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Levels are not changed to only have 1 recipient
    // because of this we just need to search for the only one
    // existance of this recipient and do our thing when needed

    // This script will not be kept on load please
    // I dont want to handle that

    [Header("General References")]
    [SerializeField] Material renderMaterial;

    [Header("Level References")]
    [SerializeField] int level = 0;

    public static GameManager instance;

    private bool restarting = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LevelStartShader(1f);
    }

    public void restartLevel()
    {
        if (restarting) return;
        restarting = true;

        LevelEndShader(.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        asyncLoad.allowSceneActivation = false;

        DOVirtual.DelayedCall(.6f, () =>
        {
            asyncLoad.allowSceneActivation = true;
        });
    }

    public void TriggerLevelEnd()
    {
        LevelEndShader(2f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        asyncLoad.allowSceneActivation = false;

        DOVirtual.DelayedCall(2.3f, () =>
        {
            asyncLoad.allowSceneActivation = true;
        });
    }


    void LevelStartShader(float duration)
    {
        // Fade Amount
        DOTween.To(
            () => renderMaterial.GetFloat("_FadeAmount"),
            x => renderMaterial.SetFloat("_FadeAmount", x),
            .0f,
            duration
        );

        // Trippy wave
        DOTween.To(
            () => renderMaterial.GetFloat("_TrippyPower"),
            x => renderMaterial.SetFloat("_TrippyPower", x),
            .0f,
            duration
        );

        // Pixelation
        DOTween.To(
            () => renderMaterial.GetFloat("_PixelPower"),
            x => renderMaterial.SetFloat("_PixelPower", x),
            .0f,
            duration
        );

        // Color LSD lol
        DOTween.To(
            () => renderMaterial.GetFloat("_ColorStreaks"),
            x => renderMaterial.SetFloat("_ColorStreaks", x),
            .0f,
            duration
        );
    }

    void LevelEndShader(float duration)
    {
        // Fade Amount
        DOTween.To(
            () => renderMaterial.GetFloat("_FadeAmount"),
            x => renderMaterial.SetFloat("_FadeAmount", x),
            1f,
            duration
        ).SetDelay(0.3f);

        // Trippy wave
        DOTween.To(
            () => renderMaterial.GetFloat("_TrippyPower"),
            x => renderMaterial.SetFloat("_TrippyPower", x),
            .7f,
            duration
        );

        // Pixelation
        DOTween.To(
            () => renderMaterial.GetFloat("_PixelPower"),
            x => renderMaterial.SetFloat("_PixelPower", x),
            .9f,
            duration
        );

        // Color LSD lol
        DOTween.To(
            () => renderMaterial.GetFloat("_ColorStreaks"),
            x => renderMaterial.SetFloat("_ColorStreaks", x),
            .9f,
            duration
        );
    }
}
