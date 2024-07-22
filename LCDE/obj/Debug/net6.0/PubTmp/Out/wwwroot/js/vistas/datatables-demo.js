// Call the dataTables jQuery plugin

$(document).ready(function () {
    $('#tbdata').DataTable({
        responsive: true,
        ordering: false,
        dom: "Bfrtip",
        buttons: [
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
});
