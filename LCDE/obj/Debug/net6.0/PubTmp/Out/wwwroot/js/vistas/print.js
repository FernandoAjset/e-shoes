// Exponer la función en el objeto global window
window.imprimirDocumento = imprimirDocumento;


function imprimirDocumento(base64) {
    printBlob(base64toBlobPdf(base64))
    return;
}


function printBlob(blob) {
    const url = URL.createObjectURL(blob);
    const iframe = document.createElement('iframe');
    iframe.style.display = 'none';
    document.body.appendChild(iframe);

    iframe.onload = function () {
        // Imprimir el documento.
        iframe.contentWindow.print();
    };

    iframe.src = url;
}



function base64toBlobPdf(base64) {
    var contentType = 'application/pdf';

    const byteCharacters = atob(base64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const file = new Blob([byteArray], { type: contentType });
    return file;
}