using ALISS.Models.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;

namespace ALISS.Business
{
    public class ALISSContext : IdentityDbContext<ApplicationUser>
    {
        public ALISSContext()
            : base("name=DefaultConnection")
        {
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<ALISSContext, Migrations.Configuration>());
        }

        public static ALISSContext Create()
        {
            return new ALISSContext();
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ServiceArea> ServiceAreas { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<ServiceLocation> ServiceLocations { get; set; }
        public DbSet<ServiceServiceArea> ServiceServiceAreas { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<OrganisationAudit> OrganisationAudits { get; set; }
        public DbSet<ServiceAudit> ServiceAudits { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionService> CollectionServices { get; set; }
        public DbSet<Improvement> Improvements { get; set; }
        public DbSet<OrganisationClaimUser> OrganisationClaimUsers { get; set; }
        public DbSet<ServiceClaim> ServiceClaims { get; set; }
        public DbSet<ServiceClaimUser> ServiceClaimUsers { get; set; }
        public DbSet<CommunityGroup> CommunityGroups { get; set; }
		public DbSet<ServiceCommunityGroup> ServiceCommunityGroups { get; set; }
        public DbSet<AccessibilityFeature> AccessibilityFeatures { get; set; }
        public DbSet<ServiceAccessibilityFeature> ServiceAccessibilityFeatures { get; set; }
        public DbSet<MediaGallery> MediaGallery { get; set; }
        public DbSet<ServiceReview> ServiceReviews { get; set; }
    }
}
