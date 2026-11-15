using System.Collections;
using TMPro;
using UnityEngine;

public class LoadID : MonoBehaviour
{
    private TextMeshProUGUI idText;
    IEnumerator Start()
    {
        idText = GetComponent<TextMeshProUGUI>();

        yield return new WaitUntil(() => AccountManagement.instance.GetID() != null);

        idText.text = AccountManagement.instance.GetID();
    }
}
