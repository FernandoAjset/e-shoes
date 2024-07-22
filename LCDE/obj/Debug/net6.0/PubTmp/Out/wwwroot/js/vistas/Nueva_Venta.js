// Importar funciones desde el objeto global window
var imprimirDocumento = window.imprimirDocumento;


let detallesFactura = [];
// Obtén la URL de la acción del controlador del atributo HTML o la variable JavaScript
var urlObtenerProductos = document.getElementById('urlObtenerProductos').value;
var urlObtenerClientes = document.getElementById('urlObtenerClientes').value;
var urlObtenerTiposPago = document.getElementById('urlObtenerTiposPago').value;
var urlValidarExistencia = document.getElementById('urlValidarExistencia').value;
var urlRegistrarVenta = document.getElementById('urlRegistrarVenta').value;
var urlPDFShowPDF = document.getElementById('urlPDFShowPDF').value;





$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})

function formatState(state) {
    if (!state.id) {
        return state.text;
    }
    var $state = $(
        '<span>' + state.text + '</span>'
    );
    return $state;
};

cboBuscarCliente

$("#cboBuscarProducto").select2({
    ajax: {
        url: urlObtenerProductos,
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        delay: 250,
        processResults: function (data) {
            return {
                results: data.map((item) => (
                    {
                        id: item.id,
                        text: item.detalle,
                        precio: parseFloat(item.precioUnidad),
                        idPromocion: item.idPromocion,
                        promocion: item.promocion,
                        descuentoPorUnidad: item.descuento
                    }))
            }
        },
        languaje: 'es',
        placeholder: 'Seleccione un producto',
        templateResult: formatState
    }
});


function formatCliente(state) {
    if (!state.id) {
        return state.text;
    }
    var $state = $(
        '<span>' + 'Nombre: ' + state.text + ', NIT: ' + state.nit + '</span>'
    );
    return $state;
};

$("#cboBuscarCliente").select2({
    ajax: {
        url: urlObtenerClientes,
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        delay: 250,
        processResults: function (data) {
            return {
                results: data.map((item) => (
                    {
                        id: item.id,
                        text: item.nombre,
                        NIT: item.nit
                    }))
            }
        },
        languaje: 'es',
        placeholder: 'Seleccione un cliente',
        templateResult: formatCliente
    }
});

function formatPagos(state) {
    if (!state.id) {
        return state.text;
    }
    var $state = $(
        '<span>' + state.text + '</span>'
    );
    return $state;
};

$("#cboIdTipoPago").select2({
    ajax: {
        url: urlObtenerTiposPago,
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        delay: 250,
        processResults: function (data) {
            return {
                results: data.map((item) => (
                    {
                        id: item.id,
                        text: item.tipo,
                    }))
            }
        },
        languaje: 'es',
        placeholder: 'Seleccione un tipo de pago',
        templateResult: formatPagos
    }
});

$("#cboBuscarProducto").on("select2:select", function (e) {
    const data = e.params.data;
    let existe = detallesFactura.filter(x => x.idProducto == data.id);
    if (existe.length > 0) {
        $("#cboBuscarProducto").val("").trigger("change");
    }
    else {
        swal({
            title: 'Ingresar cantidad',
            text: data.text,
            showCancelButton: true,
            closeOnConfirm: false,
            type: "input",
            inputPlaceHolder: "Ingrese cantidad del producto"
        },
            function (valor) {
                if (valor === false) return false;

                if (valor === "") {
                    toastr.warning("", "Debe ingresar una cantidad valida")
                    return false;
                }
                if (isNaN(parseInt(valor))) {
                    toastr.warning("", "Debe ingresar una cantidad valida")
                    return false;
                }
                if (valor <= 0) {
                    toastr.warning("", "Debe ingresar una cantidad valida")
                    return false;
                }


                $.ajax({
                    url: urlValidarExistencia,
                    method: "GET",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: {
                        id: data.id,
                        cantidadIngresada: valor
                    },
                    success: function (response) {
                        if (response) {
                            let detalle = {
                                id: 0,
                                subtotal: (parseFloat(valor) * data.precio).toString(),
                                cantidad: valor,
                                idProducto: data.id,
                                idEncabezadoFactura: 0,
                                nombreProducto: data.text,
                                precioUnidad: data.precio,
                                idPromocion: data.idPromocion,
                                promocion: data.promocion,
                                descuentoPorUnidad: data.descuentoPorUnidad,
                            };
                            detallesFactura.push(detalle);
                            $("#cboBuscarProducto").val("").trigger("change");
                            mostrarProductos_Precios();
                            swal.close();
                        }
                    },
                    error: function (xhr, status, error) {
                        if (xhr.status == 404) {
                            toastr.error("", "Ocurrio un problema al consultar el producto");
                        }
                        else if (xhr.status == 400) {
                            toastr.warning("", `La existencia del producto ${data.text} no es suficiente`);
                        }
                    }
                });

            })
    }
})

