// REPORTES
var ReporteVentasPorCategoria = document.getElementById('ReporteVentasPorCategoria').value;

let datosVentasDiariasPorCategoria = [];
console.log(ReporteVentasPorCategoria + "HOLA");

$("#buttonVentasDiariasPorCategoria").click(function () {
    var fecha = document.getElementById("fecha").value;
    var categoria = document.getElementById("categoria").value;


    $.ajax({
        url: ReporteVentasPorCategoria,
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
                        $("<td>").text(item.Producto),
                        $("<td>").text(item.CantidadVendida),
                        $("<td>").text(item.MontoVendido)
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