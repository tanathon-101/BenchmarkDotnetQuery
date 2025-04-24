# 📈 BenchmarkDotNetQuery - Performance Test for Product/Supplier Queries

โครงการนี้ใช้ [BenchmarkDotNet](https://benchmarkdotnet.org) เพื่อทดสอบประสิทธิภาพของการ query ข้อมูลจากฐานข้อมูล SQL Server ระหว่าง EF Core และ Dapper โดยใช้ตาราง `Products` และ `Suppliers` อย่างละ 10,000 รายการ

---

## 🧱 โครงสร้างฐานข้อมูล

ใช้ SQL Server (Docker / Local / Azure) แล้วรันคำสั่ง SQL เหล่านี้เพื่อสร้างและ seed ข้อมูล:

```sql
-- 🔹 สร้าง Table Suppliers
CREATE TABLE Suppliers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20)
);

-- 🔹 สร้าง Table Products (มี Foreign Key ไปยัง Suppliers)
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL,
    SupplierId INT NOT NULL,
    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
);

-- 🔹 Insert Suppliers 10,000 รายการ
DECLARE @i INT = 1;
WHILE @i <= 10000
BEGIN
    INSERT INTO Suppliers (Name, Email, Phone)
    VALUES (
        CONCAT('Supplier-', @i),
        CONCAT('supplier', @i, '@example.com'),
        CONCAT('080', RIGHT('0000' + CAST(@i AS VARCHAR), 4))
    );
    SET @i += 1;
END;

-- 🔹 Insert Products 10,000 รายการ
DECLARE @j INT = 1;
WHILE @j <= 10000
BEGIN
    INSERT INTO Products (Name, Category, Price, Stock, SupplierId)
    VALUES (
        CONCAT('Product-', @j),
        CASE WHEN @j % 3 = 0 THEN 'Electronics'
             WHEN @j % 3 = 1 THEN 'Books'
             ELSE 'Clothing' END,
        ROUND(RAND() * 1000 + 1, 2),
        FLOOR(RAND() * 500),
        FLOOR(RAND() * 10000) + 1
    );
    SET @j += 1;
END;


## 📖 คำอธิบายคอลัมน์

| คอลัมน์    | ความหมาย |
|------------|----------|
| **Method** | ชื่อ method หรือ benchmark ที่ถูกทดสอบ |
| **Mean**   | เวลาเฉลี่ยต่อรอบ (ms) – ยิ่งน้อยยิ่งดี |
| **Error**  | ค่าคลาดเคลื่อนจาก Mean – บอกความมั่นใจแค่ไหน |
| **StdDev** | ค่าเบี่ยงเบนมาตรฐาน – วัดความสม่ำเสมอของเวลา |
| **Gen0/1/2** | จำนวนครั้งที่เกิด Garbage Collection (GC) ในแต่ละ Generation |
| **Allocated** | หน่วยความจำที่ถูกจัดสรรต่อการรัน 1 ครั้ง (หน่วย: Bytes) – ยิ่งน้อยยิ่งดี |


dotnet run -c Release