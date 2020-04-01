using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.ScheduledTasks.Schedule.Cron
{
    public delegate void CrontabFieldAccumulator(int start, int end, int interval);
}
