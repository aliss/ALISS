# -*- coding: utf-8 -*-
# Generated by Django 1.11.20 on 2019-05-01 17:52
from __future__ import unicode_literals

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('aliss', '0025_auto_20190501_1732'),
    ]

    operations = [
        migrations.AlterField(
            model_name='postcode',
            name='postcode_sector',
            field=models.TextField(max_length=6),
        ),
    ]
