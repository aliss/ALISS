namespace ALISS.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceAreaGeoJson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceAreas", "GeoJson", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceAreas", "GeoJson");
        }
    }
}
