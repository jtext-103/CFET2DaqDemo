namespace Jtext103.CFET2.Things.DAQDataUploadThing
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum Status
    {
        Idle = 0,
        Ready = 1,
        Running = 2,
        Uploading = 3,
        Error = 255
    }
}
