using System;
using TMPro;
using UnityEngine;

    public class UI_Text : MonoBehaviour
    {
        public TextMeshProUGUI ContentTextUI;

        private void Start()
        {
            TextChanged();
        }

        public void TextChanged()
        {
            string text = ContentTextUI.text;
            
            SimpleTextTrimmer simpleTextTrimmer = new SimpleTextTrimmer();
            string limitText = simpleTextTrimmer.TrimTo50Characters(text);
            ContentTextUI.text = limitText;

        }
    }
