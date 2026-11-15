using UnityEngine;

public class SpinnerScript : MonoBehaviour
{
    public float speed = 400f;

    
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0f, 0f, -speed * Time.deltaTime);
    }
}
