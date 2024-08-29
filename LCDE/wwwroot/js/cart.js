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

function addToCart(productId, quantity, maxQuantity, precioUnidad) {
    quantity = parseInt(quantity, 10);
    productId = productId.toString(); // Convertir el ID a cadena

    if (isNaN(quantity) || quantity <= 0) {
        showToast('error', 'Cantidad no válida.');
        return;
    }

    let cart = JSON.parse(localStorage.getItem('shoppingCart')) || [];
    let productIndex = cart.findIndex(item => item.idProducto.toString() === productId); // Convertir el ID del carrito a cadena
    let existingQuantity = productIndex !== -1 ? parseInt(cart[productIndex].cantidad, 10) : 0;

    if (quantity + existingQuantity > maxQuantity) {
        showToast('error', `Ya tienes ${existingQuantity} en el carrito. La existencia total del producto es ${maxQuantity}.`);
        return;
    }

    if (productIndex !== -1) {
        cart[productIndex].cantidad = existingQuantity + quantity;
    } else {
        // Aquí se crea el objeto con las propiedades en minúscula
        cart.push({ idProducto: productId, cantidad: quantity, nombreProducto: '', precioUnidad: precioUnidad });
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

function loadCartView() {
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
            attachEventListeners();
        })
        .catch(error => console.error('Error:', error));
}

document.addEventListener('DOMContentLoaded', function () {
    if (window.location.pathname === '/Ecommerce/ResumenCarrito') {
        loadCartView();
    }
});

function removeFromCart(productId) {
    let cart = JSON.parse(localStorage.getItem('shoppingCart')) || [];
    cart = cart.filter(item => item.idProducto.toString() !== productId.toString());
    localStorage.setItem('shoppingCart', JSON.stringify(cart));

    // Disparar evento de almacenamiento local
    window.dispatchEvent(new Event('storage'));

    // Mostrar el toast de éxito
    showToast('success', 'Producto eliminado del carrito.');

    // Actualizar el contador del carrito
    updateCartCount();

    // Recargar la vista del carrito
    loadCartView();
}

function updateCartItem(productId, quantity, maxQuantity) {
    quantity = parseInt(quantity, 10);
    productId = productId.toString(); // Convertir el ID a cadena

    if (isNaN(quantity) || quantity <= 0) {
        showToast('error', 'Cantidad no válida.');
        return;
    }

    let cart = JSON.parse(localStorage.getItem('shoppingCart')) || [];
    let productIndex = cart.findIndex(item => item.idProducto.toString() === productId); // Convertir el ID del carrito a cadena

    if (productIndex !== -1) {
        let previousQuantity = cart[productIndex].cantidad; // Almacenar la cantidad anterior

        if (quantity > maxQuantity) {
            showToast('error', `La cantidad no puede superar la existencia total del producto (${maxQuantity}).`);
            document.querySelector(`input[data-product-id="${productId}"]`).value = previousQuantity;
            return;
        } else {
            cart[productIndex].cantidad = quantity;
            localStorage.setItem('shoppingCart', JSON.stringify(cart));
            // Disparar evento de almacenamiento local
            window.dispatchEvent(new Event('storage'));
        }
        // Mostrar el toast de éxito
        showToast('success', 'Cantidad actualizada.');
    }
}

function attachEventListeners() {
    document.querySelectorAll('.remove-item').forEach(button => {
        button.addEventListener('click', function () {
            let productId = this.dataset.productId;
            removeFromCart(productId);
        });
    });

    document.querySelectorAll('.update-quantity').forEach(input => {
        input.addEventListener('change', function () {
            let productId = this.dataset.productId;
            let quantity = this.value;
            let maxQuantity = this.dataset.maxQuantity;
            updateCartItem(productId, quantity, maxQuantity);
        });
    });
}