function mostrarProductos_Precios() {
    let total = 0;
    let totalDescuento = 0;
    $("#tbProducto tbody").html("")
    detallesFactura.forEach((item) => {
        let descuento = 0;
        if (item.idPromocion == 1) {
            descuento = item.descuentoPorUnidad * item.cantidad;
        }
        else if (item.idPromocion == 3 && item.cantidad >= 2) {
            descuento = item.precioUnidad / 2;
        }
        else if (item.idPromocion == 4 && item.cantidad >= 4) {
            descuento = item.precioUnidad;
        }
        $("#tbProducto tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idProducto", item.idProducto)
                ),
                $("<td>").text(item.nombreProducto),
                $("<td>").text(item.promocion),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precioUnidad),
                $("<td>").text(item.idPromocion == 1 ? item.descuentoPorUnidad : 0),
                $("<td>").text(descuento.toFixed(2)),
                $("<td>").text((item.subtotal - descuento).toFixed(2)),
            )
        )
        item.descuento = descuento.toFixed(2);
        totalDescuento = totalDescuento + descuento;
        total = total + (item.subtotal - descuento);
    })

    $("#txtTotal").val(total.toFixed(2))
    $("#txtDescuento").val(totalDescuento.toFixed(2))
}

$(document).on("click", "button.btn-eliminar", function () {
    const _idProducto = $(this).data("idProducto");
    detallesFactura = detallesFactura.filter(p => p.idProducto != _idProducto);
    mostrarProductos_Precios();

})

$("#btnTerminarVenta").click(function () {
    if (!detallesFactura.length > 0 || detallesFactura == undefined || detallesFactura == null) {
        toastr.warning("", "Debe ingresar productos");
        return;
    }
    idCliente = $("#cboBuscarCliente").val();
    if (idCliente == 0 || idCliente == undefined || idCliente == null) {
        toastr.warning("", "Debe seleccionar un cliente");
        return;
    }
    idTipoPago = $("#cboIdTipoPago").val();
    if (idTipoPago == 0 || idTipoPago == undefined || idTipoPago == null) {
        toastr.warning("", "Debe seleccionar un tipo de pago");
        return;
    }
    const detallesFacturaVenta = detallesFactura;
    const nuevaVenta = {
        encabezadoFactura: {
            id: '0',
            serie: 'FA0152',
            idTipoPago: $("#cboIdTipoPago").val(),
            idCliente: $("#cboBuscarCliente").val()
        },
        detallesFactura: detallesFacturaVenta
    };
    $("#btnTerminarVenta").LoadingOverlay("show");

    fetch(urlRegistrarVenta, {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(nuevaVenta)
    })
        .then(response => {
            $("#btnTerminarVenta").LoadingOverlay("hide");
            if (response.ok) {
                $("#cboBuscarCliente").val("").trigger("change");
                $("#cboIdTipoPago").val("").trigger("change");

                detallesFactura = [];
                mostrarProductos_Precios();
                return response.json();
            } else {
                swal("Error en la solicitud", "Ocurrió un problema al grabar la venta", "error");
                return Promise.reject(response);
            }
        })
        .then(data => {
            imprimirDocumento(data.archiveBase64);
        });
});