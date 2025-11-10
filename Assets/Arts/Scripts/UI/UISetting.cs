using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : UIBase
{
    private Button btnContinue;
    private Button btnExit;

    public void _btnContinueClick()
    {
        GameManager.Instance.SwitchState(GameStatus.Playing);
        UIManager.Instance.CloseTopPanel();
    }

    public void _btnExitClick()
    {
        GameManager.Instance.SwitchState(GameStatus.End);
        UIManager.Instance.CloseTopPanel();
        UIManager.Instance.ClosePanel(UIPath.game);
        UIManager.Instance.OpenPanel(UIPath.main);
    }
}
