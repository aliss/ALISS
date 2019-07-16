python manage.py extract_geodata
python manage.py collectstatic
curl -X POST -H 'Content-type: application/json' --data '{"text":"Successfuly built on Heroku"}' $NOTIFY_WEBHOOK