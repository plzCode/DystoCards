using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickCatcher : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("닫고 싶은 대상 패널")]
    public GameObject panelToClose;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (panelToClose != null)
        {
            if (UIManager.Instance.hideFeedback != null && panelToClose.layer == LayerMask.NameToLayer("UI"))
            {
                MMF_Scale scaleFeedback = UIManager.Instance.hideFeedback.GetFeedbackOfType<MMF_Scale>();
                scaleFeedback.AnimateScaleTarget = panelToClose.transform;
                UIManager.Instance.hideFeedback.PlayFeedbacks();

                float delay = UIManager.Instance.hideFeedback.TotalDuration; // MMF_Player에서 재생시간 받아오기 (GetDuration() 참고)
                //UIManager.Instance.StartCoroutine(UIManager.Instance.DisableAfter(delay, panelToClose));
                UIManager.Instance.TogglePanel(panelToClose);
            }
            else
            {
                UIManager.Instance.TogglePanel(panelToClose);
                //panelToClose.SetActive(false);
            }
        }

        gameObject.SetActive(false); // ClickCatcher 자신도 비활성화
    }
}