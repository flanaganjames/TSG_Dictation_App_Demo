# -*- coding: utf-8 -*-
from south.utils import datetime_utils as datetime
from south.db import db
from south.v2 import SchemaMigration
from django.db import models


class Migration(SchemaMigration):

    def forwards(self, orm):
        # Adding model 'Dialog'
        db.create_table('dialog', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(unique=True, max_length=250)),
        ))
        db.send_create_signal(u'dialog', ['Dialog'])

        # Adding model 'Group'
        db.create_table('group', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(max_length=250)),
            ('dialog', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['dialog.Dialog'])),
            ('all_complaints', self.gf('django.db.models.fields.BooleanField')(default=False)),
        ))
        db.send_create_signal(u'dialog', ['Group'])

        # Adding unique constraint on 'Group', fields ['name', 'dialog']
        db.create_unique('group', ['name', 'dialog_id'])

        # Adding M2M table for field complaint_groups on 'Group'
        m2m_table_name = db.shorten_name('group_complaint_groups')
        db.create_table(m2m_table_name, (
            ('id', models.AutoField(verbose_name='ID', primary_key=True, auto_created=True)),
            ('group', models.ForeignKey(orm[u'dialog.group'], null=False)),
            ('complaintgroup', models.ForeignKey(orm[u'complaint.complaintgroup'], null=False))
        ))
        db.create_unique(m2m_table_name, ['group_id', 'complaintgroup_id'])

        # Adding M2M table for field complaints on 'Group'
        m2m_table_name = db.shorten_name('group_complaints')
        db.create_table(m2m_table_name, (
            ('id', models.AutoField(verbose_name='ID', primary_key=True, auto_created=True)),
            ('group', models.ForeignKey(orm[u'dialog.group'], null=False)),
            ('complaint', models.ForeignKey(orm[u'complaint.complaint'], null=False))
        ))
        db.create_unique(m2m_table_name, ['group_id', 'complaint_id'])

        # Adding model 'Subgroup'
        db.create_table('subgroup', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(max_length=250)),
            ('group', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['dialog.Group'])),
        ))
        db.send_create_signal(u'dialog', ['Subgroup'])

        # Adding unique constraint on 'Subgroup', fields ['name', 'group']
        db.create_unique('subgroup', ['name', 'group_id'])

        # Adding model 'Element'
        db.create_table('element', (
            (u'id', self.gf('django.db.models.fields.AutoField')(primary_key=True)),
            ('name', self.gf('django.db.models.fields.CharField')(max_length=250)),
            ('group', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['dialog.Group'])),
            ('subgroup', self.gf('django.db.models.fields.related.ForeignKey')(to=orm['dialog.Subgroup'], null=True, blank=True)),
            ('all_complaints', self.gf('django.db.models.fields.BooleanField')(default=False)),
            ('EHR_keyword', self.gf('django.db.models.fields.CharField')(max_length=250)),
            ('is_present_normal', self.gf('django.db.models.fields.BooleanField')(default=True)),
            ('default_present', self.gf('django.db.models.fields.BooleanField')(default=False)),
            ('present_text', self.gf('django.db.models.fields.CharField')(max_length=500, null=True, blank=True)),
            ('not_present_text', self.gf('django.db.models.fields.CharField')(max_length=500, null=True, blank=True)),
        ))
        db.send_create_signal(u'dialog', ['Element'])

        # Adding unique constraint on 'Element', fields ['name', 'group']
        db.create_unique('element', ['name', 'group_id'])

        # Adding M2M table for field complaint_groups on 'Element'
        m2m_table_name = db.shorten_name('element_complaint_groups')
        db.create_table(m2m_table_name, (
            ('id', models.AutoField(verbose_name='ID', primary_key=True, auto_created=True)),
            ('element', models.ForeignKey(orm[u'dialog.element'], null=False)),
            ('complaintgroup', models.ForeignKey(orm[u'complaint.complaintgroup'], null=False))
        ))
        db.create_unique(m2m_table_name, ['element_id', 'complaintgroup_id'])

        # Adding M2M table for field complaints on 'Element'
        m2m_table_name = db.shorten_name('element_complaints')
        db.create_table(m2m_table_name, (
            ('id', models.AutoField(verbose_name='ID', primary_key=True, auto_created=True)),
            ('element', models.ForeignKey(orm[u'dialog.element'], null=False)),
            ('complaint', models.ForeignKey(orm[u'complaint.complaint'], null=False))
        ))
        db.create_unique(m2m_table_name, ['element_id', 'complaint_id'])


    def backwards(self, orm):
        # Removing unique constraint on 'Element', fields ['name', 'group']
        db.delete_unique('element', ['name', 'group_id'])

        # Removing unique constraint on 'Subgroup', fields ['name', 'group']
        db.delete_unique('subgroup', ['name', 'group_id'])

        # Removing unique constraint on 'Group', fields ['name', 'dialog']
        db.delete_unique('group', ['name', 'dialog_id'])

        # Deleting model 'Dialog'
        db.delete_table('dialog')

        # Deleting model 'Group'
        db.delete_table('group')

        # Removing M2M table for field complaint_groups on 'Group'
        db.delete_table(db.shorten_name('group_complaint_groups'))

        # Removing M2M table for field complaints on 'Group'
        db.delete_table(db.shorten_name('group_complaints'))

        # Deleting model 'Subgroup'
        db.delete_table('subgroup')

        # Deleting model 'Element'
        db.delete_table('element')

        # Removing M2M table for field complaint_groups on 'Element'
        db.delete_table(db.shorten_name('element_complaint_groups'))

        # Removing M2M table for field complaints on 'Element'
        db.delete_table(db.shorten_name('element_complaints'))


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
            'subgroup': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Subgroup']", 'null': 'True', 'blank': 'True'})
        },
        u'dialog.group': {
            'Meta': {'unique_together': "(('name', 'dialog'),)", 'object_name': 'Group', 'db_table': "'group'"},
            'all_complaints': ('django.db.models.fields.BooleanField', [], {'default': 'False'}),
            'complaint_groups': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.ComplaintGroup']", 'null': 'True', 'blank': 'True'}),
            'complaints': ('django.db.models.fields.related.ManyToManyField', [], {'symmetrical': 'False', 'to': u"orm['complaint.Complaint']", 'null': 'True', 'blank': 'True'}),
            'dialog': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Dialog']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'})
        },
        u'dialog.subgroup': {
            'Meta': {'unique_together': "(('name', 'group'),)", 'object_name': 'Subgroup', 'db_table': "'subgroup'"},
            'group': ('django.db.models.fields.related.ForeignKey', [], {'to': u"orm['dialog.Group']"}),
            u'id': ('django.db.models.fields.AutoField', [], {'primary_key': 'True'}),
            'name': ('django.db.models.fields.CharField', [], {'max_length': '250'})
        }
    }

    complete_apps = ['dialog']