namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceReferralUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "ReferralUrl", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "ReferralUrl");
        }
    }
}
