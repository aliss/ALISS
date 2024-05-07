namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrganisationInstagram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organisations", "Instagram", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "Instagram");
        }
    }
}
