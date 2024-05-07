namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceAreaParentNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ServiceAreas", "ParentServiceAreaId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ServiceAreas", "ParentServiceAreaId", c => c.Int(nullable: false));
        }
    }
}
