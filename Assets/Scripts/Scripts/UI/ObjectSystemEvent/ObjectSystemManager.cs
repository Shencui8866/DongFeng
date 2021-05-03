using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Josing.UI.ObjectSystem
{
    public class ObjectSystemManager : MonoBehaviour
    {
        [SerializeField]
        float delta;
        [SerializeField]
        float addAngle;
        [SerializeField]
        float limitDeltaDiscard = 30;

        [SerializeField]
        List<ObjectEvent> selectEvents = new List<ObjectEvent>();

        public static ObjectSystemManager instance;

        public float Delta { get { return delta; } }

        float deltaNewAngle;


        private void Awake() { instance = this; }

        public void SetEvent(ObjectEvent[] e)
        {
            for (int i = 0; i < e.Length; i++)
            {
                if (!selectEvents.Contains(e[i]))
                    selectEvents.Add(e[i]);
            }
        }

        public void AddEvent(ObjectEvent e) { if (!selectEvents.Contains(e)) selectEvents.Add(e); }

        public void Remove(ObjectEvent e)
        {
            selectEvents.Remove(e);
        }

        public void Remove(ObjectEvent[] e)
        {
            for (int i = 0; i < e.Length; i++)
            {
                selectEvents.Remove(e[i]);
            }
        }

        public void ClearEvent() { selectEvents.Clear(); } 

        public void ObjectDrag(Vector3 v1, Vector3 v2, Transform cursor)
        {
            Vector3 direct = (v1 - v2).normalized;
            delta = VectorExtension.DeltaAngle(direct, ref addAngle, limitDeltaDiscard);
            cursor.Rotate(Vector3.back, delta);
            cursor.position = GetObjectPos(v1, v2);
            for (int i = 0; i < selectEvents.Count; i++)
                selectEvents[i].UpdateEvent(direct, transform.position, delta);
        }

        Vector3 GetObjectPos(Vector3 v1, Vector3 v2) { return new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, 0); }
    }
}

