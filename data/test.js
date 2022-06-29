db.Device.insert({
    "_id" : ObjectId("62b0e60752880189ed9b6982"),
    "TotalMem" : 1837,
    "AvailableMem" : 349,
    "CpuCores" : 1,
    "CpuModel" : "Intel(R) Xeon(R) Platinum 8255C CPU @ 2.50GHz",
    "CpuFrequency" : 2494.14,
    "CpuCacheSize" : 36608.0
});

db.StressScene.insert({
    "_id" : ObjectId("62bcaeb963217ca8573dedf4"),
    "Name" : "My Scene",
    "Url" : "http://localhost/api/tests/get",
    "Method" : "GET",
    "Thread" : 1,
    "Connection" : 1,
    "Duration" : 5,
    "Unit" : "s",
    "CreationTime" : ISODate("2022-06-29T19:57:45.018Z")
});

db.StressTask.insert({
    "_id" : ObjectId("62bcafbf63217ca8573dedf5"),
    "SceneId" : ObjectId("62bcaeb963217ca8573dedf4"),
    "SceneName" : "My Scene",
    "Url" : "http://localhost/api/tests/get",
    "Method" : "GET",
    "Thread" : 1,
    "Connection" : 1,
    "Duration" : 5,
    "Unit" : "s",
    "Status" : 2,
    "Device" : {
        "TotalMem" : 1837,
        "AvailableMem" : 383,
        "CpuCores" : 0,
        "CpuFrequency" : 0.0,
        "CpuCacheSize" : 0.0
    },
    "Script" : "wrk.method = \"GET\"\nwrk.headers[\"Content-Type\"] = \"application/json\"\n",
    "Command" : "wrk -t 1 -c 1 -s /tmp/5f0bdc6a-287a-4531-816e-dbcc5a7a6d40.lua -d 5s --latency http://localhost/api/tests/get",
    "IsBaseline" : false,
    "Result" : {
        "Content" : "Running 5s test @ http://localhost/api/tests/get\n  1 threads and 1 connections\n  Thread Stats   Avg      Stdev     Max   +/- Stdev\n    Latency    27.07ms   90.08ms 529.00ms   91.97%\n    Req/Sec     2.92k   688.59     3.34k    93.33%\n  Latency Distribution\n     50%  298.00us\n     75%  330.00us\n     90%   60.66ms\n     99%  471.54ms\n  13214 requests in 5.01s, 3.93MB read\nRequests/sec:   2639.01\nTransfer/sec:    804.07KB\n",
        "Qps" : 2639.01,
        "LatencyP50" : 0.298,
        "LatencyP75" : 0.33,
        "LatencyP90" : 60.66,
        "LatencyP99" : 471.54,
        "LatencyAvg" : 27.07,
        "LatencyStd" : 90.08,
        "LatencyMax" : 529.0
    },
    "StartRunningTime" : ISODate("2022-06-29T20:02:09.348Z"),
    "EndRunningTime" : ISODate("2022-06-29T20:02:14.428Z"),
    "CreationTime" : ISODate("2022-06-29T20:02:07.270Z")
});

db.StressTask.insert({
    "_id" : ObjectId("62bcb05163217ca8573dedf6"),
    "SceneId" : ObjectId("62bcaeb963217ca8573dedf4"),
    "SceneName" : "My Scene",
    "Url" : "http://localhost/api/tests/get",
    "Method" : "GET",
    "Thread" : 1,
    "Connection" : 1,
    "Duration" : 5,
    "Unit" : "s",
    "Status" : 2,
    "Device" : {
        "TotalMem" : 1837,
        "AvailableMem" : 364,
        "CpuCores" : 0,
        "CpuFrequency" : 0.0,
        "CpuCacheSize" : 0.0
    },
    "Script" : "wrk.method = \"GET\"\nwrk.headers[\"Content-Type\"] = \"application/json\"\n",
    "Command" : "wrk -t 1 -c 1 -s /tmp/70a109af-4ed3-4cc6-9e16-dd2c85c3c728.lua -d 5s --latency http://localhost/api/tests/get",
    "IsBaseline" : false,
    "Result" : {
        "Content" : "Running 5s test @ http://localhost/api/tests/get\n  1 threads and 1 connections\n  Thread Stats   Avg      Stdev     Max   +/- Stdev\n    Latency   365.18us  751.85us  14.26ms   97.87%\n    Req/Sec     3.39k   251.95     3.63k    86.27%\n  Latency Distribution\n     50%  262.00us\n     75%  286.00us\n     90%  335.00us\n     99%    3.80ms\n  17215 requests in 5.10s, 5.12MB read\nRequests/sec:   3375.57\nTransfer/sec:      1.00MB\n",
        "Qps" : 3375.57,
        "LatencyP50" : 0.262,
        "LatencyP75" : 0.286,
        "LatencyP90" : 0.335,
        "LatencyP99" : 3.8,
        "LatencyAvg" : 0.36518,
        "LatencyStd" : 0.75185,
        "LatencyMax" : 14.26
    },
    "StartRunningTime" : ISODate("2022-06-29T20:04:34.326Z"),
    "EndRunningTime" : ISODate("2022-06-29T20:04:39.452Z"),
    "CreationTime" : ISODate("2022-06-29T20:04:33.476Z")
});

db.StressTask.insert({
    "_id" : ObjectId("62bcb09b63217ca8573dedf7"),
    "SceneId" : ObjectId("62bcaeb963217ca8573dedf4"),
    "SceneName" : "My Scene",
    "Url" : "http://localhost/api/tests/get",
    "Method" : "GET",
    "Thread" : 1,
    "Connection" : 1,
    "Duration" : 5,
    "Unit" : "s",
    "Status" : 2,
    "Device" : {
        "TotalMem" : 1837,
        "AvailableMem" : 348,
        "CpuCores" : 0,
        "CpuFrequency" : 0.0,
        "CpuCacheSize" : 0.0
    },
    "Script" : "wrk.method = \"GET\"\nwrk.headers[\"Content-Type\"] = \"application/json\"\n",
    "Command" : "wrk -t 1 -c 1 -s /tmp/32149c6a-64c6-4be1-b545-7c54a7da7aea.lua -d 5s --latency http://localhost/api/tests/get",
    "IsBaseline" : false,
    "Result" : {
        "Content" : "Running 5s test @ http://localhost/api/tests/get\n  1 threads and 1 connections\n  Thread Stats   Avg      Stdev     Max   +/- Stdev\n    Latency    39.37ms  125.97ms 668.12ms   91.19%\n    Req/Sec     3.16k   486.16     3.38k    97.73%\n  Latency Distribution\n     50%  289.00us\n     75%  322.00us\n     90%   96.95ms\n     99%  611.04ms\n  13906 requests in 5.00s, 4.14MB read\nRequests/sec:   2781.06\nTransfer/sec:    847.35KB\n",
        "Qps" : 2781.06,
        "LatencyP50" : 0.289,
        "LatencyP75" : 0.322,
        "LatencyP90" : 96.95,
        "LatencyP99" : 611.04,
        "LatencyAvg" : 39.37,
        "LatencyStd" : 125.97,
        "LatencyMax" : 668.12
    },
    "StartRunningTime" : ISODate("2022-06-29T20:05:49.327Z"),
    "EndRunningTime" : ISODate("2022-06-29T20:05:54.356Z"),
    "CreationTime" : ISODate("2022-06-29T20:05:47.036Z")
});