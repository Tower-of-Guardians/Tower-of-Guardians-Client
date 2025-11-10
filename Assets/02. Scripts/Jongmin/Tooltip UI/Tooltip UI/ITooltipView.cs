public interface ITooltipView
{
    void Inject(TooltipPresenter presenter);
    
    void OpenUI();
    void UpdateUI(TooltipData tooltip_string);
    void CloseUI();
}