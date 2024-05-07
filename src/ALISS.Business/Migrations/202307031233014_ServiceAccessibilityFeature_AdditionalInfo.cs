namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceAccessibilityFeature_AdditionalInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceAccessibilityFeatures", "AdditionalInfo", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceAccessibilityFeatures", "AdditionalInfo");
        }
    }
}
