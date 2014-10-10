# -*- coding: utf-8 -*-
from south.utils import datetime_utils as datetime
from south.db import db
from south.v2 import SchemaMigration
from django.db import models


class Migration(SchemaMigration):

    def forwards(self, orm):
        # Adding unique constraint on 'Element', fields ['name', 'group']
        db.create_unique('element', ['name', 'group_id'])

        # Adding unique constraint on 'Group', fields ['name', 'dialog']
        db.create_unique('group', ['name', 'dialog_id'])


    def backwards(self, orm):
        # Removing unique constraint on 'Group', fields ['name', 'dialog']
        db.delete_unique('group', ['name', 'dialog_id'])

        # Removing unique constraint on 'Element', fields ['name', 'group']
        db.delete_unique('element', ['name', 'group_id'])


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
        },
        u'dialog.dialog': {
            'Meta': {'object_name': 'Dialog', 'db_table': "'dialog'"},
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'unique': 'True', 'max_length': '250'})
        },
        u'dialog.element': {
            'EHR_keyword': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'Meta': {'unique_together': "(('name', 'group'),)", 'object_name': 'Element', 'db_table': "'element'"},
            'all_complaints': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'complaint_groups': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.ComplaintGroup']", 'null': 'True', 'blank': 'True'}),
            'complaints': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.Complaint']", 'null': 'True', 'blank': 'True'}),
            'default_present': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'group': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Group']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'is_present_normal': ('django.db.models.fields.BooleanField', [], {'default': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'not_present_text': ('django.db.models.fields.CharField', [], {'max_length': '500', 'null': 'True', 'blank': 'True'}),
            'present_text': ('django.db.models.fields.CharField', [], {'max_length': '500', 'null': 'True', 'blank': 'True'}),
            'recommended': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'subgroup': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Subgroup']", 'null': 'True', 'blank': 'True'})
        },
        u'dialog.group': {
            'Meta': {'unique_together': "(('name', 'dialog'),)", 'object_name': 'Group', 'db_table': "'group'"},
            'all_complaints': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'complaint_groups': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.ComplaintGroup']", 'null': 'True', 'blank': 'True'}),
            'complaints': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.Complaint']", 'null': 'True', 'blank': 'True'}),
            'dialog': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Dialog']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'recommended': ('django.db.models.fields.BooleanField', [], {'default': 'False'})
        },
        u'dialog.subgroup': {
            'Meta': {'unique_together': "(('name', 'group'),)", 'object_name': 'Subgroup', 'db_table': "'subgroup'"},
            'group': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Group']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'})
        }
    }

    complete_apps = ['dialog']