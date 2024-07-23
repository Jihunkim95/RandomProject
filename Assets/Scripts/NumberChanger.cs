using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class NumberChanger : MonoBehaviour
{
    public TMP_InputField teamNumInputField;
    public Button plusBtn;
    public Button minusBtn;

    public TMP_InputField weekNumInputField;
    public Button weekPlusBtn;
    public Button weekMinusBtn;

    private byte currentTeamNumber;
    private byte currentWeekNumber;

    void Start()
    {
        // 팀 버튼에 리스너 추가
        plusBtn.onClick.AddListener(() => ChangeNumber(ref currentTeamNumber, 1));
        minusBtn.onClick.AddListener(() => ChangeNumber(ref currentTeamNumber, -1));

        // 주 버튼에 리스너 추가
        weekPlusBtn.onClick.AddListener(() => ChangeNumber(ref currentWeekNumber, 1));
        weekMinusBtn.onClick.AddListener(() => ChangeNumber(ref currentWeekNumber, -1));

        UpdateDisplay();
    }

    private void ChangeNumber(ref byte number, int change)
    {
        number = (byte)Mathf.Clamp(number + change, 0, 50);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        teamNumInputField.text = currentTeamNumber.ToString();
        weekNumInputField.text = currentWeekNumber.ToString();
    }
}
