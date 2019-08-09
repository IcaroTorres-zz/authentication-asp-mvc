namespace Domain.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AuditDates_RemovedDatabaseGeneratedOption : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Categories", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Donations", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Donations", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Documents", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Documents", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DonationModes", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DonationModes", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Status", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Status", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Payments", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Payments", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Institutions", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Institutions", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Roles", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Roles", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DonationValues", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DonationValues", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Faqs", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Faqs", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Hashes", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Hashes", "ModifiedDate", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.Hashes", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Hashes", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Faqs", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Faqs", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DonationValues", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DonationValues", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Roles", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Roles", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Institutions", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Institutions", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Payments", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Payments", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Status", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Status", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DonationModes", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DonationModes", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Documents", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Documents", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Donations", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Donations", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Categories", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Categories", "CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
