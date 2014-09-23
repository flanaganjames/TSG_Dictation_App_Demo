from django.contrib import admin
from models import Dialog, Group, Subgroup, Element


class ElementInline(admin.TabularInline):
    model = Element
    # filter_horizontal = ('complaint_groups', 'complaints')

class GroupAdmin(admin.ModelAdmin):
    inlines = [ElementInline]

class SubgroupAdmin(admin.ModelAdmin):
    inlines = [ElementInline]

class ElementAdmin(admin.ModelAdmin):
    filter_horizontal = ('complaint_groups', 'complaints')


admin.site.register(Dialog)
admin.site.register(Group, GroupAdmin)
admin.site.register(Subgroup, SubgroupAdmin)
admin.site.register(Element, ElementAdmin)