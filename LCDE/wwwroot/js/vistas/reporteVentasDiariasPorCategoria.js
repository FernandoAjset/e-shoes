// REPORTES
var VentasDiariasPorCategoria = document.getElementById('VentasDiariasPorCategoría').value;
let datosVentasDiariasPorCategoria = [];
console.log(VentasDiariasPorCategoria + "HOLA");

$("#buttonVentasDiariasPorCategoria").click(function () {
    var fecha = document.getElementById("fecha").value;


    $.ajax({
        url: VentasDiariasPorCategoria,
        method: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: {
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
                        $("<td>").text(item.CantidadVendida),
                        $("<td>").text(item.MontoDescuento),
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