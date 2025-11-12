using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HandView : MonoBehaviour, IHandView
{
    [Header("UI 관련 컴포넌트")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Space(30f)]
    [Header("추가 기획 옵션")]
    [Header("카드 배치 반지름")]
    [SerializeField] private float m_radius = 130f;

    [Header("카드의 최대 각도")]
    [SerializeField] private float m_arc_angle = 30f;
    
    [Header("Z축 깊이")]
    [SerializeField] private float m_depth_multiplier = 0.5f;

    [Space(30f)]
    [Header("에디터 테스트 컴포넌트")]
    [Header("테스트 획득 버튼")]
    [SerializeField] private Button m_add_button;

    [Header("테스트 제거 버튼")]
    [SerializeField] private Button m_remove_button;

    [Header("카드 프리펩")]
    [SerializeField] private GameObject m_card_prefab;

    private HandPresenter m_presenter;
    private List<HandCardView> m_card_list;


    public IHandCardView InstantiateCardView()
    {
        // TODO: Object Pool을 통한 카드 생성
        
        var card_obj = Instantiate(m_card_prefab, transform, false);
        
        return card_obj.GetComponent<IHandCardView>();
    }

    public void Inject(HandPresenter presenter)
    {
        m_presenter = presenter;

        m_add_button.onClick.AddListener(m_presenter.InstantiateCard);
        m_remove_button.onClick.AddListener(Test_RemoveCard);
    }

    public void OpenUI()
    {
        ToggleActive(true);
    }

    public void UpdateUI()
    {
        GetChildrenRectTransform();
        MakeArcLike();
    }

    public void CloseUI()
    {
        ToggleActive(false);
    }

    private void ToggleActive(bool active)
    {
        m_canvas_group.alpha = active ? 1f : 0f;
        m_canvas_group.interactable = active;
        m_canvas_group.blocksRaycasts = active;
    }

    private void GetChildrenRectTransform()
    {
        var card_array = transform.GetComponentsInChildren<HandCardView>(false);
        m_card_list = new(card_array);
    }

    private void MakeArcLike()
    {
        var card_count = m_card_list.Count;
        if(card_count <= 0)
            return;

        var step_count = card_count > 1 ? card_count - 1 : 1f;
        var start_angle = card_count > 1 ? -m_arc_angle / 2f : 0f;
        var angle_step = card_count > 1 ? m_arc_angle / step_count : 0f;
        var radius = card_count * m_radius;
        
        for (int i = 0; i < card_count; i++)
        {
            var card = m_card_list[i].transform as RectTransform;

            var angle = start_angle + i * angle_step;
            var rad = angle * Mathf.Deg2Rad;

            var position = new Vector2(radius * Mathf.Sin(rad),
                                       radius * Mathf.Cos(rad) - radius);

            var z = -Mathf.Abs(rad) * m_depth_multiplier;

            card.localPosition = new Vector3(position.x, position.y, z);

            var rotation_z = -angle;
            card.DOLocalRotate(new Vector3(0f, 0f, rotation_z), 0.5f);
        }
    }

#region Test
    private void Test_RemoveCard()
    {
        if(m_card_list.Count == 0)
            return;

        var target = m_card_list[^1];
        m_card_list.Remove(target);

        target.transform.DOKill();
        DestroyImmediate(target.gameObject);

        UpdateUI();
    }
#endregion
}