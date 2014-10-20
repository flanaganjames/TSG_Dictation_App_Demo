from django.contrib import admin
from adminsortable.admin import SortableAdminMixin, SortableInlineAdminMixin
from models import Dialog, Group, Subgroup, Element, DialogLinkElement, TextElement


class ElementInline(SortableInlineAdminMixin, admin.TabularInline):
    model = Element
    # filter_horizontal = ('complaint_groups', 'complaints')

class DialogAdmin(admin.ModelAdmin):
    list_display = ('name', 'next_dialog')


class GroupAdmin(SortableAdminMixin, admin.ModelAdmin):
    inlines = [ElementInline]
    filter_horizontal = ('complaint_groups', 'complaints')
    list_display = ('name', 'dialog', 'recommended', 'all_complaints')
    list_filter = ('dialog',)
    search_fields = ('name',)


class SubgroupAdmin(SortableAdminMixin, admin.ModelAdmin):
    inlines = [ElementInline]
    list_display = ('name', 'group', 'group__dialog__name')
    list_filter = ('group__dialog', 'group')
    search_fields = ('name',)

    def group__dialog__name(self, obj):
        return obj.group.dialog.name
    group__dialog__name.short_description = 'Dialog'
    group__dialog__name.admin_order_field = 'group__dialog'


class ElementAdmin(SortableAdminMixin, admin.ModelAdmin):
    filter_horizontal = ('complaint_groups', 'complaints')
    list_display = ('name', 'group_or_subgroup__dialog__name', 'group', 'subgroup', 'EHR_keyword')
    list_filter = ('group__dialog', 'group', 'subgroup')
    search_fields = ('name', 'EHR_keyword')

    def formfield_for_foreignkey(self, db_field, request, **kwargs):
        if db_field.name == 'group':
            kwargs['queryset'] = Group.objects.exclude(dialog__is_text_dialog=True)
        return super(ElementAdmin, self).formfield_for_foreignkey(db_field, request, **kwargs)

    def group_or_subgroup__dialog__name(self, obj):
        return obj.subgroup.group.dialog.name if obj.subgroup else obj.group.dialog.name
    group_or_subgroup__dialog__name.short_description = 'Dialog'
    group_or_subgroup__dialog__name.admin_order_field = 'group__dialog'


class DialogLinkElementAdmin(SortableAdminMixin, admin.ModelAdmin):
    filter_horizontal = ('complaint_groups', 'complaints')
    list_display = ('name', 'group', 'subgroup', 'linked_dialog')
    list_filter = ('group__dialog', 'group', 'subgroup')
    search_fields = ('name',)

    def group__dialog__name(self, obj):
        return obj.group.dialog.name
    group__dialog__name.short_description = 'Dialog'
    group__dialog__name.admin_order_field = 'group__dialog'


class TextElementAdmin(SortableAdminMixin, admin.ModelAdmin):
    list_display = ('title', 'group', 'group__dialog__name', 'EHR_keyword')
    list_filter = ('group__dialog', 'group', 'EHR_keyword')
    search_fields = ('title', 'EHR_keyword')

    def formfield_for_foreignkey(self, db_field, request, **kwargs):
        if db_field.name == 'group':
            kwargs['queryset'] = Group.objects.filter(dialog__is_text_dialog=True)
        return super(TextElementAdmin, self).formfield_for_foreignkey(db_field, request, **kwargs)

    def group__dialog__name(self, obj):
        return obj.group.dialog.name
    group__dialog__name.short_description = 'Dialog'
    group__dialog__name.admin_order_field = 'group__dialog'


admin.site.register(Dialog, DialogAdmin)
admin.site.register(Group, GroupAdmin)
admin.site.register(Subgroup, SubgroupAdmin)
admin.site.register(Element, ElementAdmin)
admin.site.register(DialogLinkElement, DialogLinkElementAdmin)
admin.site.register(TextElement, TextElementAdmin)