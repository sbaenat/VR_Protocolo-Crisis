using UnityEngine;

public class NeedButton : MonoBehaviour
{
    public NeedBar targetBar;

    public void UseButton()
    {
        if (targetBar != null)
        {
            targetBar.ResolveFailure();
        }
    }
}