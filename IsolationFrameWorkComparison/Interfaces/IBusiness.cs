using System;

namespace IsolationFrameWorkComparison.Interfaces
{
    public interface IBusiness
    {
        Guid Id { get; set; }
        
        string Name { get; set; }
        
        DateTime Est { get; set; }

        int Age();

    }
}