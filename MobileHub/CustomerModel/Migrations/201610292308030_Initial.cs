namespace CustomerModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActionHistory",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CustomerId = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                        AddressId = c.String(nullable: false, maxLength: 128),
                        ActionCode = c.Int(nullable: false),
                        ActionTime = c.DateTime(nullable: false),
                        Memo = c.String(unicode: false, storeType: "text"),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .ForeignKey("dbo.Customer", t => t.CustomerId)
                .ForeignKey("dbo.CustomerUser", t => t.UserId)
                .Index(t => t.CustomerId)
                .Index(t => t.UserId)
                .Index(t => t.AddressId)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Street = c.String(nullable: false, maxLength: 255, unicode: false),
                        Street2 = c.String(maxLength: 255, unicode: false),
                        City = c.String(nullable: false, maxLength: 255, unicode: false),
                        Sate = c.String(maxLength: 255, unicode: false),
                        Zip = c.String(nullable: false, maxLength: 10, unicode: false),
                        CountryId = c.String(nullable: false, maxLength: 128),
                        Latitude = c.Long(),
                        Longitude = c.Long(),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Country", t => t.CountryId)
                .Index(t => t.CountryId)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.Country",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Abbreviation = c.String(nullable: false, maxLength: 10, unicode: false),
                        Name = c.String(nullable: false, maxLength: 255, unicode: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.CustomerCompany",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 255, unicode: false),
                        AddressId = c.String(nullable: false, maxLength: 128),
                        ParentCompanyId = c.String(maxLength: 128),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerParentCompany", t => t.ParentCompanyId)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .Index(t => t.AddressId)
                .Index(t => t.ParentCompanyId)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.CustomerParentCompany",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 255, unicode: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 200, unicode: false),
                        MiddleName = c.String(maxLength: 200, unicode: false),
                        LastName = c.String(nullable: false, maxLength: 200, unicode: false),
                        AddressId = c.String(nullable: false, maxLength: 128),
                        AccountManagersUserId = c.Int(nullable: false),
                        OriginalCustomerId = c.String(maxLength: 100, unicode: false),
                        CustomerTypeId = c.String(nullable: false, maxLength: 128),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerUser", t => t.AccountManagersUserId)
                .ForeignKey("dbo.CustomerType", t => t.CustomerTypeId)
                .ForeignKey("dbo.CustomerCompany", t => t.CompanyId)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .Index(t => t.CompanyId)
                .Index(t => t.AddressId)
                .Index(t => t.AccountManagersUserId)
                .Index(t => t.CustomerTypeId)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.CustomerType",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.AvailableAction",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        Description = c.String(maxLength: 200, unicode: false),
                        LocaleId = c.String(nullable: false, maxLength: 128),
                        CustomerTypeId = c.String(nullable: false, maxLength: 128),
                        ActionTypeId = c.String(nullable: false, maxLength: 128),
                        ActionCode = c.Int(nullable: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActionType", t => t.ActionTypeId)
                .ForeignKey("dbo.Locale", t => t.LocaleId)
                .ForeignKey("dbo.CustomerType", t => t.CustomerTypeId)
                .Index(t => t.LocaleId)
                .Index(t => t.CustomerTypeId)
                .Index(t => t.ActionTypeId)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.ActionType",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Code = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 100, unicode: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.Locale",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Code = c.String(nullable: false, maxLength: 10, unicode: false),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        LcidString = c.String(nullable: false, maxLength: 10, unicode: false),
                        LcidDecimal = c.Int(),
                        LcidHex = c.Int(),
                        LcidCodePage = c.Int(),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue:DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.CustomerUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LocaleId = c.Int(nullable: false),
                        Locale_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locale", t => t.Locale_Id)
                .Index(t => t.Locale_Id);
            
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                        EventTime = c.DateTime(nullable: false),
                        Purpose = c.String(maxLength: 255, unicode: false),
                        Memo = c.String(unicode: false, storeType: "text"),
                        AddressId = c.String(nullable: false, maxLength: 128),
                        CustomerId = c.String(),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerUser", t => t.UserId)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .Index(t => t.UserId)
                .Index(t => t.AddressId)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.Meeting",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                        CustomerId = c.String(nullable: false, maxLength: 128),
                        MeetingTime = c.DateTime(nullable: false),
                        Purpose = c.String(maxLength: 255, unicode: false),
                        Memo = c.String(unicode: false, storeType: "text"),
                        AddressId = c.String(nullable: false, maxLength: 128),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerUser", t => t.UserId)
                .ForeignKey("dbo.Customer", t => t.CustomerId)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .Index(t => t.UserId)
                .Index(t => t.CustomerId)
                .Index(t => t.AddressId)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.OrderHeader",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CustomerId = c.String(nullable: false, maxLength: 128),
                        AddressId = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                        MeetingId = c.String(),
                        EventId = c.String(),
                        OrderTime = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, storeType: "money"),
                        TotalTax = c.Decimal(nullable: false, storeType: "money"),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerUser", t => t.UserId)
                .ForeignKey("dbo.Customer", t => t.CustomerId)
                .ForeignKey("dbo.Address", t => t.AddressId)
                .Index(t => t.CustomerId)
                .Index(t => t.AddressId)
                .Index(t => t.UserId)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.OrderItem",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OrderId = c.String(nullable: false, maxLength: 128),
                        ItemId = c.String(nullable: false, maxLength: 128),
                        ItemQuantity = c.String(),
                        ItemPrice = c.Decimal(nullable: false, storeType: "money"),
                        TotalPrice = c.Decimal(nullable: false, storeType: "money"),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ItemId)
                .ForeignKey("dbo.OrderHeader", t => t.OrderId)
                .Index(t => t.OrderId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedAt, clustered: false);
            
            CreateTable(
                "dbo.Item",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OriginalItemId = c.String(maxLength: 100, unicode: false),
                        Title = c.String(maxLength: 100, unicode: false),
                        Description = c.String(maxLength: 255, unicode: false),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        PicUrl = c.String(maxLength: 255, unicode: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7, defaultValue: DateTimeOffset.UtcNow),
                        UpdatedAt = c.DateTimeOffset(precision: 7),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CreatedAt, clustered: false);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderHeader", "AddressId", "dbo.Address");
            DropForeignKey("dbo.Meeting", "AddressId", "dbo.Address");
            DropForeignKey("dbo.Event", "AddressId", "dbo.Address");
            DropForeignKey("dbo.Customer", "AddressId", "dbo.Address");
            DropForeignKey("dbo.CustomerCompany", "AddressId", "dbo.Address");
            DropForeignKey("dbo.Customer", "CompanyId", "dbo.CustomerCompany");
            DropForeignKey("dbo.OrderHeader", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.Meeting", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.Customer", "CustomerTypeId", "dbo.CustomerType");
            DropForeignKey("dbo.AvailableAction", "CustomerTypeId", "dbo.CustomerType");
            DropForeignKey("dbo.CustomerUser", "Locale_Id", "dbo.Locale");
            DropForeignKey("dbo.OrderHeader", "UserId", "dbo.CustomerUser");
            DropForeignKey("dbo.OrderItem", "OrderId", "dbo.OrderHeader");
            DropForeignKey("dbo.OrderItem", "ItemId", "dbo.Item");
            DropForeignKey("dbo.Meeting", "UserId", "dbo.CustomerUser");
            DropForeignKey("dbo.Event", "UserId", "dbo.CustomerUser");
            DropForeignKey("dbo.Customer", "AccountManagersUserId", "dbo.CustomerUser");
            DropForeignKey("dbo.ActionHistory", "UserId", "dbo.CustomerUser");
            DropForeignKey("dbo.AvailableAction", "LocaleId", "dbo.Locale");
            DropForeignKey("dbo.AvailableAction", "ActionTypeId", "dbo.ActionType");
            DropForeignKey("dbo.ActionHistory", "CustomerId", "dbo.Customer");
            DropForeignKey("dbo.CustomerCompany", "ParentCompanyId", "dbo.CustomerParentCompany");
            DropForeignKey("dbo.Address", "CountryId", "dbo.Country");
            DropForeignKey("dbo.ActionHistory", "AddressId", "dbo.Address");
            DropIndex("dbo.Item", new[] { "CreatedAt" });
            DropIndex("dbo.OrderItem", new[] { "CreatedAt" });
            DropIndex("dbo.OrderItem", new[] { "ItemId" });
            DropIndex("dbo.OrderItem", new[] { "OrderId" });
            DropIndex("dbo.OrderHeader", new[] { "CreatedAt" });
            DropIndex("dbo.OrderHeader", new[] { "UserId" });
            DropIndex("dbo.OrderHeader", new[] { "AddressId" });
            DropIndex("dbo.OrderHeader", new[] { "CustomerId" });
            DropIndex("dbo.Meeting", new[] { "CreatedAt" });
            DropIndex("dbo.Meeting", new[] { "AddressId" });
            DropIndex("dbo.Meeting", new[] { "CustomerId" });
            DropIndex("dbo.Meeting", new[] { "UserId" });
            DropIndex("dbo.Event", new[] { "CreatedAt" });
            DropIndex("dbo.Event", new[] { "AddressId" });
            DropIndex("dbo.Event", new[] { "UserId" });
            DropIndex("dbo.CustomerUser", new[] { "Locale_Id" });
            DropIndex("dbo.Locale", new[] { "CreatedAt" });
            DropIndex("dbo.ActionType", new[] { "CreatedAt" });
            DropIndex("dbo.AvailableAction", new[] { "CreatedAt" });
            DropIndex("dbo.AvailableAction", new[] { "ActionTypeId" });
            DropIndex("dbo.AvailableAction", new[] { "CustomerTypeId" });
            DropIndex("dbo.AvailableAction", new[] { "LocaleId" });
            DropIndex("dbo.CustomerType", new[] { "CreatedAt" });
            DropIndex("dbo.Customer", new[] { "CreatedAt" });
            DropIndex("dbo.Customer", new[] { "CustomerTypeId" });
            DropIndex("dbo.Customer", new[] { "AccountManagersUserId" });
            DropIndex("dbo.Customer", new[] { "AddressId" });
            DropIndex("dbo.Customer", new[] { "CompanyId" });
            DropIndex("dbo.CustomerParentCompany", new[] { "CreatedAt" });
            DropIndex("dbo.CustomerCompany", new[] { "CreatedAt" });
            DropIndex("dbo.CustomerCompany", new[] { "ParentCompanyId" });
            DropIndex("dbo.CustomerCompany", new[] { "AddressId" });
            DropIndex("dbo.Country", new[] { "CreatedAt" });
            DropIndex("dbo.Address", new[] { "CreatedAt" });
            DropIndex("dbo.Address", new[] { "CountryId" });
            DropIndex("dbo.ActionHistory", new[] { "CreatedAt" });
            DropIndex("dbo.ActionHistory", new[] { "AddressId" });
            DropIndex("dbo.ActionHistory", new[] { "UserId" });
            DropIndex("dbo.ActionHistory", new[] { "CustomerId" });
            DropTable("dbo.Item");
            DropTable("dbo.OrderItem");
            DropTable("dbo.OrderHeader");
            DropTable("dbo.Meeting");
            DropTable("dbo.Event");
            DropTable("dbo.CustomerUser");
            DropTable("dbo.Locale");
            DropTable("dbo.ActionType");
            DropTable("dbo.AvailableAction");
            DropTable("dbo.CustomerType");
            DropTable("dbo.Customer");
            DropTable("dbo.CustomerParentCompany");
            DropTable("dbo.CustomerCompany");
            DropTable("dbo.Country");
            DropTable("dbo.Address");
            DropTable("dbo.ActionHistory");
        }
    }
}
