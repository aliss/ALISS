import os.path
from django.core.management import call_command

save_path = 'aliss/static/location/'

name_of_file = 'location'

completeName = os.path.join(save_path, name_of_file+".txt")         

file1 = open(completeName, "w")

toFile = call_command('generate_location_report')

file1.write(toFile)

file1.close()