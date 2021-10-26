using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterPack
{
    public Sprite sprite;
    public char c;
    public int spacing;
}

public class TextManager : MonoBehaviour
{
    public CharacterPack[] characters;
    public float spacingUnit;
    public int verticalSpacing;
    public string text;
    public Color color;

    private Dictionary<char, int> spacingDict;
    private Dictionary<char, Sprite> spriteDict;

    private void Start()
    {
        spacingDict = new Dictionary<char, int>();
        spriteDict = new Dictionary<char, Sprite>();

        for (int i = 0; i < characters.Length; i++)
        {
            spacingDict[characters[i].c] = characters[i].spacing;
            spriteDict[characters[i].c] = characters[i].sprite;
        }
        SetText(text);
        GenerateText();
    }

    private void GenerateText()
    {
        float xPosition = 0f;
        float yPosition = 0f;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < text.Length; i++)
        {
            if (spacingDict.ContainsKey(text[i]))
            {
                GameObject newCharacter = new GameObject("Character_" + text[i]);
                SpriteRenderer sr = newCharacter.AddComponent<SpriteRenderer>();
                sr.sprite = spriteDict[text[i]];
                sr.color = color;
                newCharacter.transform.position = transform.position +
                    Vector3.right * xPosition +
                    Vector3.up * yPosition;
                newCharacter.transform.SetParent(transform);
                xPosition += (float)spacingDict[text[i]] * spacingUnit;
            }
            else if (text[i] == '\n')
            {
                xPosition = 0f;
                yPosition -= spacingUnit * verticalSpacing;
            }
        }
    }

    public void SetText(string text)
    {
        this.text = text;
        this.text = this.text.Replace("\\n","\n");
        GenerateText();
    }
}
