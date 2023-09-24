using Pustok.Database.Models;

namespace Pustok.Services.Abstracts
{
    public interface INotificationService
    {
        void SendOrderNotification(Order order);
        public void SendOrderApprovedNotification(Order order);
        public void SendOrderRejectedNotification(Order order);
        public void SendOrderSentNotification(Order order);
        public void SendOrderCompletedNotification(Order order);


        public void SendingToCustomersOrderStatusNotifications(Order order);

        public void SendOrderConfirmationNotice(Order order);
        public void SendOrderRejectionNotice(Order order);
        public void SendOrderShippingNotice(Order order);
        public void SendOrderCompleteNotice(Order order);

    }
}
