from django.contrib import admin
from models import Dialog, Group, Subgroup, Element, DialogLinkElement


class ElementInline(admin.TabularInline):
    model = Element
    # filter_horizontal = ('complaint_groups', 'complaints')

class DialogAdmin(admin.ModelAdmin):
    list_display = ('name', 'next_dialog')


class GroupAdmin(admin.ModelAdmin):
    inlines = [ElementInline]
    filter_horizontal = ('complaint_groups', 'complaints')
    list_display = ('name', 'dialog', 'recommended', 'all_complaints')
    list_filter = ('dialog',)


class SubgroupAdmin(admin.ModelAdmin):
    inlines = [ElementInline]
    list_display = ('name', 'group', 'group__dialog__name')
    list_filter = ('group__dialog', 'group')

    def group__dialog__name(self, obj):
        return obj.group.dialog.name
    group__dialog__name.short_description = 'Dialog'
    group__dialog__name.admin_order_field = 'group__dialog'

class ElementAdmin(admin.ModelAdmin):
    filter_horizontal = ('complaint_groups', 'complaints')
    list_display = ('name', 'group_or_subgroup__dialog__name', 'group', 'subgroup', 'EHR_keyword')
    list_filter = ('group__dialog', 'group', 'subgroup')

    def group_or_subgroup__dialog__name(self, obj):
        return obj.subgroup.group.dialog.name if obj.subgroup else obj.group.dialog.name
    group_or_subgroup__dialog__name.short_description = 'Dialog'
    group_or_subgroup__dialog__name.admin_order_field = 'group__dialog'

class DialogLinkElementAdmin(admin.ModelAdmin):
    filter_horizontal = ('complaint_groups', 'complaints')
    list_display = ('name', 'group', 'subgroup', 'linked_dialog')
    list_filter = ('group__dialog', 'group', 'subgroup')


admin.site.register(Dialog, DialogAdmin)
admin.site.register(Group, GroupAdmin)
admin.site.register(Subgroup, SubgroupAdmin)
admin.site.register(Element, ElementAdmin)
admin.site.register(DialogLinkElement, DialogLinkElementAdmin)