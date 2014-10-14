from django.contrib import admin
from models import Dialog, Group, Subgroup, Element, DialogLinkElement


class ElementInline(admin.TabularInline):
    model = Element
    # filter_horizontal = ('complaint_groups', 'complaints')

class GroupAdmin(admin.ModelAdmin):
    inlines = [ElementInline]
    list_filter = ('dialog',)

class SubgroupAdmin(admin.ModelAdmin):
    inlines = [ElementInline]
    list_display = ('name', 'group',)
    list_filter = ('group__dialog', 'group')

class ElementAdmin(admin.ModelAdmin):
    filter_horizontal = ('complaint_groups', 'complaints')
    list_display = ('name', 'group', 'subgroup', 'EHR_keyword')
    list_filter = ('group__dialog', 'group', 'subgroup')

class DialogLinkElementAdmin(admin.ModelAdmin):
    filter_horizontal = ('complaint_groups', 'complaints')
    list_display = ('name', 'group', 'subgroup', 'linked_dialog')
    list_filter = ('group__dialog', 'group', 'subgroup')


admin.site.register(Dialog)
admin.site.register(Group, GroupAdmin)
admin.site.register(Subgroup, SubgroupAdmin)
admin.site.register(Element, ElementAdmin)
admin.site.register(DialogLinkElement, DialogLinkElementAdmin)