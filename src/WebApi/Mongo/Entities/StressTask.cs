using MongoDB.Bson;
using System;

namespace WebApi.Mongo.Entities
{
    public class StressTask : Entity
    {
        public ObjectId SceneId { get; set; }

        public string SceneName { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string Body { get; set; }

        public int Thread { get; set; }

        public int Connection { get; set; }

        public int Duration { get; set; }

        public string Unit { get; set; }

        public StressTaskStatus Status { get; set; }

        public StressTaskFrom From { get; set; }

        public Device Device { get; set; }

        public string Script { get; set; }

        public string Command { get; set; }

        public string Message { get; set; }

        public bool IsBaseline { get; set; }

        public TaskResult Result { get; set; }

        public string Caller { get; set; }

        public DateTime StartRunningTime { get; set; }

        public DateTime EndRunningTime { get; set; }

        public DateTime CreationTime { get; set; }
    }

    public enum StressTaskStatus
    {
        Queue = 0,

        Running = 1,

        Done = 2,

        Error = 3
    }

    public enum StressTaskFrom
    {
        Console = 0,

        Callback = 1,

        Schedule = 2
    }

    public class TaskResult
    {
        public string Content { get; set; }

        public double Qps { get; set; }

        public double LatencyP50 { get; set; }

        public double LatencyP75 { get; set; }

        public double LatencyP90 { get; set; }

        public double LatencyP99 { get; set; }

        public double LatencyAvg { get; set; }

        public double LatencyStd { get; set; }

        public double LatencyMax { get; set; }
    }
}
