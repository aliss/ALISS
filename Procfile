release: curl -X POST -H 'Content-type: application/json' --data '{"text":"Successfuly built on Heroku"}' $NOTIFY_WEBHOOK
web: gunicorn config.wsgi --timeout 40 --log-file -
