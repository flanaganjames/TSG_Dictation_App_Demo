# -*- coding: utf-8 -*-
from south.utils import datetime_utils as datetime
from south.db import db
from south.v2 import SchemaMigration
from django.db import models


class Migration(SchemaMigration):

    def forwards(self, orm):
        # Adding field 'Element.order'
        db.add_column('element', 'order',
                      self.gf('django.db.models.fields.PositiveIntegerField')(default=0),
                      keep_default=False)

        # Adding field 'Group.order'
        db.add_column('group', 'order',
                      self.gf('django.db.models.fields.PositiveIntegerField')(default=0),
                      keep_default=False)

        # Adding field 'DialogLinkElement.order'
        db.add_column('dialogelement', 'order',
                      self.gf('django.db.models.fields.PositiveIntegerField')(default=0),
                      keep_default=False)

        # Adding field 'Subgroup.order'
        db.add_column('subgroup', 'order',
                      self.gf('django.db.models.fields.PositiveIntegerField')(default=0),
                      keep_default=False)


    def backwards(self, orm):
        # Deleting field 'Element.order'
        db.delete_column('element', 'order')

        # Deleting field 'Group.order'
        db.delete_column('group', 'order')

        # Deleting field 'DialogLinkElement.order'
        db.delete_column('dialogelement', 'order')

        # Deleting field 'Subgroup.order'
        db.delete_column('subgroup', 'order')


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
            'default_not_present_text': ('django.db.models.fields.CharField', [], {'max_length': '250', 'null': 'True', 'blank': 'True'}),
            'default_present_text': ('django.db.models.fields.CharField', [], {'max_length': '250', 'null': 'True', 'blank': 'True'}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'unique': 'True', 'max_length': '250'}),
            'next_dialog': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Dialog']", 'null': 'True', 'blank': 'True'})
        },
        u'dialog.dialoglinkelement': {
            'Meta': {'ordering': "('order',)", 'unique_together': "(('name', 'group'),)", 'object_name': 'DialogLinkElement', 'db_table': "'dialogelement'"},
            'all_complaints': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'complaint_groups': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.ComplaintGroup']", 'null': 'True', 'blank': 'True'}),
            'complaints': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.Complaint']", 'null': 'True', 'blank': 'True'}),
            'group': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Group']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'linked_dialog': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Dialog']"}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'order': ('django.db.models.fields.PositiveIntegerField', [], {'default': '0'}),
            'subgroup': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Subgroup']", 'null': 'True', 'blank': 'True'})
        },
        u'dialog.element': {
            'EHR_keyword': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'EHR_replace': ('django.db.models.fields.CharField', [], {'max_length': '500', 'null': 'True', 'blank': 'True'}),
            'Meta': {'ordering': "('order',)", 'unique_together': "(('name', 'group'),)", 'object_name': 'Element', 'db_table': "'element'"},
            'SLC_command': ('django.db.models.fields.CharField', [], {'max_length': '500', 'null': 'True', 'blank': 'True'}),
            'all_complaints': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'complaint_groups': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.ComplaintGroup']", 'null': 'True', 'blank': 'True'}),
            'complaints': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.Complaint']", 'null': 'True', 'blank': 'True'}),
            'default_present': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'group': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Group']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'is_present_normal': ('django.db.models.fields.BooleanField', [], {'default': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'not_present_text': ('django.db.models.fields.CharField', [], {'max_length': '500', 'null': 'True', 'blank': 'True'}),
            'order': ('django.db.models.fields.PositiveIntegerField', [], {'default': '0'}),
            'present_text': ('django.db.models.fields.CharField', [], {'max_length': '500', 'null': 'True', 'blank': 'True'}),
            'recommended': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'subgroup': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Subgroup']", 'null': 'True', 'blank': 'True'})
        },
        u'dialog.group': {
            'Meta': {'ordering': "('order',)", 'unique_together': "(('name', 'dialog'),)", 'object_name': 'Group', 'db_table': "'group'"},
            'all_complaints': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'complaint_groups': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.ComplaintGroup']", 'null': 'True', 'blank': 'True'}),
            'complaints': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.Complaint']", 'null': 'True', 'blank': 'True'}),
            'dialog': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Dialog']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'order': ('django.db.models.fields.PositiveIntegerField', [], {'default': '0'}),
            'recommended': ('django.db.models.fields.BooleanField', [], {'default': 'False'})
        },
        u'dialog.subgroup': {
            'Meta': {'ordering': "('order',)", 'unique_together': "(('name', 'group'),)", 'object_name': 'Subgroup', 'db_table': "'subgroup'"},
            'group': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Group']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'order': ('django.db.models.fields.PositiveIntegerField', [], {'default': '0'})
        }
    }

    complete_apps = ['dialog']