namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommunityGroupMinMax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommunityGroups", "IsMinMax", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CommunityGroups", "IsMinMax");
        }
    }
}
