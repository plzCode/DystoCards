using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;

public class MMF_Func : MonoBehaviour
{
    public MMF_Player mmf_Players;

    private void OnEnable()
    {
        if(mmf_Players != null)
        {
            mmf_Players.Initialization();
        }
    }

    public float PlayFeedBack_ByName(string feedbackName)
    {
        float duration = 0f;

        foreach (MMF_Feedback feedback in mmf_Players.FeedbacksList)
        {
            feedback.Active = feedback.Label == feedbackName;

            if (feedback.Active)
            {
                duration = Mathf.Max(duration, feedback.TotalDuration); // 총 재생 시간 가져오기
            }
        }

        mmf_Players.PlayFeedbacks();

        return duration;
    }

}
