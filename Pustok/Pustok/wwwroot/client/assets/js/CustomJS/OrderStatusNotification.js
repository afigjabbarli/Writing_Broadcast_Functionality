const orderStatusNotificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/order-status-notification-hub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

orderStatusNotificationConnection.on("SendOrderStatusNotification", (data) => {
    AddToAlertBox(data);
});

try {
    orderStatusNotificationConnection.start();
    console.log("SignalR Connected.");
} catch (err) {
    console.log(err);
}


function AddToAlertBox(data) {
   
    let alertMessagesBox = document.getElementById("notificationList");

   
    let orderStatusNotification = `
    <div class="notification-item">
        <div class="icon unread-icon">
            <i class="fas fa-envelope fa-xl"></i>
        </div>
        <div class="icon read-icon">
            <i class="fas fa-envelope fa-xl"></i>
        </div>
        <div class="content">
            <div class="notification-title">${data.Title}</div>
            <div class="notification-text">${data.Content}</div>
        </div>
        <div class="date">
            <span>${data.CreatedAt}</span>
        </div>
        <div class="commands">
            <a href="#">
                <i class="fas fa-eye"></i>
            </a>
            <a href="#">
                <i class="fas fa-trash"></i>
            </a>
        </div>
    </div>
`;

    alertMessagesBox.innerHTML += orderStatusNotification;


   

    
}