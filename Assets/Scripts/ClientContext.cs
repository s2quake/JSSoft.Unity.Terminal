using JSSoft.Communication;

namespace JSSoft.Communication.Shells
{
    class ClientContext : ClientContextBase
    {
        public ClientContext(params IServiceHost[] serviceHosts)
            : base(serviceHosts)
        {

        }
    }
}