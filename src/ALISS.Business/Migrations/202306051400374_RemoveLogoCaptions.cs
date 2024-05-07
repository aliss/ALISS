namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLogoCaptions : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Organisations", "LogoCaption");
            DropColumn("dbo.Services", "LogoCaption");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Services", "LogoCaption", c => c.String());
            AddColumn("dbo.Organisations", "LogoCaption", c => c.String());
        }
    }
}
