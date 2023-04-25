using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Localization.Settings;

public class MainMenu : MonoBehaviour
{
    public Button loadGameButton;
    [SerializeField]
    private GameObject panel;

    [Header("Yes No Prompt")]
    public YesNoPromptCustom yesNoPromptCustom;

    // Start is called before the first frame update
    void Start()
    {
        string playerName = PlayerPrefs.GetString("PlayerName");
        if (string.IsNullOrEmpty(playerName))
        {
            panel.SetActive(true);
        }
        loadGameButton.interactable = SaveManager.HasSave();
    }

    public void NewGame()
    {
        /*SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);*/
        /*  SceneManager.LoadScene(SceneTransitionManager.Location.PlayerHome.ToString());*/
        StartCoroutine(LoadGameAsync(SceneTransitionManager.Location.PlayerHome, null));
    }
    public void Continue()
    {
        StartCoroutine(LoadGameAsync(SceneTransitionManager.Location.PlayerHome, LoadGame));
    }
    public void Setting()
    {

    }

    void LoadGame()
    {
        if(GameStateManager.Instance == null)
        {
            Debug.LogError("Cannot find Game State Manager");
            return;
        }
        GameStateManager.Instance.LoadSave();
    }

    public void Quit()
    {
        string text = LocalizationSettings.StringDatabase.GetLocalizedString("LanguageTable", "ExitGameKey");

        TriggerYesNoPromptCustom(text, QuitGameOption);
    }

    void QuitGameOption() 
    {
        Application.Quit();
    }



    IEnumerator LoadGameAsync(SceneTransitionManager.Location scene, Action onFirstFrameLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.ToString());
        DontDestroyOnLoad(gameObject);
        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log("Loading!");
        }

        //Scene load
        Debug.Log("Loaded!");

        yield return new WaitForEndOfFrame();
        Debug.Log("First frame is loaded");
        onFirstFrameLoad?.Invoke();

        Destroy(gameObject);
    }

    public void MoveToStartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void TriggerYesNoPromptCustom(string message, System.Action onYesCallback)
    {
        //Set active the gameObject of the Yes No Prompt
        yesNoPromptCustom.gameObject.SetActive(true);

        yesNoPromptCustom.CreatePrompt(message, onYesCallback);

        StartCoroutine(DisableAfterDelay(5.0f));
    }

    IEnumerator DisableAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        yesNoPromptCustom.gameObject.SetActive(false);
    }
}
