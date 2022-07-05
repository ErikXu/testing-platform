using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace WebApi.Mongo.Entities
{
    public class ApiTask : Entity
    {
        public ObjectId SceneId { get; set; }

        public string SceneName { get; set; }

        public string Collection { get; set; }

        public string Environment { get; set; }

        public ApiTaskStatus Status { get; set; }

        public string Command { get; set; }

        public string Message { get; set; }

        public NewmanResult Result { get; set; }

        public bool IsSuccess { get; set; }

        public DateTime StartRunningTime { get; set; }

        public DateTime EndRunningTime { get; set; }

        public DateTime CreationTime { get; set; }
    }

    public enum ApiTaskStatus
    {
        Queue = 0,

        Running = 1,

        Done = 2,

        Error = 3
    }

    public class NewmanResult
    {
        public NewmanCollection Collection { get; set; }

        public NewmanRun Run { get; set; }
    }

    public class NewmanCollection
    {
        public List<NewmanItem> Item { get; set; }

        public NewmanInfo Info { get; set; }
    }

    public class NewmanInfo
    {
        public string Name { get; set; }

        public string Schema { get; set; }
    }

    public class NewmanItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public NewmanRequest Request { get; set; }
    }

    public class NewmanRequest
    {
        public NewmanUrl Url { get; set; }

        public string Method { get; set; }
    }

    public class NewmanUrl
    {
        public string Protocol { get; set; }

        public List<string> Path { get; set; }

        public List<string> Host { get; set; }
    }

    public class NewmanRun
    {
        public NewmanStats Stats { get; set; }

        public NewmanTimings Timings { get; set; }

        public List<NewmanExecutions> Executions { get; set; }

        public NewmanTransfers Transfers { get; set; }
    }

    public class NewmanStats
    {
        public NewmanStatsItems Iterations { get; set; }

        public NewmanStatsItems Items { get; set; }

        public NewmanStatsItems Scripts { get; set; }

        public NewmanStatsItems Prerequests { get; set; }

        public NewmanStatsItems Requests { get; set; }

        public NewmanStatsItems Tests { get; set; }

        public NewmanStatsItems Assertions { get; set; }

        public NewmanStatsItems TestScripts { get; set; }

        public NewmanStatsItems PrerequestScripts { get; set; }
    }

    public class NewmanStatsItems
    {
        public int Total { get; set; }

        public int Pending { get; set; }

        public int Failed { get; set; }
    }

    public class NewmanTimings
    {
        public double ResponseAverage { get; set; }

        public int ResponseMin { get; set; }

        public int ResponseMax { get; set; }

        public double ResponseSd { get; set; }

        public long Started { get; set; }

        public long Completed { get; set; }
    }

    public class NewmanExecutions
    {
        public NewmanExecutionResponse Response { get; set; }
    }

    public class NewmanExecutionResponse
    {
        public string Status { get; set; }

        public int Code { get; set; }

        public int ResponseTime { get; set; }

        public int ResponseSize { get; set; }
    }


    public class NewmanTransfers
    {
        public int responseTotal { get; set; }
    }
}
