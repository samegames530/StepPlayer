using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Arcadeモード専用の曲リスト1行表示
/// </summary>
public class ArcadeSongRowView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI artistText;
    [SerializeField] Image highlightImage;   // 選択中表示
    [SerializeField] Button button;

    ArcadeSongSelectScene owner;
    int index;
    SongMeta song;

    /// <summary>
    /// データバインド
    /// </summary>
    public void Bind(ArcadeSongSelectScene owner, int index, SongMeta song)
    {
        this.owner = owner;
        this.index = index;
        this.song = song;

        RefreshView();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    void RefreshView()
    {
        if (song == null) return;

        if (titleText != null)
            titleText.text = song.Title;

        if (artistText != null)
            artistText.text = song.Artist;
    }

    /// <summary>
    /// 選択状態の見た目切り替え
    /// </summary>
    public void SetSelected(bool selected)
    {
        if (highlightImage != null)
            highlightImage.enabled = selected;
    }

    void OnClick()
    {
        if (owner == null) return;
        owner.OnRowClicked(index);
    }

    /// <summary>
    /// Arcade用：選択可否切り替え
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (button != null)
            button.interactable = interactable;
    }
}
