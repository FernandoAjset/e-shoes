CREATE TABLE rol(
	Id INT PRIMARY KEY IDENTITY,
	Nombre VARCHAR(100) NOT NULL
);

ALTER TABLE usuarios 
ADD Id_role INT, activo BIT;

ALTER TABLE usuarios
ADD CONSTRAINT fk_id_role
FOREIGN KEY (id_role) REFERENCES rol(Id);


--SP PARA OBTENER TODOS LOS USUARIOS--

GO
CREATE PROCEDURE SP_OBTENER_TODOS_USUARIOS
AS
BEGIN
	Select usuarios.id, nombre_usuario, contrasennia, correo, Nombre AS Rol, activo
	from usuarios
	LEFT JOIN rol
	ON usuarios.Id_role = rol.Id
	WHERE (usuarios.activo = 1)

END
GO

--SP PARA CREAR USUARIOS--
GO
CREATE PROCEDURE [SP_CREAR_USUARIOS]
	@nombre_usuario varchar(100),
	@contrasennia varchar (max),
	@correo varchar (150),
	@Id_role int
AS
BEGIN
	DECLARE @Confirmado bit;
	SET @Confirmado=1;
 
	IF @Id_role=3
	BEGIN
		SET @Confirmado=0;
	END
	INSERT INTO usuarios (nombre_usuario, contrasennia, correo, Id_role, activo, confirmado)
	VALUES (@nombre_usuario, @contrasennia, @correo, @Id_role,1,@Confirmado);
	SELECT SCOPE_IDENTITY();
END

--SP PARA EDITAR USUARIOS--
GO
CREATE PROCEDURE SP_EDITAR_USUARIO
	@Idusuario int,
	@nombre_usuario varchar(100),
	@contrasennia varchar (max),
	@correo varchar (150),
	@Id_role int
AS
BEGIN
	UPDATE TOP (1) usuarios SET nombre_usuario=@nombre_usuario, contrasennia=@contrasennia, correo=@correo, Id_role=@Id_role
	WHERE id = @Idusuario;
END
GO

--SP PARA OBTENER USUARIO POR ID--

GO
Alter PROCEDURE SP_OBTENER_OBTENER_POR_ID
	@Idusuario int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT usuarios.id, nombre_usuario, contrasennia, correo, Id_role, activo
	FROM usuarios 
	LEFT JOIN rol
	ON usuarios.Id_role = rol.Id
	WHERE (usuarios.id= @Idusuario)
	and (usuarios.activo = 1)
END
GO


--SP PARA OBTENER USUARIO POR CORREO--
GO
CREATE PROCEDURE SP_OBTENER_OBTENER_POR_CORREO
	@correo varchar (150)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT usuarios.id, nombre_usuario, contrasennia, correo, usuarios.Id_role, Nombre AS Rol, activo
	FROM usuarios 
	LEFT JOIN rol
	ON usuarios.Id_role = rol.Id
	WHERE correo =@correo
	and (usuarios.activo = 1)
END
GO


--SP PARA ELIMNAR USUARIOS POR ID--

GO
CREATE PROCEDURE SP_ELIMINAR_USUARIOS
	@Idusuario int,
	@activo int = 0
AS
BEGIN
	UPDATE TOP (1) usuarios SET activo = @activo
	WHERE id = @Idusuario;
END
GO


