using UnityEngine;
using UnityEngine.UI;

public class WeekToggleUIManager : MonoBehaviour
{
    public Toggle weekToggle; // 주간 토글
    public CanvasGroup weekUIGroup; // 숨기고자 하는 UI 그룹

    void Start()
    {
        // 초기 설정
        weekToggle.onValueChanged.AddListener(OnWeekToggleChanged);
        SetWeekUIVisibility(weekToggle.isOn); // 초기 상태 설정
        
    }

    void OnWeekToggleChanged(bool isOn)
    {
        SetWeekUIVisibility(isOn);
    }

    void SetWeekUIVisibility(bool isVisible)
    {
        if (isVisible)
        {
            ShowWeekUI();
        }
        else
        {
            HideWeekUI();
        }
    }

    void ShowWeekUI()
    {
        weekUIGroup.alpha = 1f; // 투명도 설정
        weekUIGroup.interactable = true; // 상호작용 가능 설정
        weekUIGroup.blocksRaycasts = true; // 레이캐스트 차단 설정
    }

    void HideWeekUI()
    {
        weekUIGroup.alpha = 0f; // 투명도 설정
        weekUIGroup.interactable = false; // 상호작용 불가 설정
        weekUIGroup.blocksRaycasts = false; // 레이캐스트 차단 해제
    }
}
