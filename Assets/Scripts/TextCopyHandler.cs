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
    void Start()
    {
        copyButton.onClick.AddListener(CopyTextToClipboard);
    }

    // WebGL 빌드 시에 JavaScript 플러그인을 사용하여 클립보드 복사 기능
    void CopyTextToClipboard()
    {
        GUIUtility.systemCopyBuffer = resultText.text;
        alarmText.text = "복사 완료";
        Debug.Log("Text copied to clipboard: " + resultText.text);
    }

}
