namespace npm.api.API.Models.Backend.Dto
{
    public class LessonStatisticDto
    {
        public int LessonID { get; set; }

        public string LessonName { get; set; }

        public int TotalCount { get; set; }

        public float TotalTime { get; set; }

        public int DoneCount { get; set; } = 0;
    }
}