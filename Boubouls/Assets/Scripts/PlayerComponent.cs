using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        float xDiff = 0f;
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            xDiff -= speed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            xDiff += speed;
        }
        transform.Translate(new Vector3(xDiff * Time.deltaTime, 0f, 0f));
    }
}
