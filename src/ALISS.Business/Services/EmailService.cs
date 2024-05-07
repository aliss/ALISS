using System;
using System.Web.Hosting;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.Collection;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using ALISS.Business.Enums;
using ALISS.Business.ViewModels.Claim;
using System.Web.Helpers;
using ALISS.Business.ViewModels.BulkEmail;
using System.Web;
using System.Web.Mvc;
using System.Net.Mime;
using System.Linq.Expressions;
using System.Security.Permissions;

namespace ALISS.Business.Services
{
    public class EmailService
    {
        private string TactuumContact = ConfigurationManager.AppSettings["TactuumContact"].ToString();
        private string TesterContact = ConfigurationManager.AppSettings["TesterContact"].ToString();
        private string ALISSAdminContact = ConfigurationManager.AppSettings["ALISSAdminContact"].ToString();
        private bool UseStandardToAddress = Boolean.Parse(ConfigurationManager.AppSettings["UseStandardToAddress"].ToString());
        private bool SendToALISSAdminContact = Boolean.Parse(ConfigurationManager.AppSettings["SendToALISSAdminContact"].ToString());
        private string BaseUrl = ConfigurationManager.AppSettings["BaseSiteUrl"].ToString();
        private string logoUrl = ConfigurationManager.AppSettings["BaseSiteUrl"].ToString() + "Ui/dist/img/email-logo.png";
        private string publicBaseUrl = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();

        public void SendWelcomeEmail(string name, string username, string email)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                Logo = logoUrl
            };

