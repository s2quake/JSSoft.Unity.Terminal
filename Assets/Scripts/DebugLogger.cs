
namespace JSSoft.Communication.Shells
{
    class DebugLogger : JSSoft.Communication.Logging.ILogger
    {
        public void Debug(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Info(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Error(object message)
        {

        }

        public void Warn(object message)
        {

        }

        public void Fatal(object message)
        {

        }
    }
}