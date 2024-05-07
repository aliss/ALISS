namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_service_description_limit : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Services", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Services", "Description", c => c.String(maxLength: 750));
        }
    }
}
