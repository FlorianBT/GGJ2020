using UnityEngine.Events;

[System.Serializable]
public class TriggerOnBouboulAreaEvent : UnityEvent<BouboulAIComponent> { }
public class TriggerOnBouboulAreaComponent
            : TriggerAreaComponent<BouboulAIComponent, TriggerOnBouboulAreaEvent>
{}

