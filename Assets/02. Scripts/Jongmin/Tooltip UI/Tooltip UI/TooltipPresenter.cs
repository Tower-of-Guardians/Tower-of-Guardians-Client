public class TooltipPresenter
{
    private readonly ITooltipView m_view;

    public bool Active { get; private set; }

    public TooltipPresenter(ITooltipView view)
    {
        m_view = view;
        m_view.Inject(this);
    }

    public void OpenUI(IDescriptable descriptable)
    {
        Active = true;

        m_view.OpenUI();
        m_view.UpdateUI(descriptable.GetTooltipData());
    }

    public void CloseUI()
    {
        Active = false;

        m_view.CloseUI();
    }
}