using UnityEngine;

public class TooltipUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("툴팁 뷰")]
    [SerializeField] private TooltipView m_tooltip_view;

    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    private IDescriptable[] m_descriptables;

    private void Awake()
    {
        m_descriptables = m_canvas.GetComponentsInChildren<IDescriptable>();
    }

    public void Inject()
    {
        InstallTooltip();
        InjectTooltip();
    }

    private void InstallTooltip()
    {
        DIContainer.Register<ITooltipView>(m_tooltip_view);

        var tooltip_presenter = new TooltipPresenter(m_tooltip_view);
        DIContainer.Register<TooltipPresenter>(tooltip_presenter);
    }

    private void InjectTooltip()
    {
        var tooltip_presenter = DIContainer.Resolve<TooltipPresenter>();

        foreach(var descriptable in m_descriptables)
            descriptable.Inject(tooltip_presenter);
    }
}