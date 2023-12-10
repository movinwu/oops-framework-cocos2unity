using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFeatures
{
    /// <summary>
    /// ����ͨ�ù���
    /// <para>��Ҫ�������м̳�monobehaviour���������ں���,������Ҫ,��������������麯��,���Ӻ����̳�,����CFMͳһ����,�Ա���Ƶ���˳��</para>
    /// </summary>
    public class CommonFeature : MonoBehaviour
    {
        /// <summary>
        /// ��ʼ������
        /// <para>�滻�������ں���Awake</para>
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// ÿ֡��ѯ����
        /// <para>�滻�������ں���Update</para>
        /// </summary>
        public virtual void Tick()
        {

        }

        /// <summary>
        /// �ͷź���
        /// <para>�滻�������ں���OnDestroy</para>
        /// </summary>
        public virtual void Release()
        {

        }
    }
}