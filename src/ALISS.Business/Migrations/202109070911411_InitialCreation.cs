namespace ALISS.Business.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.UserProfiles",
                c => new
                {
                    UserProfileId = c.Int(nullable: false, identity: true),
                    Email = c.String(maxLength: 254),
                    LastLogin = c.DateTime(nullable: false),
                    Active = c.Boolean(nullable: false),
                    DateJoined = c.DateTime(nullable: false),
                    AcceptPrivacyPolicy = c.Boolean(nullable: false),
                    AcceptTermsAndConditions = c.Boolean(nullable: false),
                    Name = c.String(maxLength: 50),
                    PhoneNumber = c.String(maxLength: 15),
                    Postcode = c.String(maxLength: 10),
                    PrepopulatePostcode = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.UserProfileId);

            CreateTable(
                "dbo.Categories",
                c => new
                {
                    CategoryId = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 50),
                    Slug = c.String(maxLength: 50),
                    Icon = c.String(maxLength: 50),
                    ParentCategoryId = c.Int(),
                })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("dbo.Categories", t => t.ParentCategoryId)
                .Index(t => t.ParentCategoryId);

            CreateTable(
                "dbo.Organisations",
                c => new
                {
                    OrganisationId = c.Guid(nullable: false),
                    Name = c.String(maxLength: 100),
                    Description = c.String(),
                    PhoneNumber = c.String(maxLength: 15),
                    Email = c.String(maxLength: 254),
                    Url = c.String(maxLength: 200),
                    Facebook = c.String(maxLength: 200),
                    Twitter = c.String(maxLength: 200),
                    ClaimedUserId = c.Int(),
                    CreatedUserId = c.Int(nullable: false),
                    CreatedOn = c.DateTime(nullable: false),
                    UpdatedUserId = c.Int(),
                    UpdatedOn = c.DateTime(),
                    Published = c.Boolean(nullable: false),
                    Slug = c.String(maxLength: 120),
                    Logo = c.String(),
                })
                .PrimaryKey(t => t.OrganisationId)
                .ForeignKey("dbo.UserProfiles", t => t.ClaimedUserId)
                .ForeignKey("dbo.UserProfiles", t => t.CreatedUserId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.UpdatedUserId)
                .Index(t => t.ClaimedUserId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.UpdatedUserId);

            CreateTable(
                "dbo.ServiceAreas",
                c => new
                {
                    ServiceAreaId = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 100),
                    Slug = c.String(maxLength: 100),
                    Code = c.String(maxLength: 10),
                    Type = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ServiceAreaId);

            CreateTable(
                "dbo.Locations",
                c => new
                {
                    LocationId = c.Guid(nullable: false),
                    Name = c.String(maxLength: 100),
                    Address = c.String(maxLength: 100),
                    City = c.String(maxLength: 30),
                    Postcode = c.String(maxLength: 10),
                    Latitude = c.Double(nullable: false),
                    Longitude = c.Double(nullable: false),
                    OrganisationId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.LocationId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.OrganisationId);

            CreateTable(
                "dbo.Services",
                c => new
                {
                    ServiceId = c.Guid(nullable: false),
                    Name = c.String(maxLength: 100),
                    Description = c.String(),
                    Phone = c.String(maxLength: 15),
                    Email = c.String(maxLength: 254),
                    Url = c.String(maxLength: 200),
                    Facebook = c.String(maxLength: 200),
                    Twitter = c.String(maxLength: 200),
                    OrganisationId = c.Guid(nullable: false),
                    CreatedUserId = c.Int(nullable: false),
                    CreatedOn = c.DateTime(nullable: false),
                    UpdatedUserId = c.Int(),
                    UpdatedOn = c.DateTime(),
                    Slug = c.String(maxLength: 120),
                })
                .PrimaryKey(t => t.ServiceId)
                .ForeignKey("dbo.UserProfiles", t => t.CreatedUserId, cascadeDelete: true)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .ForeignKey("dbo.UserProfiles", t => t.UpdatedUserId)
                .Index(t => t.OrganisationId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.UpdatedUserId);

            CreateTable(
                "dbo.ServiceCategories",
                c => new
                {
                    ServiceId = c.Guid(nullable: false),
                    CategoryId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.ServiceId, t.CategoryId })
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .Index(t => t.ServiceId)
                .Index(t => t.CategoryId);

            CreateTable(
                "dbo.ServiceLocations",
                c => new
                {
                    ServiceId = c.Guid(nullable: false),
                    LocationId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => new { t.ServiceId, t.LocationId })
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .Index(t => t.ServiceId)
                .Index(t => t.LocationId);

            CreateTable(
                "dbo.ServiceServiceAreas",
                c => new
                {
                    ServiceId = c.Guid(nullable: false),
                    ServiceAreaId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.ServiceId, t.ServiceAreaId })
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .ForeignKey("dbo.ServiceAreas", t => t.ServiceAreaId)
                .Index(t => t.ServiceId)
                .Index(t => t.ServiceAreaId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ServiceServiceAreas", "ServiceAreaId", "dbo.ServiceAreas");
            DropForeignKey("dbo.ServiceServiceAreas", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceLocations", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceLocations", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.ServiceCategories", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.Services", "UpdatedUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.Services", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Services", "CreatedUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.ServiceCategories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Locations", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Organisations", "UpdatedUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.Organisations", "CreatedUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.Organisations", "ClaimedUserId", "dbo.UserProfiles");
            DropForeignKey("dbo.Categories", "ParentCategoryId", "dbo.Categories");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.ServiceServiceAreas", new[] { "ServiceAreaId" });
            DropIndex("dbo.ServiceServiceAreas", new[] { "ServiceId" });
            DropIndex("dbo.ServiceLocations", new[] { "LocationId" });
            DropIndex("dbo.ServiceLocations", new[] { "ServiceId" });
            DropIndex("dbo.Services", new[] { "UpdatedUserId" });
            DropIndex("dbo.Services", new[] { "CreatedUserId" });
            DropIndex("dbo.Services", new[] { "OrganisationId" });
            DropIndex("dbo.ServiceCategories", new[] { "CategoryId" });
            DropIndex("dbo.ServiceCategories", new[] { "ServiceId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Organisations", new[] { "UpdatedUserId" });
            DropIndex("dbo.Organisations", new[] { "CreatedUserId" });
            DropIndex("dbo.Organisations", new[] { "ClaimedUserId" });
            DropIndex("dbo.Locations", new[] { "OrganisationId" });
            DropIndex("dbo.Categories", new[] { "ParentCategoryId" });
            DropTable("dbo.ServiceServiceAreas");
            DropTable("dbo.ServiceLocations");
            DropTable("dbo.ServiceCategories");
            DropTable("dbo.Services");
            DropTable("dbo.Locations");
            DropTable("dbo.Organisations");
            DropTable("dbo.ServiceAreas");
            DropTable("dbo.Categories");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
        }
    }
}
