using System;
using System.Threading.Tasks;
using JSSoft.UI;

namespace JSSoft.Communication.Shells
{
    public interface IShell : IDisposable
    {
        Task StartAsync();

        Task StopAsync();

        string Title { get; set; }

        string Prompt { get; set; }
    }
}