
CREATE PROCEDURE sp_ObtenerListadoDetallesProductos
    @IdsProductos VARCHAR(MAX)
AS
BEGIN
    -- Convertir la lista de IDs en una tabla
    DECLARE @Productos TABLE (IdProducto INT);

    INSERT INTO @Productos (IdProducto)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@IdsProductos, ',');

    -- Obtener los detalles de los productos coincidentes
    SELECT 
        p.Id AS IdProducto,
        p.Nombre AS NombreProducto,
        p.existencia AS Cantidad,
		pp.precio_unidad as PrecioUnidad 
    FROM Productos p
	LEFT JOIN precios_producto pp on pp.id_producto = p.id
    WHERE p.Id IN (SELECT IdProducto FROM @Productos);
END;
GO

/*
exec sp_ObtenerListadoDetallesProductos '46, 47, 48, 49';
exec sp_ObtenerListadoDetallesProductos '46,47,48,49';

DROP PROCEDURE sp_ObtenerListadoDetallesProductos;
*/