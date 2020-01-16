# ALISS [![Build Status](https://travis-ci.org/aliss/ALISS.svg?branch=master)](https://travis-ci.org/aliss/ALISS)

> ALISS (A Local Information System for Scotland) is a service to help you find help and support close to you when you need it most.

## JS Plugin

Embed ALISS search features on your own site with the [aliss.js plugin](https://github.com/aliss/aliss.js).

## Links

- Production site: https://www.aliss.org
- Search API endpoint (v4): https://www.aliss.org/api/v4/services/search/
- API docs: http://docs.aliss.org
- API docs repo: https://github.com/aliss/Docs

## How to install ALISS

### Requirements

- Python 3
- pip3
- NPM (Node package manager)
- See `requirements.txt` for pip packages
- See `package.json` for node packages
- Elasticsearch >=6.1.3
- Postgres >= 9.0
- [Heroku toolbelt](https://devcenter.heroku.com/articles/heroku-cli) (only if deploying to Heroku)

### Install steps

1. Install requirements e.g. Python 3 https://www.python.org/downloads/.
2. Clone repository https://help.github.com/en/articles/cloning-a-repository.
3. If not installed download pip3 https://pip.pypa.io/en/stable/installing/.
4. Use pip3 to install the dependencies in requirements.txt on MacOS this can be achieved with `pip3 install -r requirements.txt`.
5. If not already installed download NPM https://www.npmjs.com/get-npm.
6. Install the npm packages using command `npm i`.
7. Run `gulp` to compile assets
8. Run migrations e.g. `python3 manage.py migrate`
9. Configure environment variables & seed data (see 'Configuring ALISS')

## Configuring ALISS
To run the ALISS project it is necessary to setup the environment on your machine and import data.

1. Create a hidden file `.env` this will store necessary environment variables.
2. With the use of `.env.example` copy the contents and customise with the relevant information for your environment.
3. Ensure elasticsearch server is running `systemctl start elasticsearch.service`
4. Import the postcode data with management command `heroku local:run python3 manage.py import_postcodes`.
5. Import the place names data with management command `heroku local:run python3 manage.py import_places`.
6. Import geo-boundary data with management command `heroku local:run python3 manage.py extract_geodata`.
7. Create a super_user account for local admin privileges using command `heroku local:run python3 manage.py createsuperuser` inputting an email address and password as per the prompts.

## Common Commands
|Command|Description|Further Information|
|-------|-----------|-------------------|
|`heroku local -i Procfile.dev`|Start the local server for running the app at localhost:5000 in your browser.|https://docs.djangoproject.com/en/1.11/ref/django-admin/#runserver|
|`heroku local:run python3 manage.py test`|Run the automated tests|https://docs.djangoproject.com/en/1.11/topics/testing/|
|`heroku local:run python3 manage.py shell`|Run the Django shell|https://docs.djangoproject.com/en/1.11/ref/django-admin/#shell|

## Notes on data

Regarding boundaries and service areas see https://www.opendata.nhs.scot/dataset/geography-codes-and-labels
