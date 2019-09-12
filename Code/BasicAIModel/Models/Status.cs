namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum Status
    {
        //表示可以Arm
        Idle = 0,
        //表示已经Arm，等待触发
        Ready = 1,
        //已经触发，正在接受数据
        Running = 2,

        Error = 255
    }
}
