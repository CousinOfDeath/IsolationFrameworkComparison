using System;
using IsolationFrameWorkComparison.Interfaces;

namespace IsolationFrameWorkComparison.ExternalDependencies
{
    public interface IBusinessRepository
    {
        IBusiness GetBusiness(Guid id);

        void AddBusiness(IBusiness business);
    }
}