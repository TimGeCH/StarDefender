using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [Header("==== CANVAS ====")]
    [SerializeField] Canvas optionsCanvas;

    [Header("==== BUTTONS ====")]
    [SerializeField] Button backButton;

    //[SerializeField] private Text instructionsText;

    private bool backClicked = false;

    void OnEnable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Add(backButton.gameObject.name, OnBackClicked);
    }

    void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    void Start()
    {
        GameManager.GameState = GameState.Paused;
        UIInput.Instance.SelectUI(backButton);

        //instructionsText.text = "";
    }

    void OnBackClicked()
    {
        backClicked = true;
        SceneLoader.Instance.LoadMainMenuScene(); // Use the SceneLoader to load the Main Menu scene
    }

    void Update()
    {
        if (backClicked)
        {
            GameManager.GameState = GameState.Playing;
        }
    }
}
