using System;
using System.Threading.Tasks;

namespace JSSoft.Communication.Shells
{
    public interface IShell : IDisposable
    {
        Task StartAsync();

        Task StopAsync();

        string Title { get; set; }
    }
}