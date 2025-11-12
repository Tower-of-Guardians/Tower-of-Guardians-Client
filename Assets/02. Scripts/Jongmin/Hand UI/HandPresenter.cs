public class HandPresenter
{
    private readonly IHandView m_view;

    public HandPresenter(IHandView view)
    {
        m_view = view;
        m_view.Inject(this);
    }

    public void InstantiateCard()
    {
        var card_view = m_view.InstantiateCardView();
        var card_presenter = new HandCardPresenter(card_view);

        m_view.UpdateUI();
    }

    public void OpenUI()
    {
        m_view.OpenUI();
    }

    public void CloseUI()
    {
        m_view.CloseUI();
    }
}
