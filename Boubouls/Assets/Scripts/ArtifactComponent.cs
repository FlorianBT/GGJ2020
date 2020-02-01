using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ArtifactComponent : InteractiveComponent
{
    public Sprite BrokenSprite;
    public Sprite RepairedSprite;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Used ? RepairedSprite : BrokenSprite;
    }

    public override void Interact(float elapsedTime)
    {
        if (Used) return;

        Used = elapsedTime >= holdDuration;
        if(Used)
        {
            Debug.Log("ARTIFACT REPAIRED!!");
            GetComponent<SpriteRenderer>().sprite = RepairedSprite;
            //TODO trigger shit
        }
    }
}
