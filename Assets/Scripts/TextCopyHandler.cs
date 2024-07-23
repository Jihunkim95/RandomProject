using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices; // DllImport를 사용하기 위해 추가

public class TextCopyHandler : MonoBehaviour
{
    public TMP_InputField nameInputField; // 이름 입력을 위한 TMP_InputField
    public TextMeshProUGUI resultText; // 결과를 표시할 TextMeshProUGUI
    public Button copyButton; // 복사 버튼

    public TextMeshProUGUI alarmText; // 복사 완료 알람

    [DllImport("__Internal")]
    private static extern void CopyToClipboard(string text);

    void Start()
    {
        copyButton.onClick.AddListener(CopyTextToClipboard);
    }

    void CopyTextToClipboard()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        CopyToClipboard(resultText.text);
#else
        GUIUtility.systemCopyBuffer = resultText.text;
        Debug.Log("Text copied to clipboard: " + resultText.text);
#endif
        alarmText.text = "복사 완료";
    }
}