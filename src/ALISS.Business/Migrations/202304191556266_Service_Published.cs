namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Service_Published : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Published", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "Published");
        }
    }
}
