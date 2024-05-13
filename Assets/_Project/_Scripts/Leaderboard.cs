using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myRank;
    [SerializeField] private TextMeshProUGUI myScore;
    [SerializeField] private List<TextMeshProUGUI> listNames;
    [SerializeField] private List<TextMeshProUGUI> listScores;
    [SerializeField] private List<TextMeshProUGUI> listRanks;

    private const string publicLeaderboardKey = "067a8bcb510fc974df8bdd5a48bb65019a4b71e229746ef3c971564d64beb622";

    private int getScore;

    private void OnEnable()
    {
        GameController.OnGetScore += GetScore;
    }

    private void OnDisable()
    {
        GameController.OnGetScore -= GetScore;
    }

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) =>
        {
            for (int i = 0; i < listNames.Count; i++)
            {
                listNames[i].text = msg[i].Username;
                listScores[i].text = msg[i].Score.ToString();
                listRanks[i].text = msg[i].Rank.ToString();
                if (listRanks[i].text == "1") listRanks[i].text = "";
                if (listRanks[i].text == "2") listRanks[i].text = "";
                if (listRanks[i].text == "3") listRanks[i].text = "";
                if (getScore < msg[9].Score) myRank.text = "10+";
                else if (getScore == msg[i].Score)
                {
                    myRank.text = msg[i].Rank.ToString();
                }
            }
        }));
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((_) =>
        {
            GetLeaderboard();
        }));
        LeaderboardCreator.ResetPlayer();
    }

    public void GetScore(int i)
    {
        getScore = i;
        Debug.Log(getScore);
    }
}
