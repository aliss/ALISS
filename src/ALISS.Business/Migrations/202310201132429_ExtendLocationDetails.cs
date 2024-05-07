namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendLocationDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "HealthBoardId", c => c.Int());
            AddColumn("dbo.Locations", "LocalAuthorityId", c => c.Int());
            AddColumn("dbo.Locations", "Ward", c => c.String());
            CreateIndex("dbo.Locations", "HealthBoardId");
            CreateIndex("dbo.Locations", "LocalAuthorityId");
            AddForeignKey("dbo.Locations", "HealthBoardId", "dbo.ServiceAreas", "ServiceAreaId");
            AddForeignKey("dbo.Locations", "LocalAuthorityId", "dbo.ServiceAreas", "ServiceAreaId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Locations", "LocalAuthorityId", "dbo.ServiceAreas");
            DropForeignKey("dbo.Locations", "HealthBoardId", "dbo.ServiceAreas");
            DropIndex("dbo.Locations", new[] { "LocalAuthorityId" });
            DropIndex("dbo.Locations", new[] { "HealthBoardId" });
            DropColumn("dbo.Locations", "Ward");
            DropColumn("dbo.Locations", "LocalAuthorityId");
            DropColumn("dbo.Locations", "HealthBoardId");
        }
    }
}
