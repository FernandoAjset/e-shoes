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

function loadOrderView() {
    let cart = JSON.parse(localStorage.getItem('shoppingCart')) || [];
    cart = cart.map(createCarritoItemDTO); // Crear objetos CarritoItemDTO con valores por defecto
    console.log(cart); // Verificar los datos del carrito
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
        })
        .catch(error => console.error('Error:', error));
}

document.addEventListener('DOMContentLoaded', function () {
    if (window.location.pathname === '/Ecommerce/ConfirmarOrden') {
        loadOrderView();
    }
});