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

function addToCart(productId, quantity, maxQuantity) {
    quantity = parseInt(quantity, 10);
    productId = productId.toString(); // Convertir el ID a cadena

    if (isNaN(quantity) || quantity <= 0) {
        showToast('error', 'Cantidad no válida.');
        return;
    }

    let cart = JSON.parse(localStorage.getItem('shoppingCart')) || [];
    let productIndex = cart.findIndex(item => item.id.toString() === productId); // Convertir el ID del carrito a cadena
    let existingQuantity = productIndex !== -1 ? parseInt(cart[productIndex].quantity, 10) : 0;

    if (quantity + existingQuantity > maxQuantity) {
        showToast('error', `Ya tienes ${existingQuantity} en el carrito. La existencia total del producto es ${maxQuantity}.`);
        return;
    }

    if (productIndex !== -1) {
        cart[productIndex].quantity = existingQuantity + quantity;
    } else {
        cart.push({ id: productId, quantity: quantity });
    }

    localStorage.setItem('shoppingCart', JSON.stringify(cart));

    // Disparar evento de almacenamiento local
    window.dispatchEvent(new Event('storage'));

    // Mostrar el toast de éxito
    showToast('success', 'Producto agregado al carrito.');

    // Actualizar el contador del carrito
    updateCartCount();
}

document.addEventListener('DOMContentLoaded', function () {
    if (window.location.pathname === '/Ecommerce/ResumenCarrito') {
        let cart = JSON.parse(localStorage.getItem('shoppingCart')) || [];
        fetch('/Ecommerce/ResumenCarrito', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(cart)
        })
            .then(response => response.text())
            .then(html => {
                document.querySelector('.cart_area').innerHTML = html;
            })
            .catch(error => console.error('Error:', error));
    }
});
