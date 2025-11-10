using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TooltipView : MonoBehaviour, ITooltipView
{
    [Header("UI 관련 컴포넌트")]
    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Header("툴팁 텍스트")]
    [SerializeField] private TMP_Text m_tooltip_label; 

    [Space(30f)]
    [Header("추가 기획 옵션")]
    [Header("툴팁 이동 여부")]
    [SerializeField] private bool m_is_translation_active;

    [Header("페이드 시간")]
    [SerializeField] private float m_fade_time;

    private TooltipPresenter m_presenter;
    private Coroutine m_fade_coroutine;

    private void Awake()
    {
        m_canvas_group.interactable = false;
        m_canvas_group.blocksRaycasts = false;
    }

    private void Update()
    {
        if(m_presenter.Active && m_is_translation_active)
        {
            CalculateMousePosition();
        }
    }

    public void Inject(TooltipPresenter presenter)
    {
        m_presenter = presenter;
    }

    public void OpenUI()
    {
        ToggleCanvasGroup(true);
    }

    public void UpdateUI(TooltipData tooltip_data)
    {
        (transform as RectTransform).anchoredPosition = tooltip_data.Position;
        m_tooltip_label.text = tooltip_data.Description;
    }

    public void CloseUI()
    {
        ToggleCanvasGroup(false);
    }

    private void CalculateMousePosition()
    {
        var mouse_position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        var canvas_transform = m_canvas.transform as RectTransform;
        var this_transform = transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas_transform,
                                                                mouse_position,
                                                                null,
                                                                out var local_position);
        
        local_position.x = mouse_position.x > Screen.width * 0.5f ? local_position.x - this_transform.sizeDelta.x * 0.5f
                                                                  : local_position.x + this_transform.sizeDelta.x * 0.5f;

        local_position.y = mouse_position.y > Screen.height * 0.5f ? local_position.y - this_transform.sizeDelta.y * 0.5f
                                                                   : local_position.y + this_transform.sizeDelta.y * 0.5f;

        this_transform.anchoredPosition = local_position;
    }

    private void ToggleCanvasGroup(bool active)
    {
        if(m_fade_coroutine != null)
            StopCoroutine(m_fade_coroutine);

        m_fade_coroutine = StartCoroutine(Co_FadeAlpha(active));
    }

    private IEnumerator Co_FadeAlpha(bool active)
    {
        var elapsed_time = 0f;
        var target_time = m_fade_time;

        var current_alpha = m_canvas_group.alpha;
        var target_alpha = active ? 1f : 0f; 

        while(elapsed_time < target_time)
        {
            elapsed_time += Time.deltaTime;

            var delta = elapsed_time / target_time;
            var new_alpha = Mathf.Lerp(current_alpha, target_alpha, delta);
            m_canvas_group.alpha = new_alpha;

            yield return null;
        }

        m_canvas_group.alpha = target_alpha;
    }
}
