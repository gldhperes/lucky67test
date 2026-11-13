
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [Header("Sequence Settings")]
    [SerializeField, Min(0.01f)]
    private float sequenceSpeed = 1f;

    [Header("Opening Crawl")]
    [SerializeField] private GameObject crawlPanel;
    [SerializeField] private RectTransform crawlContent;
    [SerializeField] private TMP_Text crawlText;

    [SerializeField] private float crawlDuration = 18f;
    [SerializeField] private Vector2 crawlStartPosition = new Vector2(0f, -700f);
    [SerializeField] private Vector2 crawlEndPosition = new Vector2(0f, 1200f);

    [TextArea(8, 20)]
    [SerializeField]
    private string openingCrawl =
    "Before the collapse, humanity believed that the number 67 was a symbol of luck, power, and prosperity.\n\n" +
    "Today, little remains of the humanity we once knew.\n\n" +
    "Unknown creatures have taken over the world, and military forces are on the verge of extinction.\n\n" +
    "In one final attempt at survival, a prototype weapon capable of containing these threats was developed.\n\n" +
    "Dozens of volunteers participated in the tests. None of them were successful.\n\n" +
    "Now, only one final attempt remains.\n\n" +
    "TEST No. 67.";

    [Header("NPC Dialogue")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private float letterDelay = 0.035f;

    [TextArea(2, 4)]
    [SerializeField]
    private string[] dialogueLines;

    [Header("Skip UI")]
    [SerializeField] private GameObject skipPanel;

    private bool isPlaying;

    private readonly string[] defaultDialogueLines =
    {
        "Can you hear me, volunteer?",
        "This is Test Number 67. Our last hope.",
        "You have exactly 67 seconds to contain the creatures.",
        "Every 6.7 seconds, your weapon will receive a new modification. But be careful... it's still unstable. It could give you an advantage or compromise the entire test.",
        "We have no more prototypes. No more volunteers.",
        "The future of humanity depends on you. Good luck.",
        "INITIATING TEST 67..."
    };

    private void Awake()
    {
        if (crawlPanel != null)
            crawlPanel.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        SetSkipPanel(false);
    }

    public void PlayIntroduction()
    {
        if (isPlaying)
            return;

        isPlaying = true;
        crawlText.text = openingCrawl;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        Debug.Log("[DialogueController] Starting introduction.");

        // Initial panel state
        SetSkipPanel(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (crawlPanel != null)
            crawlPanel.SetActive(true);

        // STEP 1: OPENING CRAWL
        if (crawlPanel != null && crawlContent != null && crawlText != null)
        {
            crawlText.text = openingCrawl;
            crawlContent.anchoredPosition = crawlStartPosition;

            float elapsed = 0f;

            while (elapsed < crawlDuration)
            {
                elapsed += Time.unscaledDeltaTime * sequenceSpeed;

                float progress = Mathf.Clamp01(elapsed / crawlDuration);

                crawlContent.anchoredPosition = Vector2.Lerp(
                    crawlStartPosition,
                    crawlEndPosition,
                    progress
                );

                yield return null;
            }

            crawlContent.anchoredPosition = crawlEndPosition;

            // The player can now skip the opening crawl.
            SetSkipPanel(true);

            yield return StartCoroutine(WaitForLeftClick());

            SetSkipPanel(false);
        }
        else
        {
            Debug.LogWarning(
                "[DialogueController] Opening Crawl references are incomplete."
            );
        }

        // STEP 2: TRANSITION TO NPC DIALOGUE
        if (crawlPanel != null)
            crawlPanel.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (dialogueText == null)
        {
            Debug.LogError("[DialogueController] DialogueText is missing!");

            FinishIntroduction();
            yield break;
        }

        string[] linesToPlay =
            dialogueLines != null && dialogueLines.Length > 0
                ? dialogueLines
                : defaultDialogueLines;

        Debug.Log(
            $"[DialogueController] Total dialogue lines: {linesToPlay.Length}"
        );

        // The player can interact throughout the NPC dialogue.
        SetSkipPanel(true);

        // STEP 3: NPC DIALOGUE
        foreach (string line in linesToPlay)
        {
            yield return StartCoroutine(TypeLine(line));

            yield return StartCoroutine(WaitForLeftClick());
        }

        FinishIntroduction();
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";

        int characterIndex = 0;
        float timer = 0f;

        float adjustedLetterDelay = letterDelay / sequenceSpeed;

        while (characterIndex < line.Length)
        {
            // A click during typing completes the current sentence.
            if (WasLeftClickPressed())
            {
                dialogueText.text = line;
                yield break;
            }

            timer += Time.unscaledDeltaTime;

            while (timer >= adjustedLetterDelay &&
                   characterIndex < line.Length)
            {
                dialogueText.text += line[characterIndex];

                characterIndex++;
                timer -= adjustedLetterDelay;
            }

            yield return null;
        }

        dialogueText.text = line;
    }

    private IEnumerator WaitForLeftClick()
    {
        // Prevent the previous click from advancing another line.
        while (IsLeftMouseHeld())
            yield return null;

        // Wait for a new left mouse click.
        while (!WasLeftClickPressed())
            yield return null;
    }

    private bool WasLeftClickPressed()
    {
        return Mouse.current != null &&
               Mouse.current.leftButton.wasPressedThisFrame;
    }

    private bool IsLeftMouseHeld()
    {
        return Mouse.current != null &&
               Mouse.current.leftButton.isPressed;
    }

    private void SetSkipPanel(bool active)
    {
        if (skipPanel != null)
            skipPanel.SetActive(active);
    }

    private void FinishIntroduction()
    {
        isPlaying = false;

        SetSkipPanel(false);

        if (crawlPanel != null)
            crawlPanel.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Debug.Log("[DialogueController] Introduction completed.");

        if (GameManager.Instance != null)
            GameManager.Instance.CompleteIntroduction();
    }
}
