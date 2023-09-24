namespace Pustok.Contracts
{
    public static class OrderStatusNotificationTemplates
    {
        public class Subject
        {
            public const string ORDER_STATUS_UPDATE = "Order status updated!";
        }
        public class Content
        {
            public const string APPROVED = "Dear customer {Surname} {Name}, your order #{Order_Tracking_Code} has been confirmed.";
            public const string REJECTED = "Dear customer {Surname} {Name}, your order #{Order_Tracking_Code} has been rejected.";
            public const string SENT = "Dear customer {Last Name} {First Name}, your order #{Order_Tracking_Code} has been shipped. The courier will contact you as soon as possible";
            public const string COMPLETED = "Dear customer {Surname} {Name}, your order #{Order_Tracking_Code} has been delivered.Thank you for using our services...";
        }
    }
}
