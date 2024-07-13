CREATE TABLE IF NOT EXISTS "Role" (
    "Id" UUID PRIMARY KEY,
    "Description" VARCHAR(255),
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID
);

CREATE TABLE IF NOT EXISTS "User" (
    "Id" UUID PRIMARY KEY,
    "RoleId" UUID,
    "Name" VARCHAR(255),
    "Email" VARCHAR(255),
    "Phone" VARCHAR(255),
    "UserName" VARCHAR(255),
    "Password" VARCHAR(255),
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID,
    CONSTRAINT "FK_RoleId_User" FOREIGN KEY ("RoleId") REFERENCES "Role"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Client" (
    "Id" UUID PRIMARY KEY,
    "Surname" VARCHAR(255),
    "Address" VARCHAR(255),
    "UserId" UUID,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID,
     CONSTRAINT "FK_UserId" FOREIGN KEY ("UserId") REFERENCES "Role"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Sender" (
    "Id" UUID PRIMARY KEY,
    "Address" VARCHAR(255),
    "UserId" UUID,
    "CreatedAt" TIMESTAMP  DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID,
     CONSTRAINT "FK_UserId" FOREIGN KEY ("UserId") REFERENCES "Role"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Courier" (
    "Id" UUID PRIMARY KEY,
    "Surname" VARCHAR(255),
    "UserId" UUID,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID,
     CONSTRAINT "FK_UserId" FOREIGN KEY ("UserId") REFERENCES "Role"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Package" (
    "Id" UUID PRIMARY KEY,
    "ClientId" UUID,
    "SenderId" UUID,
    "CourierId" UUID,
    "Weight" FLOAT,
    "Remark" VARCHAR(255),
    "DeliveryAddress" VARCHAR(255),
    "IsActive" BOOLEAN DEFAULT TRUE ,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID,
    CONSTRAINT "FK_ClientId_Package" FOREIGN KEY ("ClientId") REFERENCES "Client"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SenderId_Package" FOREIGN KEY ("SenderId") REFERENCES "Sender"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CourierId_Package" FOREIGN KEY ("CourierId") REFERENCES "Courier"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Payment" (
    "Id" UUID PRIMARY KEY,
    "PackageId" UUID,
    "Price" INT,
    "PaymentDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP ,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID,
    CONSTRAINT "FK_PackageId_Payment" FOREIGN KEY ("PackageId") REFERENCES "Package"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "PaymentMethod" (
    "Id" UUID PRIMARY KEY,
    "PaymentId" UUID,
    "Type" VARCHAR(255),
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID,
    CONSTRAINT "FK_PaymentId_PaymentMethod" FOREIGN KEY ("PaymentId") REFERENCES "Payment"("Id") ON DELETE CASCADE
);



CREATE TABLE IF NOT EXISTS "DeliveryStatus" (
    "Id" UUID PRIMARY KEY,
    "Status" VARCHAR(255),
    "Date" DATE,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID
);

CREATE TABLE IF NOT EXISTS "Rating" (
    "Id" UUID PRIMARY KEY,
    "ClientId" UUID,
    "PackageId" UUID,
    "RatingNumber" INT,
    "Comment" TEXT,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID,
    CONSTRAINT "FK_ClientId_Rating" FOREIGN KEY ("ClientId") REFERENCES "Client"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PackageId_Rating" FOREIGN KEY ("PackageId") REFERENCES "Package"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "DeliveryLog" (
    "Id" UUID PRIMARY KEY,
    "PackageId" UUID,
    "DeliveryStatusId" UUID,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID,
    "UpdatedBy" UUID,
    CONSTRAINT "FK_PackageId_DeliveryLog" FOREIGN KEY ("PackageId") REFERENCES "Package"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_DeliveryStatusId_DeliveryLog" FOREIGN KEY ("DeliveryStatusId") REFERENCES "DeliveryStatus"("Id") ON DELETE CASCADE
);


update "User" set "Password" = '$2a$11$8xpPIQKJrwamyaJQ/y3/IO9oun4Vbnt.M96Wjzq52egTudITeYkMW' where "Id" = '650e8400-e29b-41d4-a716-446655447818';
update "User" set "Password" = '$2a$11$3.pw7mZ1mXCDMP1if.6ZP.EEmcunylMl5qmJOFFZp0anc3/5gey3a' where "Id" = '650e8400-e29b-41d4-a716-446655447527'; 
update "User" set "Password" = '$2a$11$eis4lO6uyUFPcZz3/DlyGOW4QsdkWyGiDCnjQZqKp7MFDwPhqz/.S' where "Id" = '650e8400-e29b-41d4-a716-446655440049'; 
update "User" set "Password" = '$2a$11$n05Oun2xh9elrMuXFtUe5ujAOmipQTDMed4bYuuUKurv59ibq.6Ay' where "Id" = '650e8400-e29b-41d4-a716-446655440026'; 
update "User" set "Password" = '$2a$11$B.auXL9y.b.HD8VfDZc9JexsSamvhIdkbfSEeMOVpa4d9z97TrfW6' where "Id" = '650e8400-e29b-41d4-a716-446655445324'; 
update "User" set "Password" = '$2a$11$WI1tLH7uvjHbdbRbqxnVNemeV6vmVbGhTfAvOt/UTd5TsYoiL4bc6' where "Id" = '650e8400-e29b-41d4-a716-446655440004'; 
update "User" set "Password" = '$2a$11$Q2zfVft.RKj.agfzOAFyy.m0yKkXhalGABUJSHsZrBxSZs4bmvFUi' where "Id" = '650e8400-e29b-41d4-a716-446655440415'; 
update "User" set "Password" = '$2a$11$fj7tEC2gq2wbSinRuOkE3eDXyspaWCWd4xnLxR3Hs/ZXsLONWYAqq' where "Id" = '650e8400-e29b-41d4-a716-446655440512'; 
update "User" set "Password" = '$2a$11$GsTuzwaPMlr/eU.e0j7A8e7M840T02U9VDl9Ri1PLdB5fEjkb5Mj6' where "Id" = '650e8400-e29b-41d4-a716-446655440002'; 
update "User" set "Password" = '$2a$11$nrlypZM4XZY/a3RcWIqEWupF/xA8P5n.Wl.ztSKrUMyfZN7GxM11i' where "Id" = '650e8400-e29b-41d4-a716-446655445721'; 
update "User" set "Password" = '$2a$11$gNoHr8uQZrX5InRVCjYZdOVLTo/Q2uQS2w9ZOS.D6nrosCEzAVgtG' where "Id" = '650e8400-e29b-41d4-a716-446655440777'; 
