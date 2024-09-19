function showToast(type, message, duration = 1500) {
    let toastElement, toastBodyElement;

    if (type === 'success') {
        toastElement = document.getElementById('success-toast');
        toastBodyElement = document.getElementById('success-toast-body');
    } else if (type === 'error') {
        toastElement = document.getElementById('error-toast');
        toastBodyElement = document.getElementById('error-toast-body');
    }

    if (toastElement && toastBodyElement) {
        toastBodyElement.textContent = message;
        let toast = new bootstrap.Toast(toastElement, { delay: duration });
        toast.show();
    }
}

document.addEventListener('DOMContentLoaded', function () {
    if (window.location.pathname === '/Ecommerce/ConfirmarOrden') {
        loadOrderView();
    }
});

function loadOrderView() {
    let cart = JSON.parse(localStorage.getItem('shoppingCart')) || [];
    cart = cart.map(createCarritoItemDTO); // Crear objetos CarritoItemDTO con valores por defecto
    fetch('/Ecommerce/ConfirmarOrden', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(cart)
    })
        .then(response => response.text())
        .then(html => {
            document.querySelector('.order_area').innerHTML = html;

            // Asegurarse de que el elemento 'pagar' exista antes de agregar el evento
            const pagarButton = document.getElementById('pagar');
            if (pagarButton) {
                pagarButton.addEventListener('click', function () {
                    // Obtener los datos del cliente desde los inputs
                    const clienteInfo = {
                        id: document.getElementById('id').value,
                        id_usuario: 0,
                        nit: document.getElementById('nit').value,
                        nombre: document.getElementById('nombre').value,
                        direccion: document.getElementById('direccion').value,
                        telefono: document.getElementById('telefono').value,
                        correo: document.getElementById('correo').value
                    };

                    // Obtener las observaciones
                    const observaciones = document.getElementById('observaciones').value;

                    // Obtener los detalles del carrito desde el localStorage
                    let carrito = JSON.parse(localStorage.getItem('shoppingCart')) || [];
                    carrito = carrito.map(createCarritoItemDTO);

                    // Crear el objeto ConfirmarOrdenDTO
                    const confirmarOrdenDTO = {
                        clienteInfo: clienteInfo,
                        detallesCarrito: carrito,
                        observaciones: observaciones
                    };

                    // Enviar la información al método CrearOrden del controlador
                    fetch('/Ecommerce/CrearOrden', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify(confirmarOrdenDTO)
                    })
                        .then(response => response.json())
                        .then(data => {

                            if (data.success) {
                                // Limpiar el localstorage
                                localStorage.removeItem('shoppingCart');

                                showToast('success', 'Orden creada exitosamente');
                                // Llamar a PayPal para realizar el pago
                                return fetch('/Ecommerce/Paypal', {
                                    method: 'POST',
                                    headers: {
                                        'Content-Type': 'application/json',
                                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                                    },
                                    body: JSON.stringify({
                                        precio: data.precio.toString(),
                                        descripcion: data.descripcion,
                                        idOrden: data.idOrden
                                    })
                                });
                            } else {
                                showToast('error', 'Error al generar enlace de pago para la orden');
                            }
                        })
                        .then(response => response.json())
                        .then(data => {
                            if (data.status) {
                                var jsonresult = JSON.parse(data.respuesta);
                                var links = jsonresult.links;

                                var resultado = links.find(item => item.rel === "approve")

                                window.location.href = resultado.href
                            } else {
                                showToast('error', 'No se pudo acceder a PayPal, vuelva a intentarlo más tarde');
                            }
                        })
                        .catch(error => console.error('Error:', error));
                });
            } else {
                console.error('El botón "pagar" no se encontró en el DOM.');
            }
        })
        .catch(error => console.error('Error:', error));
}