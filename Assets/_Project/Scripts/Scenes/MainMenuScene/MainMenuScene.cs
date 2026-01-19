using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public sealed class MainMenuScene : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] RectTransform listRoot;
    [SerializeField] ModeRowView rowPrefab;

    [Header("Sound Effects")]
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioClip moveSe;
    [SerializeField] AudioClip decideSe;

    [Header("Reel Layout")]
    [SerializeField] float centerX = -20f;
    [SerializeField] float centerY = 0f;
    [SerializeField] float spacingY = 110f;

    [SerializeField] float rightXMax = 80f;
    [SerializeField] AnimationCurve xOffsetByAbsIndex =
        AnimationCurve.EaseInOut(0, 0, 6, 1);

    [Header("Reel Motion")]
    [SerializeField] float smoothTime = 0.08f;
    [SerializeField] float snapThreshold = 0.1f;

    [Header("Reel Visual")]
    [SerializeField] AnimationCurve scaleByAbsIndex =
        AnimationCurve.EaseInOut(0, 1.0f, 6, 0.88f);

    [SerializeField] AnimationCurve alphaByAbsIndex =
        AnimationCurve.EaseInOut(0, 1.0f, 6, 0.35f);

    readonly List<ModeRowView> rows = new();
    readonly List<Vector2> rowVels = new();
    readonly List<RectTransform> rowRects = new();
    readonly List<CanvasGroup> rowCgs = new();

    int selectedIndex = 0;
    bool isTransitioning = false;

    readonly string[] modes =
    {
        "Arcade Mode",
        "FreePlay Mode",
        "Option"
    };

    void Start()
    {
        BuildList();
    }

    void Update()
    {
        if (isTransitioning) return;

        if (KeyBindings.MenuUpPressedThisFrame())
            MoveSelection(-1);

        if (KeyBindings.MenuDownPressedThisFrame())
            MoveSelection(+1);

        if (KeyBindings.MenuConfirmPressedThisFrame())
            StartCoroutine(Decide(selectedIndex));

        ApplyReelLayout();
    }

    void BuildList()
    {
        rows.Clear();
        rowRects.Clear();
        rowCgs.Clear();
        rowVels.Clear();

        for (int i = listRoot.childCount - 1; i >= 0; i--)
            Destroy(listRoot.GetChild(i).gameObject);

        for (int i = 0; i < modes.Length; i++)
        {
            var row = Instantiate(rowPrefab, listRoot);
            row.Bind(this, i, modes[i]);

            rows.Add(row);

            var rt = (RectTransform)row.transform;
            rowRects.Add(rt);

            if (!rt.TryGetComponent(out CanvasGroup cg))
                cg = rt.gameObject.AddComponent<CanvasGroup>();
            rowCgs.Add(cg);

            rowVels.Add(Vector2.zero);
        }
    }

    void MoveSelection(int delta)
    {
        int prevIndex = selectedIndex;
        selectedIndex = Mathf.Clamp(selectedIndex + delta, 0, rows.Count - 1);

        if (selectedIndex != prevIndex)
            PlayMoveSe();
    }

    void ApplyReelLayout()
    {
        float dt = Mathf.Min(Time.unscaledDeltaTime, 1f / 30f);

        int visualSelected = 0;
        float best = float.MaxValue;

        for (int i = 0; i < rows.Count; i++)
        {
            var rect = rowRects[i];

            int d = i - selectedIndex;
            float abs = Mathf.Abs(d);

            float x01 = xOffsetByAbsIndex.Evaluate(abs);
            float x = Mathf.Lerp(centerX, rightXMax, x01);
            float y = centerY - d * spacingY;

            var target = new Vector2(x, y);

            var v = rowVels[i];
            var p = Vector2.SmoothDamp(
                rect.anchoredPosition,
                target,
                ref v,
                smoothTime,
                Mathf.Infinity,
                dt
            );

            if ((p - target).sqrMagnitude < snapThreshold * snapThreshold)
                p = target;

            rect.anchoredPosition = p;
            rowVels[i] = v;

            rect.localScale = Vector3.one * scaleByAbsIndex.Evaluate(abs);
            rowCgs[i].alpha = alphaByAbsIndex.Evaluate(abs);

            float dist = Mathf.Abs(rect.anchoredPosition.y - centerY);
            if (dist < best)
            {
                best = dist;
                visualSelected = i;
            }
        }

        for (int i = 0; i < rows.Count; i++)
            rows[i].SetSelected(i == visualSelected);
    }

    IEnumerator Decide(int index)
    {
        isTransitioning = true;

        PlayDecideSe();

        if (decideSe != null)
            yield return new WaitForSecondsRealtime(decideSe.length);

        switch (index)
        {
            case 0: // Arcade
			    ArcadeRunState.StartNewRun();
                SceneManager.LoadScene("ArcadeSongSelectScene");
                break;

            case 1: // FreePlay
                SceneManager.LoadScene("SongSelectScene");
                break;

            case 2: // Option
                SceneManager.LoadScene("OptionScene");
                break;
        }
    }

    public void OnRowClicked(int index)
    {
        if (isTransitioning) return;

        selectedIndex = index;
        StartCoroutine(Decide(index));
    }

    void PlayMoveSe()
    {
        if (seSource != null && moveSe != null)
            seSource.PlayOneShot(moveSe, 0.8f);
    }

    void PlayDecideSe()
    {
        if (seSource != null && decideSe != null)
            seSource.PlayOneShot(decideSe, 2.0f);
    }
}
