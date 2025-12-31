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

        public int TasksId { get; set; }

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

        public string FailedIn()
        {
            int mt = Subject.MaxTheoryMarks;
            int mi = Subject.MaxInternalMarks;
            int ml = Subject.MaxLabMarks;
            int passingPercent = Subject.PassingPercentEachComponent;
            /*(current / maximum) * 100*/

            double theoryPercent = (TheoryMarks / (Double)mt) * 100;
            double labPercent = (LabMarks / (Double)ml) * 100;
            double internalPercent = (InternalMarks / (Double)mi) * 100;

            if (theoryPercent < passingPercent && labPercent < passingPercent && internalPercent < passingPercent) return "TLI";
            else if (theoryPercent < passingPercent && labPercent < passingPercent) return "TL";
            else if (theoryPercent < passingPercent && internalPercent < passingPercent) return "TI";
            else if (internalPercent < passingPercent && labPercent < passingPercent) return "IL";
            else if (internalPercent < passingPercent) return "I";
            else if (theoryPercent < passingPercent) return "T";
            else if (labPercent < passingPercent) return "L";
            return "P";
        }


    }
}
