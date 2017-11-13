using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InitialSceneUI
{
    public class InitialInfoPage : UIBase
    {   
        private InitialInfoPageDataSystem initialInfoPageDataSystem;
        public Image[] pages;

        void Awake()
        {
            //InitialInfoPageDataSystem.singleton.Parse();
            initialInfoPageDataSystem = new InitialInfoPageDataSystem();
        }

        void Start()
        {
            
        }

        public override void Initial()
        {
            initialInfoPageDataSystem.SetPage(pages, "Name");
        }

        public override void ShowUI()
        {
            base.ShowUI();
        }

        public override void CloseUI()
        {
            base.CloseUI();
        }

        public void LoadData()
        {
            initialInfoPageDataSystem.LoadData();
        }
    }

}