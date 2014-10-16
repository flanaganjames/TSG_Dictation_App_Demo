from django.db import models


class Dialog(models.Model):
    name = models.CharField(max_length=250, unique=True)
    next_dialog = models.ForeignKey('Dialog', null=True, blank=True, help_text='The next dialog in the workflow.')
    # version (file version? format version?)
    default_present_text = models.CharField(max_length=250, null=True, blank=True, help_text='e.g. "admits"')
    default_not_present_text = models.CharField(max_length=250, null=True, blank=True, help_text='e.g. "denies"')

    class Meta:
        db_table = "dialog"

    def __unicode__(self):
        return self.name


class Group(models.Model):
    name = models.CharField(max_length=250)

    dialog = models.ForeignKey('Dialog')
    order = models.PositiveIntegerField(default=0, blank=False, null=False)

    recommended = models.BooleanField(default=False)

    complaint_groups = models.ManyToManyField('complaint.ComplaintGroup', null=True, blank=True)
    complaints = models.ManyToManyField('complaint.Complaint', null=True, blank=True)

    all_complaints = models.BooleanField(default=False, help_text='Show for all complaints.')
    
    class Meta:
        db_table = "group"
        ordering = ('order',)
        unique_together = ('name', 'dialog')

    def __unicode__(self):
        return self.name
    

class Subgroup(models.Model):
    name = models.CharField(max_length=250)
    group = models.ForeignKey('Group')
    order = models.PositiveIntegerField(default=0, blank=False, null=False)

    class Meta:
        db_table = "subgroup"
        ordering = ('order',)
        unique_together = ('name', 'group')

    def __unicode__(self):
        return self.name
    

class Element(models.Model):
    name = models.CharField(max_length=250)

    group = models.ForeignKey('Group')
    subgroup = models.ForeignKey('Subgroup', null=True, blank=True)
    order = models.PositiveIntegerField(default=0, blank=False, null=False)

    complaint_groups = models.ManyToManyField('complaint.ComplaintGroup', null=True, blank=True)
    complaints = models.ManyToManyField('complaint.Complaint', null=True, blank=True)

    all_complaints = models.BooleanField(default=False, help_text='Show for all complaints.')

    recommended = models.BooleanField(default=False)
    
    EHR_keyword = models.CharField(max_length=250, help_text='The keyword to be replaced by this element in the EHR.')
    is_present_normal = models.BooleanField(default=True, help_text='True if marking present is normal, false if marking present is abnormal.')
    default_present = models.BooleanField(default=False, help_text='Mark present by default. e.g. is part of the no-touch exam.')
    present_text = models.CharField(null=True, blank=True, max_length=500, help_text='Text to be inserted in the EHR if this element is present. If blank this state will be disabled.')
    not_present_text = models.CharField(null=True, blank=True, max_length=500, help_text='Text to be inserted in the EHR if this element is not present. If blank this state will be disabled.')
    EHR_replace = models.CharField(null=True, blank=True, max_length=500, help_text='commandreplace/insert text into template if element is selected')
    SLC_command = models.CharField(null=True, blank=True, max_length=500, help_text='Command to send to SLC if element is selected')

    class Meta:
        db_table = "element"
        ordering = ('order',)
        unique_together = ('name', 'group')

    def __unicode__(self):
        return "Element: %s" % self.name
    

class DialogLinkElement(models.Model):
    name = models.CharField(max_length=250)

    group = models.ForeignKey('Group')
    subgroup = models.ForeignKey('Subgroup', null=True, blank=True)
    order = models.PositiveIntegerField(default=0, blank=False, null=False)

    complaint_groups = models.ManyToManyField('complaint.ComplaintGroup', null=True, blank=True)
    complaints = models.ManyToManyField('complaint.Complaint', null=True, blank=True)
    all_complaints = models.BooleanField(default=False, help_text='Show for all complaints.')

    linked_dialog = models.ForeignKey('Dialog')

    class Meta:
        db_table = "dialogelement"
        ordering = ('order',)
        unique_together = ('name', 'group')

    def __unicode__(self):
        return "Dialog Element: %s" % self.name
