using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 추가
using System.Collections.Generic;
using UnityEngine.UI;

public class RandomGroupAssigner : MonoBehaviour
{
    public TMP_InputField nameInputField; // 이름 입력을 위한 TMP_InputField
    public TextMeshProUGUI resultText; // 결과를 표시할 TextMeshProUGUI
    public RectTransform contentRectTransform; // Content의 RectTransform
    public TMP_InputField TeamNumInputField; // 조별 인원
    public TextMeshProUGUI alarmText; // 복사 완료 알람
    //구분값 토글
    public Toggle EnterToggle;
    public Toggle SpacebarToggle;
    public Toggle CommaToggle;

    private List<string> players;
    private List<string>[] groups;
    private List<string> remainingPlayers; // 남은 인원을 저장할 리스트


    private byte teamNumber; // teamNumber 변수를 byte 형식으로 선언
    private byte groupsNumber; // 조 갯수
    void Start()
    {
        // InitializeGroups();
    }

    public void AssignGroups()
    {
        alarmText.text = "";
        if (byte.TryParse(TeamNumInputField.text, out byte result))
        {
            teamNumber = result;
        }
        else
        {
            resultText.text = "조별 인원이 옳바른 형식이 아닙니다.";
            teamNumber = 0;
            AdjustContentSize();
            return;

        }
        string inputText = nameInputField.text;
        if (EnterToggle.isOn){
            players = new List<string>(inputText.Split('\n'));
        }else if(SpacebarToggle.isOn){
            players = new List<string>(inputText.Split(' '));
        }else if(CommaToggle.isOn){
            players = new List<string>(inputText.Split(','));         
        }else {
            resultText.text = "옳바른 구분값을 선택해주세요.";
            AdjustContentSize();
            return;
        }
        // Debug.Log(teamNumber);

        if (players.Count < teamNumber)
        {
            resultText.text = "조별 인원보다 많은 인원을 입력해주세요.";
            AdjustContentSize();
            return;
        }



        if (teamNumber != 0){
            groupsNumber = (byte)(players.Count / teamNumber);
        }else{
            resultText.text = "조별 인원이 0 입니다.";
            AdjustContentSize();
            return;
        }
        InitializeGroups();
        Shuffle(players);

        for (int i = 0; i < players.Count; i++)
        {
            if (i < groupsNumber * teamNumber)
            {
                groups[i % groupsNumber].Add(players[i].Trim());
            }
            else
            {
                remainingPlayers.Add(players[i].Trim());
            }
        }

        DisplayGroups();
        AdjustContentSize();
    }

    private void InitializeGroups()
    {
        groups = new List<string>[groupsNumber];
        for (int i = 0; i < groupsNumber; i++)
        {
            groups[i] = new List<string>();
        }
        remainingPlayers = new List<string>(); // 남은 인원 리스트 초기화

    }

    private void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void DisplayGroups()
    {
        resultText.text = "";
        for (int i = 0; i < groups.Length; i++)
        {
            resultText.text += (i + 1) + "조: ";
            for (int j = 0; j < groups[i].Count; j++)
            {
                if (j == groups[i].Count - 1)
                {
                    // 마지막 인원일 경우
                    resultText.text += groups[i][j] + "\n";
                }
                else
                {
                    resultText.text += groups[i][j] + ", ";
                }
            }
            resultText.text += "\n";
        }

        if (remainingPlayers.Count > 0)
        {
            resultText.text += "배정 필요인원:\n";
            foreach (string player in remainingPlayers)
            {
                resultText.text += player + "\n";
            }
            resultText.text += "\n";
        }
    }

    private void AdjustContentSize()
    {
        // ResultText의 높이에 따라 Content의 높이를 조정
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, resultText.preferredHeight);
    }
}
