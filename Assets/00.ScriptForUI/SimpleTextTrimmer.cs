public class SimpleTextTrimmer
{
    public string TrimTo50Characters(string fullText)
    {
        const int maxLength = 30;

        if (string.IsNullOrEmpty(fullText))
            return "";

        if (fullText.Length <= maxLength)
            return fullText;

        string trimmed = fullText.Substring(0, maxLength).TrimEnd();
        return trimmed + "...";
    }
}