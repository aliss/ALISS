namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccessibilityFeaturesPhysicalVirtual : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccessibilityFeatures", "Virtual", c => c.Boolean(nullable: false));
            AddColumn("dbo.AccessibilityFeatures", "Physical", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccessibilityFeatures", "Physical");
            DropColumn("dbo.AccessibilityFeatures", "Virtual");
        }
    }
}
