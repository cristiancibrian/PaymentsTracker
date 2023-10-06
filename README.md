# PaymentsTracker
La Base de Datos se llamaba Payments.

Procedimientos almacenados creados.


CREATE PROCEDURE ObtenerPagos
AS
BEGIN
    SELECT * FROM Payments
END


CREATE PROCEDURE EditarPago
    @Id INT,
    @Description NVARCHAR(255)
AS
BEGIN
    UPDATE Payments
    SET Description = @Description
    WHERE Id = @Id
END


CREATE TYPE IntArray AS TABLE
(
    Id INT
);
CREATE PROCEDURE BorrarPagos
    @Ids IntArray READONLY
AS
BEGIN
    DELETE FROM Payments
    WHERE Id IN (SELECT Id FROM @Ids)
END


CREATE PROCEDURE CrearPago
    @Description NVARCHAR(255)
AS
BEGIN
    INSERT INTO Payments (Description)
    VALUES (@Description)
END
