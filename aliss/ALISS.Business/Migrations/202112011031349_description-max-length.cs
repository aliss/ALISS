namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class descriptionmaxlength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Organisations", "Description", c => c.String(maxLength: 750));
            AlterColumn("dbo.Services", "Description", c => c.String(maxLength: 750));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Services", "Description", c => c.String());
            AlterColumn("dbo.Organisations", "Description", c => c.String());
        }
    }
}
