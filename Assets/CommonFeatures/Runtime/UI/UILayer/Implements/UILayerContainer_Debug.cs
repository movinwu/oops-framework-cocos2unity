using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.UI
{
    /// <summary>
    /// ���Բ�����
    /// </summary>
    public class UILayerContainer_Debug : UILayerContainerBase
    {
        public override EUILayer Layer => EUILayer.Debug;

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