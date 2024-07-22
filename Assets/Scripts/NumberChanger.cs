
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class NumberChanger : MonoBehaviour
{
    public TMP_InputField teamNumInputField;
    public Button plusBtn;
    public Button minusBtn;
    
    private byte currentNumber;
    // Start is called before the first frame update
    void Start()
    {
        plusBtn.onClick.AddListener(PlusNumber);
        minusBtn.onClick.AddListener(MinusNumber);
        UpdateDisplay();
    }

    public void PlusNumber()
    {
        currentNumber++;
        UpdateDisplay();
    }
    public void MinusNumber()
    {
        currentNumber--;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        teamNumInputField.text = currentNumber.ToString();
    }
}
