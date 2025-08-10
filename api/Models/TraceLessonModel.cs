namespace npm.api.API.Models
{
    using System;

    public class TraceLessonModel
    {
        public string Name { get; set; }

        public int Progress { get; set; }

        public DateTime? FinishTime { get; set; }

        public int? CorrectCount { get; set; }

        public int? TotalCount { get; set; }
    }
}