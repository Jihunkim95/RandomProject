using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Globalization; // 추가

public class RandomGroupAssigner : MonoBehaviour
{
    public TMP_InputField nameInputField; // 이름 입력을 위한 TMP_InputField
    public TextMeshProUGUI resultText; // 결과를 표시할 TextMeshProUGUI
    public RectTransform contentRectTransform; // Content의 RectTransform
    public TMP_InputField teamNumInputField; // 조별 인원
    public TextMeshProUGUI alarmText; // 복사 완료 알람
    public TMP_InputField weekNumInputField; // 생성할 주
    public TMP_InputField weekFromInputField; // 시작 주
    public Toggle enterToggle;
    public Toggle spacebarToggle;
    public Toggle commaToggle;
    public Toggle weekToggle;

    private List<string> players;
    private List<string>[] groups;
    private List<string> remainingPlayers; // 남은 인원을 저장할 리스트

    private byte teamNumber; // teamNumber 변수를 byte 형식으로 선언
    private byte groupsNumber; // 조 갯수
    private int weekNumber; // 생성 주

    private Dictionary<string, Dictionary<string, int>> teamHistory; // 팀 구성 기록

    void Start()
    {
        teamHistory = new Dictionary<string, Dictionary<string, int>>();
    }

    public void AssignGroups()
    {
        alarmText.text = "";
        if (!ValidateInputs()) return;

        players = GetPlayersList(nameInputField.text);
        if (players == null || players.Count == 0)
        {
            resultText.text = "올바른 구분값을 선택해주세요.";
            AdjustContentSize();
            return;
        }

        players = players.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).ToList();
        if (players.Count < teamNumber)
        {
            resultText.text = "조별 인원보다 많은 인원을 입력해주세요.";
            AdjustContentSize();
            return;
        }

        groupsNumber = (byte)Mathf.CeilToInt((float)players.Count / teamNumber);
        InitializeGroups();

        if (weekToggle.isOn)
        {
            GenerateTeamsForWeeks(weekNumber);
        }
        else
        {
            AssignGroupsOnce();
        }

        AdjustContentSize();
    }

    private bool ValidateInputs()
    {
        if (!byte.TryParse(teamNumInputField.text, out teamNumber))
        {
            resultText.text = "조별 인원이 올바른 형식이 아닙니다.";
            AdjustContentSize();
            return false;
        }

        if (!int.TryParse(weekNumInputField.text, out weekNumber))
        {
            resultText.text = "주 숫자가 올바른 형식이 아닙니다.";
            AdjustContentSize();
            return false;
        }

        return true;
    }

    private List<string> GetPlayersList(string inputText)
    {
        if (enterToggle.isOn)
        {
            return new List<string>(inputText.Split('\n'));
        }
        else if (spacebarToggle.isOn)
        {
            return new List<string>(inputText.Split(' '));
        }
        else if (commaToggle.isOn)
        {
            return new List<string>(inputText.Split(','));
        }
        return null;
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
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void AssignGroupsOnce()
    {
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
    }

    private void GenerateTeamsForWeeks(int weeks)
    {
        string result = "";
        DateTime startDate;
        if (!DateTime.TryParseExact(weekFromInputField.text, "yyyy-MM-dd", null, DateTimeStyles.None, out startDate))
        {
            resultText.text = "시작 날짜가 올바른 형식이 아닙니다. (YYYY-MM-DD)";
            AdjustContentSize();
            return;
        }

        for (int week = 0; week < weeks; week++)
        {
            InitializeGroups();
            List<string> tempPlayers = new List<string>(players);
            Shuffle(tempPlayers);

            for (int groupIndex = 0; groupIndex < groupsNumber; groupIndex++)
            {
                List<string> team = GetNextTeam(tempPlayers);
                groups[groupIndex].AddRange(team);
            }

            DateTime currentWeekStartDate = startDate.AddDays(week * 7);
            DateTime currentWeekEndDate = startDate.AddDays(week * 7 + 6);
            result += $"{week + 1}주 차 ({currentWeekStartDate.ToString("yyyy-MM-dd")} ~ {currentWeekEndDate.ToString("yyyy-MM-dd")}):\n" + FormatGroups() + "\n";
            UpdateTeamHistory();
        }

        resultText.text = result;
    }

    private List<string> GetNextTeam(List<string> tempPlayers)
    {
        List<string> team = new List<string>();
        for (int i = 0; i < teamNumber && tempPlayers.Count > 0; i++)
        {
            string nextPlayer = GetPlayerWithLeastHistory(tempPlayers, team);
            team.Add(nextPlayer);
            tempPlayers.Remove(nextPlayer);
        }
        return team;
    }

    private string GetPlayerWithLeastHistory(List<string> tempPlayers, List<string> currentTeam)
    {
        return tempPlayers.OrderBy(player => currentTeam.Sum(member => GetHistoryCount(player, member))).First();
    }

    private int GetHistoryCount(string player1, string player2)
    {
        if (teamHistory.ContainsKey(player1) && teamHistory[player1].ContainsKey(player2))
        {
            return teamHistory[player1][player2];
        }
        return 0;
    }

    private void UpdateTeamHistory()
    {
        foreach (var team in groups)
        {
            for (int i = 0; i < team.Count; i++)
            {
                for (int j = i + 1; j < team.Count; j++)
                {
                    AddToHistory(team[i], team[j]);
                    AddToHistory(team[j], team[i]);
                }
            }
        }
    }

    private void AddToHistory(string player1, string player2)
    {
        if (!teamHistory.ContainsKey(player1))
        {
            teamHistory[player1] = new Dictionary<string, int>();
        }

        if (!teamHistory[player1].ContainsKey(player2))
        {
            teamHistory[player1][player2] = 0;
        }

        teamHistory[player1][player2]++;
    }

    private void DisplayGroups()
    {
        resultText.text = FormatGroups();
    }

    private string FormatGroups()
    {
        string result = "";
        for (int i = 0; i < groups.Length; i++)
        {
            result += $"{i + 1}조: ";
            for (int j = 0; j < groups[i].Count; j++)
            {
                result += groups[i][j];
                if (j < groups[i].Count - 1)
                {
                    result += ", ";
                }
            }
            result += "\n";
        }

        if (remainingPlayers.Count > 0)
        {
            result += "배정 필요인원:\n";
            foreach (string player in remainingPlayers)
            {
                result += player + "\n";
            }
            result += "\n";
        }

        return result;
    }

    private void AdjustContentSize()
    {
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, resultText.preferredHeight);
    }
}
