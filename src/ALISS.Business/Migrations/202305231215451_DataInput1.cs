namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataInput1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessibilityFeatures",
                c => new
                    {
                        AccessibilityFeatureId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Slug = c.String(maxLength: 50),
                        PromptQuestions = c.String(),
                        Icon = c.String(maxLength: 50),
                        DisplayOrder = c.Int(nullable: false),
                        ParentAccessibilityFeatureId = c.Int(),
                    })
                .PrimaryKey(t => t.AccessibilityFeatureId)
                .ForeignKey("dbo.Categories", t => t.ParentAccessibilityFeatureId)
                .Index(t => t.ParentAccessibilityFeatureId);
            
            CreateTable(
                "dbo.CommunityGroups",
                c => new
                    {
                        CommunityGroupId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Slug = c.String(maxLength: 50),
                        Icon = c.String(maxLength: 50),
                        DisplayOrder = c.Int(nullable: false),
                        ParentCommunityGroupId = c.Int(),
                    })
                .PrimaryKey(t => t.CommunityGroupId)
                .ForeignKey("dbo.Categories", t => t.ParentCommunityGroupId)
                .Index(t => t.ParentCommunityGroupId);
            
            CreateTable(
                "dbo.ServiceAccessibilityFeatures",
                c => new
                    {
                        ServiceId = c.Guid(nullable: false),
                        AccessibilityFeatureId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceId, t.AccessibilityFeatureId })
                .ForeignKey("dbo.AccessibilityFeatures", t => t.AccessibilityFeatureId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.AccessibilityFeatureId);
            
            CreateTable(
                "dbo.ServiceCommunityGroups",
                c => new
                    {
                        ServiceId = c.Guid(nullable: false),
                        CommunityGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceId, t.CommunityGroupId })
                .ForeignKey("dbo.CommunityGroups", t => t.CommunityGroupId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.CommunityGroupId);
            
            AddColumn("dbo.Organisations", "LogoAltText", c => c.String());
            AddColumn("dbo.Organisations", "LogoCaption", c => c.String());
            AddColumn("dbo.Organisations", "Submitted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Services", "Summary", c => c.String());
            AddColumn("dbo.Services", "LastEditedStep", c => c.Int(nullable: false));
            AddColumn("dbo.Services", "Logo", c => c.String());
            AddColumn("dbo.Services", "LogoAltText", c => c.String());
            AddColumn("dbo.Services", "LogoCaption", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceCommunityGroups", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceCommunityGroups", "CommunityGroupId", "dbo.CommunityGroups");
            DropForeignKey("dbo.ServiceAccessibilityFeatures", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceAccessibilityFeatures", "AccessibilityFeatureId", "dbo.AccessibilityFeatures");
            DropForeignKey("dbo.CommunityGroups", "ParentCommunityGroupId", "dbo.Categories");
            DropForeignKey("dbo.AccessibilityFeatures", "ParentAccessibilityFeatureId", "dbo.Categories");
            DropIndex("dbo.ServiceCommunityGroups", new[] { "CommunityGroupId" });
            DropIndex("dbo.ServiceCommunityGroups", new[] { "ServiceId" });
            DropIndex("dbo.ServiceAccessibilityFeatures", new[] { "AccessibilityFeatureId" });
            DropIndex("dbo.ServiceAccessibilityFeatures", new[] { "ServiceId" });
            DropIndex("dbo.CommunityGroups", new[] { "ParentCommunityGroupId" });
            DropIndex("dbo.AccessibilityFeatures", new[] { "ParentAccessibilityFeatureId" });
            DropColumn("dbo.Services", "LogoCaption");
            DropColumn("dbo.Services", "LogoAltText");
            DropColumn("dbo.Services", "Logo");
            DropColumn("dbo.Services", "LastEditedStep");
            DropColumn("dbo.Services", "Summary");
            DropColumn("dbo.Organisations", "Submitted");
            DropColumn("dbo.Organisations", "LogoCaption");
            DropColumn("dbo.Organisations", "LogoAltText");
            DropTable("dbo.ServiceCommunityGroups");
            DropTable("dbo.ServiceAccessibilityFeatures");
            DropTable("dbo.CommunityGroups");
            DropTable("dbo.AccessibilityFeatures");
        }
    }
}
