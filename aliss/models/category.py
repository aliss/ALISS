import uuid
from django.db import models
from django.db.models import Q

class Category(models.Model):
    name = models.CharField(max_length=50, unique=True)
    slug = models.SlugField()
    parent = models.ForeignKey('self', null=True, blank=True, related_name='children', on_delete=models.CASCADE)

    class Meta:
        verbose_name_plural = "Categories"

    def __str__(self):
        return self.name

    @property
    def is_root(self):
        return not self.parent

    @property
    def siblings(self):
        return Category.objects.filter(parent=self.parent)

    @property
    def all_children(self):
        immediate = Q(parent=self)
        grandchildren = Q(parent__in=self.children.all())
        return Category.objects.filter(immediate | grandchildren)

    @property
    def subcategories(self):
        return Category.objects.filter(parent=self)

    def filter_by_family(self, services):
        full_family = self.all_children
        full_family |= Category.objects.filter(pk=self.pk) #join parent with children
        return services.filter(categories__in=full_family)

    def save(self, *args, **kwargs):
        super(Category, self).save(*args, **kwargs)
        for s in self.services.all():
            s.update_index()

    def delete(self, *args, **kwargs):
        services = list(self.services.all())
        super(Category, self).delete(*args, **kwargs)
        for s in services:
            s.update_index()