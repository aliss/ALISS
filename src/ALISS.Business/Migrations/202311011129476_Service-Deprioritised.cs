namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceDeprioritised : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Deprioritised", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "Deprioritised");
        }
    }
}
