using ShyrochenkoPatterns.ScheduledTasks.Schedule.Cron;
using System;

namespace ShyrochenkoPatterns.ScheduledTasks.Schedule
{
    /// <summary>
    /// Task with schedule defined by Cron template 
    /// </summary>
    public class ScheduledTask 
    {
        // Cron string pattern
        protected string Schedule;

        private CrontabSchedule _cronSchedule;
        private DateTime _lastRunTime;
        protected DateTime _nextRunTime;

        /// <summary>
        /// An action wich will be triggered after task finished
        /// </summary>
        public void Increment()
        {
            if (Schedule == null)
                throw new ArgumentException(nameof(Schedule));

            if(_cronSchedule == null)
                _cronSchedule = CrontabSchedule.Parse(Schedule);

            _lastRunTime = _nextRunTime;
            _nextRunTime = _cronSchedule.GetNextOccurrence(_nextRunTime);
        }

        /// <summary>
        /// Check task to running 
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public bool ShouldRun(DateTime currentTime)
        {
            return _nextRunTime <= currentTime && _lastRunTime != _nextRunTime;
        }
    }
}
