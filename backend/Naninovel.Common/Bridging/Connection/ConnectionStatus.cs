using System.Threading.Tasks;

namespace Naninovel.Bridging;

public class ConnectionStatus
{
    public bool Connected => !MaintainTask.IsCompleted;
    public Task MaintainTask { get; }
    public ServerInfo ServerInfo { get; }

    public ConnectionStatus (Task maintainTask, ServerInfo serverInfo)
    {
        MaintainTask = maintainTask;
        ServerInfo = serverInfo;
    }
}
