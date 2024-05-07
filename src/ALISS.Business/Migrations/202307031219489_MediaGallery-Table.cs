namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaGalleryTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MediaGalleries",
                c => new
                    {
                        MediaGalleryId = c.Guid(nullable: false),
                        ServiceId = c.Guid(nullable: false),
                        Caption = c.String(),
                        AltText = c.String(),
                        MediaUrl = c.String(),
                        Approved = c.Boolean(nullable: false),
                        UploadUserId = c.Int(nullable: false),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.MediaGalleryId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaGalleries", "ServiceId", "dbo.Services");
            DropIndex("dbo.MediaGalleries", new[] { "ServiceId" });
            DropTable("dbo.MediaGalleries");
        }
    }
}
