namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Service_Claimed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "ClaimedUserId", c => c.Int());
            AddColumn("dbo.Services", "ClaimedOn", c => c.DateTime());
            CreateIndex("dbo.Services", "ClaimedUserId");
            AddForeignKey("dbo.Services", "ClaimedUserId", "dbo.UserProfiles", "UserProfileId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Services", "ClaimedUserId", "dbo.UserProfiles");
            DropIndex("dbo.Services", new[] { "ClaimedUserId" });
            DropColumn("dbo.Services", "ClaimedOn");
            DropColumn("dbo.Services", "ClaimedUserId");
        }
    }
}
