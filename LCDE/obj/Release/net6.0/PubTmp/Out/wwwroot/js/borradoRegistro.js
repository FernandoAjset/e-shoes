function showDeleteConfirmation(id, tipo) {
    Swal.fire({
        title: '¿Estás seguro de borrar el registro ?',
        icon: 'warning',
        showCancelButton: true,
        showDenyButton: true,
        confirmButtonColor: '#3085d6',
        confirmButtonText: 'Confirmar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            switch (tipo) {
                case 'cliente':
                    deleteClient(id);
                    break;
                case 'proveedor':
                    deleteProveedor(id);
                    break;
                case 'producto':
                    deleteProducto(id);
                    break;
            }   
        }
    })

}

function deleteClient(id) {
    $.ajax({
        url: 'Clientes/BorrarCliente',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            // Manejar la respuesta del servidor
            if (response.success) {
                Swal.fire(
                    '¡Eliminado!',
                    'El registro ha sido eliminado.',
                    'success'
                );
            } else {
                Swal.fire(
                    '¡Error!',
                    'Ocurrió un error durante la operación',
                    'error'
                );
            }
            setTimeout(function () {
                window.location.href = 'Clientes/Index';
            }, 1500);
        },
        error: function () {
            Swal.fire(
                '¡Error!',
                'Ocurrió un error en la petición AJAX',
                'error'
            );
        }
    });
}


function deleteProveedor(id) {
    $.ajax({
        url: 'Proveedores/BorrarProveedor',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            // Manejar la respuesta del servidor
            if (response.success) {
                Swal.fire(
                    '¡Eliminado!',
                    'El registro ha sido eliminado.',
                    'success'
                );
            } else {
                Swal.fire(
                    '¡Error!',
                    'Ocurrió un error durante la operación',
                    'error'
                );
            }
            setTimeout(function () {
                window.location.href = 'Proveedores/Index';
            }, 1500);
        },
        error: function () {
            Swal.fire(
                '¡Error!',
                'Ocurrió un error en la petición AJAX',
                'error'
            );
        }
    });
}


function deleteProducto(id) {
    $.ajax({
        url: 'Productos/BorrarProducto',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            // Manejar la respuesta del servidor
            if (response.success) {
                Swal.fire(
                    '¡Eliminado!',
                    'El registro ha sido eliminado.',
                    'success'
                );
            } else {
                Swal.fire(
                    '¡Error!',
                    'Ocurrió un error durante la operación',
                    'error'
                );
            }
            setTimeout(function () {
                window.location.href = 'Productos/Index';
            }, 1000);
        },
        error: function () {
            Swal.fire(
                '¡Error!',
                'Ocurrió un error en la petición AJAX',
                'error'
            );
        }
    });
}