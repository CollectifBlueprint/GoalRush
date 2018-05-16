namespace LBE
{
    public class BaseEngineComponent : IEngineComponent
    {
        public virtual void Startup() { }
        public virtual void Shutdown() { }

        public virtual void StartFrame() { }
        public virtual void EndFrame() { }

        bool m_started = false;
        public bool Started
        {
            get { return m_started; }
            set { m_started = value; }
        }
    }
}