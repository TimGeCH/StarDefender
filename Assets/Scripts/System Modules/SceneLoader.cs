using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistenSingleton<SceneLoader>
{
    [SerializeField] Image transitionImage;
    [SerializeField] float fadeTime = 1;

    Color color;
    const string GAMEPLAY = "Gameplay";
    const string MAIN_MENU = "MainMenu";
    const string OPTIONS_MENU = "OptionsMenu";
    const string STORY_SCENE = "StoryScene";
    const String SCORING = "Scoring";


    void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadingCorouting(string sceneName)
    {
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;

        transitionImage.gameObject.SetActive(true);

        while (color.a < 1f)
        {
            color.a = Mathf.Clamp01(color.a += Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);

        loadingOperation.allowSceneActivation = true;

        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }
        transitionImage.gameObject.SetActive(false);
    }

    public void ResetProjectileDamage()
    {
        PlayerProjectile.ResetBaseDamage();
    }

    public void LoadGamePlayScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCorouting(GAMEPLAY));
    }

    public void LoadMainMenuScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCorouting(MAIN_MENU));
    }

    public void LoadOptionsMenu()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCorouting(OPTIONS_MENU));
    }

    public void LoadStoryScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCorouting(STORY_SCENE));
    }

    internal void LoadScoringScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCorouting(SCORING));
    }
}
