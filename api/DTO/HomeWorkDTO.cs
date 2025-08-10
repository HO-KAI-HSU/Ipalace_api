namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 課後習題
    /// </summary>
    [Table]
    public partial class HomeWorkDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey]
        public int LessonID { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBJson]
        public IEnumerable<HomeWorkQuestion> Question { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBJson]
        public IEnumerable<HomeWorkData> Data { get; set; }

        public int Count { get => Question.Count(); }

        public class HomeWorkQuestion
        {
            public string Question { get; set; }

            public IEnumerable<string> Answers { get; set; }
        }

        public class HomeWorkData
        {
            public int CorrectAnswer { get; set; }

            public string Description { get; set; }
        }
    }
}