            SendEmail(email, "WelcomeEmail", "Welcome to ALISS", model);
        }

        public void SendPasswordResetEmail(string email, string username, string resetLink)
        {
            dynamic model = new
            {
                Username = username,
                Link = resetLink,
                Logo = logoUrl
            };

            SendEmail(email, "PasswordReset", "Reset your ALISS password", model);
        }

        public void SendNewOrganisationEmail(string username, int userProfileId)
        {
            dynamic model = new
            {
                Username = username,
                UserProfileLink = BaseUrl + "/User/EditUser/" + userProfileId,
                UnpublishedLink = BaseUrl + "/Organisation/Index?unpublished=true",
                Logo = logoUrl
            };

            SendEmail("hello@aliss.org", "NewOrganisation", "New organisation added to ALISS", model);
        }

        public void SendOrganisationPublishedEmail(string email, string name, string username, string organisationName, string organisationSlug)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                OrganisationName = organisationName,
                OrganisationLink = publicBaseUrl + "organisations/" + organisationSlug,
                Logo = logoUrl
            };

            SendEmail(email, "OrganisationPublished", "Your organisation has been published on ALISS", model);
        }

        public void SendServicePublishedEmail(string email, string name, string username, string serviceName, string serviceSlug)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                ServiceName = serviceName,
                ServiceLink = publicBaseUrl + "services/" + serviceSlug,
                Logo = logoUrl
            };

            SendEmail(email, "ServicePublished", "Your service has been published on ALISS", model);
        }

        public void SendServiceUnpublishedEmail(string email, string name, string username, string serviceName)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                ServiceName = serviceName,
                Logo = logoUrl
            };

            SendEmail(email, "ServiceUnpublished", "Your service has been unpublished on ALISS", model);
        }

        public void SendShareServiceEmail(ShareServiceViewModel model)
        {
            model.Logo = logoUrl;
            SendEmail(model.Email, "ShareService", "Someone has emailed you a resource from ALISS", model);
        }

        public void SendCollectionEmail(EmailCollectionViewModel model)
        {
            model.Logo = logoUrl;
            SendEmail(model.Email, "ShareCollection", "Someone has emailed you a list of recommended resources from ALISS", model);
        }

        public void SendLeadClaimantChangeEmail(string email, string name, string username, string itemName, string itemUrl, string message)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                ItemName = itemName,
                ItemLink = itemUrl.StartsWith("http") ? itemUrl : BaseUrl + itemUrl,
                Logo = logoUrl,
                Message = message
            };

            SendEmail(email, "LeadClaimantChange", "You are now a lead claimant on ALISS.org", model);
        }

        public void SendClaimApprovedEmail(string email, string name, string username, string itemName, string itemUrl, bool claimedUserRequest)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                ItemName = itemName,
                ItemLink = itemUrl.StartsWith("http") ? itemUrl : BaseUrl + itemUrl,
                Claim = claimedUserRequest,
                Logo = logoUrl
            };

            SendEmail(email, "ClaimApproved", "Your claim has been approved on ALISS", model);
        }

        public void SendClaimSubmittedEmail(string email, string name, string username, string itemName, string itemUrl, string itemType, string url, bool claimedUserRequest)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                ItemName = itemName,
                ItemLink = itemUrl.StartsWith("http") ? itemUrl : BaseUrl + itemUrl,
                ItemType = itemType,
                Url = url,
                Claim = claimedUserRequest,
                Logo = logoUrl
            };

            SendEmail(email, "ClaimSubmitted", "New Claim Submitted", model);
        }

        public void SendClaimPendingEmail(string email, string name, string username, string itemName, bool claimedUserRequest)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                ItemName = itemName,
                Claim = claimedUserRequest,
                Logo = logoUrl
            };

            SendEmail(email, "ClaimPending", "Your claim on ALISS is being reviewed", model);
        }

        public void SendClaimDeniedEmail(string email, string name, string username, string itemName, bool claimedUserRequest)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                ItemName = itemName,
                Claim = claimedUserRequest,
                Logo = logoUrl
            };

            SendEmail(email, "ClaimDenied", "Your claim has been denied on ALISS", model);
        }

        public void SendOrganisationAddedEmail(Guid organisationId, int userProfileId, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                UserProfile userProfile = dc.UserProfiles.Find(userProfileId);
                ApplicationUser applicationUser = userManager.FindByName(userProfile.Username);
                bool isBasicUser = userManager.IsInRole(applicationUser.Id, RolesEnum.BaseUser.ToString()) || userManager.IsInRole(applicationUser.Id, RolesEnum.ClaimedUser.ToString());

                dynamic model = new
                {
                    Name = userProfile.Name,
                    Username = userProfile.Username,
                    Logo = logoUrl,
                    OrganisationLink = $"{publicBaseUrl}organisations/{organisationId}",
                };

                if (isBasicUser)
                {
                    SendEmail(userProfile.Email, "OrganisationPending", "Your organisation is being reviewed", model);
                }
                else
                {
                    //SendEmail(userProfile.Email, "OrganisationApproved", "Your organisation has been added to ALISS", model);
                }
            }
        }

        public void SendGenericSystemEmail(string email, string subject, string message, string url)
        {
            dynamic model = new
            {
                Message = message,
                Url = url,
                Logo = logoUrl
            };

            SendEmail(email, "GenericSystemEmail", subject, model);
        }

        public void SendMediaForApproval(string username, int userProfileId, Guid organisationId, string organisationName, bool organisationPublished, Guid serviceId, string serviceName, bool servicePublished, Guid mediaId, string mediaType, string mediaUrl, string imageAltText = "", string imageCaption = "")
        {
           
            dynamic model = new
            {
                Username = username,
                UserProfileLink = BaseUrl + "/User/EditUser/" + userProfileId,
                OrganisationName = organisationName,
                OrganisationLink = organisationPublished ? publicBaseUrl + "organisations/" + organisationId.ToString() : BaseUrl + "/AddToAliss/OrganisationSummary/" + organisationId.ToString(),
                ServiceName = serviceName,
                ServiceLink = servicePublished ? publicBaseUrl + "services/" + serviceId.ToString() : BaseUrl + "/AddToAliss/Summary/" + serviceId.ToString(),
                MediaType = mediaType,
                MediaUrl = mediaUrl,
                MediaAltText = mediaType.ToLower() == "image" ? imageAltText : "",
                MediaCaption = mediaType.ToLower() == "image" ? imageCaption : "",
                ApproveLink = BaseUrl + "/api/mediagallery/ApproveMedia?mediaId=" + mediaId.ToString(),
                Logo = logoUrl
            };

            SendEmail("hello@aliss.org", "NewMedia", "New media added to a service", model);
        }

        public void SendMediaApprovedEmail(string email, string name, string username, string serviceName, string serviceSlug, string mediaType, string mediaUrl, string imageAltText = "", string imageCaption = "")
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                ServiceName = serviceName,
                ServiceLink = publicBaseUrl + "services/" + serviceSlug,
                MediaType = mediaType,
                MediaUrl = mediaUrl,
                MediaAltText = mediaType.ToLower() == "image" ? imageAltText : "",
                MediaCaption = mediaType.ToLower() == "image" ? imageCaption : "",
                Logo = logoUrl
            };

            SendEmail(email, "MediaApproved", "Your media has been approved on ALISS", model);
        }

        public void SendInformationReview(string email, string name, string username, int reviewEmail, bool manager = false)
        {
            dynamic model = new
            {
                Name = name,
                Username = username,
                ReviewLink = BaseUrl + "ServiceReviews/",
                Logo = logoUrl
            };

            switch (reviewEmail)
            {
                case 1:
                    SendEmail(email, "InformationReview/InformationReview1", "ALISS Information Review", model);
                    break;
                case 2:
                    SendEmail(email, "InformationReview/" + (manager ? "InformationReview2Manager" : "InformationReview2"), manager ? "ALISS Information Review" : "Reminder - ALISS Information Review", model);
                    break;
                case 3:
                    SendEmail(email, "InformationReview/InformationReview3", "ALISS Information Review - Urgent", model);
                    break;
            }
        }

        public void SendSuggestedServiceSubmittedEmail(Guid serviceId, string email, string username)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);
                dynamic model = new
                {
                    Logo = logoUrl,
                    Username = username,
                    ServiceName = service.Name,
                };
                SendEmail(email, "SuggestedServiceSubmitted", "Your service suggestion has been submitted", model);
            }
        }

        public void SendSuggestedServiceApprovedEmail(Guid serviceId, string email, string username, string name)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);
                dynamic model = new
                {
                    Logo = logoUrl,
                    Name = name,
                    Username = username,
                    ServiceName = service.Name,
                    ServiceLink = publicBaseUrl + "services/" + service.Slug,
                    ServiceClaimed = service.ClaimedUserId.HasValue || service.Organisation.ClaimedUserId.HasValue,
                };
                SendEmail(email, "SuggestedServiceApproved", "Your service suggestion has been approved", model);
            }
        }

        public void SendTextEmail(string email, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage();
            if (UseStandardToAddress)
            {
                mailMessage.To.Add(new MailAddress(TactuumContact));
                if (SendToALISSAdminContact)
                {
                    mailMessage.To.Add(new MailAddress(ALISSAdminContact));
                }
                else
                {
                    mailMessage.To.Add(new MailAddress(TesterContact));
                }
            }
            else
            {
                mailMessage.To.Add(new MailAddress(email.TrimEnd(';')));
            }
            mailMessage.Body = body;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
        }

        public void SendBulkCustomEmail(List<string> recipients, CustomEmailViewModel model)
        {
            var templatePath = HostingEnvironment.MapPath(@"\Views\EmailTemplates\") + "CustomEmail.cshtml";

            List<string> invalidEmailAddresses = new List<string>();

            if (UseStandardToAddress)
            {
                model.Body = "Number of Recipients: " + recipients.Count + "/n/n" + model.Body;
            }

            var template = File.ReadAllText(templatePath, Encoding.Default).Replace("@Model.Body", model.Body);
            var body = RazorEngine.Razor.Parse(template, model);

            MailMessage mailMessage = new MailMessage();

            if (model.Attachment != null)
            {
                Attachment attachment = new Attachment(model.Attachment.InputStream, model.Attachment.FileName);
                mailMessage.Attachments.Add(attachment);
            }

            if (UseStandardToAddress)
            {
                mailMessage.To.Add(new MailAddress(TactuumContact));
                if (SendToALISSAdminContact)
                {
                    mailMessage.To.Add(new MailAddress(ALISSAdminContact));

                    if (model.CCAlissAdmin)
                    {
                        mailMessage.CC.Add(new MailAddress(ALISSAdminContact));
                    }
                }
                else
                {
                    mailMessage.To.Add(new MailAddress(TesterContact));

                    if (model.CCAlissAdmin)
                    {
                        mailMessage.CC.Add(new MailAddress(TesterContact));
                    }
                }
            }
            else
            {
                if (model.CCAlissAdmin)
                {
                    mailMessage.CC.Add(new MailAddress(ALISSAdminContact));
                }

                foreach (string recipientEmail in recipients)
                {
                    string emailAddress = recipientEmail.TrimEnd(';').Trim(' ');
                    try
                    {

                        if (!string.IsNullOrWhiteSpace(emailAddress))
                        {
                            mailMessage.Bcc.Add(new MailAddress(emailAddress));
                        }
                    }
                    catch (Exception)
                    {
                        invalidEmailAddresses.Add(emailAddress);
                    }
                }
            }

            mailMessage.ReplyToList.Add(new MailAddress("hello@inbox"));

            mailMessage.Body = body.ToString();
            mailMessage.Subject = model.Subject;
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);

            if (invalidEmailAddresses.Any())
            {
                MailMessage invalidMailMessage = new MailMessage();
                invalidMailMessage.To.Add("canderson@tactuum.com");
                invalidMailMessage.Body = $"<p>There were some invalid email addresses in a bulk email.</p>{string.Join("<br/>", invalidEmailAddresses)}";
                invalidMailMessage.IsBodyHtml = true;
                smtpClient.Send(invalidMailMessage);
            }
        }

        public void SendCustomEmail(string recipient, CustomEmailViewModel model)
        {
            var templatePath = HostingEnvironment.MapPath(@"\Views\EmailTemplates\") + "CustomEmail.cshtml";

            var template = File.ReadAllText(templatePath, Encoding.Default).Replace("@Model.Body", model.Body);
            var body = RazorEngine.Razor.Parse(template, model);

            MailMessage mailMessage = new MailMessage();

            if (UseStandardToAddress)
            {
                mailMessage.To.Add(new MailAddress(TactuumContact));
                if (SendToALISSAdminContact)
                {
                    mailMessage.To.Add(new MailAddress(ALISSAdminContact));

                    if (model.CCAlissAdmin)
                    {
                        mailMessage.CC.Add(new MailAddress(ALISSAdminContact));
                    }
                }
                else
                {
                    mailMessage.To.Add(new MailAddress(TesterContact));

                    if (model.CCAlissAdmin)
                    {
                        mailMessage.CC.Add(new MailAddress(TesterContact));
                    }
                }
            }
            else
            {
                if (model.CCAlissAdmin)
                {
                    mailMessage.CC.Add(new MailAddress(ALISSAdminContact));
                }

                mailMessage.To.Add(new MailAddress(recipient));
            }

            mailMessage.Body = body.ToString();
            mailMessage.Subject = model.Subject;
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
        }

        private void SendEmail(string recipientEmail, string templateName, string subject, dynamic emailModelData)
        {
            var templatePath = HostingEnvironment.MapPath(@"\Views\EmailTemplates\") + templateName + ".cshtml";

            var template = File.ReadAllText(templatePath, Encoding.Default);
            var body = RazorEngine.Razor.Parse(template, emailModelData);

            MailMessage mailMessage = new MailMessage();
            if (UseStandardToAddress)
            {
                mailMessage.To.Add(new MailAddress(TactuumContact));
                if (SendToALISSAdminContact)
                {
                    mailMessage.To.Add(new MailAddress(ALISSAdminContact));
                }
                else
                {
                    mailMessage.To.Add(new MailAddress(TesterContact));
                }
            }
            else
            {
                mailMessage.To.Add(new MailAddress(recipientEmail.TrimEnd(';')));
            }

            mailMessage.Body = body.ToString();
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
        }
    }
}
