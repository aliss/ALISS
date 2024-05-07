namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaGallerySubmitted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaGalleries", "Submitted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MediaGalleries", "Submitted");
        }
    }
}
