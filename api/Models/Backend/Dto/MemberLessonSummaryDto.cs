namespace npm.api.API.Models.Backend.Dto
{
    using System.Collections.Generic;

    public class MemberLessonSummaryDto
    {
        public int AnonymousTotalCount { get; set; }

        public int MemberTotalCount { get; set; }

        public float AnonymousTotalTime { get; set; }

        public float MemberTotalTime { get; set; }

        public List<MemberLessonStatisticDto> MemberLessonStatisticList { get; set; }
    }
}