using UnityEngine;
using UnityEngine.UI;

using HK.Core;



namespace HK.UI
{
    [RequireComponent(typeof(Text))]
    public class BaseText : Base
    {
        protected Text textComponent;

        protected virtual void Awake()
        {
            textComponent = GetComponent<Text>();
        }

        public virtual Text GetTextComponent()
        {
            return textComponent;
        }

        public virtual void SetText(string text)
        {
            textComponent.text = text;
        }

        public virtual void SetText(int text)
        {
            SetText(text.ToString());
        }

        public virtual void TweenColor(Color to, float duration)
        {
            textComponent.color = to;
        }

        public virtual void TweenFade(float to, float duration)
        {
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, to);
        }

        public virtual void TweenText(string to, float duration)
        {
            textComponent.text = to;
        }
    }
}
