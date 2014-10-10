# -*- coding: utf-8 -*-
from south.utils import datetime_utils as datetime
from south.db import db
from south.v2 import SchemaMigration
from django.db import models


class Migration(SchemaMigration):

    def forwards(self, orm):
        # Adding model 'DialogLinkElement'
        db.create_table('dialogelement', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(max_length=250)),
            ('group', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['dialog.Group'])),
            ('subgroup', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['dialog.Subgroup'], null=True, blank=True)),
            ('all_complaints', self.gf('django.db.models.fields.BooleanField')(default=False)),
            ('linked_dialog', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['dialog.Dialog'])),
        ))
        db.send_create_signal(u'dialog', ['DialogLinkElement'])

        # Adding M2M table for field complaint_groups on 'DialogLinkElement'
        m2m_table_name = db.shorten_name('dialogelement_complaint_groups')
        db.create_table(m2m_table_name, (
            ('id', models.AutoField(verbose_name='ID', primary_key=True, auto_created=True)),
            ('dialoglinkelement', models.ForeignKey(orm[u'dialog.dialoglinkelement'], null=False)),
            ('complaintgroup', models.ForeignKey(orm[u'complaint.complaintgroup'], null=False))
        ))
        db.create_unique(m2m_table_name, ['dialoglinkelement_id', 'complaintgroup_id'])

        # Adding M2M table for field complaints on 'DialogLinkElement'
        m2m_table_name = db.shorten_name('dialogelement_complaints')
        db.create_table(m2m_table_name, (
            ('id', models.AutoField(verbose_name='ID', primary_key=True, auto_created=True)),
            ('dialoglinkelement', models.ForeignKey(orm[u'dialog.dialoglinkelement'], null=False)),
            ('complaint', models.ForeignKey(orm[u'complaint.complaint'], null=False))
        ))
        db.create_unique(m2m_table_name, ['dialoglinkelement_id', 'complaint_id'])

        # Adding unique constraint on 'DialogLinkElement', fields ['name', 'group']
        db.create_unique('dialogelement', ['name', 'group_id'])


    def backwards(self, orm):
        # Removing unique constraint on 'DialogLinkElement', fields ['name', 'group']
        db.delete_unique('dialogelement', ['name', 'group_id'])

        # Deleting model 'DialogLinkElement'
        db.delete_table('dialogelement')

        # Removing M2M table for field complaint_groups on 'DialogLinkElement'
        db.delete_table(db.shorten_name('dialogelement_complaint_groups'))

        # Removing M2M table for field complaints on 'DialogLinkElement'
        db.delete_table(db.shorten_name('dialogelement_complaints'))


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
        u'dialog.dialoglinkelement': {
            'Meta': {'unique_together': "(('name', 'group'),)", 'object_name': 'DialogLinkElement', 'db_table': "'dialogelement'"},
            'all_complaints': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'complaint_groups': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.ComplaintGroup']", 'null': 'True', 'blank': 'True'}),
            'complaints': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.Complaint']", 'null': 'True', 'blank': 'True'}),
            'group': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Group']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'linked_dialog': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Dialog']"}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'subgroup': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Subgroup']", 'null': 'True', 'blank': 'True'})
        },
        u'dialog.element': {
            'EHR_keyword': ('django.db.models.fields.CharField', [], {'max_length': '250'}),
            'EHR_replace': ('django.db.models.fields.CharField', [], {'max_length': '500', 'null': 'True', 'blank': 'True'}),
            'Meta': {'unique_together': "(('name', 'group'),)", 'object_name': 'Element', 'db_table': "'element'"},
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