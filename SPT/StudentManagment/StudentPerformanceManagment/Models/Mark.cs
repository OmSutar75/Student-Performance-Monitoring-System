using StudentPerformanceManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagment.Models
{
    public class Mark
    {
        [Key]
        public int MarkId { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int TaskId { get; set; }

        public Tasks Tasks { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int TheoryMarks { get; set; }
        public int LabMarks { get; set; }
        public int InternalMarks { get; set; }

        public int TotalObtained => this.TheoryMarks + this.LabMarks + this.InternalMarks;

        public int MaxTotal => Subject.MaxTheoryMarks + Subject.MaxLabMarks + Subject.MaxInternalMarks; // Returns 100
        public string ResultStatus
        {
            get
            {

                double theoryPass = Subject.MaxTheoryMarks * 0.40;   // 16
                double labPass = Subject.MaxLabMarks * 0.40;         // 16
                double internalPass = Subject.MaxInternalMarks * 0.40; // 8

                if (this.TheoryMarks >= theoryPass &&
                    this.LabMarks >= labPass &&
                    this.InternalMarks >= internalPass)
                {
                    return "Pass";
                }

                return "Fail";
            }
        }

        public double Percentage
        {
            get
            {
                if (MaxTotal == 0) return 0;
                return (double)TotalObtained / MaxTotal * 100;
            }
        }

        public string Remarks
        {
            get
            {
                var reasons = new List<string>();

                if (this.TheoryMarks < (Subject.MaxTheoryMarks * 0.40))
                    reasons.Add("Failed Theory");
                if (this.LabMarks < (Subject.MaxLabMarks * 0.40))
                    reasons.Add("Failed Lab");
                if (this.InternalMarks < (Subject.MaxInternalMarks * 0.40))
                    reasons.Add("Failed Internal");

                if (reasons.Count == 0) return "Promoted";

                return "Fail: " + string.Join(", ", reasons);
            }
        }



    }
}
