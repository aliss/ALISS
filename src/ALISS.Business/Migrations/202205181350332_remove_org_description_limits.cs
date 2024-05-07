namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_org_description_limits : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Organisations", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Organisations", "Description", c => c.String(maxLength: 750));
        }
    }
}
