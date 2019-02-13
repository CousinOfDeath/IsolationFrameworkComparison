using System;
using System.IO;
using IsolationFrameWorkComparison.ExternalDependencies;
using IsolationFrameWorkComparison.Interfaces;

namespace IsolationFrameWorkComparison
{
    public class Main
    {
        private ILogger Logger;
        private INotificationService NotificationService;
        private IBusinessRepository Repository;
        
        public Main(ILogger logger, INotificationService notificationService, IBusinessRepository repository)
        {
            this.Logger = logger;
            this.NotificationService = notificationService;
            this.Repository = repository;
        }

        public void HandleBusiness(IBusiness business)
        {
            if (business.Est > DateTime.UtcNow)
            {
                throw new Exception("can't set established date in the future");
            }
            
            Repository.AddBusiness(business);
            
            Logger.Log($"Handled business for {business.Id}");
        }

        public bool IsBusinessGood(Guid id)
        {
            var business = Repository.GetBusiness(id);

            if (business == null)
            {
                NotificationService.Notify(new Uri("https://www.notify.io"), "Could not find business");
                throw new UnauthorizedAccessException("Unauthorized access");
            }
            
            return business.Age() > 5;
        }
        
    }
}