using UnityEngine.Events;

[System.Serializable]
public class TriggerOnInteractiveAreaEvent : UnityEvent<InteractiveComponent> { }
public class TriggerOnInteractiveAreaComponent 
            : TriggerAreaComponent<InteractiveComponent, TriggerOnInteractiveAreaEvent> {}
