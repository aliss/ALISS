namespace ALISS.Business.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class removeduplicatelogindate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserProfiles", "LastLoggedInDate");
        }

        public override void Down()
        {
            AddColumn("dbo.UserProfiles", "LastLoggedInDate", c => c.DateTime());
        }
    }
}
