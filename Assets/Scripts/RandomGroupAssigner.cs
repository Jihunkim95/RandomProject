using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

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

    // 주단위 생성 토글
    public Toggle WeekToggle;

    private List<string> players;
    private List<string>[] groups;
    private List<string> remainingPlayers; // 남은 인원을 저장할 리스트

    private byte teamNumber; // teamNumber 변수를 byte 형식으로 선언
    private byte groupsNumber; // 조 갯수

    private Dictionary<string, Dictionary<string, int>> teamHistory; // 팀 구성 기록

    void Start()
    {
        teamHistory = new Dictionary<string, Dictionary<string, int>>();
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
            resultText.text = "조별 인원이 올바른 형식이 아닙니다.";
            teamNumber = 0;
            AdjustContentSize();
            return;
        }

        string inputText = nameInputField.text;
        if (EnterToggle.isOn)
        {
            players = new List<string>(inputText.Split('\n'));
        }
        else if (SpacebarToggle.isOn)
        {
            players = new List<string>(inputText.Split(' '));
        }
        else if (CommaToggle.isOn)
        {
            players = new List<string>(inputText.Split(','));
        }
        else
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

        if (WeekToggle.isOn)
        {
            GenerateTeamsForWeeks(4); // 4주 동안 다른 팀을 구성
        }
        else
        {
            AssignGroupsOnce();
        }

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
        for (int week = 0; week < weeks; week++)
        {
            InitializeGroups();
            List<string> tempPlayers = new List<string>(players);
            Shuffle(tempPlayers); // 매주 팀을 섞습니다.

            for (int groupIndex = 0; groupIndex < groupsNumber; groupIndex++)
            {
                List<string> team = GetNextTeam(tempPlayers);
                foreach (var player in team)
                {
                    groups[groupIndex].Add(player);
                }
            }

            result += $"Week {week + 1}:\n";
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
            }

            result += "\n";
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
        string bestCandidate = null;
        int minHistoryCount = int.MaxValue;

        foreach (var player in tempPlayers)
        {
            int historyCount = currentTeam.Sum(member => GetHistoryCount(player, member));
            if (historyCount < minHistoryCount)
            {
                minHistoryCount = historyCount;
                bestCandidate = player;
            }
        }

        return bestCandidate;
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
        resultText.text = "";
        for (int i = 0; i < groups.Length; i++)
        {
            resultText.text += (i + 1) + "조: ";
            for (int j = 0; j < groups[i].Count; j++)
            {
                resultText.text += groups[i][j];
                if (j < groups[i].Count - 1)
                {
                    resultText.text += ", ";
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
