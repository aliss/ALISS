import io
from django.http import FileResponse
from django.core import management

location = management.call_command("generate_location_report")
name_of_file = "location"
completeName = name_of_file + ".txt"
#Alter this line in any shape or form it is up to you.
file1 = open(completeName , "w")

toFile = location

file1.write(toFile)

file1.close()