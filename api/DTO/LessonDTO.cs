namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 課程
    /// </summary>
    [Table]
    public partial class LessonDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int LessonID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int LessonCategoryID { get; set; }

        /// <summary>
        /// 課程名稱
        /// </summary>
        public string LessonName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int VideoLength { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string LessonLength { get; set; }

        /// <summary>
        /// 課程簡介
        /// </summary>
        public string Brief { get; set; }

        /// <summary>
        /// 課程目標
        /// </summary>
        [DBIgnoreWhenList]
        public string Goal { get; set; }

        /// <summary>
        /// 課程對象
        /// </summary>
        [DBIgnoreWhenList]
        public string Target { get; set; }

        /// <summary>
        /// 章節主題
        /// </summary>
        [DBIgnoreWhenList]
        [DBJson]
        public IEnumerable<LessonChapterSubjectDTO> ChapterSubjects { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool HasChapterSubject { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool HasHomeWork { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ChapterTitle { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int VisitCount { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        ///
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        ///
        /// </summary>
        [DBNow]
        public DateTime CreateTime { get; set; }

        public class LessonChapterSubjectDTO
        {
            public int ID { get; set; }

            public string Name { get; set; }
        }
    }
}
