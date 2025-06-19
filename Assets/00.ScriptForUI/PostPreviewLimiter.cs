using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PostPreviewLimiter : MonoBehaviour
{
    private TextMeshProUGUI _tmpText;

    private void Start()
    {
        _tmpText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(TrimSelfText());
    }

    private IEnumerator TrimSelfText()
    {
        // 한 프레임 대기 → TMP 내부가 준비되도록
        yield return null;

        string fullText = _tmpText.text;

        _tmpText.ForceMeshUpdate();
        TMP_TextInfo info = _tmpText.textInfo;

        if (info.lineCount > 2)
        {
            int lastVisibleChar = info.lineInfo[1].lastCharacterIndex;
            int safeIndex = Mathf.Max(0, lastVisibleChar - 2);
            string trimmed = fullText.Substring(0, safeIndex).TrimEnd() + "...";
            _tmpText.text = trimmed;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_tmpText.rectTransform);
        }
    }
}
