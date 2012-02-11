using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Scheduler;

namespace WeatherHelper
{
    public static class TaskHelper
    {
        public static void AddTask(string taskName, string description)
        {
            RemoveTask(taskName);
            PeriodicTask task = new PeriodicTask(taskName);
            task.Description = description;
            ScheduledActionService.Add(task);
        }

        public static void RemoveTask(string taskName)
        {
            var oldTask = ScheduledActionService.Find(taskName) as PeriodicTask;
            if (oldTask != null)
            {
                ScheduledActionService.Remove(taskName);
            }
        }
    }
}
