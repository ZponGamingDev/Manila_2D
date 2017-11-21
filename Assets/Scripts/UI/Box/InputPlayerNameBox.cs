using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPlayerNameBox : UIBase 
{
    public DecisionBtn enterGameBtn;
    private List<KeyInElement> elements = new List<KeyInElement>();
    private string elmPath = PathConfig.UIElementFolder + "KeyInElement";

    public void EnterGame()
    {
        Dictionary<Color, string> nameTable = new Dictionary<Color, string>();
        for (int iElm = 0; iElm < GameManager.Singleton.numOfPlayer; ++iElm)
        {
            Color c = elements[iElm].playerSign.color;
            string pName = elements[iElm].field.text;
            Player.AddPlayerName(c, pName);
        }
        GameManager.Singleton.Response();
    }

    public override void ShowUI()
    {
        for (int iPlayer = 0; iPlayer < GameManager.Singleton.numOfPlayer; ++iPlayer)
        {
            GameObject obj = ResourceManager.Singleton.LoadResource<GameObject>(elmPath);
            KeyInElement element = Instantiate(obj, transform).GetComponent<KeyInElement>();
            elements.Add(element);
            element.playerSign.color = ColorTable.GetPlayerSignColor(iPlayer);
        }
        enterGameBtn.onClick += EnterGame;
        base.ShowUI();
    }

    public override void CloseUI()
    {
        enterGameBtn.onClick = null;
        base.CloseUI();
    }
}
