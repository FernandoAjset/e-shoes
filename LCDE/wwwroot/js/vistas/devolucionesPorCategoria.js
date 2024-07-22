// REPORTES
var ReporteDevolucionesPorCategoria = document.getElementById('ReporteDevolucionesPorCategoria').value;

let datosVentasDiariasPorCategoria = [];
console.log(ReporteDevolucionesPorCategoria + "HOLA");

$("#buttonVentasDiariasPorCategoria").click(function () {
    var fecha = document.getElementById("fecha").value;
    var categoria = document.getElementById("categoria").value;


    $.ajax({
        url: ReporteDevolucionesPorCategoria,
        method: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: {
            categoria: categoria,
            fecha: fecha
        },
        success: function (response) {
            console.log(response);

            $("#tbProducto tbody").html("")
            response.map((item) => {

                $("#tbProducto tbody").append(
                    $("<tr>").append(
                        $("<td>").text(item.Categoria),
                        $("<td>").text(item.Fecha),
                        $("<td>").text(item.CantidadDevuelta),
                        $("<td>").text(item.Motivo)
                    )
                )
            });
        },
        error: function (xhr, status, error) {
            if (xhr.status == 404) {
            }
            else if (xhr.status == 400) {
            }
        }
    });
});