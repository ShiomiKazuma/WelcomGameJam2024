using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ランキング処理がまとまったクラス。
/// タイムの設定はSetScoreを呼ぶこと！
/// </summary>
public class Ranking : MonoBehaviour
{ 
    [SerializeField] List<RankingInfo> _ranking = new List<RankingInfo>();
    [SerializeField] Text[] _rankingText;
    [SerializeField] float _time = 0;
    [SerializeField] InputField _nameInput = null;
    [SerializeField] Text _myScore = null;
    [SerializeField] GameObject[] _rankingObject;
    bool _isRanking = false;
    int _rankingIndex = 0;

    private void Start()
    {
        _nameInput.gameObject.SetActive(false);
        _rankingObject.ToList().ForEach(x => x.SetActive(false));
        var tempRanking = RankingManager.LoadRanking();
        if (tempRanking.Count != 0)
            _ranking = RankingManager.LoadRanking();
        IsRankin(_time);
        if (_isRanking)
        {
            _nameInput.gameObject.SetActive(true);
        }
        
        ShowRankingAsync();
    }

    /// <summary>
    /// タイムをセットする
    /// </summary>
    /// <param name="time"></param>
    public void SetScore(float time)
    {
        _time = time;
    }
    
    /// <summary>
    /// ランキングに入っているか判定
    /// </summary>
    /// <param name="time"></param>
    public void IsRankin(float time)
    {
        for (int i = 0; i < _ranking.Count; i++)
        {
            if (time < _ranking[i].Time)
            {
                Debug.Log($"{time}:{_ranking[i].Time}");
                Debug.Log("ランクインしました");
                _isRanking = true;
                _rankingIndex = i;
                return;
            }
        }
    }

    /// <summary>
    /// 非同期でランキングを表示。最初の表示でのみ使う
    /// </summary>
    public void ShowRankingAsync()
    {
        _ranking.Sort((a, b) => a.Time.CompareTo(b.Time));
        for (int i = 0; i < _ranking.Count; i++)
        {
            _rankingText[i].text = $"{i + 1}位 {_ranking[i].Time:F2}秒 {_ranking[i].Name}";
        }
        _myScore.text = $"あなたのタイム {_time:F2}秒";

        StartCoroutine( ShowRankingCoroutine());
    }
    
    IEnumerator ShowRankingCoroutine()
    {
        foreach (var rankingText in _rankingText)
        {   //テキストの移動開始地点の設定
            var rect = rankingText.GetComponent<RectTransform>();
            rect.position = new Vector3(rect.position.x, rect.position.y - 140, 0 );
            rect.GetComponent<Text>().color = new Color(0, 0, 0, 0);
        }

        foreach (var rankingText in _rankingText)
        {   //テキストの移動
            var rect = rankingText.GetComponent<RectTransform>();
            rect.DOMoveY(rect.position.y + 140, 1).SetLink(rect.gameObject).SetEase(Ease.OutBounce);
            rankingText.DOFade(1, 1).SetLink(rankingText.gameObject);
            yield return new WaitForSeconds(1);
        }
        
        foreach (var rankingObject in _rankingObject)
        {   //オブジェクトの移動
            rankingObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// ランキングを即座に表示。プレイヤーのスコアがランキングに入った時に使う
    /// </summary>
    void ShowRankingInstantly()
    {
        _ranking.Sort((a, b) => a.Time.CompareTo(b.Time));
        for (int i = 0; i < _ranking.Count; i++)
        {
            _rankingText[i].text = $"{i + 1}位 {_ranking[i].Time:F2}秒 {_ranking[i].Name}";
        }
        _myScore.text = $"あなたのタイム {_time:F2}秒";
        _myScore.GetComponent<RectTransform>().DOScale(1f, 1).SetEase(Ease.OutBounce);
        _myScore.GetComponent<RectTransform>().DOScale(1f, 1).SetEase(Ease.OutBounce);
    }
    
    public void SetRanking()
    {
        _ranking.Insert(_rankingIndex, new RankingInfo() { Time = _time, Name = _nameInput.text.Truncate(5)});
        _ranking.RemoveAt(_ranking.Count - 1);
        _nameInput.gameObject.SetActive(false);
        ShowRankingInstantly();
        RankingManager.SaveRanking(_ranking);
    }
}

[Serializable]
public class RankingInfo
{
    [SerializeField] public float Time = 0;
    [SerializeField] public string Name = "AAA";
}

public static class RankingManager
{
    private static string rankingKey = "ranking";

    public static void SaveRanking(List<RankingInfo> rankings)
    {
        string json = JsonUtility.ToJson(new RankingList { Rankings = rankings });
        PlayerPrefs.SetString(rankingKey, json);
        PlayerPrefs.Save();
    }

    public static List<RankingInfo> LoadRanking()
    {
        string json = PlayerPrefs.GetString(rankingKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return new List<RankingInfo>();
        }
        return JsonUtility.FromJson<RankingList>(json).Rankings;
    }
}

[System.Serializable]
public class RankingList
{
    public List<RankingInfo> Rankings;
}