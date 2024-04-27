# ALISS

> ALISS (A Local Information System for Scotland) is a service to help you find help and support close to you when you need it most.

## JS Plugin

Embed ALISS search features on your own site with the [aliss.js plugin](https://github.com/aliss/aliss.js).

## Links

- Production site: https://www.aliss.org
- Search API endpoint (v4): https://www.aliss.org/api/v4/services/search/
- Search API endpoint (v5): https://www.aliss.org/api/v5/services/search/
- API docs: http://docs.aliss.org
- API docs repo: https://github.com/aliss/Docs

## How to install ALISS

### Requirements

- Visual Studio 2019+ or Visual Studio Code
- .NET Framework 4.7.2
- SQL Server
- Node >=16.20.2
- NPM >=8.19.4
- See `package.json` for node packages
- Elasticsearch >=7.17.1

### Install steps

These steps assume that you have the above packages installed.

1. Clone repository https://help.github.com/en/articles/cloning-a-repository.
2. Deploy both database to your SQL Server instance
3. Install the npm packages using command `npm i` in `\ALISS\ALISS.Admin.Web\Ui`
4. Run `gulp` to compile assets
5. Install the npm packages using command `npm i` in `\ALISS\ALISS.CMS\ALISS.THEME`
6. Run `gulp` to compile assets
7. Copy `ALISS.Admin.Web\appSettings.config.template` to `ALISS.Admin.Web\appSettings.config`
8. Update this file with your own configuration
9. Copy `ALISS.API\appSettings.config.template` to `ALISS.API\appSettings.config`
10. Update this file with your own configuration
11. Copy `ALISS.CMS\appSettings.config.template` to `ALISS.CMS\appSettings.config`
12. Update this file with your own configuration
13. Copy `ALISS.CMS\connection.config.template` to `ALISS.CMS\connection.config`
14. Update this file with your own configuration
15. Update `ALISS.CMS\config\umbracoSettings.config` and set a valid email address for email notifications
9. Set the startup project to `ALISS.Admin.Web`
10. Open Package Manager Console and set the Default project to ALISS.Business
11. run `Update-Databases` to make sure the admin database is up to date
12. Add `admin.aliss.local` to IIS and to your hosts file, pointing to the `ALISS.Admin.Web` folder *
13. Add `api.aliss.local` to IIS and to your hosts file, pointing to the `ALISS.API` folder *
14. Add `cms.aliss.local` to IIS and to your hosts file, pointing to the `ALISS.CMS` folder *
15. Build the entire solution
16. Open the Umbraco backoffice by browsing to `http://cms.aliss.local/umbraco` Username: `admin@admin.com` Password: `1234567890` (please change this)
17. Customise the content and configuration as you desire
18. Browse to the admin site at `http://admin.aliss.local` Username: `admin@admin.com` Password: `1234567890` (please change this)
19. Go to the `Accessibility Features`, `What`, `Who`, and `Service Areas` pages under `Lookups` in the navigation
20. In each of these pages, edit and save (no need to make a change) one item from each page, which will build the corresponding index (you can add and change these as you see fit)
21. Go to the `ElasticSearch Management` page and rebuild the Organisation and Services indexes
22. Test the API is working by browsing to `http://api.aliss.local/swagger`
23. Go to the public site at `http://cms.aliss.local`

\* The site URLs can be anything you want, but the domain part (aliss.local) needs to be the same on all three sites to enable login between the admin and public sites.  You will also need to change the related appSettings values in the appSettings.config files.


## Notes on data

Regarding boundaries and service areas see https://www.opendata.nhs.scot/dataset/geography-codes-and-labels
