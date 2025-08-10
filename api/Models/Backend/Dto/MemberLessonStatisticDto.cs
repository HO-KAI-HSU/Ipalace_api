namespace npm.api.API.Models.Backend.Dto
{
    public class MemberLessonStatisticDto
    {
        public int MemberID { get; set; }

        public string MemberName { get; set; }

        public string School { get; set; }

        public int TotalCount { get; set; }

        public float TotalTime { get; set; }
    }
}