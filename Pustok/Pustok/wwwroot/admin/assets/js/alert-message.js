const alertMessageConnection = new signalR.HubConnectionBuilder()
    .withUrl("/alert-hub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

alertMessageConnection.on("ReceiveAlertMessage", (data) => {
    AddToAlertBox(data);
});

try {
    alertMessageConnection.start();
    console.log("SignalR Connected.");
} catch (err) {
    console.log(err);
}


function AddToAlertBox(data) {
    let alertMessagesBox = document.getElementById("alert-messages-box");
    let alertMessage = `<a href="#" class="dropdown-item py-3" >
        <small class="float-end text-muted ps-2">${data.createdAt}</small>
                <div class="media">
                    <div class="avatar-md bg-soft-primary">
                        <i class="ti ti-chart-arcs"></i>
                    </div>
                    <div class="media-body align-self-center ms-2 text-truncate">
                        <h6 class="my-0 fw-normal text-dark">${data.title}</h6>
                        <small   class="text-muted mb-0">${data.content}</small>
                    </div><!--end media-body-->
                </div><!--end media-->
            </a>`;

    alertMessagesBox.innerHTML += alertMessage;
}