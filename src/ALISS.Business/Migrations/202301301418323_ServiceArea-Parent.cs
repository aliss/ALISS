namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceAreaParent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceAreas", "ParentServiceAreaId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceAreas", "ParentServiceAreaId");
        }
    }
}
