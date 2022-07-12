﻿using System;

namespace WebApi.Mongo.Entities
{
    public class Agent : Entity
    {
        public string AgentAddress { get; set; }

        public DateTime LastHeartbeat { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
