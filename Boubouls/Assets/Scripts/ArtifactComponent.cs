using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ArtifactComponent : InteractiveComponent
{
    public Sprite BrokenSprite;
    public Sprite RepairedSprite;

    public bool Locked = true;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Used ? RepairedSprite : BrokenSprite;
    }

    public override void Interact(float elapsedTime)
    {
        if (Used || Locked) return;

        Used = elapsedTime >= holdDuration;
        if(Used)
        {
            Debug.Log("ARTIFACT REPAIRED!!");
            GetComponent<SpriteRenderer>().sprite = RepairedSprite;
            //TODO trigger shit

            EventManager.TriggerEvent("ArtifactDestroyed");
        }
    }

    public override bool CanInteract() {
        return !Locked;
    }
}
