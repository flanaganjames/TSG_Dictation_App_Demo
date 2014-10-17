# -*- coding: utf-8 -*-
from south.utils import datetime_utils as datetime
from south.db import db
from south.v2 import SchemaMigration
from django.db import models


class Migration(SchemaMigration):

    def forwards(self, orm):
        # Deleting model 'TextDialog'
        db.delete_table('textdialog')

        # Deleting model 'TextElement'
        db.delete_table('textelement')


    def backwards(self, orm):
        # Adding model 'TextDialog'
        db.create_table('textdialog', (
            ('EHR_keyword', self.gf('django.db.models.fields.CharField')(max_length=250)),
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(max_length=250, unique=True)),
        ))
        db.send_create_signal(u'text_dialog', ['TextDialog'])

        # Adding model 'TextElement'
        db.create_table('textelement', (
            ('textdialog', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['text_dialog.TextDialog'])),
            ('boiler_plate', self.gf('django.db.models.fields.TextField')()),
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
        ))
        db.send_create_signal(u'text_dialog', ['TextElement'])


    models = {
        
    }

    complete_apps = ['text_dialog']