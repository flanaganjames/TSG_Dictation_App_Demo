# -*- coding: utf-8 -*-
from south.utils import datetime_utils as datetime
from south.db import db
from south.v2 import SchemaMigration
from django.db import models


class Migration(SchemaMigration):

    def forwards(self, orm):
        # Adding model 'TextDialog'
        db.create_table('textdialog', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(unique=True, max_length=250)),
            ('EHR_keyword', self.gf('django.db.models.fields.CharField')(max_length=250)),
        ))
        db.send_create_signal(u'text_dialog', ['TextDialog'])

        # Adding model 'TextElement'
        db.create_table('textelement', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('textdialog', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['text_dialog.TextDialog'])),
            ('boiler_plate', self.gf('django.db.models.fields.TextField')()),
        ))
        db.send_create_signal(u'text_dialog', ['TextElement'])


    def backwards(self, orm):
        # Deleting model 'TextDialog'
        db.delete_table('textdialog')

        # Deleting model 'TextElement'
        db.delete_table('textelement')


    models = {
        u'text_dialog.textdialog': {
            'EHR_keyword': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'Meta': {'object_name': 'TextDialog', 'db_table': "'textdialog'"},
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'unique': 'True', 'max_length': '250'})
        },
        u'text_dialog.textelement': {
            'Meta': {'object_name': 'TextElement', 'db_table': "'textelement'"},
            'boiler_plate': ('django.db.models.fields.TextField', [], {}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'textdialog': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['text_dialog.TextDialog']"})
        }
    }

    complete_apps = ['text_dialog']