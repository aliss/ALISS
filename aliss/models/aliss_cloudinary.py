from cloudinary import CloudinaryResource, forms, uploader
from django.core.files.uploadedfile import UploadedFile
from django.db import models
from cloudinary.models import CloudinaryField

class ALISSCloudinaryField(CloudinaryField):

    #in response to https://github.com/cloudinary/pycloudinary/issues/98
    def to_python(self, value):
        if isinstance(value, CloudinaryResource):
            return value
        elif isinstance(value, UploadedFile):
            return value
        elif value is None or value is False:
            return value
        else:
            return self.parse_cloudinary_resource(value)
