--CREATE DATABASE KLIO_db
--ON PRIMARY
--(
--	NAME = N'KLIO_db_data',
--	FILENAME = N'E:\Code_main\WPF\HR Department\HR Department\bin\Debug\KLIO_db.mdf'
--)
--LOG ON
--(
--	NAME = N'KLIO_db_log',
--	FILENAME = N'E:\Code_main\WPF\HR Department\HR Department\bin\Debug\KLIO_db.log.ldf'
--);
--GO

USE KLIO_db
GO

--CREATE TABLE staff_table (
--    staff_id INT IDENTITY PRIMARY KEY,
--    full_name NVARCHAR(240) NOT NULL,
--	  service NVARCHAR(100) NOT NULL,
--    phone_number VARCHAR(11) NOT NULL UNIQUE,
--    email NVARCHAR(250) NOT NULL UNIQUE,
--    birth_date DATE NOT NULL,
--	birth_date_str AS (FORMAT(birth_date, 'dd/MM/yyyy')),
--	city NVARCHAR(240) NOT NULL,
--	post_number INT NOT NULL
--);
    
--CREATE TABLE admin_table (
--    admin_id INT IDENTITY PRIMARY KEY,
--    full_name NVARCHAR(240) NOT NULL,
--	  password NVARCHAR(240) NOT NULL
--);

--INSERT admin_table(full_name, password)
--VALUES ('ILNAZ_admin','123'), ('fa','1');

