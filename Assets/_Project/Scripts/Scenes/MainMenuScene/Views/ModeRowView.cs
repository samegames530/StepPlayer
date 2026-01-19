using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class ModeRowView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI modeNameText;
    [SerializeField] GameObject selectedFrame;

    MainMenuScene owner;
    int index;

    public void Bind(MainMenuScene owner, int index, string modeName)
    {
        this.owner = owner;
        this.index = index;

        if (modeNameText != null)
            modeNameText.text = modeName;
    }

    public void SetSelected(bool selected)
    {
        if (selectedFrame != null)
            selectedFrame.SetActive(selected);
    }

    // Button / EventTrigger から呼ぶ
    public void OnClick()
    {
        if (owner != null)
            owner.OnRowClicked(index);
    }
}
