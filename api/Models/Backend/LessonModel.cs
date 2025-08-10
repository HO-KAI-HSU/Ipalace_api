namespace npm.api.API.Models.Backend
{
    using npm.api.DTO;
    using System.Collections.Generic;

    public class LessonModel
    {
        public LessonDTO Lesson { get; set; }

        public HomeWorkDTO HomeWork { get; set; }

        public IEnumerable<LessonChapterDTO> Chapters { get; set; }
    }
}