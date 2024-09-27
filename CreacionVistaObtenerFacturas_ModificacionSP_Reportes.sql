
ALTER VIEW vista_facturas_detalles AS
SELECT 
    ef.id AS id_factura,
    ef.serie,
    ef.fecha,
    ef.estado_factura_id,
    c.nombre AS nombre_cliente,
    c.direccion,
    c.telefono,
    c.correo,
    c.NIT,
    df.cantidad,
    df.subtotal,
    p.nombre AS nombre_producto,
    p.detalle AS descripcion_producto,
    p.image_url,
    pp.precio_unidad,
	cp.nombre as Categoria,
    ef.url
FROM encabezado_factura ef
JOIN clientes c ON ef.id_cliente = c.id
JOIN detalle_factura df ON ef.id = df.id_encabezado_factura
JOIN productos p ON df.id_producto = p.id
JOIN categoria_productos cp on p.id_categoria = cp.id
LEFT JOIN precios_producto pp ON p.id = pp.id_producto;


select * from encabezado_factura;

USE [e-shoes_dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_REPORTES]    Script Date: 26/09/2024 11:59:40 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_REPORTES]
@Fecha varchar(10),
@Turno int,
@Categoria varchar(100),
@Operacion VARCHAR(50)

AS
BEGIN
	IF @Operacion = 'VentasDiariasPorCategoria' 
	BEGIN
	SET NOCOUNT ON;
	SELECT * FROM Reporte_Diario_Venta_por_Categoria
	WHERE fecha=@Fecha;
	END

	ELSE IF (@Operacion='CorteCajaPorTurno')
	BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ReporteCorteCajaPorTurno
	END

	ELSE IF (@Operacion='VentasPorCategoria')
	BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ReporteVentasPorCategoria
	WHERE Categoria=@Categoria AND Fecha=@Fecha;
	END

	ELSE IF (@Operacion='DevolucionesPorCategoria')
	BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ReporteDevolucionesPorCategoria
	WHERE Categoria=@Categoria AND Fecha=@Fecha;
	END

	ELSE IF (@Operacion='Facturas')
	BEGIN
	SET NOCOUNT ON;
	SELECT * FROM vista_facturas_detalles
	ORDER BY fecha;
	END
END
