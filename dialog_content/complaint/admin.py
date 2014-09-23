from django.contrib import admin
from models import ComplaintGroup, Complaint


class ComplaintInline(admin.TabularInline):
    model = Complaint
    # filter_horizontal = ('complaint_groups', 'complaints')

class ComplaintGroupAdmin(admin.ModelAdmin):
    inlines = [ComplaintInline]

admin.site.register(ComplaintGroup, ComplaintGroupAdmin)
admin.site.register(Complaint)

from django.contrib.auth.models import User
from django.contrib.auth.models import Group

admin.site.unregister(User)
admin.site.unregister(Group)