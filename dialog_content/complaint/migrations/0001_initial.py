# -*- coding: utf-8 -*-
from south.utils import datetime_utils as datetime
from south.db import db
from south.v2 import SchemaMigration
from django.db import models


class Migration(SchemaMigration):

    def forwards(self, orm):
        # Adding model 'ComplaintGroup'
        db.create_table('complaintGroup', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(unique=True, max_length=250)),
        ))
        db.send_create_signal(u'complaint', ['ComplaintGroup'])

        # Adding model 'Complaint'
        db.create_table('complaint', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(max_length=250)),
            ('complaint_group', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['complaint.ComplaintGroup'], null=True, blank=True)),
        ))
        db.send_create_signal(u'complaint', ['Complaint'])


    def backwards(self, orm):
        # Deleting model 'ComplaintGroup'
        db.delete_table('complaintGroup')

        # Deleting model 'Complaint'
        db.delete_table('complaint')


    models = {
        u'complaint.complaint': {
            'Meta': {'object_name': 'Complaint', 'db_table': "'complaint'"},
            'complaint_group': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['complaint.ComplaintGroup']", 'null': 'True', 'blank': 'True'}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'})
        },
        u'complaint.complaintgroup': {
            'Meta': {'object_name': 'ComplaintGroup', 'db_table': "'complaintGroup'"},
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'unique': 'True', 'max_length': '250'})
        }
    }

    complete_apps = ['complaint']