using UnityEngine;

public class ResultUIInjector : MonoBehaviour, IInjector
{
    [Header("의존성 목록")]
    [Header("결과 뷰")]
    [SerializeField] private ResultView m_result_view; 

    public void Inject()
    {
        InjectResult();
    }

    public void InjectResult()
    {
        DIContainer.Register<ResultView>(m_result_view);

        var result_presenter = new ResultPresenter(m_result_view);
        DIContainer.Register<ResultPresenter>(result_presenter);
    }
}