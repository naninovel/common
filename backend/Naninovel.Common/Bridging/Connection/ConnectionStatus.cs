namespace Naninovel.Bridging;

public class ConnectionStatus (Task maintainTask, ServerInfo serverInfo)
{
    public bool Connected => !MaintainTask.IsCompleted;
    public Task MaintainTask { get; } = maintainTask;
    public ServerInfo ServerInfo { get; } = serverInfo;
}
