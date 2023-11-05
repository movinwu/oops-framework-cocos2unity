using Google.Protobuf;
using System.IO;

namespace OOPS
{
    /// <summary>
    /// 通信协议
    /// </summary>
    public abstract class Protocol<T> : IProtocol where T : IMessage<T>
    {
        public abstract short Key { get; }

        /// <summary>
        /// proto数据
        /// </summary>
        protected T Data { get; private set; }

        public void SetData(T data)
        {
            this.Data = data;
        }

        public void ReceiveMessage(byte[] data)
        {
            if (null == this.Data)
            {
                this.Data = System.Activator.CreateInstance<T>();
            }

            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    ms.Write(data, 2, data.Length - 2);
                    ms.Position = 0;
                    this.Data.MergeFrom(ms);
                }

                this.OnReceive();
            }
            catch (System.Exception ex)
            {
                Logger.NetError($"消息反序列化失败, 类型为: {this.Data.GetType()}");
                Logger.NetException(ex);
            }
        }

        public void SendMessage(byte[] data)
        {
            NetManager.Instance.SendMsg(data);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage()
        {
            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    this.Data.WriteTo(ms);
                    byte[] buffer = new byte[ms.Length + 2];
                    var msgId = ProtocolManager.Instance.GetProtocolId(this.Data.GetType());
                    buffer[0] = (byte)(msgId >> 8);
                    buffer[1] = (byte)msgId;
                    ms.Position = 0;
                    ms.Read(buffer, 2, (int)ms.Length);
                    SendMessage(buffer);
                }
            }
            catch (System.Exception ex)
            {
                Logger.NetError($"消息序列化失败,类型为: {this.Data.GetType()}");
                Logger.NetException(ex);
            }
        }

        /// <summary>
        /// 当收到消息时(处理消息)
        /// </summary>
        protected abstract void OnReceive();
    }
}
