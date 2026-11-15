using Unity.VisualScripting;
using UnityEngine;

public class deleteShortly : MonoBehaviour
{
    private static deleteShortly instance;
    public float lifetime = 2f;
    void Start()
    {
        if (instance != null){
            Destroy(instance.gameObject);
        }

        instance = this;
        Destroy(gameObject, lifetime);
    }


    void Update()
    {
        
    }
}
