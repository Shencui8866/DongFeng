using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Josing.UI.ObjectSystem
{
    [System.Serializable]
    public class ObjectEvent
    {
        [SerializeField]
        Transform pos;
        [SerializeField]
        SelectEventType selectEventType;
        [SerializeField]
        ObjectEventState objectEventState;
        [SerializeField]
        float enterDis;
        [SerializeField]
        [Range(0, 30)]
        float clickMaxAngle;

        [Header("")]
        [SerializeField]
        float deltaDis;
        [SerializeField]
        float deltaAngle;
        [SerializeField]
        float time;

        public ObjectTriggerEvent onClick;
        public ObjectTriggerEvent onLeave;
        public bool isUse = true;


        public ObjectEvent()
        {
            onClick = new ObjectTriggerEvent();
            onLeave = new ObjectTriggerEvent();
        }

        public void SetSelectEvent(Transform t, SelectEventType type)
        {
            selectEventType = type;
            pos = t;
        }

        public void UpdateEvent(Vector2 direct, Vector3 selectorPos, float delta)
        {
            if (!isUse)
                return;
            deltaDis = Vector3.Distance(pos.position, selectorPos);
            deltaAngle = Vector3.Angle(direct, (pos.position - selectorPos).normalized);

            switch (selectEventType)
            {
                case SelectEventType.PositionOutAngle:
                    if (deltaDis < enterDis && objectEventState != ObjectEventState.Enter)
                    {
                        onClick?.Invoke(delta);
                        objectEventState = ObjectEventState.Stay;
                    }
                    if (deltaDis > enterDis && objectEventState != ObjectEventState.Exit)
                    {
                        objectEventState = ObjectEventState.Exit;
                        onLeave?.Invoke(0);
                    }
                    switch (objectEventState)
                    {
                        case ObjectEventState.Stay: onClick?.Invoke(delta); break;
                    }
                    break;
                case SelectEventType.Position:
                    if (deltaDis < enterDis && objectEventState != ObjectEventState.Enter)
                    {
                        objectEventState = ObjectEventState.Enter;
                        onClick?.Invoke(delta);
                    }
                    if (deltaDis > enterDis && objectEventState != ObjectEventState.Exit)
                    {
                        objectEventState = ObjectEventState.Exit;
                        onLeave?.Invoke(0);
                    }
                    break;
                case SelectEventType.AnglePostionTime:
                    if (deltaDis < enterDis && objectEventState != ObjectEventState.Enter && objectEventState != ObjectEventState.EnterExecute)
                    {
                        time = 0;
                        objectEventState = ObjectEventState.Stay;
                    }
                    if (deltaDis > enterDis && objectEventState != ObjectEventState.Exit)
                    {
                        objectEventState = ObjectEventState.Exit;
                        onLeave?.Invoke(0);
                    }
                    switch (objectEventState)
                    {
                        case ObjectEventState.Stay:
                            if (deltaAngle < clickMaxAngle)
                            {
                                time += 0.02f;
                                if (time > 2)
                                {
                                    onClick?.Invoke(time);
                                    objectEventState = ObjectEventState.EnterExecute;
                                    break;
                                }
                            }
                            else { time = 0; break; }
                            break;
                    }
                    break;
            }
        }
    }

    public enum SelectEventType
    {
        None,
        Position,
        PositionOutAngle,
        AnglePostionTime
    }

    public enum ObjectEventState
    {
        Enter,
        EnterExecute,
        Stay,
        Exit,
        ExitExecute
    }

    public class ObjectTriggerEvent : UnityEvent<float> { }
}

