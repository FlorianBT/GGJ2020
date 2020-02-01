using UnityEngine.Events;

[System.Serializable]
public class TriggerOnPlayerAreaEvent : UnityEvent<PlayerComponent> { }
public class TriggerOnPlayerAreaComponent 
            : TriggerAreaComponent<PlayerComponent, TriggerOnPlayerAreaEvent> { }