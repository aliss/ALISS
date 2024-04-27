namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceCommunityGroupMinMax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceCommunityGroups", "MinValue", c => c.Int(nullable: false));
            AddColumn("dbo.ServiceCommunityGroups", "MaxValue", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceCommunityGroups", "MaxValue");
            DropColumn("dbo.ServiceCommunityGroups", "MinValue");
        }
    }
}
