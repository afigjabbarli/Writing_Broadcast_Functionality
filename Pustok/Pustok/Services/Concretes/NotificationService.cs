using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pustok.Areas.Admin.ViewModels.AlertMessage;
using Pustok.Contracts;
using Pustok.Database;
using Pustok.Database.Models;
using Pustok.Exceptions;
using Pustok.Hubs;
using Pustok.Services.Abstracts;
using Pustok.ViewModels;
using System.Text;

namespace Pustok.Services.Concretes;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly PustokDbContext _pustokDbContext;
    private readonly IUserService _userService;
    private readonly IHubContext<AlertMessageHub> _hubContext;
    private readonly IAlertMessageService _aletMessageService;
    private readonly IHubContext<OrderStatusNotificationHub> _orderStatusNotificationHubContext;
    private readonly IOrderStatusNotificationService _orderStatusNotificationService;

    public NotificationService(
        IEmailService emailService,
        PustokDbContext pustokDbContext,
        IUserService userService,
        IHubContext<AlertMessageHub> hubContext,
        IOrderStatusNotificationService orderStatusNotificationService,
        IAlertMessageService aletMessageService,
        IHubContext<OrderStatusNotificationHub> orderStatusNotificationHubContext)
       
    {
        _emailService = emailService;
        _pustokDbContext = pustokDbContext;
        _userService = userService;
        _hubContext = hubContext;
        _aletMessageService = aletMessageService;
        _orderStatusNotificationService = orderStatusNotificationService;
        _orderStatusNotificationHubContext = orderStatusNotificationHubContext;
    }

    public void SendOrderNotification(Order order)
    {
        switch (order.Status)
        {
            case OrderStatus.Created:
                SendOrderCreatedNotification(order);
                break;
            case OrderStatus.Approved:
                SendOrderApprovedNotification(order);
                break;
            case OrderStatus.Rejected:
                SendOrderRejectedNotification(order);
                break;
            case OrderStatus.Sent:
                SendOrderSentNotification(order);
                break;
            case OrderStatus.Completed:
                SendOrderCompletedNotification(order);
                break;
            default:
                throw new NotificationNotImplementedException();
        }
    }

    public void SendOrderCreatedNotification(Order order)
    {
        var content = PrepareOrderCreatedAlertMessageContent(order);
        var staffMembers = _userService.GetAllStaffMembers();
        
        foreach (var staffMember in staffMembers)
        {
            var alertMessage = new AlertMessage
            {
                Title = AlertMessageTemplates.Order.TITLE,
                Content = content,
                UserId = staffMember.Id
            };

            _pustokDbContext.AlertMessages.Add(alertMessage);

            var connectIds = _aletMessageService.GetConnectionIds(staffMember);

            var alerMessageViewModel = new AlertMessageViewModel
            {
                Title = alertMessage.Title,
                Content = alertMessage.Content,
                CreatedAt = DateTime.Now
            };

            _hubContext.Clients
                .Clients(connectIds)
                .SendAsync("ReceiveAlertMessage", alerMessageViewModel)
                .Wait();
        }
    }
    public string PrepareOrderCreatedAlertMessageContent(Order order)
    {
        var templateBuilder = new StringBuilder(AlertMessageTemplates.Order.CREATED)
            .Replace("{order_number}", order.TrackingCode);

        return templateBuilder.ToString();
    }



    public void SendOrderApprovedNotification(Order order)
    {
        var message = PrepareOrderApprovedMessage(order);
        _emailService.SendEmail(EmailTemplates.Order.SUBJECT, message, order.User.Email);
    }
    private string PrepareOrderApprovedMessage(Order order)
    {
        var templayeBuilder = new StringBuilder(EmailTemplates.Order.APPROVED)
            .Replace("{firstName}", order.User.Name)
            .Replace("{lastName}", order.User.LastName)
            .Replace("{order_number}", order.TrackingCode);

        return templayeBuilder.ToString();
    }


    public void SendOrderCompletedNotification(Order order)
    {
        var message = PrepareOrderCompletedMessage(order);
        _emailService.SendEmail(EmailTemplates.Order.SUBJECT, message, order.User.Email);
    }
    private string PrepareOrderCompletedMessage(Order order)
    {
        var templayeBuilder = new StringBuilder(EmailTemplates.Order.COMPLETED)
            .Replace("{firstName}", order.User.Name)
            .Replace("{lastName}", order.User.LastName)
            .Replace("{order_number}", order.TrackingCode);

        return templayeBuilder.ToString();
    }


    public void SendOrderRejectedNotification(Order order)
    {
        var message = PrepareOrderRejectedMessage(order);
        _emailService.SendEmail(EmailTemplates.Order.SUBJECT, message, order.User.Email);
    }
    private string PrepareOrderRejectedMessage(Order order)
    {
        var templayeBuilder = new StringBuilder(EmailTemplates.Order.REJECTED)
            .Replace("{firstName}", order.User.Name)
            .Replace("{lastName}", order.User.LastName)
            .Replace("{order_number}", order.TrackingCode);

        return templayeBuilder.ToString();
    }


    public void SendOrderSentNotification(Order order)
    {
        var message = PrepareOrderRejectedMessage(order);
        _emailService.SendEmail(EmailTemplates.Order.SUBJECT, message, order.User.Email);
    }
    private string PrepareOrderSentMessage(Order order)
    {
        var templayeBuilder = new StringBuilder(EmailTemplates.Order.SENT)
            .Replace("{firstName}", order.User.Name)
            .Replace("{lastName}", order.User.LastName)
            .Replace("{order_number}", order.TrackingCode);

        return templayeBuilder.ToString();
    }






    public void SendingToCustomersOrderStatusNotifications(Order order)
    {
          //var user = _pustokDbContext.Users.SingleOrDefault(u => u.Id == order.UserId);
          //if(user is not null)
          //{

          //}
                switch (order.Status)
                {
                     case OrderStatus.Created:
                           SendOrderCreatedNotice(order);  
                     break;
                     case OrderStatus.Approved:
                           SendOrderConfirmationNotice(order);
                     break;
                     case OrderStatus.Rejected:
                          SendOrderRejectionNotice(order);
                     break;
                     case OrderStatus.Sent:
                          SendOrderShippingNotice(order); 
                     break;
                     case OrderStatus.Completed:
                          SendOrderCompleteNotice(order); 
                     break;
                     default:
                          throw new NotificationNotImplementedException();
                }
    }


    public void SendOrderCreatedNotice(Order order)
    {
        var user = _userService.GetAllCustomers().SingleOrDefault(u => u.Id == order.UserId);
        var notification = PrepareNotificationContentForOrderCreatedStatus(order, user);
        if (user != null)
        {
            var OrderStatusNotification = new AlertMessage
            {
                Title = OrderStatusNotificationTemplates.Subject.ORDER_STATUS_UPDATE,
                Content = notification,
                UserId = user.Id,

            };
            _pustokDbContext.AlertMessages.Add(OrderStatusNotification);
            _pustokDbContext.SaveChanges(); 
            var connectionIds = _orderStatusNotificationService.GetConnectionIds(user);

            var OrderStatusNotificationViewModel = new OrderStatusNotificationViewModel
            {
                Content = OrderStatusNotification.Content,
                Title = OrderStatusNotification.Title,
                CreatedAt = DateTime.Now
            };
            OrderStatusNotificationViewModel.CreatedAt.ToString("dd/MM/yyyy");
            _hubContext.Clients.Clients(connectionIds)
            .SendAsync("SendOrderStatusNotification", OrderStatusNotificationViewModel)
            .Wait();
        }
    }
    private string PrepareNotificationContentForOrderCreatedStatus(Order order, User user)
    {
        var templayeBuilder = new StringBuilder(OrderStatusNotificationTemplates.Content.CREATED)
            .Replace("{Surname}", user.LastName)
            .Replace("{Name}", user.Name)
            .Replace("{Order_Tracking_Code}", order.TrackingCode);
        return templayeBuilder.ToString();
    }


    public void SendOrderConfirmationNotice(Order order)
    {
        var user = _userService.GetAllCustomers().SingleOrDefault(u => u.Id == order.UserId);
        var notification = PrepareNotificationContentForOrderApprovedStatus(order, user);
        if (user != null)
        {
            var OrderStatusNotification = new AlertMessage
            {
                Title = OrderStatusNotificationTemplates.Subject.ORDER_STATUS_UPDATE,
                Content = notification,
                UserId = user.Id,
               
            };
            _pustokDbContext.AlertMessages.Add(OrderStatusNotification);
            _pustokDbContext.SaveChanges();
            var connectionIds = _orderStatusNotificationService.GetConnectionIds(user);

            var OrderStatusNotificationViewModel = new OrderStatusNotificationViewModel 
            { Content = OrderStatusNotification.Content, Title = OrderStatusNotification.Title, 
              CreatedAt = DateTime.Now};
            OrderStatusNotificationViewModel.CreatedAt.ToString("dd/MM/yyyy");
            _hubContext.Clients.Clients(connectionIds)
            .SendAsync("SendOrderStatusNotification", OrderStatusNotificationViewModel)
            .Wait();
        }
    }
    private string PrepareNotificationContentForOrderApprovedStatus(Order order, User user)
    {
        var templayeBuilder = new StringBuilder(OrderStatusNotificationTemplates.Content.APPROVED)
            .Replace("{Surname}", user.LastName)
            .Replace("{Name}", user.Name)
            .Replace("{Order_Tracking_Code}", order.TrackingCode);
        return templayeBuilder.ToString();  
    }

    public void SendOrderRejectionNotice(Order order)
    {
        var user = _userService.GetAllCustomers().SingleOrDefault(u => u.Id == order.UserId);
        var notification = PrepareNotificationContentForOrderRejectedStatus(order, user);
        if (user != null)
        {
            var OrderStatusNotification = new AlertMessage
            {
                Title = OrderStatusNotificationTemplates.Subject.ORDER_STATUS_UPDATE,
                Content = notification,
                UserId = user.Id,
               
            };
            _pustokDbContext.AlertMessages.Add(OrderStatusNotification);
            _pustokDbContext.SaveChanges();
            var connectionIds = _orderStatusNotificationService.GetConnectionIds(user);
            var OrderStatusNotificationViewModel = new OrderStatusNotificationViewModel
            {
                Content = OrderStatusNotification.Content,
                Title = OrderStatusNotification.Title,
                CreatedAt = DateTime.Now,
               
            };

            OrderStatusNotificationViewModel.CreatedAt.ToString("dd/MM/yyyy");
            _hubContext.Clients.Clients(connectionIds)
            .SendAsync("SendOrderStatusNotification", OrderStatusNotificationViewModel)
            .Wait();
        }
    }
    private string PrepareNotificationContentForOrderRejectedStatus(Order order, User user)
    {
        var templayeBuilder = new StringBuilder(OrderStatusNotificationTemplates.Content.REJECTED)
            .Replace("{Surname}", user.LastName)
            .Replace("{Name}", user.Name)
            .Replace("{Order_Tracking_Code}", order.TrackingCode);
        return templayeBuilder.ToString();
    }

    public void SendOrderShippingNotice(Order order)
    {
       
        var user = _userService.GetAllCustomers().SingleOrDefault(u => u.Id == order.UserId);
        var notification = PrepareNotificationContentForOrderSentStatus(order, user);
        if (user != null)
        {
            var OrderStatusNotification = new AlertMessage
            {
                Title = OrderStatusNotificationTemplates.Subject.ORDER_STATUS_UPDATE,
                Content = notification,
                UserId = user.Id,
               
            };
            _pustokDbContext.AlertMessages.Add(OrderStatusNotification);
            _pustokDbContext.SaveChanges();
            var connectionIds = _orderStatusNotificationService.GetConnectionIds(user);

            var OrderStatusNotificationViewModel = new OrderStatusNotificationViewModel
            {
                Content = OrderStatusNotification.Content,
                Title = OrderStatusNotification.Title,
                CreatedAt = DateTime.Now,
               
            };
            OrderStatusNotificationViewModel.CreatedAt.ToString("dd/MM/yyyy");
            _hubContext.Clients.Clients(connectionIds)
            .SendAsync("SendOrderStatusNotification", OrderStatusNotificationViewModel)
            .Wait();
        }
    }
    private string PrepareNotificationContentForOrderSentStatus(Order order, User user)
    {
        var templayeBuilder = new StringBuilder(OrderStatusNotificationTemplates.Content.SENT)
            .Replace("{Surname}", user.LastName)
            .Replace("{Name}", user.Name)
            .Replace("{Order_Tracking_Code}", order.TrackingCode);
        return templayeBuilder.ToString();
    }


    public void SendOrderCompleteNotice(Order order)
    {
       
        var user = _userService.GetAllCustomers().SingleOrDefault(u => u.Id == order.UserId);
        var notification = PrepareNotificationContentForOrderCompletedStatus(order, user);
        if (user != null)
        {
            var OrderStatusNotification = new AlertMessage
            {
                Title = OrderStatusNotificationTemplates.Subject.ORDER_STATUS_UPDATE,
                Content = notification,
                UserId = user.Id,
               
            };
            _pustokDbContext.AlertMessages.Add(OrderStatusNotification);
            _pustokDbContext.SaveChanges();
            var connectionIds = _orderStatusNotificationService.GetConnectionIds(user);

            var OrderStatusNotificationViewModel = new OrderStatusNotificationViewModel
            {
                Content = OrderStatusNotification.Content,
                Title = OrderStatusNotification.Title,
                CreatedAt = DateTime.Now,
              
            };
            OrderStatusNotificationViewModel.CreatedAt.ToString("dd/MM/yyyy");
            _hubContext.Clients.Clients(connectionIds)
            .SendAsync("SendOrderStatusNotification", OrderStatusNotificationViewModel)
            .Wait();
        }
    }
    private string PrepareNotificationContentForOrderCompletedStatus(Order order, User user )
    {
        var templayeBuilder = new StringBuilder(OrderStatusNotificationTemplates.Content.COMPLETED)
            .Replace("{Surname}", user.LastName)
            .Replace("{Name}", user.Name)
            .Replace("{Order_Tracking_Code}", order.TrackingCode);
        return templayeBuilder.ToString();
    }


}
