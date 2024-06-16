﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using CommonFeatures.Log;

namespace BundleMaster
{
    public static partial class AssetComponent
    {
        /// <summary>
        /// Bundle初始化的信息
        /// </summary>
        internal static readonly Dictionary<string, BundleRuntimeInfo> BundleNameToRuntimeInfo = new Dictionary<string, BundleRuntimeInfo>();

#if !(Nintendo_Switch || BMWebGL)
        /// <summary>
        /// 初始化
        /// </summary>
        public static async UniTask<bool> Initialize(string bundlePackageName, string secretKey = null)
        {
            if (AssetComponentConfig.AssetLoadMode == EAssetLoadMode.Editor)
            {
                CommonLog.Resource("AssetLoadMode = Develop 不需要初始化Bundle配置文件");
                return false;
            }
            if (BundleNameToRuntimeInfo.ContainsKey(bundlePackageName))
            {
                CommonLog.ResourceError(bundlePackageName + " 重复初始化");
                return false;
            }
            BundleRuntimeInfo bundleRuntimeInfo = new BundleRuntimeInfo(bundlePackageName, secretKey);
            BundleNameToRuntimeInfo.Add(bundlePackageName, bundleRuntimeInfo);

            string filePath = BundleFileExistPath(bundlePackageName, "FileLogs.txt", true);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(filePath))
            {
                await webRequest.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (webRequest.result != UnityWebRequest.Result.Success)
#else
                if (!string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    CommonLog.ResourceError("初始化分包未找到FileLogs 分包名: " + bundlePackageName + "\t" + filePath);
                    return false;
                }
                string fileLogs = webRequest.downloadHandler.text;
                Regex reg = new Regex(@"\<(.+?)>");
                MatchCollection matchCollection = reg.Matches(fileLogs);
                List<string> dependFileName = new List<string>();
                foreach (Match m in matchCollection)
                {
                    string[] fileLog = m.Groups[1].Value.Split('|');
                    LoadFile loadFile = new LoadFile();
                    loadFile.FilePath = fileLog[0];
                    loadFile.AssetBundleName = fileLog[1];
                    
                    if (fileLog.Length > 2)
                    {
                        for (int i = 2; i < fileLog.Length; i++)
                        {
                            dependFileName.Add(fileLog[i]);
                        }
                    }
                    loadFile.DependFileName = dependFileName.ToArray();
                    dependFileName.Clear();
                    bundleRuntimeInfo.LoadFileDic.Add(loadFile.FilePath, loadFile);
                }
            }
            string dependPath = BundleFileExistPath(bundlePackageName, "DependLogs.txt", true);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(dependPath))
            {
                await webRequest.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (webRequest.result != UnityWebRequest.Result.Success)
#else
                if (!string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    CommonLog.ResourceError("初始化分包未找到DependLogs 分包名: " + bundlePackageName + "\t" + dependPath);
                    return false;
                }
                string dependLogs = webRequest.downloadHandler.text;
                Regex reg = new Regex(@"\<(.+?)>");
                MatchCollection matchCollection = reg.Matches(dependLogs);
                foreach (Match m in matchCollection)
                {
                    string[] dependLog = m.Groups[1].Value.Split('|');
                    LoadDepend loadDepend = new LoadDepend();
                    loadDepend.FilePath = dependLog[0];
                    loadDepend.AssetBundleName = dependLog[1];
                    bundleRuntimeInfo.LoadDependDic.Add(loadDepend.FilePath, loadDepend);
                }
            }
            string groupPath = BundleFileExistPath(bundlePackageName, "GroupLogs.txt", true);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(groupPath))
            {
                await webRequest.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (webRequest.result != UnityWebRequest.Result.Success)
#else
                if (!string.IsNullOrEmpty(webRequest.error))
#endif
                {
                    CommonLog.ResourceError("初始化分包未找到GroupLogs 分包名: " + bundlePackageName+ "\t" + groupPath);
                    return false;
                }
                string groupLogs = webRequest.downloadHandler.text;
                Regex reg = new Regex(@"\<(.+?)>");
                MatchCollection matchCollection = reg.Matches(groupLogs);
                foreach (Match m in matchCollection)
                {
                    string[] groupLog = m.Groups[1].Value.Split('|');
                    LoadGroup loadGroup = new LoadGroup();
                    loadGroup.FilePath = groupLog[0];
                    loadGroup.AssetBundleName = groupLog[1];
                    if (groupLog.Length > 2)
                    {
                        for (int i = 2; i < groupLog.Length; i++)
                        {
                            loadGroup.DependFileName.Add(groupLog[i]);
                        }
                    }
                    bundleRuntimeInfo.LoadGroupDic.Add(loadGroup.FilePath, loadGroup);
                    bundleRuntimeInfo.LoadGroupDicKey.Add(loadGroup.FilePath);
                }
            }
            //加载当前分包的shader
            await LoadShader(bundlePackageName);
            return true;
        }
        
        /// <summary>
        /// 加载Shader文件
        /// </summary>
        private static async UniTask LoadShader(string bundlePackageName)
        {
            string shaderPath = BundleFileExistPath(bundlePackageName, "shader_" + bundlePackageName.ToLower(), true);
            if (BundleNameToRuntimeInfo[bundlePackageName].Encrypt)
            {
                byte[] shaderData = await VerifyHelper.GetDecryptDataAsync(shaderPath, null, BundleNameToRuntimeInfo[bundlePackageName].SecretKey);
                if (shaderData == null)
                {
                    return;
                }
                else
                {
                    AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(shaderData);
                    request.completed += operation =>
                    {
                        BundleNameToRuntimeInfo[bundlePackageName].Shader = request.assetBundle;
                        return;
                    };
                }
            }
            else
            {
                byte[] shaderData = await VerifyHelper.GetDecryptDataAsync(shaderPath, null, BundleNameToRuntimeInfo[bundlePackageName].SecretKey);
                if (shaderData == null)
                {
                    return;
                }
                else
                {
                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(BundleFileExistPath(bundlePackageName, "shader_" + bundlePackageName.ToLower(), false));
                    request.completed += operation =>
                    {
                        BundleNameToRuntimeInfo[bundlePackageName].Shader = request.assetBundle;
                        return;
                    };
                }
            }
        }
#endif
        
    }
}