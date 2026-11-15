using UnityEngine;
using UnityEngine.UI;
using static InputActionsparams;
public class displaySegments : MonoBehaviour
{
    [SerializeField] Image[] segments;
    direction currentDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        InputActionsparams.Instance.motionEvent += OnDirectionChanged;
    }

    private void OnDisable()
    {
        InputActionsparams.Instance.motionEvent -= OnDirectionChanged;
    }
    private void OnDirectionChanged(InputActionsparams.direction dir){
        dirToImage(currentDir)?.gameObject.SetActive(false);
        dirToImage(dir)?.gameObject.SetActive(true);
        currentDir = dir;
    }


    private Image dirToImage(direction dir) {
        switch (dir){
            case direction.Neutral: return null;
            case direction.Forward: return segments[0];
            case direction.UpForward: return segments[1];
            case direction.Up: return segments[2];
            case direction.UpBack: return segments[3];
            case direction.Back: return segments[4];
            case direction.DownBack: return segments[5];
            case direction.Down: return segments[6];
            case direction.DownForward: return segments[7];
        }
        return null;
    }
}
