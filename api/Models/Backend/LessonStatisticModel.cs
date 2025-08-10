namespace npm.api.API.Models.Backend
{
    public class LessonStatisticModel
    {
        public int LessonID { get; set; }

        public int TotalCount { get; set; }

        public float TotalTime { get; set; }

        public int DoneCount { get; set; }
    }
}