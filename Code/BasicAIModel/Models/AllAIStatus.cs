namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// 必须保证包含所有AI的Status，且相对应
    /// </summary>
    public enum AllAIStatus
    {
        AllIdle = 0,

        AllReady = 1,

        AllRunning = 2,

        Chaos = 3,

        Error = 255
    }
}
