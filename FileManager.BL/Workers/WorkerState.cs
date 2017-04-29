using System;

namespace FileManager.BL.Workers
{
    [Flags]
    [Serializable]
    public enum WorkerState
    {
        Running = 0,
        Unstarted = 1,
        Suspended = 2,
        Stopped = 4,
    }
}