from django.core.management import call_command
import sys



stdout_backup, sys.stdout = sys.stdout, open('location.txt', 'w+')
call_command('generate_location_report')
sys.stdout = stdout_backup