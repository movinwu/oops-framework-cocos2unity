using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ��ʾ������
    /// </summary>
    public class UILayerContainer_Notice : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Notice;

        public override void HideUI(UILayerContainerModel model)
        {
            throw new System.NotImplementedException();
        }

        public override void ShowUI(UILayerContainerModel model)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnInit()
        {
            
        }
    }
}