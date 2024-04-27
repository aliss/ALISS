namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceAccessibilityFeaturesUpdateKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ServiceAccessibilityFeatures");
            AddColumn("dbo.ServiceAccessibilityFeatures", "ServiceAccessibilityFeatureId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.ServiceAccessibilityFeatures", "ServiceAccessibilityFeatureId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ServiceAccessibilityFeatures");
            DropColumn("dbo.ServiceAccessibilityFeatures", "ServiceAccessibilityFeatureId");
            AddPrimaryKey("dbo.ServiceAccessibilityFeatures", new[] { "ServiceId", "AccessibilityFeatureId" });
        }
    }
}
