using System;
using System.Collections.Generic;

namespace Conf2021GraphQL.Models
{
 
    public partial class ExamPaper
    {
        public ExamPaper()
        {
            UserExams = new HashSet<UserExam>();
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Memo { get; set; }

        public DateTime CreateTime { get; set; }      

        public virtual ICollection<UserExam> UserExams { get; set; }    
        public virtual ICollection<Question> Questions { get; set; }
    }
}
