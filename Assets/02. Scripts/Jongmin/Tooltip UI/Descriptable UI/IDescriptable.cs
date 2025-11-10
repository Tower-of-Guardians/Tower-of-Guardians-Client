using UnityEngine.EventSystems;

public interface IDescriptable : IPointerEnterHandler, IPointerExitHandler
{
    void Inject(TooltipPresenter tooltip_presenter);
    TooltipData GetTooltipData();
}