namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WhereHowServiceAccessed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "HowServiceAccessed", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "HowServiceAccessed");
        }
    }
}
