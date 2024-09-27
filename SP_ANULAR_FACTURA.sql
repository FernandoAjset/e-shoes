SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Edgar Fernando Ajset
-- Create date: 2024-09-18
-- Description:	Procedimiento para anular una factura y devolver la existencia.
-- =============================================
CREATE PROCEDURE SP_ANULAR_FACTURA
	@IdEncabezado int
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        SET NOCOUNT ON;

        -- Verificar si la factura existe y no está anulada
        IF EXISTS (SELECT 1 FROM encabezado_factura WHERE id = @IdEncabezado AND estado_factura_id != 3) -- 3. Anulado
        BEGIN
            -- Declarar cursor para recorrer los detalles de la factura
            DECLARE detalle_cursor CURSOR LOCAL FOR
            SELECT id_producto, cantidad
            FROM detalle_factura
            WHERE id_encabezado_factura = @IdEncabezado;

            DECLARE @ProductoID INT, @CantidadProducto INT;

            OPEN detalle_cursor;

            FETCH NEXT FROM detalle_cursor INTO @ProductoID, @CantidadProducto;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                -- Actualizar existencia de cada producto
                UPDATE productos
                SET existencia = existencia + @CantidadProducto
                WHERE id = @ProductoID;

                FETCH NEXT FROM detalle_cursor INTO @ProductoID, @CantidadProducto;
            END

            CLOSE detalle_cursor;
            DEALLOCATE detalle_cursor;

            -- Marcar la factura como anulada
            UPDATE encabezado_factura
            SET estado_factura_id = 3
            WHERE id = @IdEncabezado;

        END
        ELSE
        BEGIN
            -- Mostrar mensaje si la factura no existe o ya está anulada
            SELECT 'Factura no encontrada o ya está anulada' AS ErrorMessage;
            ROLLBACK;
            RETURN;
        END

        -- Confirmar transacción
        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        -- Mostrar el mensaje de error
        SELECT ERROR_MESSAGE() AS ErrorMessage;
    END CATCH;
END
GO