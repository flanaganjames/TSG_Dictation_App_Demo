from django.db import models

class ComplaintGroup(models.Model):
    name = models.CharField(max_length=250, unique=True)

    class Meta:
        db_table = "complaintGroup"

    def __unicode__(self):
        return "ComplaintGroup: %s" % self.name


class Complaint(models.Model):
    name = models.CharField(max_length=250)
    complaint_group = models.ForeignKey(ComplaintGroup, null=True, blank=True)

    class Meta:
        db_table = "complaint"

    def __unicode__(self):
        return "Complaint: %s" % self.name
