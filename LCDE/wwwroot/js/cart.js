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

function addToCart(productId, quantity) {
    quantity = parseInt(quantity, 10);

    if (isNaN(quantity) || quantity <= 0) {
        showToast('error', 'Por favor, ingrese una cantidad válida.');
        return;
    }

    let cart = JSON.parse(localStorage.getItem('shoppingCart')) || [];
    let productIndex = cart.findIndex(item => item.id === productId);

    if (productIndex !== -1) {
        let existingQuantity = parseInt(cart[productIndex].quantity, 10);
        if (isNaN(existingQuantity)) {
            existingQuantity = 0;
        }
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
