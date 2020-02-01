using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundSwitchComponent : MonoBehaviour
{
    public int m_BouboulsNeeded = 5;
    public UnityEvent OnFillEvent;

    private bool m_isTriggered = false;
    private List<BouboulAIComponent> m_BouboulsTrapped;

    private void Awake()
    {
        m_BouboulsTrapped = new List<BouboulAIComponent>();

        if (OnFillEvent == null)
        {
            OnFillEvent = new UnityEvent();
        }
    }

    public void OnBouboulCaught(BouboulAIComponent bouboul)
    {
        if (m_BouboulsTrapped.Contains(bouboul) || m_isTriggered) return;

        m_BouboulsTrapped.Add(bouboul);
        if(m_BouboulsTrapped.Count >= m_BouboulsNeeded)
        {
            m_isTriggered = true;
            OnFillEvent.Invoke();
            Debug.Log("TRIGGERED");

            foreach (BouboulAIComponent b in m_BouboulsTrapped)
            {
                b.SquashDead();
            }
            m_BouboulsTrapped.Clear();
        }
    }

    public void OnBouboulJumpedOut(BouboulAIComponent bouboul)
    {
        m_BouboulsTrapped.Remove(bouboul);
    }
}
