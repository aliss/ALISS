import uuid

from django.db import models
from django.utils import timezone
from django.contrib.auth.models import BaseUserManager, AbstractBaseUser, PermissionsMixin
from django.utils.translation import ugettext_lazy as _
from django.core import validators
from django.core.urlresolvers import reverse
from django.conf import settings
from .organisation import Organisation


class ALISSUserManager(BaseUserManager):
        
    def create_user(self, email, password, **extra_fields):
        if not email:
            raise ValueError('The given email must be set')

        email = self.normalize_email(email)
        email_two = self.normalize_email(email)
        user = self.model(email=email, **extra_fields)
        user.set_password(password)

        user.save(using=self._db)
        return user

    def create_superuser(self, email, email_two, password, **extra_fields):
        extra_fields.setdefault('is_staff', True)
        extra_fields.setdefault('is_superuser', True)

        if extra_fields.get('is_staff') is not True:
            raise ValueError('Superuser must have is_staff=True.')
        if extra_fields.get('is_superuser') is not True:
            raise ValueError('Superuser must have is_superuser=True.')

        return self.create_user(email, email_two, password, **extra_fields)


class ALISSUser(AbstractBaseUser, PermissionsMixin):
    email = models.EmailField(
        unique=True,
        error_messages={
            'unique': _("A user with that email address already exists."),
        },
    )
    email_two  = models.EmailField(
        unique=True,
        error_messages={
            'unique': _("A user with that email address already exists."),
        },
    )
    
    
    is_staff = models.BooleanField(
        _('staff status'),
        default=False,
        help_text=_('Designates whether the user can log into this admin site.'),
    )
    is_active = models.BooleanField(
        _('active'),
        default=True,
        help_text=_(
            'Designates whether this user should be treated as active. '
            'Unselect this instead of deleting accounts.'
        ),
    )

    is_editor = models.BooleanField(default=False)
    date_joined = models.DateTimeField(_('date joined'), default=timezone.now)
    name = models.CharField(max_length=50)
    postcode = models.CharField(max_length=9, blank=True)
    phone_number = models.CharField(max_length=15, blank=True)

    saved_services = models.ManyToManyField('aliss.Service', blank=True)
    helpful_services = models.ManyToManyField(
        'aliss.Service',
        blank=True,
        related_name='helped_users'
    )

    accept_terms_and_conditions = models.BooleanField(default=True)
    accept_privacy_policy = models.BooleanField(default=True)

    prepopulate_postcode = models.BooleanField(default=False)

    objects = ALISSUserManager()

    USERNAME_FIELD = 'email'
    REQUIRED_FIELDS = ['email, name, postcode']

    class Meta:
        verbose_name = _('user')
        verbose_name_plural = _('users')
        ordering = ['-date_joined']

    def get_absolute_url(self):
        return reverse('user_detail', args=[str(self.username)])

    def get_full_name(self):
        return self.name or self.email or self.email_two

    def get_short_name(self):
        return self.name or self.email or self.email_two

    def claimed_organisations(self):
        return Organisation.objects.filter(claimed_by=self)

    def organisations_to_review(self):
        claimed_orgs= self.claimed_organisations()
        created_orgs = Organisation.objects.filter(created_by=self).exclude(claimed_by=self)
        last_to_edit_orgs = Organisation.objects.filter(updated_by=self).exclude(created_by=self).exclude(claimed_by=self)
        orgs_to_review = list(claimed_orgs) + list(created_orgs) + list(last_to_edit_orgs)
        unique_orgs_to_review = set(orgs_to_review)
        return unique_orgs_to_review

    def services_to_review(self):
        orgs_to_review = self.organisations_to_review()
        services_to_review = []
        for org in orgs_to_review:
            for service in org.services.all():
                services_to_review.append(service)
        return services_to_review

    def services_to_review_ids(self):
        services_to_review_ids = []
        for service in self.services_to_review():
            service_id = service.check_last_reviewed()
            if service_id:
                services_to_review_ids.append(service_id)
        return services_to_review_ids


class RecommendedServiceList(models.Model):
    id = models.UUIDField(primary_key=True, default=uuid.uuid4)

    user = models.ForeignKey('aliss.ALISSUser')
    name = models.CharField(max_length=50)

    services = models.ManyToManyField('aliss.Service', blank=True)

    created_on = models.DateTimeField(auto_now_add=True)
    updated_on = models.DateTimeField(auto_now=True)

    def __str__(self):
        return "Recommended for {name}".format(name=self.name)
