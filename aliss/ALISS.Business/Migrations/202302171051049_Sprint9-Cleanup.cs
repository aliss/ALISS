namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sprint9Cleanup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "ReferralUrl", c => c.String(maxLength: 500));
            AddColumn("dbo.ServiceAreas", "ParentServiceAreaId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceAreas", "ParentServiceAreaId");
            DropColumn("dbo.Services", "ReferralUrl");
        }
    }
}
