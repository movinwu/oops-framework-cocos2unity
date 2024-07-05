using CommonFeatures.FSM;
using CommonFeatures.Resource;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures.GML
{
    /// <summary>
    /// GameMainLoop
    /// 游戏主循环
    /// </summary>
    public class CommonFeature_GML : CommonFeature
    {
        /// <summary>
        /// 游戏主循环状态机
        /// </summary>
        private FSM<CommonFeature_GML> m_FSM;

        public async UniTask StartGame()
        {
            //正式开始游戏
            var states = new FSMState<CommonFeature_GML>[]
            {
                new FSMState_GML_InitializePackage(),
                new FSMState_GML_StartGame(),
                new FSMState_GML_CreatePackageDownloader(),
                new FSMState_GML_DownloadPackageFiles(),
                new FSMState_GML_UpdatePackageManifest(),
                new FSMState_GML_UpdatePackageVersion(),
            };
            m_FSM = CommonFeaturesManager.FSM.CreateFSM(states, this);

            //初始化数据
            var blackboard = new GameMainLoopBlackboard();
            var config = CommonFeaturesManager.Config.GetConfig<ResourceConfig>();
            blackboard.DefaultBuildPipeline = config.DefaultBuildPipeline;
            blackboard.PackageName = config.PackageName;
            blackboard.PlayMode = config.PlayMode;
            m_FSM.BlackBoard = blackboard;

            //显示加载界面
            await CommonFeaturesManager.UI.ShowBaseUI(UI.EBaseLayerUIType.Splash);
            
            //开始初始化包
            await m_FSM.StartFSM<FSMState_GML_InitializePackage>();
        }
    }
}
