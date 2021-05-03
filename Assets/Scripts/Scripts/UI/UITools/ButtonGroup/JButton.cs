using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;


namespace Josing.UI
{
    public class JButton : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField]
        ButtonPressEffect pressEffect;
        [SerializeField]
        Color NormalColor = Color.white;
        [SerializeField]
        Color PressColor = Color.white;
        [SerializeField]
        Sprite NormalSprite;
        [SerializeField]
        Sprite PressSprite;
        [SerializeField]
        bool fixedPressState = false;
        [SerializeField]
        UnityEngine.UI.Image m_image;

        [SerializeField]
        UnityEvent onClick = new UnityEvent();

        bool isPress;



        public Action<JButton> onSelectEvent;
        public UnityEvent OnClick { get { return onClick; } }

        private void Awake()
        {
            m_image = GetComponent<UnityEngine.UI.Image>();
            if (pressEffect == ButtonPressEffect.Color) m_image.color = NormalColor;
            else m_image.sprite = NormalSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (pressEffect == ButtonPressEffect.Color) m_image.color = PressColor;
            else m_image.sprite = PressSprite;
            isPress = true;
            onSelectEvent?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(!fixedPressState)
            {
                if (pressEffect == ButtonPressEffect.Color) m_image.color = NormalColor;
                else m_image.sprite = NormalSprite;
                isPress = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }

        [ContextMenu("Reset")]
        public void OnReset()
        {
            if (pressEffect == ButtonPressEffect.Color) m_image.color = NormalColor;
            else m_image.sprite = NormalSprite;
            isPress = false;
        }

        public void Select()
        {
            if (pressEffect == ButtonPressEffect.Color) m_image.color = PressColor;
            else m_image.sprite = PressSprite;
            isPress = true;
        }

        public void SetNormalSprite(Sprite sprite)
        {
            NormalSprite = sprite;
            if(!isPress) m_image.sprite = sprite;

        }
        public void SetPressSprite(Sprite sprite)
        {
            PressSprite = sprite;
            if (isPress) m_image.sprite = sprite;
        }

        private void OnValidate()
        {
            m_image = GetComponent<UnityEngine.UI.Image>();
            if (pressEffect == ButtonPressEffect.Color) m_image.color = NormalColor;
            else m_image.sprite = NormalSprite;
        }
    }

    public enum ButtonPressEffect
    {
        Color,
        Sprite
    }
}

