from django.db import models

class Dialog(models.Model):
    name = models.CharField(max_length=250, unique=True)
    # version (file version? format version?)

    def __unicode__(self):
        return "Dialog: %s" % self.name


class Group(models.Model):
    name = models.CharField(max_length=250)

    dialog = models.ForeignKey('Dialog')
    complaint_groups = models.ManyToManyField('complaint.ComplaintGroup', null=True, blank=True)
    complaints = models.ManyToManyField('complaint.Complaint', null=True, blank=True)

    all_complaints = models.BooleanField(default=False, help_text='Show for all complaints.')

    class Meta:
        unique_together = ('name', 'dialog')

    def __unicode__(self):
        return "Group: %s" % self.name
    

class Subgroup(models.Model):
    name = models.CharField(max_length=250)
    group = models.ForeignKey('Group')

    class Meta:
        unique_together = ('name', 'group')

    def __unicode__(self):
        return "Subgroup: %s" % self.name
    

class Element(models.Model):
    name = models.CharField(max_length=250)

    group = models.ForeignKey('Group')
    subgroup = models.ForeignKey('Subgroup', null=True, blank=True)

    complaint_groups = models.ManyToManyField('complaint.ComplaintGroup', null=True, blank=True)
    complaints = models.ManyToManyField('complaint.Complaint', null=True, blank=True)

    all_complaints = models.BooleanField(default=False, help_text='Show for all complaints.')
    
    EHR_keyword = models.CharField(max_length=250, help_text='The keyword to be replaced by this element in the EHR.')
    is_present_normal = models.BooleanField(default=True, help_text='True if marking present is normal, false if marking present is abnormal.')
    default_present = models.BooleanField(default=False, help_text='Mark present by default. e.g. is part of the no-touch exam.')
    present_text = models.CharField(null=True, blank=True, max_length=500, help_text='Text to be inserted in the EHR if this element is present. If blank this state will be disabled.')
    not_present_text = models.CharField(null=True, blank=True, max_length=500, help_text='Text to be inserted in the EHR if this element is not present. If blank this state will be disabled.')

    class Meta:
        unique_together = ('name', 'group')

    def __unicode__(self):
        return "Element: %s" % self.name
