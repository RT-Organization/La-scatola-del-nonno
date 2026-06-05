using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character; // Character speaking the line
    public List<string> lines; // List of lines for this dialogue
    public string lineId; // Unique ID for this line
    public List<DialogueQuestion> questions; // List of questions for this line

    // Add this to your DialogueLine class
    public string GetFullText()
    {
        return lines.Count > 0 ? lines[0] : string.Empty;
        // Or if you want to concatenate all lines:
        // return string.Join(" ", lines);
    }
}