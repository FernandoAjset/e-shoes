use [e-shoes_dev]

INSERT INTO categoria_productos
(nombre)
VALUES('Caballero'),
('Damas'),
('Ni�os'),
('Tenis')


INSERT INTO proveedores
(nombre, direccion, telefono, NIT)
VALUES ('Oxford', 'GT', '22000001','84845152');

INSERT INTO productos
(detalle, existencia, id_categoria, id_proveedor, image_url, nombre)
VALUES('Botas Oxford de media ca�a para hombre - Botas con cremallera de piel sint�tica de color liso con suela de goma, forro transpirable y c�modo para ropa informal de negocios',
10,11,13,'','Botas Oxford'
);

INSERT INTO productos
(detalle, existencia, id_categoria, id_proveedor, image_url, nombre)
VALUES('Botas Oxford de media ca�a para hombre - Botas con cremallera de piel sint�tica de color liso con suela de goma, forro transpirable y c�modo para ropa informal de negocios',
10,11,13,'https://resourcesdev.blob.core.windows.net/resources-web/proyectos/eshoes/Botas Oxford.jpg','Botas Oxford'
);

INSERT INTO precios_producto
(id_producto, precio_unidad)
VALUES(46,650.00)