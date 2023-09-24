
const onlineUserTrackerConnection = new signalR.HubConnectionBuilder()
    .withUrl("/online-user-hub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await onlineUserTrackerConnection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

onlineUserTrackerConnection.onclose(async () => {
    await start();
});

// Start the connection.
start();