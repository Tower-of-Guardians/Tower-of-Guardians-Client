using UnityEngine;

public class HandCardPresenter
{
    private readonly IHandCardView m_view;
    private string m_card_id;

    public HandCardPresenter(IHandCardView view)
    {
        m_view = view;
    }

    public void InitUI(CardData card_data)
    {
        m_card_id = card_data.Id;
        m_view.InitUI(card_data);
    }
}
