namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class organisation_claimed_date : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organisations", "ClaimedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "ClaimedOn");
        }
    }
}
