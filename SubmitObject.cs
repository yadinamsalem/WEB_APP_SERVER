using System;

namespace StudentStatusTrackerServer.Objects
{
    public class SubmitObject
    {
        public string SubmitType { get; set; }
        public int SubmitScore { get; set; }
        public int SubmitAttempt { get; set; }

        public SubmitObject(string type, int score, int attempt)
        {
            this.SubmitType = type;
            this.SubmitScore = score;
            this.SubmitAttempt = attempt;
        }
    }
}