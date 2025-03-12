using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public GameObject dialoguePanel;
    public Image panelImage;  
    public string[] lines;
    public float textSpeed;
    private int index;

    private Color[] speakerColors = { Color.gray, Color.red }; 

    void Start()
    {
        Debug.Log("Start method called");

        PlayerPrefs.DeleteKey("DialogueShown");

        if (PlayerPrefs.GetInt("DialogueShown", 0) == 0)
        {
            Debug.Log("Starting dialogue");
            dialoguePanel.SetActive(true);
            textComponent.text = string.Empty;
            panelImage.color = speakerColors[0]; 
            StartDialogue();
            PlayerPrefs.SetInt("DialogueShown", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Dialogue already shown, skipping...");
            dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        Time.timeScale = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char character in lines[index].ToCharArray())
        {
            textComponent.text += character;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            panelImage.color = speakerColors[index % speakerColors.Length]; 
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        Time.timeScale = 1;
        dialoguePanel.SetActive(false);
    }
}