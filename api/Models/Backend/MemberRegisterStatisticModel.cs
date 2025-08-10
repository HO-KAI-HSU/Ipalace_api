namespace npm.api.API.Models.Backend
{
    public class MemberRegisterStatisticModel
    {
        public string Title { get; set; }

        public int Type { get; set; }

        public int Count { get; set; }

        public float GrowthRate { get; set; } = 1.0f;

        public int GeneralTotalRegisterCount { get; set; } = 0;

        public int StudentTotalRegisterCount { get; set; } = 0;

        public int TeacherTotalRegisterCount { get; set; } = 0;

        public int TotalRegisterCount { get; set; } = 0;
    }
}