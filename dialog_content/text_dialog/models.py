from django.db import models


class TextDialog(models.Model):
    name = models.CharField(max_length=250, unique=True)
    # version (file version? format version?)

    EHR_keyword = models.CharField(max_length=250, help_text='The keyword to be replaced by this element in the EHR.')

    class Meta:
        db_table = "textdialog"

    def __unicode__(self):
        return "Text Dialog: %s" % self.name


class TextElement(models.Model):
    textdialog = models.ForeignKey('TextDialog')

    boiler_plate = models.TextField()

    class Meta:
        db_table = "textelement"

    def __unicode__(self):
        return "Text Element: %s" % self.textdialog