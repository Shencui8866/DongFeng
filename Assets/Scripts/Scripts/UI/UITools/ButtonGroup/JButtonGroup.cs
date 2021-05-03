using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Josing.UI
{
    public class JButtonGroup : MonoBehaviour
    {
        [SerializeField]
        bool hasDefSelect = true;
        [SerializeField]
        JButton[] buttons;

        private void Awake()
        {
            buttons = transform.GetComponentsInChildren<JButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].onSelectEvent = onSelect;
            }
            OnReset();
        }

        public void onSelect(JButton button)
        {
            for(int i = 0; i < buttons.Length; i++)
            {
                if(buttons[i] != button)
                    buttons[i].OnReset();
            }
        }

        public void onSelect(int index)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (index == i)
                    buttons[i].Select();
                else
                    buttons[i].OnReset();
            }
        }

        public void OnReset()
        {
            for (int i = 1; i < buttons.Length; i++)
                buttons[i].OnReset();
            if (hasDefSelect)
                buttons[0].Select();
        }
    }

}

