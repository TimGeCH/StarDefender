using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StorySenceUIController : MonoBehaviour
{
    [Header("==== CANVAS ====")]
    [SerializeField] Canvas storyCanvas;

    [Header("==== BUTTONS ====")]
    [SerializeField] Button continueButton;

    [SerializeField] private Text storyText;
    [SerializeField] private float typingSpeed = 0.1f;
    private string story;

    private bool continueClicked = false;

    void OnEnable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Add(continueButton.gameObject.name, OnContinueClicked);
    }

    void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    void Start()
    {
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Playing;
        UIInput.Instance.SelectUI(continueButton);

        story = "In the distant reaches of the cosmos, an insidious threat stirs. Earth, the last bastion of humanity, faces an onslaught of innumerable alien invaders. Our defenses have been shattered, our hopes, dwindling. As the world teeters on the brink of extinction, all eyes turn to a single individual - the final ace pilot, the Star Defender.\r\n\r\nGifted with unmatched skill and courage, the Star Defender commands the last fighter craft capable of repelling the invaders. Their mission - to hold the line against an endless tide of adversaries, fighting until the last bullet is fired and the last drop of blood is shed.\r\n\r\nThe odds may be against us, but the Star Defender is our beacon of hope. Fight valiantly, Star Defender, for as long as you can stand, humanity will stand with you. It's a battle against the infinite, a test of our indomitable spirit. We will fight. We will survive. To the stars we look, to the stars we shall prevail !\r\n";
        StartCoroutine(TypeSentence(story));
    }

    IEnumerator TypeSentence(string sentence)
    {
        storyText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            storyText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
            if (continueClicked)
            {
                break; // If the continue button was clicked, stop typing
            }
        }
    }

    void OnContinueClicked()
    {
        continueClicked = true;
        SceneLoader.Instance.LoadGamePlayScene(); // Use the SceneLoader to load the gameplay scene
    }
}
