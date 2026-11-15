using UnityEngine;
using UnityEngine.EventSystems;

public class UiAutoSelect : MonoBehaviour
{
    

    private void OnEnable()
    {
        StartCoroutine(SelectNextFrame());
    }

    private System.Collections.IEnumerator SelectNextFrame()
    {
        yield return null; // wait one frame
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}