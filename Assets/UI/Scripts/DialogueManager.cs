using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    public string[] lines;
    private int index;

    public float typingSpeed = 0.05f;
    private bool isTyping = false;

    void Start()
    {
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && !isTyping)
        {
            if (index < lines.Length - 1)
            {
                index++;
                StartCoroutine(TypeLine());
            }
            else
            {
                // Ya se mostraron todas las líneas
                dialoguePanel.SetActive(false);
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in lines[index].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}

