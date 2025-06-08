using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class NPC : MonoBehaviour
{
    public NPCscript npcData;
    public GameObject dialogueUI;
    public Text nameText;
    public Text dialogueText;
    public float typingSpeed = 0.05f;

    private int currentLine = 0;
    private bool isTalking = false;
    private bool playerInRange = false;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isQuestComplete = false;
    private bool hasCompletedDialogue = false;

    public Move playerMove;
    private string[] currentDialogueLines;

    public GameObject sampah;
    public GameObject nextLevelDoor;

    public GameObject interactIcon; // Ikon E

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                StartDialogue();
            }
            else
            {
                if (isTyping)
                {
                    SkipTyping();
                }
                else
                {
                    NextLine();
                }
            }
        }
    }

    public void StartDialogue()
    {
        if (npcData == null) return;

        isTalking = true;
        currentLine = 0;
        dialogueUI.SetActive(true);
        nameText.text = npcData.npcName;

        currentDialogueLines = isQuestComplete ? npcData.dialogueAfterQuest : npcData.dialogueBeforeQuest;

        if (playerMove != null)
            playerMove.canMove = false;

        ShowLine();

        if (interactIcon != null)
            interactIcon.SetActive(false); // Sembunyikan saat mulai bicara
    }

    void ShowLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeLine(currentDialogueLines[currentLine]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentDialogueLines[currentLine];
        isTyping = false;
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine == 1 && sampah != null)
        {
            sampah.SetActive(true);
        }

        if (currentLine < currentDialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueUI.SetActive(false);
        isTalking = false;

        if (playerMove != null)
            playerMove.canMove = true;

        if (isQuestComplete && !hasCompletedDialogue)
        {
            hasCompletedDialogue = true;

            if (nextLevelDoor != null)
                nextLevelDoor.SetActive(true);

            StartCoroutine(RemoveNPC());
        }
    }

    public void OnQuestComplete()
    {
        isQuestComplete = true;
    }

    private IEnumerator RemoveNPC()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactIcon != null)
                interactIcon.SetActive(true); // Tampilkan ikon E saat player dekat
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (isTalking) EndDialogue();

            if (interactIcon != null)
                interactIcon.SetActive(false); // Sembunyikan ikon saat keluar
        }
    }
}
