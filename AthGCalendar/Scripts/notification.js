document.addEventListener('DOMContentLoaded', function () {

    if (Notification.permission !== "granted") {
        Notification.requestPermission();
    }

});

(() => fetch(`${window.location.origin}/GoogleCalendar/GetNextEvent`)
    .then(x => x.json()).then(x => notify(x.title, new Date(x.start), 'a'))
    .catch(err => console.warn(err))
)()

function notify(title, desc, url) {

    if (Notification.permission !== "granted") {
        Notification.requestPermission();
    }
    else {
        var notification = new Notification(title, {
            body: desc,
        });

    }
}

console.log('notify.js')