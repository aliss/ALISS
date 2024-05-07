namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Collections_Improvements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Collections",
                c => new
                    {
                        CollectionId = c.Guid(nullable: false),
                        UserProfileId = c.Int(nullable: false),
                        Name = c.String(maxLength: 100),
                        CanDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CollectionId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
                .Index(t => t.UserProfileId);
            
            CreateTable(
                "dbo.CollectionServices",
                c => new
                    {
                        CollectionId = c.Guid(nullable: false),
                        ServiceId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CollectionId, t.ServiceId })
                .ForeignKey("dbo.Collections", t => t.CollectionId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: false)
                .Index(t => t.CollectionId)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.Improvements",
                c => new
                    {
                        ImprovementId = c.Guid(nullable: false),
                        ServiceId = c.Guid(),
                        OrganisationId = c.Guid(),
                        SuggestedImprovement = c.String(nullable: false, maxLength: 1024),
                        Name = c.String(maxLength: 250),
                        Email = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.ImprovementId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CollectionServices", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.CollectionServices", "CollectionId", "dbo.Collections");
            DropForeignKey("dbo.Collections", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.CollectionServices", new[] { "ServiceId" });
            DropIndex("dbo.CollectionServices", new[] { "CollectionId" });
            DropIndex("dbo.Collections", new[] { "UserProfileId" });
            DropTable("dbo.Improvements");
            DropTable("dbo.CollectionServices");
            DropTable("dbo.Collections");
        }
    }
}
