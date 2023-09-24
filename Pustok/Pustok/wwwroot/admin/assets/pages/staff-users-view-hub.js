const staffUsersViewConnection = new signalR.HubConnectionBuilder()
    .withUrl("/staff-users-view-hub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

staffUsersViewConnection.on("UserOnlineStatusUpdate", (data) => {
    const spanId = `online-tracked-${data.userId}`;
    const spanElement = document.getElementById(spanId);

    if (spanElement) {

        if (data.isOnline) {
            spanElement.className = 'badge badge-soft-success';
            spanElement.textContent = 'Online';
        } else {
            spanElement.className = 'badge badge-soft-danger';
            spanElement.textContent = 'Offline';
        }
    }

});

try {
    staffUsersViewConnection.start();
    console.log("SignalR Connected.");
} catch (err) {
    console.log(err);
}