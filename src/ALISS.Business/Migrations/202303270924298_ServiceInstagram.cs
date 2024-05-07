namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceInstagram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Instagram", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "Instagram");
        }
    }
}
