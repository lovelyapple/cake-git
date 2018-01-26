using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugWindow : MonoBehaviour
{
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
	[SerializeField] WindowIndex debugWindowIndex;
    [SerializeField] GameState debugGameState;
    void OnGUI()
    {
        if (!GameMainObject.Get().IsDebugMode) { return; }
        if (GUI.Button(new Rect(0, 0, Screen.width / 6, Screen.height / 8), "LoadWindow"))
        {
            ResourcesManager.Get().CreateOpenWindow(debugWindowIndex, (w) =>
        {
            w.MoveToTop();
        });
        }
        if (GUI.Button(new Rect(0, Screen.height / 8, Screen.width / 6, Screen.height / 8), "CloseWindow"))
        {
            ResourcesManager.Get().CloseWindow(debugWindowIndex);
        }
        if (GUI.Button(new Rect(0, Screen.height / 8 * 2, Screen.width / 6, Screen.height / 8), "ClearWindow"))
        {
            ResourcesManager.Get().CloseWindow(debugWindowIndex);
        }
        if (GUI.Button(new Rect(0, Screen.height / 8 * 3, Screen.width / 6, Screen.height / 8), "ChangeState"))
        {
            GameMainObject.Get().RequestChangeState(debugGameState);
        }
    }
}
