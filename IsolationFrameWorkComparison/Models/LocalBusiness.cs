using System;
using IsolationFrameWorkComparison.Interfaces;

namespace IsolationFrameWorkComparison.Models
{
    public class LocalBusiness : IBusiness
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Est { get; set; }
        public int Age()
        {
            return DateTime.UtcNow.Year - Est.Year;
        }
    }
}