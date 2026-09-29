using UnityEngine;

public class AttractScreen1 : MonoBehaviour
{
    public void OnAnimationFinished()
    {
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.ShowNextAttractScreen();
        }
    }
}
