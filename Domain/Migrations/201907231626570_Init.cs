namespace Domain.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 40, unicode: false),
                    Description = c.String(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.Donations",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RecurrenceToken = c.String(),
                    CustomRef = c.String(),
                    IsAnonymous = c.Boolean(nullable: false),
                    IsRecurrence = c.Boolean(nullable: false),
                    UserId = c.Int(nullable: false),
                    DonationModality = c.Int(nullable: false),
                    StatusId = c.Int(nullable: false),
                    DocumentId = c.Int(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                    Category_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.DonationModes", t => t.DonationModality)
                .ForeignKey("dbo.Status", t => t.StatusId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Id, unique: true);

            CreateTable(
                "dbo.Documents",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    Extention = c.String(nullable: false),
                    File = c.Binary(nullable: false),
                    FilePathUrl = c.String(),
                    Description = c.String(),
                    DonationId = c.Int(),
                    PaymentId = c.Int(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true);

            CreateTable(
                "dbo.DonationModes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 20, unicode: false),
                    Description = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.Status",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 15, unicode: false),
                    Description = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    Email = c.String(maxLength: 60, unicode: false),
                    DocumentIdentificationNumber = c.String(),
                    RoleId = c.Int(nullable: false),
                    IsAnonymous = c.Boolean(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .Index(t => t.Id, unique: true)
                .Index(t => t.Email, unique: true);

            CreateTable(
                "dbo.Payments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    UserId = c.Int(nullable: false),
                    CategoryId = c.Int(nullable: false),
                    InstitutionId = c.Int(nullable: false),
                    DocumentId = c.Int(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Documents", t => t.DocumentId)
                .ForeignKey("dbo.Institutions", t => t.InstitutionId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.Id, unique: true);

            CreateTable(
                "dbo.Institutions",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    Cnpj = c.String(nullable: false),
                    Bank = c.String(nullable: false),
                    Agency = c.String(nullable: false),
                    Account = c.String(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true);

            CreateTable(
                "dbo.Roles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 5, unicode: false),
                    Description = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.DonationValues",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true)
                .Index(t => t.Amount, unique: true);

            CreateTable(
                "dbo.Faqs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Question = c.String(nullable: false, maxLength: 250, unicode: false),
                    Answer = c.String(nullable: false),
                    Tags = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true)
                .Index(t => t.Question, unique: true);

            CreateTable(
                "dbo.Hashes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    HashCode = c.String(nullable: false, maxLength: 128, unicode: false),
                    Type = c.Int(nullable: false),
                    Attempts = c.Int(nullable: false),
                    BlockExpirationDate = c.DateTime(),
                    ExpirationDate = c.DateTime(),
                    UserId = c.Int(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedBy = c.String(),
                    Disabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.Id, unique: true)
                .Index(t => t.HashCode);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Hashes", "UserId", "dbo.Users");
            DropForeignKey("dbo.Donations", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Payments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Payments", "institutionId", "dbo.Institutions");
            DropForeignKey("dbo.Payments", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Payments", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Donations", "UserId", "dbo.Users");
            DropForeignKey("dbo.Donations", "StatusId", "dbo.Status");
            DropForeignKey("dbo.Donations", "DonationModality", "dbo.DonationModes");
            DropForeignKey("dbo.Donations", "DocumentId", "dbo.Documents");
            DropIndex("dbo.Hashes", new[] { "HashCode" });
            DropIndex("dbo.Hashes", new[] { "Id" });
            DropIndex("dbo.Faqs", new[] { "Question" });
            DropIndex("dbo.Faqs", new[] { "Id" });
            DropIndex("dbo.DonationValues", new[] { "Amount" });
            DropIndex("dbo.DonationValues", new[] { "Id" });
            DropIndex("dbo.Roles", new[] { "Name" });
            DropIndex("dbo.Roles", new[] { "Id" });
            DropIndex("dbo.Institutions", new[] { "Id" });
            DropIndex("dbo.Payments", new[] { "Id" });
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Users", new[] { "Id" });
            DropIndex("dbo.Status", new[] { "Name" });
            DropIndex("dbo.Status", new[] { "Id" });
            DropIndex("dbo.DonationModes", new[] { "Name" });
            DropIndex("dbo.DonationModes", new[] { "Id" });
            DropIndex("dbo.Documents", new[] { "Id" });
            DropIndex("dbo.Donations", new[] { "Id" });
            DropIndex("dbo.Categories", new[] { "Name" });
            DropIndex("dbo.Categories", new[] { "Id" });
            DropTable("dbo.Hashes");
            DropTable("dbo.Faqs");
            DropTable("dbo.DonationValues");
            DropTable("dbo.Roles");
            DropTable("dbo.Institutions");
            DropTable("dbo.Payments");
            DropTable("dbo.Users");
            DropTable("dbo.Status");
            DropTable("dbo.DonationModes");
            DropTable("dbo.Documents");
            DropTable("dbo.Donations");
            DropTable("dbo.Categories");
        }
    }
}
