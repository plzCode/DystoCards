using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickCatcher : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("�ݰ� ���� ��� �г�")]
    public GameObject panelToClose;

    public void OnPointerClick(PointerEventData eventData)
    {
        /*if (panelToClose != null) 
        {
            if(UIManager.Instance.hideFeedback !=null && panelToClose.layer == LayerMask.NameToLayer("UI"))
            {
                MMF_Scale scaleFeedback = UIManager.Instance.hideFeedback.GetFeedbackOfType<MMF_Scale>();
                scaleFeedback.AnimateScaleTarget = panelToClose.transform;
                UIManager.Instance.hideFeedback.PlayFeedbacks();

                float delay = UIManager.Instance.hideFeedback.TotalDuration; // MMF_Player���� ����ð� �޾ƿ��� (GetDuration() ����)
                StartCoroutine(UIManager.Instance.DisableAfter(delay, panelToClose));
            }
            else
            {
                panelToClose.SetActive(false);
            }
        }
            
        gameObject.SetActive(false); // ClickCatcher �ڽŵ� ��Ȱ��ȭ*/
    }
}