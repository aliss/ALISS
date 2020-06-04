import os.path
from django.core import management

location_export = management.call_command("generate_location_report")

save_path = 'aliss/static/location/'

name_of_file = 'location'

completeName = os.path.join(save_path, name_of_file+".txt")         

file1 = open(completeName, "w")

toFile = location_export

file1.write(toFile)

file1.close()