public class ResultPresenter
{
    private readonly IResultView m_view;

    public ResultPresenter(IResultView view)
    {
        m_view = view;
        m_view.Inject(this);
    }

    public void OpenUI(ResultData result_data)
    {
        m_view.OpenUI(result_data.Type == BattleResultType.Victory);

        if(result_data.Type == BattleResultType.Victory)
        {
            // TODO: 전투 보상, 전투 상점 활성화
        }
    }

    public void CloseUI()
    {
        m_view.CloseUI();

        // TODO: 전투 보상, 전투 상점 비활성화화
    }
}
