namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceAccessibilityFeaturesLocationInformation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceAccessibilityFeatures", "LocationId", c => c.Guid());
            AddColumn("dbo.ServiceAccessibilityFeatures", "Virtual", c => c.Boolean(nullable: false));
            CreateIndex("dbo.ServiceAccessibilityFeatures", "LocationId");
            AddForeignKey("dbo.ServiceAccessibilityFeatures", "LocationId", "dbo.Locations", "LocationId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceAccessibilityFeatures", "LocationId", "dbo.Locations");
            DropIndex("dbo.ServiceAccessibilityFeatures", new[] { "LocationId" });
            DropColumn("dbo.ServiceAccessibilityFeatures", "Virtual");
            DropColumn("dbo.ServiceAccessibilityFeatures", "LocationId");
        }
    }
}
