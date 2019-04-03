﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl.Foundation.MvvmCross.ViewModels.Pomodoro 
{
    public class PomodoroWorkflow : IEquatable<PomodoroWorkflow>
    {
        public string Id { get; }
        public PomodoroWorkflowType Type { get; }
        public string Name { get; }
        public IReadOnlyList<PomodoroWorkflowItem> Items { get; }

        public PomodoroWorkflow(string id, PomodoroWorkflowType type, string name, IEnumerable<PomodoroWorkflowItem> items)
        {
            Id = id;
            Type = type;
            Name = name;
            Items = items.ToList();
        }

        public bool Equals(PomodoroWorkflow other)
        {
            if (other == null)
                return false;

            return other == this || other.Id == Id;
        }
    }
}
